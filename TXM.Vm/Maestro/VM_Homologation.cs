namespace TXM.Vm.Maestro
    {
    public class VM_Homologation : BaseVM
        {
        public ObservableCollection<Concours_Officiel> Concours { get; } = [];

        public ICommand CmdValider { get; }
        public ICommand CmdRejeter { get; }
        public ICommand CmdHistorique { get; }

        public VM_Homologation()
            {
            Recharger();

            CmdValider = new RelayCommand(c => Valider(c as Concours_Officiel), c => c is Concours_Officiel);
            CmdRejeter = new RelayCommand(c => Rejeter(c as Concours_Officiel), c => c is Concours_Officiel);
            CmdHistorique = new RelayCommand(c => AfficherHistorique(c as Concours_Officiel), c => c is Concours_Officiel);
            }

        public void Recharger()
            {
            Concours.Clear();
            foreach (Concours_Officiel c in Service_Concours_Officiels.Lister())
                Concours.Add(c);
            }

        private static void Valider(Concours_Officiel? concours)
            {
            if (concours == null) return;
            Service_Homologation.EnregistrerDécision(concours.Id, "Homologué", "Arbitre X", "Validé sans réserve");
            concours.Statut = "Homologué";
            }

        private static void Rejeter(Concours_Officiel? concours)
            {
            if (concours == null) return;
            Service_Homologation.EnregistrerDécision(concours.Id, "Rejeté", "Arbitre X", "Problème d'homologation");
            concours.Statut = "Rejeté";
            }

        private static void AfficherHistorique(Concours_Officiel? concours)
            {
            if (concours == null) return;
            IList<HistoriqueHomologation> histo = Service_Homologation.ListerPourConcours(concours.Id);
            string message = string.Join(Environment.NewLine,
                histo.Select(h => $"{h.DateAction:g} – {h.Décision} par {h.Arbitre} ({h.Commentaire})"));

            MessageBox.Show(message,
                            $"Historique du concours {concours.Nom}",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
            }
        }
    }
