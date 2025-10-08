using TXM.Services.Export;

namespace TXM.Maestro.Vues
{
    public partial class Vue_Tableaux: UserControl
    {
        private readonly VM_Tableaux _vm;
        private readonly int _idConcours;

        public Vue_Tableaux(int idConcours)
        {
            InitializeComponent();
            _idConcours = idConcours;
            _vm = new VM_Tableaux(idConcours);
            DataContext = _vm;
        }

        private void OnTiragePremierTour(object sender, RoutedEventArgs e)
        {
            var inscriptions = Service_Inscriptions.Lister()
                .Where(i => i.IdConcours == _idConcours)
                .ToList();

            if (inscriptions.Count < 2)
            {
                MessageBox.Show("Pas assez d'équipes inscrites pour générer un tirage.");
                return;
            }

            Service_Tirage.GénérerMatchesPremierTour(_idConcours, inscriptions);
            _vm.Charger();
        }
        private void OnTirageTourSuivant(object sender, RoutedEventArgs e)
        {
            var matches = _vm.Matches;
            if (matches.Count == 0)
            {
                MessageBox.Show("Aucun match à partir duquel générer un tour suivant.");
                return;
            }

            var tourActuel = matches.Max(m => m.Tour);
            Service_Tirage.GénérerTourSuivant(_idConcours, tourActuel);
            _vm.Charger();
        }

        private void OnExporterTableau(object sender, RoutedEventArgs e)
        {
            var matches = _vm.Matches.ToList();
            if (matches.Count == 0)
            {
                MessageBox.Show("Aucun match à exporter.");
                return;
            }

            var nomConcours = "Concours officiel"; // TODO: récupérer via VM
            var chemin = Service_Export_Pdf
                            .Exporter_Tableau(matches, nomConcours);

            MessageBox.Show($"Export terminé : {chemin}", "PDF généré",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
