namespace TXM.Modèles
{
    public sealed class Règlement
    {
        public int Id { get; init; }
        public int Année { get; init; }
        public string Titre { get; init; } = "";
        public string Catégorie { get; init; } = "";
        public string Version { get; init; } = "";         // ex. "2025-01", ou vide
        public string Source_Url { get; init; } = "";       // si importé d’une URL
        public string SourceUrl => Source_Url;
        public string Fichier_Local { get; init; } = "";    // chemin absolu vers le PDF local
        public DateTime? Date_Publication { get; init; }    // si connue
        public DateTime Date_Ajout { get; init; }           // quand on l’a archivé
        public string Hash { get; init; } = "";             // SHA256 pour éviter les doublons
    }
}
