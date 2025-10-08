using TXM.Modèles.Licences;

namespace TXM.Vm.Commun
    {
    public class VM_Licence : INotifyPropertyChanged
        {
        // === Informations générales ===
        public string État =>
            Service_Licence.EstEnModeRestreint()
                ? "Démo restreint"
                : (Service_Licence.EstValide() ? "Valide" : "Expirée");

        public string Type => Service_Licence.LicenceActuelle.Type.ToString();
        public string DateActivation => Service_Licence.LicenceActuelle.DateActivation.ToShortDateString();
        public string DateExpiration => Service_Licence.LicenceActuelle.DateExpiration?.ToShortDateString() ?? "Illimitée";

        public string CléSaisie
            {
            get;
            set
                {
                if (field != value)
                    {
                    field = value;
                    OnPropertyChanged();
                    (CmdActiver as RelayCommand)?.RaiseCanExecuteChanged();
                    }
                }
            } = "";

        public bool EstSaisieVisible =>
            Service_Licence.LicenceActuelle.Type == TypeLicence.Demo ||
            Service_Licence.EstEnModeRestreint();

        public string MessageRésultat
            {
            get;
            set { field = value; OnPropertyChanged(); }
            } = "";

        public Brush CouleurRésultat
            {
            get;
            set { field = value; OnPropertyChanged(); }
            } = Brushes.Transparent;

        // === Commande ===
        public ICommand CmdActiver { get; }

        public VM_Licence()
            {
            CmdActiver = new RelayCommand(_ => Activer(), _ => !string.IsNullOrWhiteSpace(CléSaisie));

            // 🔄 écoute automatique des changements de licence
            Service_Licence.LicenceChangée += (_, __) => Rafraîchir();
            }

        private void Activer()
            {
            try
                {
                bool ok = Service_Licence.Activer(CléSaisie);
                if (!ok)
                    {
                    MessageRésultat = "❌ Clé invalide ou non reconnue.";
                    CouleurRésultat = Brushes.IndianRed;
                    _ = MessageBox.Show("Clé invalide ou non reconnue.", "Activation", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                    }

                Rafraîchir();
                MessageRésultat = "✅ Licence activée avec succès !";
                CouleurRésultat = Brushes.LimeGreen;
                _ = MessageBox.Show("Licence activée avec succès !", "Activation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            catch (Exception ex)
                {
                MessageRésultat = "⚠ Erreur lors de l'activation.";
                CouleurRésultat = Brushes.OrangeRed;
                _ = MessageBox.Show($"Erreur lors de l'activation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        /// <summary>
        /// Rafraîchit les propriétés affichées.
        /// </summary>
        private void Rafraîchir()
            {
            OnPropertyChanged(nameof(État));
            OnPropertyChanged(nameof(Type));
            OnPropertyChanged(nameof(DateActivation));
            OnPropertyChanged(nameof(DateExpiration));
            OnPropertyChanged(nameof(EstSaisieVisible));
            }

        // === INotifyPropertyChanged ===
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
