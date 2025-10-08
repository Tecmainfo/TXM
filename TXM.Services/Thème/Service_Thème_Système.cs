/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Shell Interfaces)
*  Projet Principal        : TXM.Interfaces
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C2 - Interne
*  Version                 : 25.10.7.1.0
*  Nom du fichier          : Service_Thème_Système.cs
*  Description             : Gestion complète du thème adaptatif (clair/sombre)
*                            et du branding actif (TripleX / Maestro / Démo / Grands Tournois)
* * * * * * * * * * * * *
*/

namespace TXM.Services.Thème
    {
    /// <summary>
    /// Service central gérant les thèmes TXM et leur adaptation au mode clair/sombre Windows 11.
    /// </summary>
    public sealed class Service_Thème_Système
        {
        private static ResourceDictionary? _courant;
        private const string _settingsFile = "appsettings.json";

        // === chemins de thèmes ===
        private static readonly string _path_TripleX_Clair =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/TripleX_Thème.xaml";
        private static readonly string _path_TripleX_Sombre =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/TripleX_Thème.xaml";

        private static readonly string _path_Maestro_Clair =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/Maestro_Thème.xaml";
        private static readonly string _path_Maestro_Sombre =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/Maestro_Thème.xaml";

        private static readonly string _path_Démo =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/Démo_Thème.xaml";

        private static readonly string _path_GrandsTournois_Clair =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/GrandsTournois_Thème_Clair.xaml";
        private static readonly string _path_GrandsTournois_Sombre =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/GrandsTournois_Thème_Sombre.xaml";

        public static Thème_Id ThèmeActif { get; private set; } = Thème_Id.TripleX;
        public static event Action<bool>? ThèmeSombreChangée; // true = sombre, false = clair

        // --- Initialisation ---
        public static void Initialiser()
            {
            AppliquerSelonWindows();

            SystemEvents.UserPreferenceChanged += (_, e) =>
            {
                if (e.Category == UserPreferenceCategory.General)
                    {
                    Application.Current?.Dispatcher.Invoke(AppliquerSelonWindows, DispatcherPriority.Background);
                    }
            };
            }

        // --- Application directe d’un thème ---
        public static void Appliquer_Thème(Thème_Id id)
            {
            if (Application.Current is not Application app)
                {
                return;
                }

            string path = id switch
                {
                    Thème_Id.TripleX => _path_TripleX_Clair,
                    Thème_Id.Maestro => _path_Maestro_Clair,
                    Thème_Id.Démo => _path_Démo,
                    Thème_Id.GrandsTournois => _path_GrandsTournois_Clair,
                    _ => _path_TripleX_Clair
                    };

            var nouveau = new ResourceDictionary { Source = new Uri(path, UriKind.Absolute) };

            if (_courant != null)
                {
                _ = app.Resources.MergedDictionaries.Remove(_courant);
                }

            app.Resources.MergedDictionaries.Add(nouveau);
            _courant = nouveau;
            ThèmeActif = id;
            Sauver_Thème(id);
            }

        // --- Persistance du choix ---
        private static void Sauver_Thème(Thème_Id id)
            {
            try
                {
                var json = JsonSerializer.Serialize(new { Thème = id.ToString() },
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFile, json);
                }
            catch { /* tolérant */ }
            }

        // --- Adaptation automatique au mode système ---
        private static void AppliquerSelonWindows()
            {
            try
                {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                bool useLight = (int?)key?.GetValue("AppsUseLightTheme", 1) == 1;
                bool estSombre = !useLight;

                // --- Détermination du branding actif ---
                string path = _path_TripleX_Clair;

                var licence = Service_Licence.LicenceActuelle.Type.ToString().ToLowerInvariant();
                if (licence.Contains("maestro"))
                    {
                    path = useLight ? _path_Maestro_Clair : _path_Maestro_Sombre;
                    }
                else
                    {
                    path = licence.Contains("triplex")
                        ? useLight ? _path_TripleX_Clair : _path_TripleX_Sombre
                        : licence.Contains("grands") || licence.Contains("multi")
                    ? useLight ? _path_GrandsTournois_Clair : _path_GrandsTournois_Sombre
                    : _path_Démo;
                    }

                // --- Application du thème ---
                if (Application.Current is Application app)
                    {
                    var rd = new ResourceDictionary { Source = new Uri(path, UriKind.Absolute) };

                    if (_courant != null)
                        {
                        _ = app.Resources.MergedDictionaries.Remove(_courant);
                        }

                    app.Resources.MergedDictionaries.Add(rd);
                    _courant = rd;
                    }

                // --- Couleurs de base TXM globales ---
                Application.Current.Resources["App.CouleurFond"] =
                    new SolidColorBrush(useLight ? Color.FromRgb(250, 250, 250) : Color.FromRgb(18, 18, 18));
                Application.Current.Resources["App.CouleurTexte"] =
                    new SolidColorBrush(useLight ? Color.FromRgb(33, 33, 33) : Color.FromRgb(240, 240, 240));
                Application.Current.Resources["App.CouleurSecondaire"] =
                    new SolidColorBrush(useLight ? Color.FromRgb(230, 230, 230) : Color.FromRgb(43, 43, 43));
                Application.Current.Resources["App.BarreTitreFond"] =
                    new SolidColorBrush(useLight ? Color.FromRgb(255, 255, 255) : Color.FromRgb(27, 27, 27));
                Application.Current.Resources["App.BarreTitreTexte"] =
                    new SolidColorBrush(useLight ? Color.FromRgb(0, 0, 0) : Color.FromRgb(255, 255, 255));

                // --- Application du mode MaterialDesign ---
                if (Application.Current.Resources.MergedDictionaries[0] is BundledTheme theme)
                    {
                    theme.BaseTheme = useLight ? BaseTheme.Light : BaseTheme.Dark;
                    }

                // --- Notification et Fluent ---
                ThèmeSombreChangée?.Invoke(estSombre);
                if (Application.Current.MainWindow != null)
                    {
                    Service_Fluent.AppliquerEffet(Application.Current.MainWindow, estSombre);
                    }

                Console.WriteLine($"[TXM Thème] Thème {licence} {(useLight ? "clair" : "sombre")} appliqué.");
                }
            catch (Exception ex)
                {
                Console.WriteLine($"[TXM Thème] Erreur d'application : {ex.Message}");
                }
            }
        }
    }
