namespace TXM.Modèles.Joueurs
    {
    /// <summary>
    /// Modèle de joueur – commun à TripleX et Pétanque Maestro.
    /// Les champs fédéraux (catégorie, sexe, etc.) sont facultatifs
    /// et activés uniquement pour Maestro via les Feature Flags.
    /// </summary>
    public sealed class Joueur
        {
        public int Id { get; init; }

        // Champs de base (communs)
        public string Nom { get; set; } = "";
        public string Licence { get; set; } = "";
        public string Club { get; set; } = "";

        // Champs fédéraux (Maestro uniquement)
        public string Sexe { get; set; } = "";               // M / F
        public string Catégorie { get; set; } = "";          // Senior, Féminin, Junior...
        public string Nationalité { get; set; } = "";
        public string Club_Code_FFPJP { get; set; } = "";
        public DateTime? Date_Naissance { get; set; }
        public int Points_FFPJP { get; set; }
        public string Statut { get; set; } = "Actif";        // Actif / Suspendu / Radié

        // Utilitaire d’affichage
        public override string ToString() => $"{Nom} ({Licence})";
        }
    }
