namespace TXM.Services
{
    /// <summary>
    /// Interface définissant les fonctionnalités activables/désactivables
    /// selon la licence active (TripleX ou Maestro).
    /// </summary>
    public interface IFeature_Flags
        {
        /// <summary>
        /// Indique si l’export homologué (FFPJP/Officiel) est disponible.
        /// </summary>
        bool Export_Homologue { get; }

        /// <summary>
        /// Indique si le mode Lyonnaise est activé.
        /// </summary>
        bool Mode_Lyonnaise { get; }

        /// <summary>
        /// Indique si le multi-site est pris en charge.
        /// </summary>
        bool Multi_Site { get; }
        }
}
