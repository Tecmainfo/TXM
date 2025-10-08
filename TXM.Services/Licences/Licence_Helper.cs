namespace TXM.Services.Licences
    {
    /// <summary>
    /// Fournit des helpers simples pour interroger le type de licence active.
    /// </summary>
    public static class Licence_Helper
        {
        /// <summary>
        /// Retourne vrai si la licence active correspond à TripleX (Association).
        /// </summary>
        public static bool Est_TripleX =>
            Service_Licence.LicenceActuelle.Type == TypeLicence.TripleX;

        /// <summary>
        /// Retourne vrai si la licence active correspond à Pétanque Maestro (Compétition).
        /// </summary>
        public static bool Est_Maestro =>
            Service_Licence.LicenceActuelle.Type == TypeLicence.Maestro;

        /// <summary>
        /// Retourne vrai si la licence active est MultiSite / Grands Tournois.
        /// (clé contenant "MULTI" ou branding TXM.GrandsTournois actif)
        /// </summary>
        public static bool Est_MultiSite =>
            Service_Licence.LicenceActuelle.Type == TypeLicence.MultiSite;

        /// <summary>
        /// Retourne vrai si la licence active est une licence Démo.
        /// </summary>
        public static bool Est_Démo =>
            Service_Licence.LicenceActuelle.Type == TypeLicence.Demo;

        /// <summary>
        /// Indique si la licence active est expirée ou invalide.
        /// </summary>
        public static bool Est_Expirée =>
            Service_Licence.LicenceActuelle.Type == TypeLicence.Demo &&
            Service_Licence.EstEnModeRestreint();

        /// <summary>
        /// Retourne un texte lisible décrivant le type de licence actuelle.
        /// </summary>
        public static string Libellé =>
            Service_Licence.LicenceActuelle.Type switch
                {
                    TypeLicence.MultiSite => "Grands Tournois (Multi-site)",
                    TypeLicence.TripleX => "TripleX (Association)",
                    TypeLicence.Maestro => "Pétanque Maestro (Compétition)",
                    _ => "TXM Démo"
                    };
        }
    }
