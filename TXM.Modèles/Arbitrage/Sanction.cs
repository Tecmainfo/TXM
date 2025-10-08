namespace TXM.Modèles.Arbitrage
{
    public sealed class Sanction
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string Type { get; init; } = "";
        public string Motif { get; init; } = "";
        public int? Id_Incident { get; init; }
        public string Article_Règlement { get; init; } = ""; // ex: "Art. 32 - Retard"
        public string Arbitre { get; init; } = "";
        public int Durée_Minutes { get; init; } // utile pour exclusions/temps
        public int? IdConcours { get; set; }
    }
}
