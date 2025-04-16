using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Proje_Aktarim;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Proje_Aktarim
{
    public sealed partial class KullaniciSayfasi : Page
    {
        private string ortakKlasor = @"D:\test_cagri_merkezi\";
        private List<SoruCevap> sorular = new();
        private NLPMatcher matcher;

        public KullaniciSayfasi()
        {
            this.InitializeComponent();
            YukleVeri();
        }

        private void YukleVeri()
        {
            string yol = Path.Combine(ortakKlasor, "sorubunlar.json");

            if (File.Exists(yol))
            {
                var json = File.ReadAllText(yol);
                sorular = JsonConvert.DeserializeObject<List<SoruCevap>>(json);
                matcher = new NLPMatcher(sorular.Select(s => s.soru).ToList());
            }
        }

        private void btnAra_Click(object sender, RoutedEventArgs e)
        {
            var girilen = txtSoru.Text.Trim();
            if (string.IsNullOrWhiteSpace(girilen)) return;

            var sonuclar = matcher.GetTopMatches(girilen, sorular.Select(s => s.soru).ToList(), 3);

            lstSonuclar.ItemsSource = sonuclar
                .Select(s => $"{s.Soru} ({s.Similarity:P0})")
                .ToList();

            txtCevap.Text = "";
        }

        private void lstSonuclar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSonuclar.SelectedIndex >= 0)
            {
                var secilen = lstSonuclar.SelectedItem.ToString().Split('(')[0].Trim();
                var cevap = sorular.FirstOrDefault(s => s.soru == secilen)?.cevap;
                txtCevap.Text = cevap;
            }
        }

        private void txtSoru_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                btnAra_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
