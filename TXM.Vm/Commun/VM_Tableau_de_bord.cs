namespace TXM.Vm.Commun
    {
    public class VM_Tableau_de_bord
        {
        public int NbConcours { get; private set; }
        public int NbJoueurs { get; private set; }
        public int NbIncidents { get; private set; }
        public int NbSanctions { get; private set; }

        public ObservableCollection<Match> MatchesDuJour { get; } = [];

        private readonly DispatcherTimer _timer;

        public VM_Tableau_de_bord()
            {
            Charger();
            _timer = new DispatcherTimer
                {
                Interval = TimeSpan.FromSeconds(60) // toutes les 60 secondes
                };
            _timer.Tick += (s, e) => Charger();
            _timer.Start();
            }

        public void Charger()
            {
            NbConcours = Service_Concours_Officiels.CompterEnCours();
            NbJoueurs = Service_Joueurs.Compter();
            NbIncidents = Service_Incidents.Compter();
            NbSanctions = Service_Sanctions.Compter();

            MatchesDuJour.Clear();
            foreach (Match m in Service_Matches.ListerTousPourDate(DateTime.Today))
                {
                MatchesDuJour.Add(m);
                }

            // 🔔 Notifier le binding WPF
            OnPropertyChanged(nameof(NbConcours));
            OnPropertyChanged(nameof(NbJoueurs));
            OnPropertyChanged(nameof(NbIncidents));
            OnPropertyChanged(nameof(NbSanctions));
            }

        // === INotifyPropertyChanged minimal ===
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string nom)
            {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nom));
            }
        }
    }
