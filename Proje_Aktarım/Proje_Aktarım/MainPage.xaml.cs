using System; // Temel .NET s�n�flar�n� i�eren namespace
using Microsoft.UI.Xaml; // XAML tabanl� UI olu�turmak i�in gerekli namespace
using Microsoft.UI.Xaml.Controls; // UI kontrollerini i�erir (Button, TextBox, Page vs.)
using Microsoft.UI.Xaml.Media.Imaging; // G�rs el ve bitmap i�lemleri i�in gerekli s�n�flar
using System.Diagnostics; // D�� i�lemler (Process.Start) ba�latmak i�in gerekli namespace
using System.IO; // Dosya i�lemleri (File.Exists, File.AppendAllText vs.) i�in
using Proje_Aktarim.Helpers; // Yard�mc� s�n�flar� i�eren kullan�c� tan�ml� namespace

namespace Proje_Aktarim // Projenin namespace�i
{
    public sealed partial class MainPage : Page // MainPage adl� UI sayfas�
    {
        public MainPage() // Constructor
        {
            this.InitializeComponent(); // XAML bile�enlerini ba�lat�r
            this.Loaded += MainPage_Loaded; // Sayfa y�klendi�inde �al��acak olay� tan�mlar

            BitmapImage bitmap = new BitmapImage(); // Yeni bir bitmap (g�rsel) nesnesi olu�turulur
            bitmap.DecodePixelWidth = 600; // G�rselin geni�li�i 600 piksel olarak ayarlan�r
            bitmap.UriSource = new Uri("ms-appx:///Assets/logo_optimized.png"); // G�rselin kayna�� ayarlan�r
            imgLogo.Source = bitmap; // imgLogo adl� UI ��esine g�rsel atan�r
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e) // Sayfa y�klendi�inde �al��acak olay
        {
            // Sayfa y�klendi�inde g�ncelleme kontrol� yap
            string updatePath = @"\\10.40.98.4\cm$\update.msix"; // G�ncelleme dosyas�n�n yolu

            if (File.Exists(updatePath)) // G�ncelleme dosyas� varsa
            {
                ContentDialog dialog = new() // Onay penceresi olu�turuluyor
                {
                    Title = "G�ncelleme Mevcut", // Ba�l�k
                    Content = "Yeni bir s�r�m bulundu. G�ncellemek ister misiniz?", // ��erik
                    PrimaryButtonText = "Evet", // Ana buton
                    CloseButtonText = "Hay�r", // Kapatma butonu
                    XamlRoot = this.XamlRoot // XAML a�ac�n�n k�k� (g�rsel ba�lama)
                };

                var result = await dialog.ShowAsync(); // Onay penceresini g�ster

                if (result == ContentDialogResult.Primary) // Kullan�c� "Evet" dediyse
                {
                    var psi = new ProcessStartInfo(updatePath) // G�ncelleme dosyas�n� �al��t�rmak i�in ayar
                    {
                        UseShellExecute = true // Shell �zerinden �al��t�rma izni
                    };
                    File.AppendAllText(@"\\10.40.98.4\cm$\guncelleme_tarih.txt", $"G�ncelleme: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Versiyon: {Versiyon.Mevcut}{Environment.NewLine}"); // G�ncelleme tarihini ve versiyonu dosyaya yaz

                    Process.Start(psi); // G�ncellemeyi ba�lat

                    Application.Current.Exit(); // Uygulamay� kapat
                }
            }
        }

        private async void Yonetici_Click(object sender, RoutedEventArgs e) // Y�netici butonuna t�klan�nca �al��acak olay
        {
            var girisDialog = new ContentDialog // Giri� penceresi olu�turuluyor
            {
                Title = "Y�netici Giri�i", // Ba�l�k
                PrimaryButtonText = "Giri� Yap", // Giri� butonu
                CloseButtonText = "�ptal", // Kapatma butonu
                DefaultButton = ContentDialogButton.Primary, // Varsay�lan buton
                XamlRoot = this.XamlRoot // XAML k�k�
            };

            var kullaniciBox = new TextBox // Kullan�c� ad� giri� kutusu
            {
                PlaceholderText = "Kullan�c� Ad�", // Yer tutucu yaz�
                Margin = new Thickness(0, 0, 0, 10) // Alt bo�luk
            };

            var sifreBox = new PasswordBox // �ifre kutusu
            {
                PlaceholderText = "�ifre" // Yer tutucu yaz�
            };

            var stackPanel = new StackPanel(); // Giri� kutular�n� tutacak yatay dikey panel
            stackPanel.Children.Add(kullaniciBox); // Kullan�c� kutusu eklenir
            stackPanel.Children.Add(sifreBox); // �ifre kutusu eklenir

            girisDialog.Content = stackPanel; // Giri� penceresine i�erik olarak panel eklenir

            bool enterIleGiris = false; // Enter ile giri� kontrol� i�in bayrak

            sifreBox.KeyDown += (s, args) => // �ifre kutusunda bir tu�a bas�ld���nda
            {
                if (args.Key == Windows.System.VirtualKey.Enter) // E�er Enter tu�una bas�lm��sa
                {
                    enterIleGiris = true; // Enter ile giri� yap�lacak
                    girisDialog.Hide(); // Giri� kutusunu kapat
                }
            };

            var sonuc = await girisDialog.ShowAsync(); // Giri� kutusunu g�ster

            if (sonuc == ContentDialogResult.Primary || enterIleGiris) // "Giri� Yap" veya Enter tu�u
            {
                var kullaniciAdi = kullaniciBox.Text.Trim(); // Kullan�c� ad�n� al
                var sifre = sifreBox.Password.Trim(); // �ifreyi al

                if ((kullaniciAdi == "admin" && sifre == "it123") || (kullaniciAdi == "Sehergunes" && sifre == "sg1234")) // Giri� bilgileri do�ruysa
                {
                    ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(YoneticiSayfasi)); // Y�netici sayfas�na ge�
                }
                else // Giri� bilgileri yanl��sa
                {
                    ContentDialog hataDialog = new ContentDialog // Hata mesaj� penceresi
                    {
                        Title = "Hatal� Giri�", // Ba�l�k
                        Content = "Kullan�c� ad� veya �ifre yanl��!", // ��erik
                        CloseButtonText = "Tamam", // Kapat butonu
                        XamlRoot = this.XamlRoot // XAML k�k�
                    };
                    await hataDialog.ShowAsync(); // Hata mesaj�n� g�ster
                }
            }
        }

        private void Kullanici_Click(object sender, RoutedEventArgs e) // Kullan�c� butonuna t�klan�nca
        {
            ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(KullaniciSayfasi)); // Kullan�c� sayfas�na ge�
        }

        private void toggleTheme_Toggled(object sender, RoutedEventArgs e) // Tema de�i�tirici (toggle) �al��t���nda
        {
            var isDark = toggleTheme.IsOn; // Se�ili tema koyu mu?
            this.RequestedTheme = isDark ? ElementTheme.Dark : ElementTheme.Light; // Temay� ayarla
        }
    }
}
