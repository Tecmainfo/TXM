namespace TXM.Infrastructure.Branding
    {
    /// <summary>
    /// Branding appliqué lorsque la licence est absente ou invalide.
    /// Couleurs spécifiques rouge/gris pour signaler le mode Démo.
    /// </summary>
    public sealed class Démo_Branding : IBranding_Provider
        {
        public string Nom_Produit => "TXM Démo";
        public string Slogan => "Version Démonstration – Licence absente";

        // 🔹 Corrigé : URI pleinement défini (pack:// pour WPF)
        public Uri Logo_Uri => new("pack://application:,,,/Ressources/logos/logo-txm.png", UriKind.Absolute);

        public string Couleur_Primaire_Hex => "#FF3B30";     // Rouge alerte
        public string Couleur_Secondaire_Hex => "#9E9E9E";   // Gris neutre
        }
    }
