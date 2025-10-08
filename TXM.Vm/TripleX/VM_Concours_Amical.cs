namespace TXM.Vm.TripleX
    {
    public class VM_Concours_Amical : BaseVM
        {
        public ObservableCollection<Concours> Concours { get; } = [];

        public VM_Concours_Amical()
            {
            foreach (Concours c in Service_Concours.Lister_Concours("Amical"))
                {
                Concours.Add(c);
                }
            }

        public void CréerConcours(string nom, DateTime date)
            {
            Concours c = Service_Concours.Ajouter_Concours(nom, date, "Amical");
            Concours.Insert(0, c);
            }
        }
    }
