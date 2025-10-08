namespace TXM.Infrastructure.Branding
{
    public sealed class Maestro_Branding : IBranding_Provider
        {
        public string Nom_Produit => "Pétanque Maestro";
        public string Slogan => "Gestion officielle FFPJP & international";
        public Uri Logo_Uri => new("pack://application:,,,/Ressources/logos/logo-maestro.png", UriKind.Absolute);
        public string Couleur_Primaire_Hex => "#009688";
        public string Couleur_Secondaire_Hex => "#E91E63";
        }
}
