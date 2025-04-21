using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace Proje_Aktarim
{
    public sealed partial class MainWindow : Window
    {
        private Frame rootFrame;

        public MainWindow()
        {
            this.InitializeComponent();


            // Uygulama penceresi boyutlandırma ve taşıma
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(wndId);
            appWindow.Title = "Vosmer Çağrı Merkezi Uygulaması";
            appWindow.SetIcon("Assets/favicon.ico");
            appWindow.Resize(new SizeInt32(1000, 900));
            appWindow.Move(new PointInt32(300, 100));

            // Frame oluştur ve Content'e yerleştir
            rootFrame = new Frame();
            this.Content = rootFrame;

            // Ana sayfa olarak butonların bulunduğu sayfayı yükle
            rootFrame.Navigate(typeof(MainPage)); // MainPage senin ana buton ekranın olacak
        }

        public void NavigateToPage(Type pageType)
        {
            if (this.Content is Frame frame)
            {
                frame.Navigate(pageType);
            }
        }

    }
}