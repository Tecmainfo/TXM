namespace TXM.Vm.Maestro
    {
    public class VM_Matches : BaseVM
        {
        public ObservableCollection<Match> Matches { get; } = [];
        private Poule? _pouleSélectionnée;
        public Poule? PouleSélectionnée
        {
            get => _pouleSélectionnée;
            set
            {
                if (_pouleSélectionnée != value)
                {
                    _pouleSélectionnée = value;
                    OnPropertyChanged(nameof(PouleSélectionnée));
                    Charger();
                }
            }
        }

        public int IdConcours => PouleSélectionnée?.IdConcours ?? 0;

        public VM_Matches(Poule poule)
        {
            PouleSélectionnée = poule;
            Charger();
        }

        public void Charger()
        {
            Matches.Clear();
            if (PouleSélectionnée != null)
            {
                foreach (var m in Service_Matches.ListerPourPoule(PouleSélectionnée.Id))
                    Matches.Add(m);
            }
        }
    }
}
