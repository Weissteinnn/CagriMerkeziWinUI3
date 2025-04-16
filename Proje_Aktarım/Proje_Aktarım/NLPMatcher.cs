using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML; // NLP için ML.NET kütüphanesi
using Microsoft.ML.Data; // ML.NET veri yapılarını kullanmak için
using System.Text;
using System.Threading.Tasks;

public class SoruData
{
    public string Soru { get; set; } // NLP tarafından analiz edilecek metin (soru)
}

public class SoruVector // Sorunun vektör temsili (NLP modeli tarafından oluşturulur)
{
    [VectorType(FeaturizedSoruLength)]
    public float[] Features { get; set; }

    public const int FeaturizedSoruLength = 600;
}

public class MatchResult // Eşleşme sonuçlarını (soru ve benzerlik oranı) tutan sınıf
{
    public string Soru { get; set; } // Eşleşen soru
    public float Similarity { get; set; } // Benzerlik skoru (0-1 arasında)
}

public class NLPMatcher // NLP eşleştirme işlemini yapan temel sınıf
{
    private MLContext mlContext; // ML.NET temel nesnesi
    private PredictionEngine<SoruData, SoruVector> predictionEngine; // Tahmin motoru
    // Yapıcı metot (constructor)
    // Verilen sorular listesini kullanarak NLP modelini oluşturur
    public NLPMatcher(List<string> sorular)
    {
        mlContext = new MLContext(); // MLContext nesnesini oluştur

        var data = sorular.Select(s => new SoruData { Soru = s }); // Gelen soruları ML.NET modelinin kullanabileceği hale getir
        var trainData = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Text.FeaturizeText( // NLP pipeline oluşturulur (metni sayısal vektöre dönüştürür)
            outputColumnName: "Features", inputColumnName: nameof(SoruData.Soru));

        var model = pipeline.Fit(trainData);  // Modeli eğitir ve oluşturur
        predictionEngine = mlContext.Model.CreatePredictionEngine<SoruData, SoruVector>(model); // Tahmin (prediction) motorunu oluşturur
    }

    public List<MatchResult> GetTopMatches(string kullaniciSorusu, List<string> sorular, int topN = 3) // Kullanıcının sorusu için en yakın 'topN' eşleşen soruları bulur
    {
        var kullaniciVector = predictionEngine.Predict(new SoruData { Soru = kullaniciSorusu }).Features; // Kullanıcının sorusunu vektör haline getirir

        List<MatchResult> similarities = new List<MatchResult>(); // Eşleşme sonuçlarını tutmak için liste oluştururuz

        foreach (var soru in sorular) // Tüm sorularla kullanıcı sorusu arasında benzerliği hesaplar
        {
            var soruVector = predictionEngine.Predict(new SoruData { Soru = soru }).Features;
            float similarity = CosineSimilarity(kullaniciVector, soruVector); // Kosinüs benzerliğini hesaplayıp skoru elde ederiz

            similarities.Add(new MatchResult // Hesaplanan benzerliği listeye ekler
            {
                Soru = soru,
                Similarity = similarity
            });
        }

        return similarities.OrderByDescending(x => x.Similarity).Take(topN).ToList(); // Benzerlik skoruna göre sıralar ve en iyi sonuçları döndürür
    }

    private float CosineSimilarity(float[] vec1, float[] vec2)
    {
        float dot = 0, mag1 = 0, mag2 = 0;

        for (int i = 0; i < vec1.Length; i++)
        {
            dot += vec1[i] * vec2[i];
            mag1 += vec1[i] * vec1[i];
            mag2 += vec2[i] * vec2[i];
        }

        float denominator = (float)(Math.Sqrt(mag1) * Math.Sqrt(mag2));

        // Sıfıra bölme hatasını önlemek için basit kontrol
        if (denominator == 0)
            return 0;  // sıfıra bölme varsa benzerliği 0 olarak kabul et

        return dot / denominator;
    }

}
