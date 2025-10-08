using TXM.Modèles;

namespace TXM.Services
    {
    public static class Service_Rapports
        {
        public static IList<Rapport> Lister()
            {
            List<Rapport> liste = new List<Rapport>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, date, type, utilisateur, fichier 
                                FROM rapports 
                                ORDER BY date DESC;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Rapport
                    {
                    Id = rd.GetInt32(0),
                    Date = DateTime.TryParse(rd.GetString(1), out DateTime d) ? d : DateTime.MinValue,
                    Type = rd.GetString(2),
                    Utilisateur = rd.GetString(3),
                    NomFichier = rd.GetString(4)
                    });
                }
            return liste;
            }

        public static IList<Rapport> ListerTous()
            {
            List<Rapport> liste = new List<Rapport>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, date, type, utilisateur, fichier
                                FROM rapports
                                ORDER BY date DESC;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Rapport
                    {
                    Id = rd.GetInt32(0),
                    Date = DateTime.Parse(rd.GetString(1)),
                    Type = rd.GetString(2),
                    Utilisateur = rd.GetString(3),
                    NomFichier = rd.GetString(4)
                    });
                }
            return liste;
            }

        public static bool Ajouter(Rapport rapport)
            {
            // Autorisation globale
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerRapport);

            // Restriction numérique
            if (!Service_Restrictions.PeutAjouterRapport())
                throw new InvalidOperationException("Limite atteinte : maximum de rapports autorisés en mode Démo restreint.");

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO rapports(date, type, utilisateur, fichier)
                                VALUES($d, $t, $u, $f);";
            cmd.Parameters.AddWithValue("$d", rapport.Date.ToString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("$t", rapport.Type);
            cmd.Parameters.AddWithValue("$u", rapport.Utilisateur);
            cmd.Parameters.AddWithValue("$f", rapport.NomFichier);
            cmd.ExecuteNonQuery();
            return true;
            }

        // === Nouvelle surcharge pour compatibilité Vue_Arbitrage ===
        public static bool Ajouter(string type, string utilisateur, string fichier)
            {
            Rapport rapport = new Rapport
                {
                Date = DateTime.Now,
                Type = type,
                Utilisateur = utilisateur,
                NomFichier = fichier
                };
            return Ajouter(rapport);
            }

        public static void Supprimer(int id)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM rapports WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
            }
        }
    }
