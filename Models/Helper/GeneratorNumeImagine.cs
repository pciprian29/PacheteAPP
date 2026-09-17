using System.Diagnostics.CodeAnalysis;
using System.Text;
namespace PacheteAPP.Models.Helper
{
    public class GeneratorNumeImagine
    {
        private const string CaractereRandom = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int LungimeRandom = 8;
        private const string ExtensieFisier = ".png";

        public static string Genereaza(string? awb, int idEntitate, int idTipImagine, DateTime dataCreare)
        {
            string identificator = !string.IsNullOrWhiteSpace(awb)
                ? awb
                : $"FARAAWB-{idEntitate}";

            string data = dataCreare.ToString("ddMMyyyy");
            string random = GenereazaCodRandom(LungimeRandom);
            string litera = idTipImagine switch
            {
                1 => "I",
                2 => "D",
                _ => throw new ArgumentOutOfRangeException(nameof(idTipImagine), idTipImagine, "Tip de entitate necunoscut")
            };

            return $"{identificator}-{data}-{random}-{litera}{ExtensieFisier}";
        }

        public static string GenereazaCodRandom(int lungime)
        {
            var sb = new StringBuilder(lungime);
            for (int i = 0; i < lungime; i++)
            {
                int index = Random.Shared.Next(CaractereRandom.Length);
                sb.Append(CaractereRandom[index]);
            }
            return sb.ToString();
        }
    }
}
