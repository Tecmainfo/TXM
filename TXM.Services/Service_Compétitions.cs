namespace TXM.Services
    {
    public static class Service_Compétitions
        {
        public static IList<Compétition> Lister_Compétitions()
            {
            List<Compétition> liste = [];
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nom, lieu, date, statut 
                                FROM compétitions ORDER BY date DESC;";
            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Compétition
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    Lieu = rd.GetString(2),
                    Date = DateTime.Parse(rd.GetString(3)),
                    Statut = rd.GetString(4)
                    });
                }
            return liste;
            }

        public static Compétition Ajouter(string nom, string lieu, DateTime date)
            {
            // Vérification autorisation
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerConcours);

            // Vérification restriction numérique
            if (!Service_Restrictions.PeutAjouterCompetition())
                {
                throw new InvalidOperationException("Limite atteinte : maximum de compétitions autorisées en mode Démo restreint.");
                }

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO compétitions(nom, lieu, date, statut) 
                                VALUES($n, $l, $d, 'Prévu');
                                SELECT last_insert_rowid();";
            _ = cmd.Parameters.AddWithValue("$n", nom);
            _ = cmd.Parameters.AddWithValue("$l", lieu);
            _ = cmd.Parameters.AddWithValue("$d", date.ToString("yyyy-MM-dd"));
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Compétition
                {
                Id = id,
                Nom = nom,
                Lieu = lieu,
                Date = date,
                Statut = "Prévu"
                };
            }

        public static void MettreÀJourStatut(int idCompétition, string statut)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE compétitions SET statut=$s WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", idCompétition);
            _ = cmd.Parameters.AddWithValue("$s", statut);
            _ = cmd.ExecuteNonQuery();
            }

        public static void Supprimer(int idCompétition)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM compétitions WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", idCompétition);
            _ = cmd.ExecuteNonQuery();
            }
        }
    }
