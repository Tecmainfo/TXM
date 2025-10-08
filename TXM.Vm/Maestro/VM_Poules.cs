namespace TXM.Vm.Maestro
    {
    public class VM_Poules : BaseVM
        {
        public ObservableCollection<Poule> Poules { get; } = [];
        private readonly int _idConcours;

        private Poule? _pouleSélectionnée;
        public Poule? PouleSélectionnée
            {
            get => _pouleSélectionnée;
            set
                {
                if (_pouleSélectionnée != value)
                    {
                    _pouleSélectionnée = value;
                    OnPropertyChanged(nameof(PouleSélectionnée));
                    }
                }
            }

        public ObservableCollection<Inscription> ÉquipesDansPoule { get; } = [];

        public void ChargerÉquipes()
            {
            ÉquipesDansPoule.Clear();
            if (PouleSélectionnée != null)
                {
                foreach (Inscription i in Service_Poules.ListerÉquipesDansPoule(PouleSélectionnée.Id))
                    ÉquipesDansPoule.Add(i);
                }
            }

        public VM_Poules(int idConcours)
            {
            _idConcours = idConcours;
            Charger();
            }

        public void Charger()
            {
            Poules.Clear();
            foreach (Poule p in Service_Poules.Lister(_idConcours))
                Poules.Add(p);

            PouleSélectionnée = Poules.FirstOrDefault();
            }

        public void AjouterPoule(string nom, DateTime heureDébut)
            {
            Poule poule = Service_Poules.Ajouter(nom, _idConcours, heureDébut);
            Poules.Add(poule);
            PouleSélectionnée = poule;
            }

        public void AjouterÉquipeÀPoule(int idInscription)
            {
            if (PouleSélectionnée == null) return;
            Service_Poules.AjouterÉquipeDansPoule(PouleSélectionnée.Id, idInscription);
            ChargerÉquipes();
            }

        public void RépartirÉquipesAléatoirement()
            {
            IList<Inscription> inscriptions = Service_Inscriptions.ListerPourConcours(_idConcours);
            var poules = Poules.ToList();
            Service_Poules.RépartirÉquipesAléatoirement(_idConcours, poules, inscriptions);
            ChargerÉquipes();
            }
        public ObservableCollection<Joueur> JoueursDansPoule { get; } = [];

        public void ChargerJoueurs()
            {
            JoueursDansPoule.Clear();
            if (PouleSélectionnée != null)
                {
                foreach (Joueur j in Service_Poules.ListerJoueursDansPoule(PouleSélectionnée.Id))
                    JoueursDansPoule.Add(j);
                }
            }

        public void RépartirJoueursAléatoirement()
            {
            var poules = Poules.ToList();
            IList<Joueur> joueurs = Service_Joueurs.ListerTous();
            Service_Poules.RépartirJoueursAléatoirement(poules, joueurs);
            ChargerJoueurs();
            }
        }
    }
