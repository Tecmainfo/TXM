namespace TXM.Modèles
    {
    /// <summary>
    /// Représente les paramètres globaux de l’application TripleX / Pétanque Maestro.
    /// </summary>
    public class Paramètres_Application
        {
        public bool Skip_Splash { get; set; } = false;
        public string Langue { get; set; } = "FR";
        public string Licence_Type { get; set; } = "Non activée";
        public DateTime Licence_Expiration { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Type d’effet Fluent/Mica à appliquer (Windows 11 uniquement)
        /// </summary>
        public string Effet_Fluent { get; set; } = "Mica"; // valeurs possibles : "Mica", "Acrylic", "Aucun"
        }
    }
