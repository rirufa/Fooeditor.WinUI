﻿using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FooEditEngine.WinUI;
using FooEditor;
using FooEditor.WinUI.Models;
using System;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using CommunityToolkit.WinUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FooEditor.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            //EncodeDetectを正常に動作させるために必要
            //http://www.atmarkit.co.jp/ait/articles/1509/30/news039.html
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("fooeditor.main");
            mainInstance.Activated += MainInstance_Activated;
            if (!mainInstance.IsCurrent && !AppSettings.Current.AllowMultiInstance)
            {
                await mainInstance.RedirectActivationToAsync(activatedEventArgs);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            m_window = new MainWindow();
            m_window.SetRootFrame(new Frame());
            if(activatedEventArgs.Kind == ExtendedActivationKind.File)
            {
                var fileargs = activatedEventArgs.Data as FileActivatedEventArgs;
                var filepaths = from file in fileargs.Files
                                select file.Path;
                ObjectToXmlConverter conv = new ObjectToXmlConverter();
                //配列で渡さないとシリアライズできない
                var param = conv.Convert(filepaths.ToArray(), typeof(string[]), null, null);
                await m_window.Init(param, false, null);
            }
            else
            {
                bool require_restore = await FooEditor.FileModel.IsExist(FileModelBuildType.LocalFolder,DocumentCollection.collection_name);
                await m_window.Init(null, require_restore , null);
            }
            FooTextBox.OwnerWindow = m_window;

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
            var windowID = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var wnd = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowID);

            wnd.Title = AboutModel.AppName;

            wnd.Closing += async (s, e) =>
            {
                e.Cancel = true;
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
                var windowID = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                var wnd = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowID);
                AppSettings.Current.WindowLocation = new Windows.Foundation.Rect(
                    wnd.Position.X,
                    wnd.Position.Y,
                    wnd.Size.Width,
                    wnd.Size.Height
                    );
                await AppSettings.Current.Save();
                App.Current.Exit();
            };

            var rect = AppSettings.Current.WindowLocation;
            if (rect != Windows.Foundation.Rect.Empty)
            {
                wnd.MoveAndResize(new Windows.Graphics.RectInt32(
                        (int)rect.X,
                        (int)rect.Y,
                        (int)rect.Width,
                        (int)rect.Height
                    )
                    );
            }

            m_window.Activate();
        }

        private void MainInstance_Activated(object sender, AppActivationArguments e)
        {
            var activatedEventArgs = e;
            if (activatedEventArgs.Kind == ExtendedActivationKind.File)
            {
                var fileargs = activatedEventArgs.Data as FileActivatedEventArgs;
                var filepaths = from file in fileargs.Files
                                select file.Path;
                ObjectToXmlConverter conv = new ObjectToXmlConverter();
                //配列で渡さないとシリアライズできない
                var param = conv.Convert(filepaths.ToArray(), typeof(string[]), null, null);
                if(m_window != null)
                {
                    m_window.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,() => {
                        m_window.OpenFromArgs(param);
                    });
                }
            }
        }

        private MainWindow m_window;
    }
}
