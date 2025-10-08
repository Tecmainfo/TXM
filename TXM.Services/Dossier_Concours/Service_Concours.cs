namespace TXM.Services.Dossier_Concours
    {
    public static class Service_Concours
        {
        public static IList<Concours> Lister_Concours(string type)
            {
            List<Concours> liste = [];
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nom, date, type 
                                FROM concours WHERE type=$type ORDER BY date DESC;";
            _ = cmd.Parameters.AddWithValue("$type", type);

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Concours
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    Date = DateTime.Parse(rd.GetString(2)),
                    Type = rd.GetString(3)
                    });
                }
            return liste;
            }

        public static Concours Ajouter_Concours(string nom, DateTime date, string type)
            {
            // Autorisation globale
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerConcours);

            // Si type = "officiel", appliquer aussi la limite
            if (type.Equals("officiel", StringComparison.OrdinalIgnoreCase)
                && !Service_Restrictions.PeutAjouterConcours())
                {
                throw new InvalidOperationException("Un seul concours officiel est autorisé en mode Démo restreint.");
                }

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO concours(nom, date, type) 
                                VALUES($nom, $date, $type);
                                SELECT last_insert_rowid();";
            _ = cmd.Parameters.AddWithValue("$nom", nom);
            _ = cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
            _ = cmd.Parameters.AddWithValue("$type", type);
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Concours
                {
                Id = id,
                Nom = nom,
                Date = date,
                Type = type
                };
            }

        public static void Supprimer(int idConcours)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM concours WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", idConcours);
            _ = cmd.ExecuteNonQuery();
            }
        }
    }
