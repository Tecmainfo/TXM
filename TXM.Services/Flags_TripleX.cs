namespace TXM.Services
{
    /// <summary>
    /// Définition des fonctionnalités activées dans l’édition
    /// <b>TripleX</b> (licence village/association).
    /// </summary>
    public sealed class Flags_TripleX : IFeature_Flags
        {
        /// <inheritdoc />
        public bool Export_Homologue => false;

        /// <inheritdoc />
        public bool Mode_Lyonnaise => true;

        /// <inheritdoc />
        public bool Multi_Site => false;
        }
}
