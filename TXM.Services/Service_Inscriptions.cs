using TXM.Modèles;

namespace TXM.Services
    {
    public static class Service_Inscriptions
        {
        // Liste globale
        public static IList<Inscription> Lister()
            {
            List<Inscription> liste = new List<Inscription>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT i.id, i.nom_équipe, i.joueurs, i.id_concours, c.nom
                                FROM inscriptions i
                                JOIN concours_officiels c ON c.id = i.id_concours
                                ORDER BY c.date DESC;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Inscription
                    {
                    Id = rd.GetInt32(0),
                    NomÉquipe = rd.GetString(1),
                    Joueurs = rd.GetString(2),
                    IdConcours = rd.GetInt32(3),
                    NomConcours = rd.GetString(4)
                    });
                }
            return liste;
            }

        public static Inscription Ajouter(string nomÉquipe, string joueurs, int idConcours)
            {
            // Vérification autorisation globale
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerInscription);

            // Vérification quotas
            if (!Service_Restrictions.PeutAjouterEquipe())
                throw new InvalidOperationException("Limite atteinte : maximum d’équipes autorisées en mode Démo restreint.");

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO inscriptions(nom_équipe, joueurs, id_concours)
                                VALUES($nom, $joueurs, $concours);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$nom", nomÉquipe);
            cmd.Parameters.AddWithValue("$joueurs", joueurs);
            cmd.Parameters.AddWithValue("$concours", idConcours);
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            // Récupérer le nom du concours
            using SqliteCommand cmd2 = conn.CreateCommand();
            cmd2.CommandText = "SELECT nom FROM concours_officiels WHERE id=$c;";
            cmd2.Parameters.AddWithValue("$c", idConcours);
            string nomConcours = cmd2.ExecuteScalar()?.ToString() ?? "";

            return new Inscription
                {
                Id = id,
                NomÉquipe = nomÉquipe,
                Joueurs = joueurs,
                IdConcours = idConcours,
                NomConcours = nomConcours
                };
            }

        public static void MettreÀJour(int id, string nomÉquipe, string joueurs)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE inscriptions
                                SET nom_équipe=$n, joueurs=$j
                                WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$n", nomÉquipe);
            cmd.Parameters.AddWithValue("$j", joueurs);
            cmd.ExecuteNonQuery();
            }

        // Liste des inscriptions pour un concours donné
        public static IList<Inscription> ListerPourConcours(int idConcours)
            {
            List<Inscription> liste = new List<Inscription>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nom_équipe, joueurs, id_concours
                                FROM inscriptions
                                WHERE id_concours=$c;";
            cmd.Parameters.AddWithValue("$c", idConcours);

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Inscription
                    {
                    Id = rd.GetInt32(0),
                    NomÉquipe = rd.GetString(1),
                    Joueurs = rd.GetString(2),
                    IdConcours = rd.GetInt32(3)
                    });
                }
            return liste;
            }

        public static void Supprimer(int idInscription)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM inscriptions WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", idInscription);
            cmd.ExecuteNonQuery();
            }
        }
    }
