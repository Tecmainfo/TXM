namespace TXM.Données.Configuration
    {
    public static class Configuration_Base_de_données
        {
        public static string Dossier_App => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TXM");

        public static string Fichier_DB => Path.Combine(Dossier_App, "txm.sqlite");

        public static string Chaine_Connexion =>
            $"Data Source={Fichier_DB};Cache=Shared;Pooling=True;";

        public static void Assurer_Dossier()
            {
            if (!Directory.Exists(Dossier_App))
                {
                _ = Directory.CreateDirectory(Dossier_App);
                }
            }
        }
    }
