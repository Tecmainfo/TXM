using TXM.Vm.TripleX;

namespace TXM.TripleX.Vues
    {
    public partial class Vue_Concours_Amical : UserControl
        {
        private readonly VM_Concours_Amical _vm;

        public Vue_Concours_Amical()
            {
            InitializeComponent();
            _vm = new VM_Concours_Amical();
            DataContext = _vm;
            }

        private void OnCréerConcours(object sender, System.Windows.RoutedEventArgs e)
            {
            string nom = SaisieNomConcours.Text;
            DateTime date = SaisieDate.SelectedDate ?? DateTime.Today;
            _vm.CréerConcours(nom, date);
            }
        }
    }
