namespace TXM.Modèles
{
    public sealed class Rapport
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string Type { get; init; } = "";
        public string Utilisateur { get; init; } = "";
        public string NomFichier { get; init; } = "";
        public string Fichier => NomFichier;
    }
}
