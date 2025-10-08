using TXM.Modèles.Joueurs;

namespace TXM.Vm.TripleX
    {
    /// <summary>
    /// ViewModel des joueurs – version simplifiée pour TripleX.
    /// </summary>
    public class VM_Joueurs : BaseVM
        {
        public ObservableCollection<Joueur> Joueurs { get; } = new();

        private Joueur? _joueurSélectionné;
        public Joueur? JoueurSélectionné
            {
            get => _joueurSélectionné;
            set
                {
                if (_joueurSélectionné != value)
                    {
                    _joueurSélectionné = value;
                    OnPropertyChanged();
                    }
                }
            }

        public VM_Joueurs()
            {
            Charger();
            }

        public void Charger()
            {
            Joueurs.Clear();
            foreach (Joueur j in Service_Joueurs.ListerTous())
                Joueurs.Add(j);

            JoueurSélectionné = Joueurs.FirstOrDefault();
            }

        public void AjouterJoueur(string nom, string licence, string club)
            {
            // TripleX n'utilise que les trois champs de base
            var joueur = new Joueur { Nom = nom, Licence = licence, Club = club };
            Service_Joueurs.Ajouter(joueur);
            Charger();
            }

        public void SupprimerJoueur(Joueur joueur)
            {
            if (joueur == null) return;
            Service_Joueurs.Supprimer(joueur.Id);
            Charger();
            }

        public void MettreÀJourJoueur(Joueur joueur)
            {
            if (joueur == null) return;
            Service_Joueurs.MettreÀJour(joueur);
            Charger();
            }
        }
    }
