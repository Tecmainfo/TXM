namespace TXM.Modèles
{
    public sealed class Poule
    {
        public int Id { get; init; }
        public string Nom { get; init; } = "";
        public int IdConcours { get; init; }
        public DateTime HeureDébut { get; set; }
    }
}
