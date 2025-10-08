namespace TXM.Vm.Maestro
    {
    public class VM_Équipes : BaseVM
        {
        public ObservableCollection<Équipe> Équipes { get; } = [];

        public VM_Équipes()
            {
            Charger();
            }

        public void Charger()
            {
            Équipes.Clear();
            foreach (Équipe e in Service_Équipes.ListerToutes())
                Équipes.Add(e);
            }

        public void AjouterÉquipe(string nom)
            {
            Service_Équipes.Ajouter(nom);
            Charger();
            }

        public void SupprimerÉquipe(Équipe équipe)
            {
            Service_Équipes.Supprimer(équipe.Id);
            Charger();
            }

        public void MettreÀJourÉquipe(int id, string nom)
            {
            Service_Équipes.MettreÀJour(id, nom);
            Charger();
            }

        public ObservableCollection<Joueur> JoueursDeÉquipe { get; } = [];

        private Équipe? _équipeSélectionnée;
        public Équipe? ÉquipeSélectionnée
            {
            get => _équipeSélectionnée;
            set
                {
                if (_équipeSélectionnée != value)
                    {
                    _équipeSélectionnée = value;
                    OnPropertyChanged(nameof(ÉquipeSélectionnée));
                    ChargerJoueurs();
                    }
                }
            }

        public void ChargerJoueurs()
            {
            JoueursDeÉquipe.Clear();
            if (ÉquipeSélectionnée != null)
                {
                foreach (Joueur j in Service_Équipes.ListerJoueursDansÉquipe(ÉquipeSélectionnée.Id))
                    JoueursDeÉquipe.Add(j);
                }
            }

        public void AjouterJoueurDansÉquipe(int idJoueur)
            {
            if (ÉquipeSélectionnée == null) return;
            Service_Équipes.AjouterJoueurDansÉquipe(ÉquipeSélectionnée.Id, idJoueur);
            ChargerJoueurs();
            }

        public void RetirerJoueurDeÉquipe(Joueur joueur)
            {
            if (ÉquipeSélectionnée == null) return;
            Service_Équipes.RetirerJoueurDeÉquipe(ÉquipeSélectionnée.Id, joueur.Id);
            ChargerJoueurs();
            }
        }
    }
