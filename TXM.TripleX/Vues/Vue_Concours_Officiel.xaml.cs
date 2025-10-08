using TXM.Vm.TripleX;

namespace TXM.TripleX.Vues
{
    public partial class Vue_Concours_Officiel: UserControl
    {
        private readonly VM_Concours_Officiel _vm;

        public Vue_Concours_Officiel()
        {
            InitializeComponent();
            _vm = new VM_Concours_Officiel();
            DataContext = _vm;
        }

        private void OnCréerConcours(object sender, System.Windows.RoutedEventArgs e)
        {
            var nom = SaisieNomConcours.Text;
            var date = SaisieDate.SelectedDate ?? DateTime.Today;
            var homologation = SaisieHomologation.Text;
            var arbitre = SaisieArbitre.Text;
            _vm.CréerConcours(nom, date, homologation, arbitre);
        }
    }
}
