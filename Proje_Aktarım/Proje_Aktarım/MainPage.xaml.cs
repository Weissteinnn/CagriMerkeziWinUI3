using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Proje_Aktarim
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            BitmapImage bitmap = new BitmapImage();
            bitmap.DecodePixelWidth = 600; 
            bitmap.UriSource = new Uri("ms-appx:///Assets/logo_optimized.png");
            imgLogo.Source = bitmap;

        }

        private async void Yonetici_Click(object sender, RoutedEventArgs e)
        {
            var girisDialog = new ContentDialog
            {
                Title = "Yönetici Giriþi",
                PrimaryButtonText = "Giriþ Yap",
                CloseButtonText = "Ýptal",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var kullaniciBox = new TextBox
            {
                PlaceholderText = "Kullanýcý Adý",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var sifreBox = new PasswordBox
            {
                PlaceholderText = "Þifre"
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(kullaniciBox);
            stackPanel.Children.Add(sifreBox);

            girisDialog.Content = stackPanel;

            bool enterIleGiris = false;

            sifreBox.KeyDown += (s, args) =>
            {
                if (args.Key == Windows.System.VirtualKey.Enter)
                {
                    enterIleGiris = true;
                    girisDialog.Hide();
                }
            };

            var sonuc = await girisDialog.ShowAsync();

            if (sonuc == ContentDialogResult.Primary || enterIleGiris)
            {
                var kullaniciAdi = kullaniciBox.Text.Trim();
                var sifre = sifreBox.Password.Trim();

                if ((kullaniciAdi == "admin" && sifre == "it123") || (kullaniciAdi == "Sehergunes" && sifre == "sg1234"))
                {
                    ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(YoneticiSayfasi));
                }
                else
                {
                    ContentDialog hataDialog = new ContentDialog
                    {
                        Title = "Hatalý Giriþ",
                        Content = "Kullanýcý adý veya þifre yanlýþ!",
                        CloseButtonText = "Tamam",
                        XamlRoot = this.XamlRoot
                    };
                    await hataDialog.ShowAsync();
                }
            }
        }

        private void Kullanici_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(KullaniciSayfasi));
        }

        private void toggleTheme_Toggled(object sender, RoutedEventArgs e)
        {
            var isDark = toggleTheme.IsOn;
            this.RequestedTheme = isDark ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
