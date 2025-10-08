namespace TXM.Interfaces.Pages
    {
    public partial class Page_Accueil : Page
        {
        public Page_Accueil()
            {
            InitializeComponent();
            DataContext = new VM_Tableau_de_bord();
            }
        }
    }
