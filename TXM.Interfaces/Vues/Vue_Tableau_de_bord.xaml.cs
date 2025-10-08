namespace TXM.Interfaces.Vues
    {
    public partial class Vue_Tableau_de_bord : UserControl
        {
        public Vue_Tableau_de_bord()
            {
            InitializeComponent();
            DataContext = new VM_Tableau_de_bord();
            }
        }
    }
