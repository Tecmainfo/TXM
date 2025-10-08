namespace TXM.Vm.Maestro
    {
    /// <summary>
    /// ViewModel des joueurs – version complète pour Pétanque Maestro (FFPJP).
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

        public void AjouterJoueur(Joueur joueur)
            {
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

        // 🔹 Utilitaire pour filtrer selon statut, catégorie, etc.
        public IEnumerable<Joueur> Filtrer(string? categorie = null, string? statut = null)
            {
            IEnumerable<Joueur> source = Joueurs;
            if (!string.IsNullOrWhiteSpace(categorie))
                source = source.Where(j => j.Catégorie.Equals(categorie, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(statut))
                source = source.Where(j => j.Statut.Equals(statut, StringComparison.OrdinalIgnoreCase));
            return source;
            }
        }
    }
