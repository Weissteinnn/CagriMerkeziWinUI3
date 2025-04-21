using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;


namespace Proje_Aktarim
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(wndId);
            appWindow.SetIcon("Assets/favicon.ico");

            appWindow.Title = "Vosmer Çağrı Merkezi Uygulaması";
            m_window.Activate();
        }

        public Window? m_window;

        // ?? Ekledik:
        public MainWindow? MainWindowRef => m_window as MainWindow;


    }
}