namespace TXM.Maestro.Vues
    {
    public partial class Vue_Joueurs : UserControl
        {
        private readonly VM_Joueurs _vm;

        public Vue_Joueurs()
            {
            InitializeComponent();
            _vm = new VM_Joueurs();
            DataContext = _vm;
            }

        private void OnAjouterJoueur(object sender, RoutedEventArgs e)
            {
            if (string.IsNullOrWhiteSpace(SaisieNom.Text))
                {
                MessageBox.Show("Le nom du joueur est obligatoire.",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
                }

            // Création d’un joueur complet (version Maestro)
            Joueur joueur = new Joueur
                {
                Nom = SaisieNom.Text,
                Licence = SaisieLicence.Text,
                Club = SaisieClub.Text,
                Sexe = SaisieSexe?.Text ?? "",
                Catégorie = SaisieCategorie?.Text ?? "",
                Nationalité = SaisieNationalite?.Text ?? "",
                Club_Code_FFPJP = "",     // optionnel, à remplir plus tard
                Points_FFPJP = 0,
                Statut = "Actif"
                };

            _vm.AjouterJoueur(joueur);

            // Nettoyage du formulaire
            SaisieNom.Clear();
            SaisieLicence.Clear();
            SaisieClub.Clear();
            if (SaisieSexe != null) SaisieSexe.Clear();
            if (SaisieCategorie != null) SaisieCategorie.Clear();
            if (SaisieNationalite != null) SaisieNationalite.Clear();
            }

        private void OnEnregistrerModifs(object sender, RoutedEventArgs e)
            {
            if (_vm.JoueurSélectionné != null)
                {
                _vm.MettreÀJourJoueur(_vm.JoueurSélectionné);
                MessageBox.Show("Modifications enregistrées ✅",
                                "Succès",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                }
            }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
            {
            if (e.Row.Item is Joueur joueur && e.EditAction == DataGridEditAction.Commit)
                {
                _vm.MettreÀJourJoueur(joueur);
                }
            }

        private void OnSupprimerClick(object sender, RoutedEventArgs e)
            {
            if (sender is Button b && b.Tag is Joueur joueur)
                {
                if (MessageBox.Show($"Supprimer {joueur.Nom} ?", "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                    _vm.SupprimerJoueur(joueur);
                    }
                }
            }
        }
    }
