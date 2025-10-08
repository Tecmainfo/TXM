namespace TXM.Modèles
{
    public sealed class Utilisateur
    {
        public int Id { get; init; }
        public string Nom { get; init; } = "";
        public string Email { get; init; } = "";
        public string Rôle { get; init; } = "";
        public string MotDePasseHash { get; init; } = "";
    }
}
