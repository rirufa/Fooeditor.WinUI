﻿using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FooEditor.WinUI.Models
{
    class AdminiOperation
    {
        StreamWriter sw;
        string scriptPath;

        /// <summary>
        /// コンストラクター
        /// </summary>
        public AdminiOperation() : this(null)
        {
        }

        public string ExecutablePath
        {
            get;set;
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="path">ファイルに対するフルパスを指定する。nullを指定した場合はTempフォルダーにスクリプトを生成する</param>
        public AdminiOperation(string path)
        {
            if (path == null)
                this.scriptPath = Path.GetTempPath() + Path.GetRandomFileName();
            else
                this.scriptPath = path;
            sw = new StreamWriter(this.scriptPath);
        }

        /// <summary>
        /// 操作を１行書き込む
        /// </summary>
        /// <param name="s">コマンドを指定する。詳しいことはAdminiOperation\Program.csを参照してください</param>
        public void WriteCode(string s)
        {
            sw.WriteLine(s);
        }

        /// <summary>
        /// そうさを実行する
        /// </summary>
        /// <returns>成功した場合は真。そうでない場合は偽</returns>
        public bool Execute()
        {
            //TODO:実行ファイルの場所を指定する
            string AdminCopyPath = Path.Combine(ExecutablePath, "AdminOperation.exe");

            sw.WriteLine("\u001a");
            sw.Close();

            if (!File.Exists(AdminCopyPath))
                return false;

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = this.scriptPath;
            info.FileName = AdminCopyPath;
            info.Verb = "runas";
            info.UseShellExecute = true;
            Process process = Process.Start(info);

            process.WaitForExit();

            File.Delete(this.scriptPath);

            return process.ExitCode == 0;
        }

        ~AdminiOperation()
        {
            sw.Close();
        }
    }
}
