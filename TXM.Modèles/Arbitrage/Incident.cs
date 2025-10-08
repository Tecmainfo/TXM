namespace TXM.Modèles.Arbitrage
{
    public sealed class Incident
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string Description { get; init; } = "";
        public string Gravité { get; init; } = "Mineure";
        public int? Id_Équipe { get; init; }
        public int? Id_Joueur { get; init; }

        // Nouveau
        public DateTime Horodatage { get; init; } = DateTime.Now;
        public string Arbitre { get; init; } = "";
    }
}
