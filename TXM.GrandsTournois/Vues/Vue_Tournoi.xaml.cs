namespace TXM.GrandsTournois.Vues
    {
    public partial class Vue_Tournoi : UserControl
        {
        private readonly VM_Tournoi _vm = new();

        public Vue_Tournoi()
            {
            InitializeComponent();
            DataContext = _vm;
            }

        private void OnAjouter(object sender, RoutedEventArgs e)
            {
            if (string.IsNullOrWhiteSpace(Nom.Text))
                {
                _ = MessageBox.Show("Le nom du tournoi est obligatoire.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            _vm.Ajouter(new Tournoi
                {
                Nom = Nom.Text,
                Lieu = Lieu.Text,
                Organisateur = Organisateur.Text,
                Date_Début = System.DateTime.Today,
                Date_Fin = System.DateTime.Today.AddDays(1)
                });

            Nom.Clear(); Lieu.Clear(); Organisateur.Clear();
            }

        private void OnSupprimer(object sender, RoutedEventArgs e)
            {
            if (_vm.TournoiSélectionné == null)
                {
                return;
                }

            if (MessageBox.Show($"Supprimer le tournoi {_vm.TournoiSélectionné.Nom} ?",
                                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                _vm.Supprimer(_vm.TournoiSélectionné);
                }
            }
        }
    }
