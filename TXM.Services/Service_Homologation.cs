using TXM.Modèles;

namespace TXM.Services
    {
    public static class Service_Homologation
        {
        /// <summary>
        /// Enregistre une décision d’homologation et met à jour le statut du concours.
        /// </summary>
        public static void EnregistrerDécision(int idConcours, string décision, string arbitre, string commentaire)
            {
            // Autorisation globale
            Service_Passerelle.VérifierOuThrow(ActionRestriction.GérerHomologation);

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO homologation_historique(id_concours, date_action, décision, arbitre, commentaire)
                                VALUES($concours, $date, $décision, $arbitre, $commentaire);";
            cmd.Parameters.AddWithValue("$concours", idConcours);
            cmd.Parameters.AddWithValue("$date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$décision", décision);
            cmd.Parameters.AddWithValue("$arbitre", arbitre);
            cmd.Parameters.AddWithValue("$commentaire", commentaire);
            cmd.ExecuteNonQuery();

            using SqliteCommand cmd2 = conn.CreateCommand();
            cmd2.CommandText = @"UPDATE concours_officiels SET statut=$statut WHERE id=$id;";
            cmd2.Parameters.AddWithValue("$id", idConcours);
            cmd2.Parameters.AddWithValue("$statut", décision);
            cmd2.ExecuteNonQuery();
            }
        /// <summary>
        /// Retourne l’historique des décisions pour un concours.
        /// </summary>
        public static IList<HistoriqueHomologation> ListerPourConcours(int idConcours)
            {
            // Autorisation globale
            Service_Passerelle.VérifierOuThrow(ActionRestriction.GérerHomologation);

            List<HistoriqueHomologation> liste = new List<HistoriqueHomologation>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, id_concours, date_action, décision, arbitre, commentaire
                                FROM homologation_historique
                                WHERE id_concours=$c ORDER BY date_action DESC;";
            cmd.Parameters.AddWithValue("$c", idConcours);
            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new HistoriqueHomologation
                    {
                    Id = rd.GetInt32(0),
                    IdConcours = rd.GetInt32(1),
                    DateAction = DateTime.Parse(rd.GetString(2)),
                    Décision = rd.GetString(3),
                    Arbitre = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    Commentaire = rd.IsDBNull(5) ? "" : rd.GetString(5)
                    });
                }
            return liste;
            }

        /// <summary>
        /// Retourne tout l’historique, tous concours confondus (utile export global).
        /// </summary>
        public static IList<HistoriqueHomologation> ListerTout()
            {
            List<HistoriqueHomologation> liste = new List<HistoriqueHomologation>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT id, id_concours, date_action, décision, arbitre, commentaire
FROM homologation_historique
ORDER BY date_action DESC;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new HistoriqueHomologation
                    {
                    Id = rd.GetInt32(0),
                    IdConcours = rd.GetInt32(1),
                    DateAction = DateTime.TryParse(rd.GetString(2), out DateTime dt) ? dt : DateTime.MinValue,
                    Décision = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Arbitre = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    Commentaire = rd.IsDBNull(5) ? "" : rd.GetString(5)
                    });
                }
            return liste;
            }
        }
    }
