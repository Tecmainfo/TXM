using TXM.Vm.TripleX;

namespace TXM.TripleX.Vues
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
            if (!string.IsNullOrWhiteSpace(SaisieNom.Text))
                {
                _vm.AjouterJoueur(SaisieNom.Text, SaisieLicence.Text, SaisieClub.Text);
                SaisieNom.Clear();
                SaisieLicence.Clear();
                SaisieClub.Clear();
                }
            else
                {
                _ = MessageBox.Show("Le nom du joueur est obligatoire.",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                }
            }

        private void OnEnregistrerModifs(object sender, RoutedEventArgs e)
            {
            if (_vm.JoueurSélectionné != null)
                {
                _vm.MettreÀJourJoueur(_vm.JoueurSélectionné);
                _ = MessageBox.Show("Modifications enregistrées ✅",
                                "Succès",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                }
            }
        }
    }
