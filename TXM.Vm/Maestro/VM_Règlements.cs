namespace TXM.Vm.Maestro
    {
    public class VM_Règlements : BaseVM
        {
        public ObservableCollection<Règlement> Règlements { get; } = [];

        private Règlement? _règlementSélectionné;
        public Règlement? RèglementSélectionné
            {
            get => _règlementSélectionné;
            set { _règlementSélectionné = value; OnPropertyChanged(nameof(RèglementSélectionné)); }
            }

        public void Charger(int? année = null, string? filtre = null)
            {
            Règlements.Clear();
            foreach (Règlement r in Service_Règlements.Lister(année, filtre))
                Règlements.Add(r);
            RèglementSélectionné = Règlements.FirstOrDefault();
            }

        public static void Supprimer(Règlement r) => Service_Règlements.Supprimer(r.Id);
        public static void Ouvrir(Règlement r) => Service_Règlements.Ouvrir(r);

        public static void Importer_Depuis_Fichier(int année, string titre, string catégorie, string fichier)
            => Service_Règlements.Ajouter_Depuis_Fichier(année, titre, catégorie, fichier);

        public static async Task Importer_Depuis_Url(int année, string titre, string catégorie, string url)
            => await Service_Règlements.Ajouter_Depuis_Url(année, titre, catégorie, url);

        public static void MettreÀJour_Métadonnées(Règlement r)
            => Service_Règlements.MettreÀJour_Métadonnées(r);

        public string Exporter_Csv()
            => Service_Règlements.Exporter_Csv([.. Règlements]);

        }
    }
