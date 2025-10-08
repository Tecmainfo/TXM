using TXM.Modèles.Dossier_Concours;

namespace TXM.Maestro.Vues
    {
    public partial class Vue_Homologation : UserControl
        {
        private readonly VM_Homologation _vm;

        public Vue_Homologation()
            {
            InitializeComponent();
            _vm = new VM_Homologation();
            DataContext = _vm;
            }

        private void OnValider(object sender, RoutedEventArgs e)
            {
            if (sender is Button bouton && bouton.DataContext is Concours_Officiel concours)
                {
                Service_Homologation.EnregistrerDécision(concours.Id, "Homologué", "Arbitre X", "Validé sans réserve");
                concours.Statut = "Homologué";
                }
            }

        private void OnRejeter(object sender, RoutedEventArgs e)
            {
            if (sender is Button bouton && bouton.DataContext is Concours_Officiel concours)
                {
                Service_Homologation.EnregistrerDécision(concours.Id, "Rejeté", "Arbitre X", "Problème d'homologation");
                concours.Statut = "Rejeté";
                }
            }

        private void OnAfficherHistorique(object sender, RoutedEventArgs e)
            {
            if (sender is Button bouton && bouton.DataContext is Concours_Officiel concours)
                {
                var histo = Service_Homologation.ListerPourConcours(concours.Id);
                var message = string.Join(Environment.NewLine,
                    histo.Select(h => $"{h.DateAction:g} – {h.Décision} par {h.Arbitre} ({h.Commentaire})"));

                _ = MessageBox.Show(message,
                                $"Historique du concours {concours.Nom}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                }
            }
        }
    }
