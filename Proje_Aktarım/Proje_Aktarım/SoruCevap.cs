using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CagriMerkeziSSS_Yonetici
{
    public class SoruCevap
    {
        public int id { get; set; } //benzersiz ID
        public string soru { get; set; } //soru metni 
        public string cevap { get; set; } //cevap metni 
    }
}