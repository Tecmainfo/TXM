using TXM.Modèles.Licences;

namespace TXM.Interfaces.Vues
    {
    public partial class Fenêtre_Splash : Window
        {
        private bool _attente_Choix;
        private bool _chargement_Lancé;

        public Fenêtre_Splash()
            {
            // 🔹 Appliquer le thème AVANT InitializeComponent
            Appliquer_Thème_Avec_Licence();

            InitializeComponent();

            Service_Settings.Load();
            Skip_Splash = Service_Settings.Skip_Splash;

            Loaded += async (_, __) =>
            {
                _ = Focus();
                _ = Keyboard.Focus(this);

                if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift && Service_Settings.Skip_Splash)
                    {
                    await Démarrer_Chargement_Async();
                    return;
                    }

                _attente_Choix = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

                if (_attente_Choix)
                    Barre.IsIndeterminate = true;
                else
                    await Démarrer_Chargement_Async();
            };
            }

        // === Reste inchangé ===
        public bool Skip_Splash
            {
            get => (bool)GetValue(Skip_SplashProperty);
            set => SetValue(Skip_SplashProperty, value);
            }

        public static readonly DependencyProperty Skip_SplashProperty =
            DependencyProperty.Register(nameof(Skip_Splash), typeof(bool), typeof(Fenêtre_Splash),
                new PropertyMetadata(false, (d, e) =>
                {
                    Service_Settings.Skip_Splash = (bool)e.NewValue;
                }));

        private static void Appliquer_Thème_Avec_Licence()
            {
            Licence licence = Service_Licence.LicenceActuelle;

            if (licence == null)
                {
                Service_Branding.Initialiser("demo");
                Branding_Ressource_Service.Charger_Dans_Ressources(Application.Current);
                App.Service_Thème.Appliquer_Thème(Thème_Id.Démo);
                return;
                }

            switch (licence.Type)
                {
                case TypeLicence.Maestro:
                    Service_Branding.Initialiser("maestro");
                    Branding_Ressource_Service.Charger_Dans_Ressources(Application.Current);
                    App.Service_Thème.Appliquer_Thème(Thème_Id.Maestro);
                    break;

                case TypeLicence.TripleX:
                    Service_Branding.Initialiser("triplex");
                    Branding_Ressource_Service.Charger_Dans_Ressources(Application.Current);
                    App.Service_Thème.Appliquer_Thème(Thème_Id.TripleX);
                    break;

                default:
                    Service_Branding.Initialiser("demo");
                    Branding_Ressource_Service.Charger_Dans_Ressources(Application.Current);
                    App.Service_Thème.Appliquer_Thème(Thème_Id.Démo);
                    break;
                }
            }

        private async Task Démarrer_Chargement_Async()
            {
            if (_chargement_Lancé)
                return;

            _chargement_Lancé = true;

            Barre.IsIndeterminate = false;
            Progress<int> progression = new Progress<int>(p => Barre.Value = p);
            await Chargeur_Demarrage.ExecuterAsync(progression, CancellationToken.None, 1500);

            await FadeOut_Async();
            Close();
            }

        private Task<bool> FadeOut_Async()
            {
            if (Resources["FadeOutStoryboard"] is not Storyboard sb)
                return Task.FromResult(false);

            TaskCompletionSource<bool> tcs = new();
            sb.Completed += (_, __) => tcs.TrySetResult(true);
            sb.Begin(this);
            return tcs.Task;
            }
        }
    }
