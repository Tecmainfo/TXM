namespace TXM.Services
    {
    public static class Service_Gestion_Paramètres
        {
        private static readonly string cheminFichier =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "parametres.json");

        private static readonly string dossierLicences = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "TXM");

        /// <summary>
        /// Charger les paramètres depuis JSON + synchroniser avec licence .dat si présente.
        /// </summary>
        public static Paramètres_Application Charger()
            {
            Paramètres_Application param;

            // Charger depuis JSON
            if (File.Exists(cheminFichier))
                {
                try
                    {
                    string json = File.ReadAllText(cheminFichier);
                    param = JsonSerializer.Deserialize<Paramètres_Application>(json)
                            ?? new Paramètres_Application();
                    }
                catch
                    {
                    param = new Paramètres_Application();
                    }
                }
            else
                {
                param = new Paramètres_Application();
                }

            // 🔹 Vérifier s’il existe un fichier .dat
            if (Directory.Exists(dossierLicences))
                {
                foreach (string fichier in Directory.GetFiles(dossierLicences, "*.dat"))
                    {
                    try
                        {
                        byte[] données = File.ReadAllBytes(fichier);
                        string clair = Encoding.UTF8.GetString(Crypto_Service.Dechiffrer(données));

                        Dictionary<string, string> dict = clair.Split(';')
                                        .Select(part => part.Split('='))
                                        .Where(p => p.Length == 2)
                                        .ToDictionary(p => p[0], p => p[1]);

                        if (dict.TryGetValue("TYPE", out string? type) &&
                            dict.TryGetValue("EXP", out string? expStr) &&
                            DateTime.TryParse(expStr, out DateTime exp))
                            {
                            // Priorité aux infos licence .dat
                            param.Licence_Type = type;
                            param.Licence_Expiration = exp;
                            break;
                            }
                        }
                    catch
                        {
                        // On ignore les fichiers corrompus
                        }
                    }
                }

            return param;
            }

        /// <summary>
        /// Sauvegarder les paramètres dans un fichier JSON.
        /// </summary>
        public static void Sauvegarder(Paramètres_Application paramètres)
            {
            JsonSerializerOptions options = new() { WriteIndented = true };
            string json = JsonSerializer.Serialize(paramètres, options);
            File.WriteAllText(cheminFichier, json);
            }
        }
    }
