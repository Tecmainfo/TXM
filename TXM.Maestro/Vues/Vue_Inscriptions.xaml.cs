using TXM.Modèles.Dossier_Concours;

namespace TXM.Maestro.Vues
    {
    public partial class Vue_Inscriptions : UserControl
        {
        private readonly VM_Inscriptions _vm;

        public Vue_Inscriptions()
            {
            InitializeComponent();
            _vm = new VM_Inscriptions();
            DataContext = _vm;
            }

        private void OnAjouterÉquipe(object sender, RoutedEventArgs e)
            {
            if (string.IsNullOrWhiteSpace(SaisieNomÉquipe.Text) ||
                string.IsNullOrWhiteSpace(SaisieJoueurs.Text) ||
                ComboConcours.SelectedItem is not Concours_Officiel concours)
                {
                MessageBox.Show("Merci de remplir tous les champs avant d'ajouter une équipe.",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
                }

            _vm.AjouterÉquipe(SaisieNomÉquipe.Text,
                              SaisieJoueurs.Text,
                              concours.Id);

            MessageBox.Show($"Équipe '{SaisieNomÉquipe.Text}' inscrite au concours {concours.Nom}.",
                            "Inscription validée",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            // Reset formulaire
            SaisieNomÉquipe.Clear();
            SaisieJoueurs.Clear();
            ComboConcours.SelectedIndex = -1;
            }

        private void OnSupprimerInscription(object sender, RoutedEventArgs e)
            {
            if (sender is Button b && b.Tag is Inscription insc)
                {
                if (MessageBox.Show($"Supprimer l’équipe {insc.NomÉquipe} du concours {insc.NomConcours} ?",
                                    "Confirmation",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                    Service_Inscriptions.Supprimer(insc.Id);
                    _vm.Inscriptions.Remove(insc);
                    }
                }
            }

        private void OnRepartirPoules(object sender, RoutedEventArgs e)
            {
            if (ComboConcours.SelectedItem is not Concours_Officiel concours)
                {
                MessageBox.Show("Choisir un concours d’abord.", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            IList<Inscription> inscriptions = Service_Inscriptions.ListerPourConcours(concours.Id);
            IList<Poule> poules = Service_Poules.Lister(concours.Id);

            if (poules.Count == 0)
                {
                MessageBox.Show("Aucune poule définie pour ce concours.", "Info",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }

            Service_Poules.RépartirÉquipesAléatoirement(concours.Id, poules, inscriptions);
            MessageBox.Show("Répartition des équipes en poules effectuée ✅",
                            "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
            {
            if (e.Row.Item is Inscription insc && e.EditAction == DataGridEditAction.Commit)
                {
                _vm.MettreÀJourInscription(insc.Id, insc.NomÉquipe, insc.Joueurs);
                }
            }


        }
    }
