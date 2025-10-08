using TXM.Modèles.Arbitrage;

namespace TXM.Services
    {
    public static class Service_Incidents
        {
        public static IList<Incident> Lister()
            {
            // Autorisation globale
            Service_Passerelle.VérifierOuThrow(ActionRestriction.GérerArbitrage);

            List<Incident> liste = [];
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, date, description, gravité, id_équipe, id_joueur, arbitre
                                FROM incidents
                                ORDER BY date DESC;";
            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Incident
                    {
                    Id = rd.GetInt32(0),
                    Date = DateTime.TryParse(rd.GetString(1) ?? "", out DateTime d) ? d : DateTime.MinValue,
                    Description = rd.GetString(2),
                    Gravité = rd.GetString(3),
                    Id_Équipe = rd.IsDBNull(4) ? null : rd.GetInt32(4),
                    Id_Joueur = rd.IsDBNull(5) ? null : rd.GetInt32(5),
                    Arbitre = rd.IsDBNull(6) ? "" : rd.GetString(6)
                    });
                }
            return liste;
            }

        public static Incident Ajouter(DateTime date, string description, string gravité, string arbitre, int? idÉquipe, int? idJoueur)
            {
            // Autorisation globale
            Service_Passerelle.VérifierOuThrow(ActionRestriction.GérerArbitrage);

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO incidents(date, description, gravité, id_équipe, id_joueur, arbitre)
                        VALUES($d, $desc, $g, $e, $j, $a);
                        SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$d", date.ToString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("$desc", description);
            cmd.Parameters.AddWithValue("$g", gravité);
            cmd.Parameters.AddWithValue("$e", (object?)idÉquipe ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$j", (object?)idJoueur ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$a", arbitre ?? "");

            int id = 0;
            object? scalar = cmd.ExecuteScalar();
            if (scalar is not null and not DBNull)
                id = Convert.ToInt32(scalar);

            return new Incident
                {
                Id = id,
                Date = date,
                Description = description,
                Gravité = gravité,
                Id_Équipe = idÉquipe,
                Id_Joueur = idJoueur,
                Arbitre = arbitre ?? string.Empty
                };
            }

        public static void Supprimer(int idIncident)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM incidents WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", idIncident);
            cmd.ExecuteNonQuery();
            }

        public static int Compter()
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM incidents;";
            return Convert.ToInt32(cmd.ExecuteScalar());
            }

        }
    }
