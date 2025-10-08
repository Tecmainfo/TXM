
using TXM.Modèles.Dossier_Concours;

namespace TXM.Maestro.Vues
    {
    public partial class Vue_Matches : UserControl
        {
        private readonly VM_Matches _vm;

        public Vue_Matches(Poule poule)
            {
            InitializeComponent();
            _vm = new VM_Matches(poule);
            DataContext = _vm;
            }

        private void OnEnregistrerScores(object sender, RoutedEventArgs e)
            {
            foreach (Match match in _vm.Matches)
                {
                Service_Matches.MettreÀJourScore(match.Id, match.ScoreA, match.ScoreB);
                }
            MessageBox.Show("Scores enregistrés ✅", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            _vm.Charger();
            }

        private void OnGenererMatchesPoule(object sender, RoutedEventArgs e)
            {
            if (_vm.PouleSélectionnée == null) return;

            IList<Inscription> équipes = Service_Poules.ListerÉquipesDansPoule(_vm.PouleSélectionnée.Id);
            Service_Matches.GénérerMatchesPourPoule(_vm.PouleSélectionnée.IdConcours,
                                                    _vm.PouleSélectionnée.Id,
                                                    équipes,
                                                    _vm.PouleSélectionnée.HeureDébut);
            MessageBox.Show("Matches de la poule générés ✅", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        private void OnGenererMatches(object sender, RoutedEventArgs e)
            {
            if (_vm.PouleSélectionnée == null)
                {
                MessageBox.Show("Sélectionner une poule d’abord.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            var vue = new Vue_Matches(_vm.PouleSélectionnée);
            var fen = new Window
                {
                Title = $"Matches – Poule {_vm.PouleSélectionnée.Nom}",
                Content = vue,
                Width = 800,
                Height = 600
                };
            fen.ShowDialog();
            }

        }
    }
