using System; // Temel .NET sınıflarını içeren namespace
using Microsoft.UI; // WinUI API'lerini içeren namespace
using Microsoft.UI.Windowing; // WinUI'de pencere yönetimi için gerekli sınıfları içerir
using Microsoft.UI.Xaml; // XAML tabanlı UI oluşturmak için gerekli sınıflar
using Microsoft.UI.Xaml.Controls; // UI kontrol sınıflarını içerir (örneğin, Frame, Button)
using System.Runtime.InteropServices; // Yerel (native) kodlarla etkileşim için kullanılır
using Windows.Graphics; // Grafik nesneleri, örneğin pencere boyutu ve konumu için kullanılır

namespace Proje_Aktarim 
{
    public sealed partial class MainWindow : Window // Uygulamanın ana penceresi; sealed, kalıtımı engeller
    {
        private Frame rootFrame; // Uygulamada sayfa geçişi yapmak için kullanılan Frame nesnesi

        public MainWindow() // Constructor (yapıcı metot)
        {
            this.InitializeComponent(); // XAML tarafında tanımlı bileşenleri başlatır

            // Frame oluştur ve Content'e yerleştir
            rootFrame = new Frame(); // Yeni bir Frame nesnesi oluşturulur
            this.Content = rootFrame; // Frame, pencerenin içeriği olarak ayarlanır

            // Pencere ayarları
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // Bu pencereye ait pencere tanıtıcısı (handle) alınır
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd); // Tanıtıcıdan WindowId elde edilir
            AppWindow appWindow = AppWindow.GetFromWindowId(wndId); // WindowId üzerinden AppWindow nesnesi alınır
            appWindow.Title = "Vosmer Çağrı Merkezi Uygulaması"; // Pencerenin başlığı ayarlanır
            appWindow.SetIcon("Assets/favicon.ico"); // Pencere için simge (icon) ayarlanır
            appWindow.Resize(new SizeInt32(1000, 900)); // Pencerenin boyutu 1000x900 piksel olarak ayarlanır
            appWindow.Move(new PointInt32(300, 100)); // Pencere ekran üzerinde (300, 100) koordinatlarına taşınır

            // Ana sayfaya yönlendir
            rootFrame.Navigate(typeof(MainPage)); // Frame içinde MainPage adlı sayfaya yönlendirme yapılır
        }

        public void NavigateToPage(Type pageType) // Belirli bir sayfaya yönlendirme yapmak için yardımcı metot
        {
            if (this.Content is Frame frame) // Content'in gerçekten bir Frame olup olmadığı kontrol edilir
            {
                frame.Navigate(pageType); // Eğer öyleyse, belirtilen sayfaya geçiş yapılır
            }
        }
    }
}
