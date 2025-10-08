namespace TXM.Vm.TripleX
    {
    public class VM_Concours_Officiel : BaseVM
        {
        public ObservableCollection<Concours_Officiel> Concours { get; } = [];

        public VM_Concours_Officiel()
            {
            foreach (Concours_Officiel c in Service_Concours_Officiels.Lister())
                Concours.Add(c);
            }

        public void CréerConcours(string nom, DateTime date, string numHomologation, string arbitre)
            {
            Concours_Officiel c = Service_Concours_Officiels.Ajouter(nom, date, numHomologation, arbitre);
            Concours.Insert(0, c);
            }
        }
    }
