using TXM.Modèles.Dossier_Concours;

namespace TXM.Services
    {
    public static class Service_Matches
        {
        public static IList<Match> Lister(int idConcours)
            {
            List<Match> liste = new List<Match>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, tour, equipeA, equipeB, scoreA, scoreB, date_heure
                                FROM matches WHERE id_concours=$c ORDER BY tour;";
            cmd.Parameters.AddWithValue("$c", idConcours);

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Match
                    {
                    Id = rd.GetInt32(0),
                    Tour = rd.GetInt32(1),
                    EquipeA = rd.GetString(2),
                    EquipeB = rd.GetString(3),
                    ScoreA = rd.GetInt32(4),
                    ScoreB = rd.GetInt32(5),
                    DateHeure = DateTime.TryParse(rd.GetString(6), out DateTime d) ? d : DateTime.MinValue,
                    IdConcours = idConcours
                    });
                }
            return liste;
            }

        public static IList<Match> ListerTousPourDate(DateTime date)
            {
            List<Match> liste = new List<Match>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, tour, equipeA, equipeB, scoreA, scoreB, id_concours, date_heure
                                FROM matches
                                WHERE date(date) = $d
                                ORDER BY tour;";
            cmd.Parameters.AddWithValue("$d", date.ToString("yyyy-MM-dd"));

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Match
                    {
                    Id = rd.GetInt32(0),
                    Tour = rd.GetInt32(1),
                    EquipeA = rd.GetString(2),
                    EquipeB = rd.GetString(3),
                    ScoreA = rd.GetInt32(4),
                    ScoreB = rd.GetInt32(5),
                    IdConcours = rd.GetInt32(6),
                    DateHeure = DateTime.TryParse(rd.GetString(7), out DateTime d) ? d : DateTime.MinValue
                    });
                }
            return liste;
            }

        public static IList<Match> ListerPourPoule(int idPoule)
            {
            List<Match> liste = new List<Match>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, tour, equipeA, equipeB, scoreA, scoreB, id_concours, date_heure, id_poule
                        FROM matches
                        WHERE id_poule=$p
                        ORDER BY tour, id;";
            cmd.Parameters.AddWithValue("$p", idPoule);

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Match
                    {
                    Id = rd.GetInt32(0),
                    Tour = rd.GetInt32(1),
                    EquipeA = rd.GetString(2),
                    EquipeB = rd.GetString(3),
                    ScoreA = rd.GetInt32(4),
                    ScoreB = rd.GetInt32(5),
                    IdConcours = rd.GetInt32(6),
                    DateHeure = DateTime.TryParse(rd.GetString(7), out DateTime dh) ? dh : DateTime.MinValue,
                    IdPoule = rd.IsDBNull(8) ? null : rd.GetInt32(8)
                    });
                }
            return liste;
            }

        public static void Enregistrer(int idConcours, int tour, string equipeA, string equipeB, DateTime? dateHeure = null, int? idPoule = null)
            {
            if (!Service_Restrictions.PeutAjouterMatch(idConcours))
                throw new InvalidOperationException("Limite atteinte : nombre de matches autorisés en mode Démo restreint.");

            using SqliteConnection conn = Service_SQLite.Ouvrir();

            DateTime dateMatch;
            if (dateHeure.HasValue)
                dateMatch = dateHeure.Value;
            else
                {
                using SqliteCommand cmdConcours = conn.CreateCommand();
                cmdConcours.CommandText = "SELECT date FROM concours_officiels WHERE id=$id;";
                cmdConcours.Parameters.AddWithValue("$id", idConcours);
                string? result = cmdConcours.ExecuteScalar()?.ToString();
                DateTime dateConcours = DateTime.TryParse(result, out DateTime d) ? d : DateTime.Today;
                dateMatch = dateConcours.AddHours(9);
                }

            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO matches(tour, equipeA, equipeB, scoreA, scoreB, id_concours, date_heure, id_poule)
                                VALUES($t, $a, $b, 0, 0, $c, $dh, $p);";
            cmd.Parameters.AddWithValue("$t", tour);
            cmd.Parameters.AddWithValue("$a", equipeA);
            cmd.Parameters.AddWithValue("$b", equipeB);
            cmd.Parameters.AddWithValue("$c", idConcours);
            cmd.Parameters.AddWithValue("$dh", dateMatch.ToString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("$p", (object?)idPoule ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            }

        public static void MettreÀJourScore(int idMatch, int scoreA, int scoreB)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE matches SET scoreA=$sa, scoreB=$sb WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", idMatch);
            cmd.Parameters.AddWithValue("$sa", scoreA);
            cmd.Parameters.AddWithValue("$sb", scoreB);
            cmd.ExecuteNonQuery();
            }

        public static void GénérerMatchesPourPoule(int idConcours, int idPoule, IList<Inscription> équipes, DateTime? heureDébut = null)
            {
            if (équipes.Count < 2) return;

            int tour = 1;
            for (int i = 0; i < équipes.Count; i++)
                {
                for (int j = i + 1; j < équipes.Count; j++)
                    {
                    DateTime date = heureDébut ?? DateTime.Today.AddHours(9);
                    Enregistrer(idConcours, tour, équipes[i].NomÉquipe, équipes[j].NomÉquipe, date, idPoule);
                    tour++;
                    }
                }
            }

        public static void GénérerMatchesPourPoule(int idConcours, int idPoule, IList<Inscription> équipes, DateTime dateDébut)
            {
            if (équipes.Count < 2) return;

            int tour = 1;
            for (int i = 0; i < équipes.Count; i++)
                {
                for (int j = i + 1; j < équipes.Count; j++)
                    {
                    Enregistrer(idConcours, tour, équipes[i].NomÉquipe, équipes[j].NomÉquipe, dateDébut, idPoule);
                    }
                }
            }

        }
    }
