namespace TXM.Infrastructure.Branding
    {
    public sealed class TripleX_Branding : IBranding_Provider
        {
        public string Nom_Produit => "TripleX";
        public string Slogan => "Concours de pétanque village & associations";
        public Uri Logo_Uri => new("pack://application:,,,/Ressources/logos/logo-triplex.png", UriKind.Absolute);
        public string Couleur_Primaire_Hex => "#3F51B5";
        public string Couleur_Secondaire_Hex => "#FFC107";
        }
}
