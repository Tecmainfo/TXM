namespace TXM.Maestro.Vues
{
    public partial class Vue_Documents_Officiels: UserControl
    {
        private readonly VM_Documents_Officiels _vm;

        public Vue_Documents_Officiels()
        {
            InitializeComponent();
            _vm = new VM_Documents_Officiels();
            DataContext = _vm;
        }
    }
}
