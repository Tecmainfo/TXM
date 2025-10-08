namespace TXM.Services.Licences
    {
    /// <summary>
    /// Lecture directe d’un fichier de licence chiffré (.dat).
    /// Utilisé en compatibilité ou pour recharger manuellement une licence.
    /// </summary>
    public static class Licence_Loader
        {
        private static readonly string DossierLicences = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "TXM");

        /// <summary>
        /// Charge la première licence valide trouvée dans %ProgramData%\TXM.
        /// </summary>
        /// <returns>Une instance de <see cref="Licence"/> ou null si aucune licence valide n’a été trouvée.</returns>
        public static Licence? Charger_Licence()
            {
            if (!Directory.Exists(DossierLicences))
                {
                return null;
                }

            foreach (string fichier in Directory.GetFiles(DossierLicences, "*.dat"))
                {
                try
                    {
                    byte[] données = File.ReadAllBytes(fichier);
                    byte[] clair = Crypto_Service.Dechiffrer(données);
                    string contenu = Encoding.UTF8.GetString(clair);

                    if (!contenu.StartsWith("CLE=", StringComparison.OrdinalIgnoreCase))
                        {
                        continue;
                        }

                    // Format attendu : CLE=...;TYPE=...;DATE=...;EXP=...
                    string[] parts = contenu.Split(';', StringSplitOptions.RemoveEmptyEntries);

                    string type = "";
                    DateTime activation = DateTime.UtcNow;
                    DateTime? expiration = null;

                    foreach (string p in parts)
                        {
                        var kv = p.Split('=');
                        if (kv.Length != 2)
                            {
                            continue;
                            }

                        switch (kv[0].ToUpperInvariant())
                            {
                            case "TYPE":
                                type = kv[1].Trim();
                                break;
                            case "DATE":
                                if (DateTime.TryParse(kv[1].Trim(), out var d))
                                    {
                                    activation = d;
                                    }

                                break;
                            case "EXP":
                                if (DateTime.TryParse(kv[1].Trim(), out var e))
                                    {
                                    expiration = e;
                                    }

                                break;
                            }
                        }

                    // Mapping du type
                    TypeLicence typeLicence = type.ToUpperInvariant() switch
                        {
                            "ASSOCIATION" or "MULTISITE" or "TRIPLEX" => TypeLicence.TripleX,
                            "COMPETITION" or "MAESTRO" => TypeLicence.Maestro,
                            _ => TypeLicence.Demo
                            };

                    return new Licence
                        {
                        Type = typeLicence,
                        DateActivation = activation,
                        DateExpiration = expiration
                        };
                    }
                catch
                    {
                    // Ignorer les fichiers corrompus ou non déchiffrables
                    }
                }

            return null;
            }
        }
    }
