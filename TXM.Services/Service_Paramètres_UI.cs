namespace TXM.Services
    {
    public class Service_Paramètres_UI
        {
        public bool Ignorer_Écran_Accueil { get; set; } = false;
        public string Langue { get; set; } = "FR";
        public string Licence_Type { get; set; } = "Non activée";
        public DateTime Licence_Expiration { get; set; } = DateTime.MinValue;

        // 🎨 Nouveau
        public string Effet_Fluent { get; set; } = "Mica";
        }
    }
