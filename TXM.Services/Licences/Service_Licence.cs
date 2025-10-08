namespace TXM.Services.Licences
    {
    /// <summary>
    /// Service de gestion de la licence TXM – lecture, écriture et activation.
    /// Unifie l'ancien système JSON et le système .dat chiffré.
    /// </summary>
    public static class Service_Licence
        {
        private static readonly string DossierGlobal =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TXM");

        private static readonly string DossierLocal =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TXM");

        private static readonly string FichierLicenceJson = Path.Combine(DossierLocal, "licence.json");

        public static Licence LicenceActuelle { get; private set; } = new();

        /// <summary>
        /// 🔔 Événement déclenché à chaque changement de licence.
        /// </summary>
        public static event EventHandler? LicenceChangée;

        // --- Chargement principal ---
        public static void Charger()
            {
            try
                {
                // 1️⃣ Tentative de lecture fichier .dat chiffré
                Licence? info = LireDepuisDat();
                if (info != null)
                    {
                    LicenceActuelle = info;
                    Sauvegarder(GetOptions()); // synchro JSON pour compatibilité
                    NotifierChangement();
                    return;
                    }

                // 2️⃣ Fallback : licence.json
                if (File.Exists(FichierLicenceJson))
                    {
                    string json = File.ReadAllText(FichierLicenceJson);
                    LicenceActuelle = JsonSerializer.Deserialize<Licence>(json) ?? new Licence();
                    }
                else
                    {
                    // 3️⃣ Première exécution : licence démo 30j
                    LicenceActuelle = new Licence
                        {
                        Type = TypeLicence.Demo,
                        DateActivation = DateTime.Now,
                        DateExpiration = DateTime.Now.AddDays(30)
                        };
                    Sauvegarder(GetOptions());
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine($"[Licence] Erreur chargement : {ex.Message}");
                LicenceActuelle = new Licence { Type = TypeLicence.Demo, DateActivation = DateTime.Now };
                }

            NotifierChangement();
            }

        // --- Activation depuis clé saisie ---
        public static bool Activer(string clé)
            {
            if (string.IsNullOrWhiteSpace(clé))
                {
                return false;
                }

            try
                {
                string type = "DEMO";
                DateTime expiration = DateTime.UtcNow.AddMonths(1);

                if (clé.StartsWith("M-") || clé.Contains("MAESTRO") || clé.Contains("COMP"))
                    {
                    type = "COMPETITION";
                    expiration = DateTime.UtcNow.AddYears(5);
                    LicenceActuelle.Type = TypeLicence.Maestro;
                    }
                else if (clé.StartsWith("T-") || clé.Contains("TRIPLEX") || clé.Contains("ASSO"))
                    {
                    type = "ASSOCIATION";
                    expiration = DateTime.MaxValue;
                    LicenceActuelle.Type = TypeLicence.TripleX;
                    }
                else if (clé.StartsWith("MU-") || clé.Contains("MULTI"))
                    {
                    type = "MULTISITE";
                    expiration = DateTime.MaxValue;
                    LicenceActuelle.Type = TypeLicence.MultiSite;
                    }
                else
                    {
                    return false;
                    }

                LicenceActuelle.DateActivation = DateTime.Now;
                LicenceActuelle.DateExpiration = expiration == DateTime.MaxValue ? null : expiration;

                // 🔒 Contenu chiffré
                string contenu = $"CLE={clé};TYPE={type};DATE={DateTime.UtcNow:O};EXP={expiration:O}";
                byte[] clair = Encoding.UTF8.GetBytes(contenu);
                byte[] chiffre = Crypto_Service.Chiffrer(clair);

                // 🔹 Enregistrement dans ProgramData ou AppData
                string dossier = DossierGlobal;
                try
                    {
                    _ = Directory.CreateDirectory(dossier);
                    string fichier = Path.Combine(dossier, $"{Guid.NewGuid():N}.dat");
                    File.WriteAllBytes(fichier, chiffre);
                    File.SetAttributes(fichier, FileAttributes.Hidden);
                    }
                catch (UnauthorizedAccessException)
                    {
                    dossier = DossierLocal;
                    _ = Directory.CreateDirectory(dossier);
                    string fichier = Path.Combine(dossier, $"{Guid.NewGuid():N}.dat");
                    File.WriteAllBytes(fichier, chiffre);
                    File.SetAttributes(fichier, FileAttributes.Hidden);
                    }

                // 🔁 Sauvegarde JSON miroir
                Sauvegarder(GetOptions());
                NotifierChangement();

                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine($"[Licence] Erreur activation : {ex.Message}");
                return false;
                }
            }

        // --- Lecture des fichiers .dat ---
        private static Licence? LireDepuisDat()
            {
            foreach (string dossier in new[] { DossierGlobal, DossierLocal })
                {
                if (!Directory.Exists(dossier))
                    {
                    continue;
                    }

                foreach (string fichier in Directory.GetFiles(dossier, "*.dat"))
                    {
                    try
                        {
                        byte[] bytes = File.ReadAllBytes(fichier);
                        string clair = Encoding.UTF8.GetString(Crypto_Service.Dechiffrer(bytes));

                        string[] dict = clair.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        string type = "";
                        DateTime date = DateTime.MinValue;
                        DateTime exp = DateTime.MaxValue;

                        foreach (string part in dict)
                            {
                            string[] kv = part.Split('=');
                            if (kv.Length != 2)
                                {
                                continue;
                                }

                            switch (kv[0])
                                {
                                case "TYPE": type = kv[1]; break;
                                case "DATE": _ = DateTime.TryParse(kv[1], out date); break;
                                case "EXP": _ = DateTime.TryParse(kv[1], out exp); break;
                                }
                            }

                        if (!string.IsNullOrWhiteSpace(type))
                            {
                            TypeLicence t = type.ToUpperInvariant() switch
                                {
                                    "MULTISITE" or "GRANDSTOURNOIS" or "TOURNOI" => TypeLicence.MultiSite,
                                    "ASSOCIATION" or "TRIPLEX" or "ASSO" => TypeLicence.TripleX,
                                    "COMPETITION" or "MAESTRO" => TypeLicence.Maestro,
                                    _ => TypeLicence.Demo
                                    };

                            return new Licence
                                {
                                Type = t,
                                DateActivation = date == DateTime.MinValue ? DateTime.Now : date,
                                DateExpiration = exp == DateTime.MaxValue ? null : exp
                                };
                            }
                        }
                    catch
                        {
                        // ignorer fichiers corrompus
                        }
                    }
                }

            return null;
            }

        // --- Utilitaires ---
        public static JsonSerializerOptions GetOptions()
            {
            return new() { WriteIndented = true };
            }

        public static void Sauvegarder(JsonSerializerOptions opt)
            {
            try
                {
                _ = Directory.CreateDirectory(Path.GetDirectoryName(FichierLicenceJson)!);
                string json = JsonSerializer.Serialize(LicenceActuelle, opt);
                File.WriteAllText(FichierLicenceJson, json);
                }
            catch (Exception ex)
                {
                Console.WriteLine($"[Licence] Erreur sauvegarde : {ex.Message}");
                }
            }

        public static bool EstValide()
            {
            return LicenceActuelle.Type != TypeLicence.Demo || !LicenceActuelle.DateExpiration.HasValue || DateTime.Now <= LicenceActuelle.DateExpiration.Value;
            }

        public static bool EstEnModeRestreint()
            {
            return LicenceActuelle.Type == TypeLicence.Demo && !EstValide();
            }

        /// <summary>
        /// 🔔 Notifie tout le logiciel qu’un changement de licence a eu lieu.
        /// </summary>
        public static void NotifierChangement()
            {
            LicenceChangée?.Invoke(null, EventArgs.Empty);
            }
        }
    }
