using System; // Temel .NET işlevleri için gerekli namespace
using System.Collections.Generic; // Koleksiyonlar (List, Dictionary vs.) için
using System.IO; // Dosya işlemleri için
using System.Linq; // LINQ sorguları için
using System.Runtime.InteropServices.WindowsRuntime; // Windows Runtime işlemleri için
using Microsoft.UI.Xaml; // WinUI uygulama yaşam döngüsü ve temel sınıflar için
using Microsoft.UI.Xaml.Controls; // UI kontrolleri
using Microsoft.UI.Xaml.Controls.Primitives; // Gelişmiş UI kontrolleri
using Microsoft.UI.Xaml.Data; // Veri bağlama işlemleri için
using Microsoft.UI.Xaml.Input; // Giriş olayları için
using Microsoft.UI.Xaml.Media; // Görsel işleme ve fırça sınıfları
using Microsoft.UI.Xaml.Navigation; // Sayfa yönlendirme işlemleri için
using Windows.Foundation; // Temel Windows veri türleri
using Windows.Foundation.Collections; // Koleksiyon sınıfları
using Microsoft.UI; // WinUI API'si için
using Microsoft.UI.Windowing; // WinUI pencere işlemleri için
using Windows.Graphics; // Grafikle ilgili veri türleri (örneğin pencere boyutu/konumu)

namespace Proje_Aktarim // Proje namespace’i
{
    public partial class App : Application // Uygulamanın giriş sınıfı (uygulama yaşam döngüsünü kontrol eder)
    {
        public App() // Constructor
        {
            this.InitializeComponent(); // XAML tarafındaki bileşenleri başlatır
        }

        public MainWindow? MainWindowRef { get; private set; } // Ana pencere referansı, diğer sınıflarda kullanılabilir

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) // Uygulama başlatıldığında çalışır
        {
            MainWindowRef = new MainWindow(); // Ana pencere nesnesi oluşturulur

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindowRef); // Pencerenin native handle'ı alınır
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd); // Handle üzerinden WindowId elde edilir
            AppWindow appWindow = AppWindow.GetFromWindowId(wndId); // WindowId üzerinden AppWindow nesnesi alınır
            appWindow.SetIcon("Assets/favicon.ico"); // Pencereye ikon atanır

            appWindow.Title = "Vosmer Çağrı Merkezi Uygulaması"; // Pencere başlığı ayarlanır
            MainWindowRef.Activate(); // Ana pencere ekranda görünür hale getirilir
        }
    }
}
