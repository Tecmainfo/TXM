namespace TXM.Services
{
    /// <summary>
    /// Attribut permettant de restreindre l’accès à une classe ou une méthode
    /// en fonction d’un drapeau fonctionnel (<c>feature flag</c>) actif.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de l’attribut <see cref="Feature_Flag_Attribute"/>.
    /// </remarks>
    /// <param name="nom">Nom du drapeau fonctionnel.</param>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class Feature_Flag_Attribute(string nom) : Attribute
        {
        /// <summary>
        /// Nom du drapeau fonctionnel requis pour activer la fonctionnalité.
        /// </summary>
        public string Nom { get; } = nom;
        }
}
