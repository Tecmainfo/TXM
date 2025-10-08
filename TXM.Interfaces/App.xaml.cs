/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Shell Interfaces)
*  Projet Principal        : TXM.Interfaces
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C2 - Interne
*  Version                 : 25.10.3.2.0
*  Nom du fichier          : App.xaml.cs
*  Description             : Point d’entrée principal – initialisation de la base, licence,
*                            thème adaptatif Windows 11, branding et splash screen.
* * * * * * * * * * * * *
*/

using TXM.Modèles.Licences;
using TXM.Services.Thème;

namespace TXM.Interfaces
    {
    /// <summary>
    /// Application TXM – Point d’entrée principal (WPF .NET 9 / Windows 11+)
    /// </summary>
    public partial class App : Application
        {
        public static Service_Thème Service_Thème { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
            {
            base.OnStartup(e);

            try
                {
                Console.WriteLine("=== 🏁 Démarrage TXM ===");

                // 1️⃣ Initialisation de la base locale SQLite
                Schéma_SQLite.Initialiser();
                ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // 2️⃣ Charger les paramètres utilisateur
                Paramètres_Application paramètres = Service_Paramètres.Charger();
                Console.WriteLine("[Paramètres] Chargés avec succès.");

                // 3️⃣ Charger la licence (.dat / fallback JSON)
                Service_Licence.Charger();
                Console.WriteLine($"[Licence] Type : {Service_Licence.LicenceActuelle?.Type}");

                // 4️⃣ Activer la surveillance du thème système Windows 11
                Service_Thème_Système.Initialiser();
                Console.WriteLine("[Thème] Synchronisation automatique avec Windows activée.");

                // --- Synchronisation JSON ↔ .dat automatique faite par Service_Licence ---

                // 5️⃣ Déterminer le branding et le thème selon la licence
                TypeLicence type = Service_Licence.LicenceActuelle?.Type ?? TypeLicence.Demo;
                string brandingKey;
                Thème_Id themeId;

                switch (type)
                    {
                    case TypeLicence.MultiSite:
                        brandingKey = "GrandsTournois";
                        themeId = Thème_Id.GrandsTournois;
                        break;

                    case TypeLicence.TripleX:
                        brandingKey = "triplex";
                        themeId = Thème_Id.TripleX;
                        break;

                    case TypeLicence.Maestro:
                        brandingKey = "maestro";
                        themeId = Thème_Id.Maestro;
                        break;

                    default:
                        brandingKey = "demo";
                        themeId = Thème_Id.Démo;
                        break;
                    }

                // 6️⃣ Initialiser le branding et le thème
                Service_Branding.Initialiser(brandingKey);
                Branding_Ressource_Service.Charger_Dans_Ressources(Application.Current);
                Service_Thème.Appliquer_Thème(themeId);
                Console.WriteLine($"[Branding] {brandingKey} | [Thème] {themeId}");

                // 7️⃣ Activer les "feature flags"
                Feature_Flag_Service.Initialiser(brandingKey);

                // 8️⃣ Splash screen (désactivable via paramètre ou SHIFT)
                bool forceSplash = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
                bool afficherSplash = !paramètres.Skip_Splash || forceSplash;

                if (afficherSplash)
                    {
                    try
                        {
                        Fenêtre_Splash splash = new();
                        _ = splash.ShowDialog();
                        Console.WriteLine("[Splash] Affiché avec succès.");
                        }
                    catch (Exception ex)
                        {
                        Console.WriteLine($"[Splash] Ignoré : {ex.Message}");
                        }
                    }

                // 9️⃣ Lancer la fenêtre principale (effet Fluent + thème actif)
                MainWindow main = new();

                // Réappliquer branding + thème après chargement visuel
                Branding_Ressource_Service.Charger_Dans_Ressources(Current);
                Service_Thème.Appliquer_Thème(themeId);

                // Appliquer Fluent selon thème système
                bool modeSombre = (int?)Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")
                    ?.GetValue("AppsUseLightTheme", 1) == 0;

                Service_Fluent.AppliquerEffet(main, modeSombre);

                MainWindow = main;
                main.Show();

                ShutdownMode = ShutdownMode.OnMainWindowClose;
                Console.WriteLine("=== ✅ TXM prêt à l’utilisation ===");
                }
            catch (Exception ex)
                {
                _ = MessageBox.Show(
                    $"Erreur critique au démarrage :\n{ex.Message}",
                    "Erreur TXM",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Console.WriteLine($"[Erreur TXM] {ex}");
                Current.Shutdown();
                }
            }
        }
    }
