namespace TXM.Vm.Maestro
    {
    public class VM_Tableaux : BaseVM
        {
        public ObservableCollection<Match> Matches { get; } = [];
        private readonly int _idConcours;

        public VM_Tableaux(int idConcours)
        {
            _idConcours = idConcours;
            Charger();
        }

        public void Charger()
        {
            Matches.Clear();
            foreach (var m in Service_Matches.Lister(_idConcours))
                Matches.Add(m);
        }

        public static void SauverScore(Match match)
        {
            Service_Matches.MettreÀJourScore(match.Id, match.ScoreA, match.ScoreB);
        }

    }
}
