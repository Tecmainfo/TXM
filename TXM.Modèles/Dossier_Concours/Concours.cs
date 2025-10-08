namespace TXM.Modèles.Dossier_Concours
    {
    public sealed class Concours
        {
        public int Id { get; init; }
        public string Nom { get; init; } = "";
        public DateTime Date { get; init; }
        public string Type { get; init; } = "Amical"; // Amical / Officiel
        }
    }
