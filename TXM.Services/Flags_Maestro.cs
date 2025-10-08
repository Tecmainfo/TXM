namespace TXM.Services
{
    /// <summary>
    /// Ensemble des fonctionnalités activées pour l’édition
    /// <b>Pétanque Maestro</b>.
    /// </summary>
    public sealed class Flags_Maestro : IFeature_Flags
        {
        /// <inheritdoc/>
        public bool Export_Homologue => true;

        /// <inheritdoc/>
        public bool Mode_Lyonnaise => true;

        /// <inheritdoc/>
        public bool Multi_Site => true;
        }
}
