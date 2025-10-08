namespace TXM.Infrastructure.Branding
    {
    public static class Branding_Ressource_Service
        {
        public static void Charger_Dans_Ressources(Application app)
            {
            IBranding_Provider b = Service_Branding.Courant;

            ResourceDictionary dic = new()
                {
                { "Branding.Nom_Produit", b.Nom_Produit },
                { "Branding.Slogan", b.Slogan },
                { "Branding.Logo_Uri", b.Logo_Uri },
                { "Branding.Couleur_Primaire", (SolidColorBrush)new BrushConverter().ConvertFromString(b.Couleur_Primaire_Hex)! },
                { "Branding.Couleur_Secondaire", (SolidColorBrush)new BrushConverter().ConvertFromString(b.Couleur_Secondaire_Hex)! }
            };

            // Supprimer ancienne version si déjà existante
            ResourceDictionary? existant = app.Resources.MergedDictionaries.FirstOrDefault(rd => rd.Contains("Branding.Nom_Produit"));
            if (existant != null)
                {
                _ = app.Resources.MergedDictionaries.Remove(existant);
                }

            app.Resources.MergedDictionaries.Add(dic);
            }
        }
    }
