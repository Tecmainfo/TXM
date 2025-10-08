namespace TXM.Vm.Maestro
    {
    public class VM_Inscriptions : BaseVM
        {
        public ObservableCollection<Inscription> Inscriptions { get; } = [];
        public ObservableCollection<Concours_Officiel> ConcoursDisponibles { get; } = [];

        public VM_Inscriptions()
            {
            foreach (Inscription insc in Service_Inscriptions.Lister())
                Inscriptions.Add(insc);

            foreach (Concours_Officiel c in Service_Concours_Officiels.Lister())
                ConcoursDisponibles.Add(c);
            Charger();
            }

        public void AjouterÉquipe(string nomÉquipe, string joueurs, int idConcours)
            {
            Inscription insc = Service_Inscriptions.Ajouter(nomÉquipe, joueurs, idConcours);
            Inscriptions.Insert(0, insc);
            }

        public void MettreÀJourInscription(int id, string nomÉquipe, string joueurs)
            {
            Service_Inscriptions.MettreÀJour(id, nomÉquipe, joueurs);
            Charger(); // on recharge la liste pour refléter les changements
            }

        public void Charger()
            {
            Inscriptions.Clear();
            foreach (Inscription insc in Service_Inscriptions.Lister())
                Inscriptions.Add(insc);

            ConcoursDisponibles.Clear();
            foreach (Concours_Officiel c in Service_Concours_Officiels.Lister())
                ConcoursDisponibles.Add(c);
            }

        }
    }
