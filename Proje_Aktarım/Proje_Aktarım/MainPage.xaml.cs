using System; // Temel .NET sýnýflarýný içeren namespace
using Microsoft.UI.Xaml; // XAML tabanlý UI oluþturmak için gerekli namespace
using Microsoft.UI.Xaml.Controls; // UI kontrollerini içerir (Button, TextBox, Page vs.)
using Microsoft.UI.Xaml.Media.Imaging; // Görs el ve bitmap iþlemleri için gerekli sýnýflar
using System.Diagnostics; // Dýþ iþlemler (Process.Start) baþlatmak için gerekli namespace
using System.IO; // Dosya iþlemleri (File.Exists, File.AppendAllText vs.) için
using Proje_Aktarim.Helpers; // Yardýmcý sýnýflarý içeren kullanýcý tanýmlý namespace

namespace Proje_Aktarim // Projenin namespace’i
{
    public sealed partial class MainPage : Page // MainPage adlý UI sayfasý
    {
        public MainPage() // Constructor
        {
            this.InitializeComponent(); // XAML bileþenlerini baþlatýr
            this.Loaded += MainPage_Loaded; // Sayfa yüklendiðinde çalýþacak olayý tanýmlar

            BitmapImage bitmap = new BitmapImage(); // Yeni bir bitmap (görsel) nesnesi oluþturulur
            bitmap.DecodePixelWidth = 600; // Görselin geniþliði 600 piksel olarak ayarlanýr
            bitmap.UriSource = new Uri("ms-appx:///Assets/logo_optimized.png"); // Görselin kaynaðý ayarlanýr
            imgLogo.Source = bitmap; // imgLogo adlý UI öðesine görsel atanýr
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e) // Sayfa yüklendiðinde çalýþacak olay
        {
            // Sayfa yüklendiðinde güncelleme kontrolü yap
            string updatePath = @"\\10.40.98.4\cm$\update.msix"; // Güncelleme dosyasýnýn yolu

            if (File.Exists(updatePath)) // Güncelleme dosyasý varsa
            {
                ContentDialog dialog = new() // Onay penceresi oluþturuluyor
                {
                    Title = "Güncelleme Mevcut", // Baþlýk
                    Content = "Yeni bir sürüm bulundu. Güncellemek ister misiniz?", // Ýçerik
                    PrimaryButtonText = "Evet", // Ana buton
                    CloseButtonText = "Hayýr", // Kapatma butonu
                    XamlRoot = this.XamlRoot // XAML aðacýnýn kökü (görsel baðlama)
                };

                var result = await dialog.ShowAsync(); // Onay penceresini göster

                if (result == ContentDialogResult.Primary) // Kullanýcý "Evet" dediyse
                {
                    var psi = new ProcessStartInfo(updatePath) // Güncelleme dosyasýný çalýþtýrmak için ayar
                    {
                        UseShellExecute = true // Shell üzerinden çalýþtýrma izni
                    };
                    File.AppendAllText(@"\\10.40.98.4\cm$\guncelleme_tarih.txt", $"Güncelleme: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Versiyon: {Versiyon.Mevcut}{Environment.NewLine}"); // Güncelleme tarihini ve versiyonu dosyaya yaz

                    Process.Start(psi); // Güncellemeyi baþlat

                    Application.Current.Exit(); // Uygulamayý kapat
                }
            }
        }

        private async void Yonetici_Click(object sender, RoutedEventArgs e) // Yönetici butonuna týklanýnca çalýþacak olay
        {
            var girisDialog = new ContentDialog // Giriþ penceresi oluþturuluyor
            {
                Title = "Yönetici Giriþi", // Baþlýk
                PrimaryButtonText = "Giriþ Yap", // Giriþ butonu
                CloseButtonText = "Ýptal", // Kapatma butonu
                DefaultButton = ContentDialogButton.Primary, // Varsayýlan buton
                XamlRoot = this.XamlRoot // XAML kökü
            };

            var kullaniciBox = new TextBox // Kullanýcý adý giriþ kutusu
            {
                PlaceholderText = "Kullanýcý Adý", // Yer tutucu yazý
                Margin = new Thickness(0, 0, 0, 10) // Alt boþluk
            };

            var sifreBox = new PasswordBox // Þifre kutusu
            {
                PlaceholderText = "Þifre" // Yer tutucu yazý
            };

            var stackPanel = new StackPanel(); // Giriþ kutularýný tutacak yatay dikey panel
            stackPanel.Children.Add(kullaniciBox); // Kullanýcý kutusu eklenir
            stackPanel.Children.Add(sifreBox); // Þifre kutusu eklenir

            girisDialog.Content = stackPanel; // Giriþ penceresine içerik olarak panel eklenir

            bool enterIleGiris = false; // Enter ile giriþ kontrolü için bayrak

            sifreBox.KeyDown += (s, args) => // Þifre kutusunda bir tuþa basýldýðýnda
            {
                if (args.Key == Windows.System.VirtualKey.Enter) // Eðer Enter tuþuna basýlmýþsa
                {
                    enterIleGiris = true; // Enter ile giriþ yapýlacak
                    girisDialog.Hide(); // Giriþ kutusunu kapat
                }
            };

            var sonuc = await girisDialog.ShowAsync(); // Giriþ kutusunu göster

            if (sonuc == ContentDialogResult.Primary || enterIleGiris) // "Giriþ Yap" veya Enter tuþu
            {
                var kullaniciAdi = kullaniciBox.Text.Trim(); // Kullanýcý adýný al
                var sifre = sifreBox.Password.Trim(); // Þifreyi al

                if ((kullaniciAdi == "admin" && sifre == "it123") || (kullaniciAdi == "Sehergunes" && sifre == "sg1234")) // Giriþ bilgileri doðruysa
                {
                    ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(YoneticiSayfasi)); // Yönetici sayfasýna geç
                }
                else // Giriþ bilgileri yanlýþsa
                {
                    ContentDialog hataDialog = new ContentDialog // Hata mesajý penceresi
                    {
                        Title = "Hatalý Giriþ", // Baþlýk
                        Content = "Kullanýcý adý veya þifre yanlýþ!", // Ýçerik
                        CloseButtonText = "Tamam", // Kapat butonu
                        XamlRoot = this.XamlRoot // XAML kökü
                    };
                    await hataDialog.ShowAsync(); // Hata mesajýný göster
                }
            }
        }

        private void Kullanici_Click(object sender, RoutedEventArgs e) // Kullanýcý butonuna týklanýnca
        {
            ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(KullaniciSayfasi)); // Kullanýcý sayfasýna geç
        }

        private void toggleTheme_Toggled(object sender, RoutedEventArgs e) // Tema deðiþtirici (toggle) çalýþtýðýnda
        {
            var isDark = toggleTheme.IsOn; // Seçili tema koyu mu?
            this.RequestedTheme = isDark ? ElementTheme.Dark : ElementTheme.Light; // Temayý ayarla
        }
    }
}
