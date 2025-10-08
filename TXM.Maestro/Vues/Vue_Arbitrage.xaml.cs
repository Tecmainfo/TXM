namespace TXM.Maestro.Vues
    {
    public partial class Vue_Arbitrage : UserControl
        {
        private readonly VM_Arbitrage _vm;

        public Vue_Arbitrage()
            {
            InitializeComponent();
            _vm = new VM_Arbitrage();
            DataContext = _vm;
            }

        private void OnExporterIncidents(object sender, RoutedEventArgs e)
            {
            var chemin = Service_Export_Pdf.Exporter_Incidents([.. _vm.Incidents]);
            _ = Service_Rapports.Ajouter("Export PDF Incidents", Environment.UserName, chemin);
            Ouvrir(chemin);
            }

        private void OnExporterSanctions(object sender, RoutedEventArgs e)
            {
            var chemin = Service_Export_Pdf.Exporter_Sanctions([.. _vm.Sanctions]);
            _ = Service_Rapports.Ajouter("Export PDF Sanctions", Environment.UserName, chemin);
            Ouvrir(chemin);
            }

        private void OnExporterIncidentsCsv(object sender, RoutedEventArgs e)
            {
            var source = _vm.IncidentsFiltrés.Any() ? _vm.IncidentsFiltrés : _vm.Incidents;
            var chemin = Service_Export_Csv.Exporter_Incidents(source);
            _ = Service_Rapports.Ajouter("Export CSV Incidents", Environment.UserName, chemin);
            Ouvrir(chemin);
            }

        private void OnExporterSanctionsCsv(object sender, RoutedEventArgs e)
            {
            var source = _vm.SanctionsFiltrées.Any() ? _vm.SanctionsFiltrées : _vm.Sanctions;
            var chemin = Service_Export_Csv.Exporter_Sanctions(source);
            _ = Service_Rapports.Ajouter("Export CSV Sanctions", Environment.UserName, chemin);
            Ouvrir(chemin);
            }

        private static void Ouvrir(string chemin)
            {
            try
                {
                if (File.Exists(chemin))
                    {
                    _ = Process.Start(new ProcessStartInfo
                        {
                        FileName = chemin,
                        UseShellExecute = true
                        });
                    }
                }
            catch (Exception ex)
                {
                _ = MessageBox.Show("Impossible d’ouvrir le fichier : " + ex.Message,
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        // === Incidents ===
        private void OnAjouterIncident(object sender, RoutedEventArgs e)
            {
            var date = DateIncident.SelectedDate ?? DateTime.Now;
            var desc = DescriptionIncident.Text?.Trim() ?? "";
            var gravité = (GravitéIncident.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Moyenne";
            var arbitre = ArbitreIncident.Text?.Trim() ?? "";
            var idÉquipe = ComboÉquipeIncident.SelectedValue as int?;
            var idJoueur = ComboJoueurIncident.SelectedValue as int?;

            if (string.IsNullOrWhiteSpace(desc))
                {
                _ = MessageBox.Show("La description est obligatoire.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }

            _vm.AjouterIncident(date, desc, gravité, arbitre, idÉquipe, idJoueur);
            DescriptionIncident.Text = "";
            ArbitreIncident.Text = "";
            GravitéIncident.SelectedIndex = -1;
            ComboÉquipeIncident.SelectedIndex = -1;
            ComboJoueurIncident.SelectedIndex = -1;
            }

        private void OnSupprimerIncident(object sender, RoutedEventArgs e)
            {
            if (_vm.IncidentSélectionné == null)
                {
                return;
                }

            if (MessageBox.Show("Supprimer l’incident sélectionné ?", "Confirmation",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                _vm.SupprimerIncident(_vm.IncidentSélectionné.Id);
                }
            }

        private void OnCreerSanctionDepuisIncident(object sender, RoutedEventArgs e)
            {
            var incident = _vm.IncidentSélectionné;
            if (incident == null)
                {
                _ = MessageBox.Show("Sélectionner un incident d’abord.", "Info",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }

            // Pré-remplir le formulaire Sanctions
            DateSanction.SelectedDate = DateTime.Now;
            TypeSanction.SelectedIndex = 0; // Avertissement
            MotifSanction.Text = incident.Description;
            ArticleSanction.Text = "";
            ArbitreSanction.Text = incident.Arbitre ?? "";
            DuréeSanction.Text = "0";
            IncidentLié.SelectedValue = incident.Id;

            _ = MotifSanction.Focus();
            }

        // === Sanctions ===
        private void OnAjouterSanction(object sender, RoutedEventArgs e)
            {
            var date = DateSanction.SelectedDate ?? DateTime.Now;
            var type = (TypeSanction.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Avertissement";
            var motif = MotifSanction.Text?.Trim() ?? "";
            var article = ArticleSanction.Text?.Trim() ?? "";
            var arbitre = ArbitreSanction.Text?.Trim() ?? "";
            _ = int.TryParse(DuréeSanction.Text?.Trim(), out var durée);
            var idIncident = IncidentLié.SelectedValue as int?;

            if (string.IsNullOrWhiteSpace(motif))
                {
                _ = MessageBox.Show("Le motif est obligatoire.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }

            _vm.AjouterSanction(date, type, motif, article, arbitre, durée, idIncident);
            MotifSanction.Text = "";
            ArticleSanction.Text = "";
            ArbitreSanction.Text = "";
            DuréeSanction.Text = "";
            IncidentLié.SelectedIndex = -1;
            TypeSanction.SelectedIndex = -1;
            }

        private void OnSupprimerSanction(object sender, RoutedEventArgs e)
            {
            if (_vm.SanctionSélectionnée == null)
                {
                return;
                }

            if (MessageBox.Show("Supprimer la sanction sélectionnée ?", "Confirmation",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                _vm.SupprimerSanction(_vm.SanctionSélectionnée.Id);
                }
            }

        private void OnFiltreDateChanged(object sender, SelectionChangedEventArgs e)
            {
            _vm.AppliquerFiltres(FiltreDate.SelectedDate, FiltreTexte.Text);
            }

        private void OnFiltreTexteChanged(object sender, TextChangedEventArgs e)
            {
            _vm.AppliquerFiltres(FiltreDate.SelectedDate, FiltreTexte.Text);
            }
        }
    }
