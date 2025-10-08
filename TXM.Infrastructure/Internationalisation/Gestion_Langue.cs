using System.Globalization;

namespace TXM.Infrastructure.Internationalisation
    {
    public static class Gestion_Langue
        {
        public static void Initialiser(string code)
            {
            var culture = new CultureInfo(code);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            // Le dictionnaire FR.xaml est déjà mergé dans App.xaml (par défaut).
            // Pour basculer dynamiquement, on remplacera le ResourceDictionary à chaud.
            }
        }
    }
