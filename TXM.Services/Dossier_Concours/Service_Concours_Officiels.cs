namespace TXM.Services.Dossier_Concours
    {
    public static class Service_Concours_Officiels
        {
        public static IList<Concours_Officiel> Lister()
            {
            List<Concours_Officiel> liste = [];
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nom, date, homologation, arbitre, statut 
                                FROM concours_officiels ORDER BY date DESC;";
            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Concours_Officiel
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    Date = DateTime.Parse(rd.GetString(2)),
                    Numéro_Homologation = rd.GetString(3),
                    Arbitre = rd.GetString(4),
                    Statut = rd.GetString(5)
                    });
                }
            return liste;
            }

        public static Concours_Officiel Ajouter(string nom, DateTime date, string homologation, string arbitre)
            {
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerConcours);

            if (!Service_Restrictions.PeutAjouterConcours())
                {
                throw new InvalidOperationException(
                    "Un seul concours officiel est autorisé en mode Démo restreint.");
                }

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO concours_officiels(nom, date, homologation, arbitre, statut) 
                                VALUES($nom, $date, $homologation, $arbitre, 'Prévu');
                                SELECT last_insert_rowid();";
            _ = cmd.Parameters.AddWithValue("$nom", nom);
            _ = cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
            _ = cmd.Parameters.AddWithValue("$homologation", homologation);
            _ = cmd.Parameters.AddWithValue("$arbitre", arbitre);
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Concours_Officiel
                {
                Id = id,
                Nom = nom,
                Date = date,
                Numéro_Homologation = homologation,
                Arbitre = arbitre,
                Statut = "Prévu"
                };
            }

        public static void Valider(int idConcours)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE concours_officiels SET statut='Homologué' WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", idConcours);
            _ = cmd.ExecuteNonQuery();
            }

        public static void Mettre_À_Jour_Statut(int idConcours, string statut)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE concours_officiels SET statut=$statut WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", idConcours);
            _ = cmd.Parameters.AddWithValue("$statut", statut);
            _ = cmd.ExecuteNonQuery();
            }

        public static int CompterEnCours()
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM concours_officiels WHERE statut='En cours';";
            return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
