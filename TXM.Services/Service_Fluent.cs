/*
* * * * *
* Nom du fichier       : Service_Fluent.cs
* Projet Principal     : TXM.Services
* Auteur               : Kano
* Version              : 2025.10.3.2.0
* Description          : Application d’un effet Fluent (Mica/Acrylic) adaptatif selon le mode clair/sombre Windows 11
* * * * *
*/

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace TXM.Services
    {
    /// <summary>
    /// Service Fluent/Mica pour appliquer un effet de fond natif Windows 11
    /// (Mica, Acrylic ou solide), synchronisé avec le thème TXM.
    /// </summary>
    public static class Service_Fluent
        {
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38; // Windows 11 build >= 22000
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// <summary>
        /// Applique un effet Fluent sur une fenêtre donnée.
        /// </summary>
        /// <param name="fenetre">Fenêtre cible</param>
        /// <param name="modeSombre">True pour le mode sombre</param>
        /// <param name="typeEffet">"Mica", "Acrylic", "Aucun"</param>
        public static void AppliquerEffet(Window fenetre, bool modeSombre, string? typeEffet = "Mica")
            {
            if (fenetre == null) return;

            try
                {
                var handle = new WindowInteropHelper(fenetre).EnsureHandle();

                // 🔸 Type d’effet
                uint effetType = typeEffet?.ToLower() switch
                    {
                        "acrylic" => 3, // Acrylic
                        "aucun" => 1,   // Solide
                        _ => 2          // Mica (par défaut)
                        };

                _ = DwmSetWindowAttribute(handle, DWMWA_SYSTEMBACKDROP_TYPE, ref effetType, sizeof(uint));

                // 🔸 Mode sombre ou clair
                int dark = modeSombre ? 1 : 0;
                _ = DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));

                // 🔸 Couleur de fond translucide adaptée
                fenetre.Background = new SolidColorBrush(modeSombre
                    ? Color.FromArgb(190, 25, 25, 25)  // Sombre
                    : Color.FromArgb(240, 250, 250, 250)); // Clair
                }
            catch (Exception ex)
                {
                Console.WriteLine($"[Fluent] Erreur : {ex.Message}");
                fenetre.Background = new SolidColorBrush(Colors.Gray);
                }
            }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref uint attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        }
    }
