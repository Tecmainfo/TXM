namespace TXM.Modèles
{
    public sealed class HistoriqueHomologation
    {
        public int Id { get; init; }
        public int IdConcours { get; init; }
        public DateTime DateAction { get; init; }
        public string Décision { get; init; } = "";
        public string Arbitre { get; init; } = "";
        public string Commentaire { get; init; } = "";
    }
}
