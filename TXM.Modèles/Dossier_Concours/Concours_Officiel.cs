namespace TXM.Modèles.Dossier_Concours
    {
    public sealed class Concours_Officiel : INotifyPropertyChanged
        {
        public int Id { get; init; }
        public string Nom { get; init; } = "";
        public DateTime Date { get; init; }
        public string Numéro_Homologation { get; init; } = "";
        public string Arbitre { get; init; } = "";

        public string Statut
            {
            get;
            set
                {
                if (field != value)
                    {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Statut)));
                    }
                }
            } = "Prévu";

        public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
