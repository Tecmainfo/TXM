using TXM.Services.Dossier_Concours;

namespace TXM.Maestro.Vues
    {
    public partial class Vue_Équipes : UserControl
        {
        private readonly VM_Équipes _vm;

        public Vue_Équipes()
            {
            InitializeComponent();
            _vm = new VM_Équipes();
            DataContext = _vm;
            RafraîchirJoueursDispo();
            }

        private void OnAjouterJoueurEquipe(object sender, RoutedEventArgs e)
            {
            if (DataContext is VM_Équipes vm && ComboJoueursDispo.SelectedValue is int idJoueur)
                {
                vm.AjouterJoueurDansÉquipe(idJoueur);
                RafraîchirJoueursDispo(); // ⚡ recharger la liste
                }
            }

        private void OnAjouterÉquipe(object sender, RoutedEventArgs e)
            {
            if (!string.IsNullOrWhiteSpace(SaisieNomÉquipe.Text))
                {
                _vm.AjouterÉquipe(SaisieNomÉquipe.Text);
                SaisieNomÉquipe.Text = "";
                }
            else
                {
                MessageBox.Show("Le nom de l’équipe est obligatoire.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private void OnSupprimerClick(object sender, RoutedEventArgs e)
            {
            if (sender is Button b && b.Tag is Équipe équipe)
                {
                if (MessageBox.Show($"Supprimer l’équipe {équipe.Nom} ?", "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                    _vm.SupprimerÉquipe(équipe);
                    }
                }
            }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
            {
            if (e.Row.Item is Équipe équipe && e.EditAction == DataGridEditAction.Commit)
                {
                _vm.MettreÀJourÉquipe(équipe.Id, équipe.Nom);
                }
            }

        private void OnRetirerJoueurEquipe(object sender, RoutedEventArgs e)
            {
            if (sender is Button b && b.Tag is Joueur joueur && DataContext is VM_Équipes vm)
                {
                vm.RetirerJoueurDeÉquipe(joueur);
                }
            }

        private void RafraîchirJoueursDispo()
            {
            ComboJoueursDispo.ItemsSource = Service_Joueurs.ListerTous();
            }

        }
    }
