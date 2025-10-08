namespace TXM.Maestro.Vues
{
    public partial class Vue_Poules: UserControl
    {
        private readonly VM_Poules _vm;
        private readonly int _idConcours;

        public Vue_Poules(int idConcours)
        {
            InitializeComponent();
            _idConcours = idConcours;
            _vm = new VM_Poules(idConcours);
            DataContext = _vm;
            ComboInscriptions.ItemsSource = Service_Inscriptions.ListerPourConcours(_idConcours);

        }

        private void OnAjouterPoule(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SaisieNomPoule.Text)) return;
            if (SaisieDate.SelectedDate is not DateTime date) return;
            if (!TimeSpan.TryParse(SaisieHeure.Text, out var heure)) return;

            var début = date.Add(heure);
            _vm.AjouterPoule(SaisieNomPoule.Text, début);
        }

        private void OnGenererMatches(object sender, RoutedEventArgs e)
        {
            if (DataContext is VM_Poules vm && vm.PouleSélectionnée != null)
            {
                var inscriptions = Service_Poules.ListerÉquipesDansPoule(vm.PouleSélectionnée.Id);
                Service_GénérationPoules.GénérerMatchesPourPoule(vm.PouleSélectionnée, inscriptions);
                MessageBox.Show($"Matches générés pour la poule {vm.PouleSélectionnée.Nom}",
                                "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnAjouterÉquipeÀPoule(object sender, RoutedEventArgs e)
        {
            if (DataContext is VM_Poules vm && ComboInscriptions.SelectedValue is int idInscription)
            {
                vm.AjouterÉquipeÀPoule(idInscription);
            }
        }

        private void OnRepartirAleatoirement(object sender, RoutedEventArgs e)
        {
            if (DataContext is VM_Poules vm)
            {
                vm.RépartirÉquipesAléatoirement();
                MessageBox.Show("Répartition aléatoire effectuée ✅",
                                "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnRepartirJoueursAleatoirement(object sender, RoutedEventArgs e)
        {
            if (DataContext is VM_Poules vm)
            {
                vm.RépartirJoueursAléatoirement();
                MessageBox.Show("Répartition aléatoire des joueurs effectuée ✅",
                                "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
