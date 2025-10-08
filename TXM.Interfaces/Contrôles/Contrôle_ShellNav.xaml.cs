using TXM.Modèles.Licences;

namespace TXM.Interfaces.Contrôles
    {
    public partial class Contrôle_ShellNav : UserControl
        {
        public event Action<object?, string>? NavigationDemandée;

        public Contrôle_ShellNav()
            {
            InitializeComponent();
            Loaded += OnLoaded;
            }

        private void OnLoaded(object sender, RoutedEventArgs e)
            {
            AppliquerVisibilitéSelonLicence();

            // Écoute les changements de licence à chaud
            Service_Licence.LicenceChangée += (_, __) =>
            {
                Dispatcher.Invoke(AppliquerVisibilitéSelonLicence);
            };
            }

        private void AppliquerVisibilitéSelonLicence()
            {
            TypeLicence type = Service_Licence.LicenceActuelle.Type;
            bool estDemo = type == TypeLicence.Demo;
            bool estTripleX = type == TypeLicence.TripleX;
            bool estMaestro = type == TypeLicence.Maestro;
            bool estMulti = type == TypeLicence.MultiSite;

            // Pétanque Maestro
            _ = SectionMaestro?.Visibility = estMaestro || estDemo
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            // TripleX
            _ = SectionTripleX?.Visibility = estTripleX || estDemo
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            // Grands Tournois (réservé MultiSite ou Démo)
            _ = SectionGrandsTournois?.Visibility = estMulti || estDemo
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

        private void OnNavClick(object sender, RoutedEventArgs e)
            {
            if (sender is Button b && b.Tag is string tag)
                {
                NavigationDemandée?.Invoke(this, tag);
                }
            }
        }
    }
