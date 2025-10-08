namespace TXM.Migrateur
    {
    internal class Program
        {
        private static readonly JsonSerializerOptions _jsonOptions = new()
            {
            WriteIndented = true
            };

        private static void Main(string[] args)
            {
            Console.WriteLine("=== TXM – Migrateur SQLite ===");

            if (args.Length == 0)
                {
                Console.WriteLine("Utilisation :");
                Console.WriteLine("  migrate       -> applique les migrations");
                Console.WriteLine("  version       -> affiche la version du schéma");
                Console.WriteLine("  reset         -> recrée une base vide (ATTENTION données perdues)");
                Console.WriteLine("  check         -> vérifie l’intégrité de la base");
                Console.WriteLine("  dump <fichier.json> -> exporte toutes les tables en JSON");
                return;
                }

            try
                {
                switch (args[0].ToLowerInvariant())
                    {
                    case "migrate":
                        Schéma_SQLite.Initialiser();
                        Success("Base migrée avec succès ✅");
                        break;

                    case "version":
                        int version = LireVersion();
                        Console.WriteLine($"Version actuelle du schéma : {version}");
                        break;

                    case "reset":
                        ResetBase();
                        Warning("Base recréée (schéma neuf, données perdues).");
                        break;

                    case "check":
                        CheckBase();
                        break;

                    case "dump":
                        if (args.Length < 2)
                            {
                            Console.WriteLine("Usage: dump <fichier.json>");
                            return;
                            }
                        DumpJson(args[1]);
                        Success($"Export terminé → {args[1]}");
                        break;

                    default:
                        Console.WriteLine("Commande inconnue. Utilise : migrate | version | reset | check | dump");
                        break;
                    }
                }
            catch (Exception ex)
                {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur : " + ex.Message);
                Console.ResetColor();
                }
            }

        private static int LireVersion()
            {
            using SqliteConnection conn = new(Configuration_Base_de_données.Chaine_Connexion);
            conn.Open();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT valeur FROM __métadonnées WHERE clé='version_schéma';";
            string? valeur = cmd.ExecuteScalar()?.ToString();
            return int.TryParse(valeur, out int v) ? v : 1;
            }

        private static void ResetBase()
            {
            string chemin = Configuration_Base_de_données.Fichier_DB;
            if (File.Exists(chemin))
                {
                File.Delete(chemin);
                }

            Schéma_SQLite.Initialiser();
            }

        private static void CheckBase()
            {
            using SqliteConnection conn = new(Configuration_Base_de_données.Chaine_Connexion);
            conn.Open();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA integrity_check;";
            string? result = cmd.ExecuteScalar()?.ToString();
            if (result == "ok")
                {
                Success("Base intègre ✅");
                }
            else
                {
                Warning("Problèmes détectés : " + result);
                }
            }

        private static void DumpJson(string cheminFichier)
            {
            using SqliteConnection conn = new(Configuration_Base_de_données.Chaine_Connexion);
            conn.Open();

            string[] tables = new[]
            {
        "joueurs","équipes","incidents","sanctions",
        "concours_officiels","inscriptions","matches","homologation_historique"
    };

            Dictionary<string, List<Dictionary<string, object?>>> export = [];

            foreach (string? table in tables)
                {
                using SqliteCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM {table};";
                using SqliteDataReader reader = cmd.ExecuteReader();

                List<Dictionary<string, object?>> rows = [];
                while (reader.Read())
                    {
                    Dictionary<string, object?> row = [];
                    for (int i = 0; i < reader.FieldCount; i++)
                        {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                    rows.Add(row);
                    }
                export[table] = rows;
                }

            string json = JsonSerializer.Serialize(export, _jsonOptions);
            File.WriteAllText(cheminFichier, json);
            }

        // Helpers colorés
        private static void Success(string msg)
            {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
            }

        private static void Warning(string msg)
            {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
            }
        }
    }
