using System; // Temel .NET işlevleri için namespace
using System.Collections.Generic; // Liste gibi koleksiyon sınıfları için gerekli
using System.IO; // Dosya işlemleri için kullanılır
using System.Linq; // LINQ sorguları için gerekli namespace
using System.Runtime.InteropServices.WindowsRuntime; // Windows Runtime ile etkileşim için
using CagriMerkeziSSS_Yonetici; // SoruCevap ve NLPMatcher gibi özel sınıflar bu namespace’te
using Microsoft.UI.Xaml; // WinUI temel sınıflar
using Microsoft.UI.Xaml.Controls; // UI kontrolleri (Button, TextBox, ListBox vs.)
using Microsoft.UI.Xaml.Controls.Primitives; // Gelişmiş kontroller için
using Microsoft.UI.Xaml.Data; // Veri bağlama işlemleri için gerekli
using Microsoft.UI.Xaml.Input; // Giriş olayları (KeyDown vs.)
using Microsoft.UI.Xaml.Media; // Görsel (renk, fırça vs.) işlemleri
using Microsoft.UI.Xaml.Navigation; // Sayfa yönlendirme işlevleri için
using Newtonsoft.Json; // JSON serileştirme ve seriden çıkarma işlemleri
using Windows.Foundation; // Temel Windows yapılarını içerir
using Windows.Foundation.Collections; // Koleksiyonlar için Windows Foundation sınıfları
using Proje_Aktarim; // Proje içi sınıflar için namespace

// Kullanıcı sayfası, sorulara göre cevap arama işlevi sunar
namespace Proje_Aktarim
{
    public sealed partial class KullaniciSayfasi : Page // Sayfa sınıfı, sealed olduğu için kalıtılamaz
    {
        private string ortakKlasor = @"\\10.40.98.4\cm$\"; // JSON dosyasının bulunduğu klasör yolu
        private List<SoruCevap> sorular = new(); // Tüm soru-cevapları tutacak liste
        private NLPMatcher matcher; // NLP tabanlı eşleştirme sınıfı

        public KullaniciSayfasi() // Constructor
        {
            this.InitializeComponent(); // XAML bileşenlerini başlatır
            YukleVeri(); // Sayfa yüklenince verileri getir
        }

        private void YukleVeri() // JSON dosyasından verileri yükler
        {
            string yol = Path.Combine(ortakKlasor, "sorubunlar.json"); // JSON dosyasının tam yolu

            if (File.Exists(yol)) // Dosya var mı kontrolü
            {
                var json = File.ReadAllText(yol); // Dosyayı oku
                sorular = JsonConvert.DeserializeObject<List<SoruCevap>>(json); // JSON'dan listeye çevir
                matcher = new NLPMatcher(sorular.Select(s => s.soru).ToList()); // Soruları NLP sınıfına aktar
            }
        }

        private void btnAra_Click(object sender, RoutedEventArgs e) // "Ara" butonuna tıklanma olayı
        {
            var girilen = txtSoru.Text.Trim(); // Kullanıcının yazdığı metni al ve boşlukları temizle
            if (string.IsNullOrWhiteSpace(girilen)) return; // Boşsa çık

            var sonuclar = matcher.GetTopMatches(girilen, sorular.Select(s => s.soru).ToList(), 3); // En uygun 3 sonucu getir

            lstSonuclar.ItemsSource = sonuclar
                .Select(s => $"{s.Soru} ({s.Similarity:P0})") // Eşleşen soruları benzerlik oranıyla birlikte göster
                .ToList();

            txtCevap.Text = ""; // Cevap kutusunu temizle
        }

        private void lstSonuclar_SelectionChanged(object sender, SelectionChangedEventArgs e) // Listeden seçim yapılınca
        {
            if (lstSonuclar.SelectedIndex >= 0) // Eğer bir öğe seçildiyse
            {
                var secilen = lstSonuclar.SelectedItem.ToString().Split('(')[0].Trim(); // Parantezden önceki kısmı (soru) al
                var cevap = sorular.FirstOrDefault(s => s.soru == secilen)?.cevap; // Seçilen soruya ait cevabı bul
                txtCevap.Text = cevap; // Cevabı kutuya yaz
            }
        }

        private void txtSoru_KeyDown(object sender, KeyRoutedEventArgs e) // Soru kutusunda tuşlara basıldığında
        {
            if (e.Key == Windows.System.VirtualKey.Enter) // Eğer Enter'a basıldıysa
            {
                btnAra_Click(sender, e); // "Ara" işlemini tetikle
                e.Handled = true; // Olayı işlenmiş olarak işaretle
            }
        }

        private void BtnkAnaSayfa_Click(object sender, RoutedEventArgs e) // "Ana Sayfa" butonuna tıklanınca
        {
            ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(MainPage)); // Ana sayfaya geçiş yap
        }
    }
}
