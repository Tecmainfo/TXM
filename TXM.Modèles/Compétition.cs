namespace TXM.Modèles
{
    public sealed class Compétition
    {
        public int Id { get; init; }
        public string Nom { get; init; } = "";
        public string Lieu { get; init; } = "";
        public DateTime Date { get; init; }
        public string Statut { get; init; } = "Prévue";
    }
}
