namespace TXM.Vm.Maestro
    {
    public sealed class VM_Documents_Officiels : BaseVM
        {
        public ObservableCollection<string> ProfilsCompétition { get; } =
            new(new[] { "CNC Open", "CNC Féminin", "CNC Vétéran", "CNC Jeunes", "CRC", "CDC", "Provençal" });

        public string ProfilActuel
            {
            get;
            set { field = value; OnPropertyChanged(); }
            } = "CNC Open";

        public ObservableCollection<ExportItem> Exports { get; } = [];

        public string Statut
            {
            get;
            set { field = value; OnPropertyChanged(); }
            } = "Prêt";
        public ICommand CmdFeuilleJury { get; }
        public ICommand CmdFeuillesMatch { get; }
        public ICommand CmdBordereau { get; }
        public ICommand CmdIndemnités { get; }
        public ICommand CmdRapportArbitral { get; }
        public ICommand CmdOuvrirDossierModèles { get; }
        public ICommand CmdOuvrirDossierExports { get; }

        public VM_Documents_Officiels()
            {
            CmdFeuilleJury = new RelayCommand(_ => Générer("FeuilleJury"));
            CmdFeuillesMatch = new RelayCommand(_ => Générer("FeuillesMatch"));
            CmdBordereau = new RelayCommand(_ => Générer("Bordereau"));
            CmdIndemnités = new RelayCommand(_ => Générer("Indemnités"));
            CmdRapportArbitral = new RelayCommand(_ => Générer("RapportArbitral"));
            CmdOuvrirDossierModèles = new RelayCommand(_ => Ouvrir("modèles"));
            CmdOuvrirDossierExports = new RelayCommand(_ => Ouvrir("exports"));
            }

        private void Générer(string type)
            {
            // TODO: ExportService.Generate(type, ProfilActuel, données)
            string fichier = $"Export_{type}_{ProfilActuel}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            Exports.Insert(0, new ExportItem { Type = type, Profil = ProfilActuel, DateHeure = DateTime.Now, Fichier = fichier });
            Statut = $"{type} généré ({ProfilActuel})";
            }

        private static void Ouvrir(string dossier)
            {
            try
                {
                if (Directory.Exists(dossier))
                    {
                    _ = Process.Start(new ProcessStartInfo
                        {
                        FileName = "explorer.exe",
                        Arguments = dossier,
                        UseShellExecute = true
                        });
                    }
                }
            catch (Exception ex)
                {
                _ = MessageBox.Show($"Impossible d’ouvrir le dossier : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    public sealed class ExportItem
        {
        public string Type { get; set; } = "";
        public string Profil { get; set; } = "";
        public DateTime DateHeure { get; set; }
        public string Fichier { get; set; } = "";
        }

    }
