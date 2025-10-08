using TXM.Vm.Maestro;

namespace TXM.Interfaces.Pages
    {
    public partial class Page_Compétitions : Page
        {
        public Page_Compétitions()
            {
            InitializeComponent();
            DataContext = new VM_Compétitions();
            }

        private void OnNouvelleCompétition(object sender, RoutedEventArgs e)
            {
            string nom = Microsoft.VisualBasic.Interaction.InputBox("Nom de la compétition :", "Nouvelle compétition");
            string lieu = Microsoft.VisualBasic.Interaction.InputBox("Lieu :", "Nouvelle compétition");
            DateTime date = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(nom))
                {
                var nouvelle = TXM.Services.Service_Compétitions.Ajouter(nom, lieu, date);

                if (DataContext is VM_Compétitions vm)
                    vm.Compétitions.Add(nouvelle);
                }
            }
        }
    }
