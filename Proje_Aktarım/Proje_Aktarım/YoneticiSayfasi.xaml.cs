using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CagriMerkeziSSS_Yonetici;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Proje_Aktarim
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class YoneticiSayfasi : Page
    {

        private string ortakKlasor = @"\\10.40.98.4\cm$";
        private string jsonDosya => Path.Combine(ortakKlasor, "sorubunlar.json");
        private string tarihDosya => Path.Combine(ortakKlasor, "guncelleme_tarih.txt");

        private List<SoruCevap> sorular = new();
        public YoneticiSayfasi()
        {
            this.InitializeComponent();
            this.Loaded += YoneticiSayfasi_Loaded;
        }

        private void YoneticiSayfasi_Loaded(object sender, RoutedEventArgs e)
        {
            VerileriYukle();
        }

        private void VerileriYukle()
        {
            if (File.Exists(jsonDosya))
            {
                var json = File.ReadAllText(jsonDosya);
                sorular = JsonConvert.DeserializeObject<List<SoruCevap>>(json) ?? new();
                ListeYenile();
            }
        }

        private void ListeYenile()
        {
            lvSorular.ItemsSource = null;
            lvSorular.ItemsSource = sorular.Select(s => s.soru).ToList();
        }


        private void BtnKaydet_Click(object sender, RoutedEventArgs e)
        {
            var soru = txtSoru.Text.Trim();
            var cevap = txtCevap.Text.Trim();

            if (string.IsNullOrWhiteSpace(soru) || string.IsNullOrWhiteSpace(cevap))
            {
                ShowMsg("Lütfen hem soru hem cevap alanını doldurun.");
                return;
            }

            int secilenIndex = lvSorular.SelectedIndex;

            if (secilenIndex >= 0 && secilenIndex < sorular.Count)
            {
                // Seçili soru varsa, güncelleme yap
                sorular[secilenIndex].soru = soru;
                sorular[secilenIndex].cevap = cevap;

                ShowMsg("Soru başarıyla güncellendi.");
            }
            else
            {
                // Seçili soru yoksa, yeni ekle
                if (sorular.Any(s => s.soru.Equals(soru, StringComparison.OrdinalIgnoreCase)))
                {
                    ShowMsg("Bu soru zaten listede mevcut.");
                    return;
                }

                sorular.Add(new SoruCevap
                {
                    id = sorular.Count + 1,
                    soru = soru,
                    cevap = cevap
                });

                ShowMsg("Yeni soru başarıyla eklendi.");
            }

            KaydetJSON();
            ListeYenile();
            txtSoru.Text = "";
            txtCevap.Text = "";
            lvSorular.SelectedIndex = -1;
        }

        private void KaydetJSON()
        {
            File.WriteAllText(jsonDosya, JsonConvert.SerializeObject(sorular, Formatting.Indented));
            File.WriteAllText(tarihDosya, "Sorular güncellendi: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
        }

        private void lvSorular_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lvSorular.SelectedIndex;
            if (index >= 0 && index < sorular.Count)
            {
                txtSoru.Text = sorular[index].soru;
                txtCevap.Text = sorular[index].cevap;
            }
        }

        private async void BtnSil_Click(object sender, RoutedEventArgs e)
        {
            int index = lvSorular.SelectedIndex;
            if (index >= 0)
            {
                var secilen = sorular[index].soru;

                ContentDialog dialog = new()
                {
                    Title = "Soruyu Sil",
                    Content = $"“{secilen}” sorusunu silmek istiyor musunuz?",
                    PrimaryButtonText = "Evet",
                    CloseButtonText = "Hayır",
                    XamlRoot = this.XamlRoot
                };

                dialog.PrimaryButtonClick += (_, _) =>
                {
                    sorular.RemoveAt(index);

                    // ID'leri yeniden sırala
                    for (int i = 0; i < sorular.Count; i++)
                        sorular[i].id = i + 1;

                    KaydetJSON();
                    ListeYenile();
                    txtSoru.Text = "";
                    txtCevap.Text = "";
                    ShowMsg("Soru silindi.");
                };

                _ = dialog.ShowAsync();
            }
            else
            {
                ShowMsg("Lütfen silmek istediğiniz bir soruyu seçin.");
            }
        }

        private void BtnSecimiKaldir_Click(object sender, RoutedEventArgs e)
        {
            lvSorular.SelectedIndex = -1;
            txtSoru.Text = "";
            txtCevap.Text = "";
        }

        private void BtnAnaSayfa_Click(object sender, RoutedEventArgs e)
        {

            ((App)Application.Current).MainWindowRef?.NavigateToPage(typeof(MainPage));

        }


        private async Task ShowMsg(string mesaj)
        {
            var dialog = new ContentDialog
            {
                Title = "Bilgi",
                Content = mesaj,
                CloseButtonText = "Tamam",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
