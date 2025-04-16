using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Proje_Aktarim
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Yonetici_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(YoneticiSayfasi));
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
