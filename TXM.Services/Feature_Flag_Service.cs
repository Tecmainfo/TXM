namespace TXM.Services
{
    /// <summary>
    /// Service permettant d’activer/désactiver certaines fonctionnalités
    /// en fonction du branding/licence (TripleX ou Pétanque Maestro).
    /// </summary>
    public static class Feature_Flag_Service
        {
        /// <summary>
        /// Ensemble des drapeaux fonctionnels actifs.
        /// </summary>
        public static IFeature_Flags Flags { get; private set; } = new Flags_TripleX();

        /// <summary>
        /// Initialise les drapeaux en fonction de la clé de marque fournie.
        /// </summary>
        /// <param name="brandKey">Identifiant du branding (ex. "triplex" ou "maestro").</param>
        public static void Initialiser(string brandKey)
            {
            Flags = brandKey.ToLowerInvariant() switch
                {
                    "maestro" => new Flags_Maestro(),
                    _ => new Flags_TripleX()
                    };
            }
        }
}
