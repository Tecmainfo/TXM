namespace TXM.Interfaces
    {
    public partial class MainWindow : Window
        {
        public MainWindow()
            {
            InitializeComponent();

            // 🎫 Écoute les changements de licence
            Service_Licence.LicenceChangée += (_, __) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Title = $"TXM – {Service_Licence.LicenceActuelle.Type}";
                });
            };

            // 🎚️ Écoute les changements de paramètres (Fluent, langue, etc.)
            Service_Paramètres.ParamètresChangés += OnParamètresChangés;

            Service_Licence.Charger();
            _ = Service_Paramètres.Charger();

            // 🎨 Définir le titre selon la licence
            Title = (object)Service_Licence.LicenceActuelle.Type switch
                {
                    TypeLicence.MultiSite => "TXM - Grands Tournois",
                    TypeLicence.Maestro => "TXM – Pétanque Maestro",
                    TypeLicence.TripleX => "TXM – TripleX",
                    _ => "TXM – Démo (mode " + (Service_Licence.EstEnModeRestreint() ? "restreint" : "complet") + ")",
                    };

            // ⚙️ Charger les concours disponibles
            ComboConcoursActifs.ItemsSource = Service_Concours_Officiels.Lister();
            if (ComboConcoursActifs.Items.Count > 0)
                {
                ComboConcoursActifs.SelectedIndex = 0;
                }

            // 🏁 Charger la page d’accueil
            _ = CadrePrincipal.Navigate(new Page_Accueil());
            }

        private void OnConcoursActifChanged(object _, SelectionChangedEventArgs e)
            {
            if (ComboConcoursActifs.SelectedItem is Concours_Officiel concours)
                {
                Service_Context.ConcoursActif = concours;
                }
            }

        private void OnNavigationDemandée(object? _, string destination)
            {
            int idConcours = Service_Context.IdConcoursActif;

            bool estTripleX = Service_Licence.LicenceActuelle.Type == TypeLicence.TripleX;
            bool estMaestro = Service_Licence.LicenceActuelle.Type == TypeLicence.Maestro;
            _ = Service_Licence.LicenceActuelle.Type == TypeLicence.MultiSite;
            bool estRestreint = Service_Licence.EstEnModeRestreint();

            // 🔐 Blocage des modules non autorisés
            if ((estTripleX || estRestreint) &&
                (destination == "homologation" || destination == "documents" || destination == "arbitrage"))
                {
                _ = MessageBox.Show("Cette fonctionnalité est réservée à la licence Pétanque Maestro.",
                                "Accès non autorisé",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
                }

            // 🧭 Navigation dynamique
            Page page = destination switch
                {
                    "accueil" => new Page_Accueil(),
                    "rapports" => new Page_Rapports(),
                    "compétitions" => new Page_Compétitions(),

                    // 🎯 Ici on choisit la vue joueur selon la licence
                    "joueurs" => new Page
                        {
                        Content = estMaestro
                            ? new Maestro.Vues.Vue_Joueurs()
                            : (UserControl)new TXM.TripleX.Vues.Vue_Joueurs()
                        },

                    "licence" => new Page { Content = new Vue_Licence() },
                    "paramètres" => new Page { Content = new Vue_Paramètres() },

                    // Pétanque Maestro
                    "reglements" => new Page { Content = new Vue_Règlements() },
                    "homologation" => new Page { Content = new Vue_Homologation() },
                    "documents" => new Page { Content = new Vue_Documents_Officiels() },
                    "arbitrage" => new Page { Content = new Vue_Arbitrage() },
                    "poules" => new Page { Content = new Vue_Poules(idConcours) },

                    // TripleX
                    "concours_amical" => new Page { Content = new Vue_Concours_Amical() },
                    "concours_officiel" => new Page { Content = new Vue_Concours_Officiel() },

                    // Grand Tournois
                    "tournois" => new Page { Content = new Vue_Tournoi() },
                    "sites" => new Page { Content = new Vue_Sites() },
                    "terrains" => new Page { Content = new Vue_Terrains() },
                    "fichematch" => new Page { Content = new Vue_FicheMatch() },
                    _ => new Page_Accueil()
                    };

            _ = CadrePrincipal.Navigate(page);
            }

        // 🔄 Application immédiate de Fluent/Mica
        private void OnParamètresChangés()
            {
            Dispatcher.Invoke(() =>
            {
                bool estSombre = Service_Licence.LicenceActuelle.Type == TypeLicence.Maestro;
                string effet = Service_Paramètres.Courants.Effet_Fluent;
                Service_Fluent.AppliquerEffet(this, estSombre, effet);
            });
            }

        // 🪩 Appliquer Fluent/Mica au chargement
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
            {
            bool estSombre = Service_Licence.LicenceActuelle.Type == TypeLicence.Maestro;
            Service_Fluent.AppliquerEffet(this, estSombre, Service_Paramètres.Courants.Effet_Fluent);
            }
        }
    }
