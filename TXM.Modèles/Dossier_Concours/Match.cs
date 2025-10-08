namespace TXM.Modèles.Dossier_Concours
    {
    public sealed class Match
        {
        public int Id { get; init; }
        public int Tour { get; init; }
        public string EquipeA { get; init; } = "";
        public string EquipeB { get; init; } = "";
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
        public int IdConcours { get; init; }
        public int? IdPoule { get; set; }
        public DateTime DateHeure { get; set; }

        public string Score => $"{ScoreA} - {ScoreB}";
        public string? Vainqueur => ScoreA > ScoreB ? EquipeA :
                                    ScoreB > ScoreA ? EquipeB : null;
        }
    }
