using TXM.Modèles;

namespace TXM.Interfaces.Pages
    {
    public partial class Page_Rapports : Page
        {
        private List<Rapport> _rapports = [];

        public Page_Rapports()
            {
            InitializeComponent();
            Charger();
            }

        private void Charger()
            {
            _rapports = [.. Service_Rapports.Lister()];
            GridRapports.ItemsSource = _rapports;
            }

        private void OnFilterChanged(object sender, EventArgs e)
            {
            if (_rapports == null) return;

            string filtreTexte = SearchBox.Text?.ToLower() ?? "";
            DateTime? dateFiltre = FiltreDate.SelectedDate;

            List<Rapport> filtrés = [.. _rapports.Where(r =>
                (string.IsNullOrEmpty(filtreTexte) ||
                 r.Type.Contains(filtreTexte, StringComparison.CurrentCultureIgnoreCase) ||
                 r.Utilisateur.Contains(filtreTexte, StringComparison.CurrentCultureIgnoreCase)) &&
                (!dateFiltre.HasValue || r.Date.Date == dateFiltre.Value.Date)
            )];

            GridRapports.ItemsSource = filtrés;
            }

        private void OnResetFilterClick(object sender, RoutedEventArgs e)
            {
            SearchBox.Text = "";
            FiltreDate.SelectedDate = null;
            GridRapports.ItemsSource = _rapports;
            }
        }
    }
