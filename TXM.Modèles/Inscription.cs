namespace TXM.Modèles
{
    public sealed class Inscription
    {
        public int Id { get; init; }
        public string NomÉquipe { get; set; } = "";
        public string Joueurs { get; set; } = "";   // liste simple (séparée par virgule)
        public int IdConcours { get; set; }
        public string NomConcours { get; set; } = "";
    }
}
