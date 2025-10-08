namespace TXM.Vm.Maestro
    {
    public class VM_Arbitrage : BaseVM
        {
        // Sources
        public ObservableCollection<Incident> Incidents { get; } = [];
        public ObservableCollection<Sanction> Sanctions { get; } = [];

        // Filtres
        public ObservableCollection<Incident> IncidentsFiltrés { get; } = [];
        public ObservableCollection<Sanction> SanctionsFiltrées { get; } = [];

        public Incident? IncidentSélectionné
            {
            get;
            set { field = value; OnPropertyChanged(nameof(IncidentSélectionné)); }
            }

        public Sanction? SanctionSélectionnée
            {
            get;
            set { field = value; OnPropertyChanged(nameof(SanctionSélectionnée)); }
            }

        // Listes annexes
        public ObservableCollection<Joueur> Joueurs { get; } = [];
        public ObservableCollection<Équipe> Équipes { get; } = [];

        public VM_Arbitrage()
            {
            Charger();
            }

        public void Charger()
            {
            // Incidents
            Incidents.Clear();
            foreach (Incident i in Service_Incidents.Lister())
                {
                Incidents.Add(i);
                }

            // Sanctions
            Sanctions.Clear();
            foreach (Sanction s in Service_Sanctions.Lister())
                {
                Sanctions.Add(s);
                }

            // Annexes
            Joueurs.Clear();
            foreach (Joueur j in Service_Joueurs.ListerTous())
                {
                Joueurs.Add(j);
                }

            Équipes.Clear();
            foreach (Équipe e in Service_Équipes.ListerToutes())
                {
                Équipes.Add(e);
                }

            // Filtres init
            AppliquerFiltres(null, "");
            }

        public void AppliquerFiltres(DateTime? date, string? texte)
            {
            string t = (texte ?? "").Trim().ToLower();

            IncidentsFiltrés.Clear();
            foreach (Incident? i in Incidents.Where(i =>
                     (!date.HasValue || i.Date.Date == date.Value.Date) &&
                     (string.IsNullOrEmpty(t) ||
                      (i.Description?.ToLower().Contains(t, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                      (i.Arbitre?.ToLower().Contains(t, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                      (i.Gravité?.ToLower().Contains(t, StringComparison.CurrentCultureIgnoreCase) ?? false))))
                {
                IncidentsFiltrés.Add(i);
                }

            SanctionsFiltrées.Clear();
            foreach (Sanction? s in Sanctions.Where(s =>
                     (!date.HasValue || s.Date.Date == date.Value.Date) &&
                     (string.IsNullOrEmpty(t) ||
                      (s.Motif?.ToLower().Contains(t, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                      (s.Type?.ToLower().Contains(t, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                      (s.Arbitre?.ToLower().Contains(t, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                      (s.Article_Règlement?.ToLower().Contains(t, StringComparison.CurrentCultureIgnoreCase) ?? false))))
                {
                SanctionsFiltrées.Add(s);
                }
            }

        // === Incidents ===
        public void AjouterIncident(DateTime date, string description, string gravité, string arbitre, int? idÉquipe, int? idJoueur)
            {
            Incident inc = Service_Incidents.Ajouter(date, description, gravité, arbitre, idÉquipe, idJoueur);
            Incidents.Insert(0, inc);
            AppliquerFiltres(null, ""); // rafraîchit le cache filtré
            }

        public void SupprimerIncident(int id)
            {
            Service_Incidents.Supprimer(id);
            Incident? trouvé = Incidents.FirstOrDefault(i => i.Id == id);
            if (trouvé != null)
                {
                _ = Incidents.Remove(trouvé);
                }

            AppliquerFiltres(null, "");
            }

        // === Sanctions ===
        public void AjouterSanction(DateTime date, string type, string motif, string article, string arbitre, int duréeMinutes, int? idIncident)
            {
            Sanction s = Service_Sanctions.Ajouter(date, type, motif, article, arbitre, duréeMinutes, idIncident);
            Sanctions.Insert(0, s);
            AppliquerFiltres(null, "");
            }

        public void SupprimerSanction(int id)
            {
            Service_Sanctions.Supprimer(id);
            Sanction? trouvé = Sanctions.FirstOrDefault(s => s.Id == id);
            if (trouvé != null)
                {
                _ = Sanctions.Remove(trouvé);
                }

            AppliquerFiltres(null, "");
            }
        }
    }
