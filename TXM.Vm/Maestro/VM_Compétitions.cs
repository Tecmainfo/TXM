namespace TXM.Vm.Maestro
    {
    public class VM_Compétitions : BaseVM
        {
        public ObservableCollection<Compétition> Compétitions { get; } = [];

        public VM_Compétitions()
            {
            foreach (Compétition c in Service_Compétitions.Lister_Compétitions())
                Compétitions.Add(c);
            }
        }
    }
