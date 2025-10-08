namespace TXM.Interfaces.Vues
{
    public partial class Vue_Licence: UserControl
    {
        public Vue_Licence()
        {
            InitializeComponent();
            DataContext = new VM_Licence();
        }
    }
}
