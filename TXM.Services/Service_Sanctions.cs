using TXM.Modèles.Arbitrage;

namespace TXM.Services
    {
    public static class Service_Sanctions
        {
        // === Liste complète ===
        public static IList<Sanction> Lister()
            {
            List<Sanction> liste = new List<Sanction>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, date, type, motif, article_règlement, arbitre, durée_minutes, id_incident
                                FROM sanctions
                                ORDER BY date DESC;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Sanction
                    {
                    Id = rd.GetInt32(0),
                    Date = DateTime.Parse(rd.GetString(1)),
                    Type = rd.GetString(2),
                    Motif = rd.GetString(3),
                    Article_Règlement = rd.GetString(4),
                    Arbitre = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    Durée_Minutes = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),
                    Id_Incident = rd.IsDBNull(7) ? null : rd.GetInt32(7)
                    });
                }
            return liste;
            }

        // === Liste par concours ===
        public static IList<Sanction> ListerPourConcours(int _)
            {
            List<Sanction> liste = new List<Sanction>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT s.id, s.date, s.type, s.motif, s.article_règlement, s.arbitre, s.durée_minutes, s.id_incident
                                FROM sanctions s
                                JOIN incidents i ON i.id = s.id_incident
                                WHERE i.id_équipe IS NOT NULL OR i.id_joueur IS NOT NULL
                                ORDER BY s.date DESC;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Sanction
                    {
                    Id = rd.GetInt32(0),
                    Date = DateTime.Parse(rd.GetString(1)),
                    Type = rd.GetString(2),
                    Motif = rd.GetString(3),
                    Article_Règlement = rd.GetString(4),
                    Arbitre = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    Durée_Minutes = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),
                    Id_Incident = rd.IsDBNull(7) ? null : rd.GetInt32(7)
                    });
                }
            return liste;
            }

        // === Ajout d'une nouvelle sanction ===
        public static Sanction Ajouter(DateTime date, string type, string motif, string article, string arbitre, int duréeMinutes, int? idIncident)
            {
            // Vérification licence / restrictions
            Service_Passerelle.VérifierOuThrow(ActionRestriction.GérerArbitrage);

            if (!Service_Restrictions.PeutAjouterSanction())
                throw new InvalidOperationException("Limite atteinte : maximum de sanctions autorisées en mode Démo restreint.");

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO sanctions(date, type, motif, article_règlement, arbitre, durée_minutes, id_incident)
                                VALUES($d, $t, $m, $a, $ar, $dur, $inc);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$d", date.ToString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("$t", type);
            cmd.Parameters.AddWithValue("$m", motif);
            cmd.Parameters.AddWithValue("$a", article);
            cmd.Parameters.AddWithValue("$ar", arbitre);
            cmd.Parameters.AddWithValue("$dur", duréeMinutes);
            cmd.Parameters.AddWithValue("$inc", (object?)idIncident ?? DBNull.Value);

            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Sanction
                {
                Id = id,
                Date = date,
                Type = type,
                Motif = motif,
                Article_Règlement = article,
                Arbitre = arbitre,
                Durée_Minutes = duréeMinutes,
                Id_Incident = idIncident
                };
            }

        // === Suppression ===
        public static void Supprimer(int idSanction)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM sanctions WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", idSanction);
            cmd.ExecuteNonQuery();
            }

        // === Compteur global ===
        public static int Compter()
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM sanctions;";
            return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
