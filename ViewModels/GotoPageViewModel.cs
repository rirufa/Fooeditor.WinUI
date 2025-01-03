﻿using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using FooEditor.WinUI.Services;
using FooEditor.WinUI.Models;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using EncodeDetect;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.Graphics.Printing;
using Windows.System.Threading;
using CommunityToolkit.Mvvm.Input;

namespace FooEditor.WinUI.ViewModels
{
    class GotoPageViewModel : ViewModelBase
    {
        string _Result;
        public string Result
        {
            get
            {
                return this._Result;
            }
            set
            {
                SetProperty(ref this._Result, value);
            }
        }

        int _ToRow;
        public int ToRow
        {
            get
            {
                return this._ToRow;
            }
            set
            {
                this._ToRow = value;
                this.OnPropertyChanged();
            }
        }

        public int MaxRow
        {
            get
            {
                return DocumentCollection.Instance.Current.DocumentModel.Document.LayoutLines.Count;
            }
        }

        public RelayCommand<object> JumpLineCommand
        {
            get
            {
                return new RelayCommand<object>((s) =>
                {
                    var newPostion = new FooEditEngine.TextPoint();
                    newPostion.row = this.ToRow - 1;
                    newPostion.col = 0;
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    if (this.ToRow < 0 || this.ToRow > MaxRow)
                    {
                        this.Result = string.Format(loader.GetString("LineNumberOutOutOfRange"), 1, this.MaxRow);
                        return;
                    }
                    DocumentCollection.Instance.Current.DocumentModel.Document.CaretPostion = newPostion;
                    DocumentCollection.Instance.Current.DocumentModel.Document.RequestRedraw();
                });
            }
        }
    }
}
