
namespace TXM.Maestro.Vues
    {
    public partial class Vue_Règlements : UserControl
        {
        private readonly VM_Règlements _vm;

        // champ privé : facteur de zoom courant
        private double _zoom = 1.0;

        public Vue_Règlements()
            {
            InitializeComponent();
            _vm = new VM_Règlements();
            DataContext = _vm;

            int now = DateTime.Now.Year;
            for (int a = now; a >= now - 10; a--)
                {
                _ = ComboAnnée.Items.Add(a);
                }

            ComboAnnée.SelectedItem = now;

            // init WebView2 (non bloquant si runtime absent)
            Loaded += async (_, __) =>
            {
                try
                    {
                    await PdfViewer.EnsureCoreWebView2Async();
                    PdfViewer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                    TrySetZoom(_zoom);
                    }
                catch
                    {
                    // Runtime manquant → on utilisera l’ouverture externe
                    }
            };
            }

        private void OnRafraichir(object sender, RoutedEventArgs e)
            {
            int? an = ComboAnnée.SelectedItem is int a ? a : null;
            string filtre = FiltreTexte.Text;
            _vm.Charger(an, filtre);
            MettreApercu(_vm.RèglementSélectionné);
            }

        private async void OnImporterUrl(object sender, RoutedEventArgs e)
            {
            if (ComboAnnée.SelectedItem is not int année)
                {
                _ = MessageBox.Show("Choisir une année.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }
            if (string.IsNullOrWhiteSpace(SaisieUrl.Text) ||
                string.IsNullOrWhiteSpace(SaisieTitre.Text) ||
                string.IsNullOrWhiteSpace(SaisieCatégorie.Text))
                {
                _ = MessageBox.Show("URL, Titre et Catégorie sont requis.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }
            await VM_Règlements.Importer_Depuis_Url(année, SaisieTitre.Text.Trim(), SaisieCatégorie.Text.Trim(), SaisieUrl.Text.Trim());
            OnRafraichir(sender, e);
            }

        private void OnImporterFichier(object sender, RoutedEventArgs e)
            {
            if (ComboAnnée.SelectedItem is not int année)
                {
                _ = MessageBox.Show("Choisir une année.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }
            var dlg = new OpenFileDialog
                {
                Filter = "PDF|*.pdf",
                Title = "Choisir un fichier PDF",
                Multiselect = true
                };
            if (dlg.ShowDialog() == true)
                {
                foreach (string? file in dlg.FileNames)
                    {
                    string titre = Path.GetFileNameWithoutExtension(file);
                    string catégorie = "Général";
                    VM_Règlements.Importer_Depuis_Fichier(année, titre, catégorie, file);
                    }
                OnRafraichir(sender, e);
                }
            }

        private void OnSupprimerClick(object sender, RoutedEventArgs e)
            {
            if (sender is Button b && b.Tag is Règlement r)
                {
                if (MessageBox.Show($"Supprimer « {r.Titre} » ({r.Année}) ?", "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                    VM_Règlements.Supprimer(r);
                    OnRafraichir(sender, e);
                    }
                }
            }

        private void OnOuvrirClick(object sender, RoutedEventArgs e)
            {
            if (sender is Button b && b.Tag is Règlement r)
                {
                VM_Règlements.Ouvrir(r);
                }
            }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            if (DataContext is VM_Règlements vm)
                {
                MettreApercu(vm.RèglementSélectionné);
                }
            }

        // === Zoom ===
        private void OnZoomPlus(object sender, RoutedEventArgs e)
            {
            _zoom = Math.Min(3.0, _zoom + 0.1);
            TrySetZoom(_zoom);
            }

        private void OnZoomMoins(object sender, RoutedEventArgs e)
            {
            _zoom = Math.Max(0.5, _zoom - 0.1);
            TrySetZoom(_zoom);
            }

        private void TrySetZoom(double value)
            {
            // 1) API WPF : propriété ZoomFactor sur le contrôle WebView2
            try
                {
                PdfViewer.ZoomFactor = value; // <-- CORRECT
                return;
                }
            catch
                {
                // 2) fallback : zoom CSS (si le SDK ne l’expose pas)
                if (PdfViewer?.CoreWebView2 != null)
                    {
                    string z = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    _ = PdfViewer.CoreWebView2.ExecuteScriptAsync(
                        $"document.body && (document.body.style.zoom='{z}')");
                    }
                }
            }

        // Impression
        private async void OnImprimer(object sender, RoutedEventArgs e)
            {
            if (PdfViewer?.CoreWebView2 is null)
                {
                return;
                }

            try { _ = await PdfViewer.CoreWebView2.ExecuteScriptAsync("window.print()"); }
            catch { /* silencieux */ }
            }

        // Menus contextuels
        private void OnOuvrirDepuisMenu(object sender, RoutedEventArgs e)
            {
            if (GridReglements.SelectedItem is Règlement r)
                {
                VM_Règlements.Ouvrir(r);
                }
            }

        private void OnRevelerDepuisMenu(object sender, RoutedEventArgs e)
            {
            if (GridReglements.SelectedItem is Règlement r)
                {
                Service_Règlements.Révéler_Dans_Explorateur(r);
                }
            }

        private void OnSupprimerDepuisMenu(object sender, RoutedEventArgs e)
            {
            if (GridReglements.SelectedItem is Règlement r)
                {
                if (MessageBox.Show($"Supprimer « {r.Titre} » ?", "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                    VM_Règlements.Supprimer(r);
                    OnRafraichir(sender, e);
                    }
                }
            }

        private void MettreApercu(Règlement? r)
            {
            if (r == null)
                {
                return;
                }

            if (File.Exists(r.Fichier_Local))
                {
                var uri = new Uri(r.Fichier_Local);

                // Si le core est prêt, on navigue via Core; sinon on assigne Source
                if (PdfViewer.CoreWebView2 != null)
                    {
                    PdfViewer.CoreWebView2.Navigate(uri.AbsoluteUri);
                    }
                else
                    {
                    PdfViewer.Source = uri;
                    }

                // remettre le zoom en cohérence
                TrySetZoom(_zoom);
                }
            }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
            {
            if (e.EditAction != DataGridEditAction.Commit)
                {
                return;
                }

            if (e.Row.Item is Règlement r)
                {
                VM_Règlements.MettreÀJour_Métadonnées(r);
                }
            }

        private void OnExporterCsv(object sender, RoutedEventArgs e)
            {
            string chemin = _vm.Exporter_Csv();
            try
                {
                _ = Process.Start(new ProcessStartInfo { FileName = chemin, UseShellExecute = true });
                }
            catch
                {
                _ = MessageBox.Show($"Exporté: {chemin}", "CSV", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        private void OnOuvrirDossier(object sender, RoutedEventArgs e)
            {
            string dossier = Service_Règlements.Dossier_Racine;
            _ = Directory.CreateDirectory(dossier);
            _ = Process.Start(new ProcessStartInfo { FileName = dossier, UseShellExecute = true });
            }

        // === Drag & Drop ===
        private void OnPreviewDragOver(object sender, DragEventArgs e)
            {
            bool ok = false;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ok = files.All(f => Path.GetExtension(f).Equals(".pdf", StringComparison.OrdinalIgnoreCase));
                }
            else if (e.Data.GetDataPresent(DataFormats.Text))
                {
                string txt = (e.Data.GetData(DataFormats.Text) as string)?.Trim() ?? "";
                ok = Uri.TryCreate(txt, UriKind.Absolute, out var u) && u.AbsoluteUri.ToLower().EndsWith(".pdf");
                }

            e.Effects = ok ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
            }

        private async void OnDropFiles(object sender, DragEventArgs e)
            {
            if (ComboAnnée.SelectedItem is not int année)
                {
                _ = MessageBox.Show("Choisir une année d’abord.");
                return;
                }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string? f in files.Where(f => Path.GetExtension(f).Equals(".pdf", StringComparison.OrdinalIgnoreCase)))
                    {
                    string titre = Path.GetFileNameWithoutExtension(f);
                    VM_Règlements.Importer_Depuis_Fichier(année, titre, "Général", f);
                    }
                OnRafraichir(sender, e);
                return;
                }

            if (e.Data.GetDataPresent(DataFormats.Text))
                {
                string txt = (e.Data.GetData(DataFormats.Text) as string)?.Trim() ?? "";
                if (Uri.TryCreate(txt, UriKind.Absolute, out var u) && u.AbsoluteUri.ToLower().EndsWith(".pdf"))
                    {
                    string titre = "PDF FFPJP";
                    await VM_Règlements.Importer_Depuis_Url(année, titre, "Général", u.AbsoluteUri);
                    OnRafraichir(sender, e);
                    }
                }
            }
        }
    }
