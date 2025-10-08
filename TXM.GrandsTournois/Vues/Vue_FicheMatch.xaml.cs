namespace TXM.GrandsTournois.Vues
    {
    public partial class Vue_FicheMatch : UserControl
        {
        private readonly VM_FicheMatch _vm = new();

        public Vue_FicheMatch()
            {
            InitializeComponent();
            DataContext = _vm;
            }

        private void OnExporter(object sender, RoutedEventArgs e)
            {
            _vm.Exporter();
            MessageBox.Show("Feuille de match exportée avec succès ✅",
                            "Export PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
