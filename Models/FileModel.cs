﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace FooEditor
{
    public enum FileModelBuildType
    {
        AbsolutePath,
        LocalFolder,
        Uri,
    }
    public class FileModel
    {

        private StorageFile File;

        private FileModel(StorageFile file)
        {
            this.File = file;
        }

        public string Path
        {
            get
            {
                return this.File.Path;
            }
        }

        public string Name
        {
            get
            {
                return this.File.Name;
            }
        }

        public string Extension
        {
            get
            {
                return this.File.FileType;
            }
        }

        public async Task<ulong> GetLength()
        {
            var prop = await File.GetBasicPropertiesAsync();
            return prop.Size;
        }

        public static string TrimFullPath(string filepath)
        {
            if (filepath == null || filepath == "")
                return string.Empty;
            string DirectoryPart = System.IO.Path.GetDirectoryName(filepath);
            string FilenamePart = System.IO.Path.GetFileName(filepath);
            string[] slice = DirectoryPart.Split('\\');
            if (slice.Length > 3)
            {
                DirectoryPart = slice[0] + "\\..\\" + slice[slice.Length - 1];
                return DirectoryPart + "\\" + FilenamePart;
            }
            else
                return filepath;
        }

        public static Task<FileModel> GetFileModel(StorageFile file)
        {
            return Task.FromResult(new FileModel(file));
        }

        public static async Task<FileModel> GetFileModel(FileModelBuildType type,string s)
        {
            StorageFile file = null;
            switch (type)
            {
                case FileModelBuildType.AbsolutePath:
                    file = await StorageFile.GetFileFromPathAsync(s);
                    break;
                case FileModelBuildType.LocalFolder:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(s);
                    break;
                case FileModelBuildType.Uri:
                    file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(s));
                    break;
            }
            return await GetFileModel(file);
        }

        public static async Task<bool> IsExist(FileModelBuildType type, string s)
        {
            try
            {
                var file = await GetFileModel(type, s);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public static async Task<FileModel> CreateFileModel(string file_name,bool overwrite = false)
        {
            CreationCollisionOption coll_opt = overwrite ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.FailIfExists;
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(file_name, coll_opt);
            return await GetFileModel(file);
        }

        public static async Task<FileModel> CreateTempFile(string name)
        {
            CreationCollisionOption coll_opt = CreationCollisionOption.ReplaceExisting;
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(name,coll_opt );
            return await GetFileModel(file);
        }

        public async Task<Stream> GetReadStreamAsync()
        {
            return await this.File.OpenStreamForReadAsync();
        }

        public async Task<Stream> GetWriteStreamAsync()
        {
            return await this.File.OpenStreamForWriteAsync();
        }

        public async Task DeleteAsync()
        {
            await this.File.DeleteAsync();
        }

        public bool IsNeedUserActionToSave()
        {
            //ドラッグアンドドロップされたファイルはリードオンリーだが、ファイルピッカーで選ばせれば次からは保存できる
            return (this.File.Attributes & Windows.Storage.FileAttributes.ReadOnly) == Windows.Storage.FileAttributes.ReadOnly;
        }

        public static implicit operator StorageFile(FileModel model) => model.File;
    }
}
