namespace TXM.Vm.Commun
    {
    /// <summary>
    /// Vue-modèle pour la page des paramètres TXM (langue, splash, Fluent/Mica)
    /// </summary>
    public class VM_Paramètres : BaseVM
        {
        private readonly Service_Paramètres_UI _ui;

        // === Paramètres de démarrage ===
        public bool Ignorer_Écran_Accueil
            {
            get => _ui.Ignorer_Écran_Accueil;
            set { _ui.Ignorer_Écran_Accueil = value; OnPropertyChanged(); }
            }

        // === Langues ===
        public ObservableCollection<string> Langues_Disponibles { get; } = ["FR", "EN"];

        public string Langue
            {
            get => _ui.Langue;
            set { _ui.Langue = value; OnPropertyChanged(); }
            }

        // === Effet Fluent / Mica ===
        public ObservableCollection<string> Effets_Disponibles { get; } = ["Mica", "Acrylic", "Aucun"];

        public string Effet_Fluent
            {
            get => _ui.Effet_Fluent;
            set
                {
                if (_ui.Effet_Fluent != value)
                    {
                    _ui.Effet_Fluent = value;
                    OnPropertyChanged();

                    // Application dynamique immédiate sur la fenêtre principale
                    if (Application.Current.MainWindow is not null)
                        {
                        bool estSombre = Service_Licence.LicenceActuelle.Type == TypeLicence.Maestro;
                        Service_Fluent.AppliquerEffet(Application.Current.MainWindow, estSombre, value);
                        }
                    }
                }
            }

        // === Commandes ===
        public ICommand Commande_Sauvegarder { get; }

        // === Constructeur ===
        public VM_Paramètres()
            {
            Paramètres_Application app = Service_Paramètres.Charger();
            _ui = new Service_Paramètres_UI
                {
                Licence_Type = app.Licence_Type,
                Licence_Expiration = app.Licence_Expiration,
                Ignorer_Écran_Accueil = app.Skip_Splash,
                Langue = app.Langue,
                Effet_Fluent = app.Effet_Fluent
                };

            Commande_Sauvegarder = new RelayCommand(_ => Sauvegarder());
            }

        // === Sauvegarde ===
        public void Sauvegarder()
            {
            Paramètres_Application app = new()
                {
                Licence_Type = _ui.Licence_Type,
                Licence_Expiration = _ui.Licence_Expiration,
                Skip_Splash = _ui.Ignorer_Écran_Accueil,
                Langue = _ui.Langue,
                Effet_Fluent = _ui.Effet_Fluent
                };

            Service_Paramètres.Sauvegarder(app);

            _ = MessageBox.Show("Paramètres sauvegardés avec succès.",
                "TXM – Paramètres",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            }
        }
    }
