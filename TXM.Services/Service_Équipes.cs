using TXM.Modèles.Joueurs;

namespace TXM.Services
    {
    public static class Service_Équipes
        {
        public static IList<Équipe> ListerToutes()
            {
            List<Équipe> liste = new List<Équipe>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, nom FROM équipes ORDER BY nom;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Équipe
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1)
                    });
                }
            return liste;
            }

        public static void Ajouter(string nom)
            {
            if (!Service_Restrictions.PeutAjouterEquipe())
                throw new InvalidOperationException("Limite atteinte : maximum d’équipes autorisées en mode Démo restreint.");

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO équipes(nom) VALUES($n);";
            cmd.Parameters.AddWithValue("$n", nom);
            cmd.ExecuteNonQuery();
            }

        public static void Supprimer(int id)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM équipes WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
            }

        public static void MettreÀJour(int id, string nom)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE équipes SET nom=$n WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$n", nom);
            cmd.ExecuteNonQuery();
            }

        // === Gestion des relations équipe ↔ joueurs ===
        public static void AjouterJoueurDansÉquipe(int idÉquipe, int idJoueur)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO équipes_joueurs(id_équipe, id_joueur)
                                VALUES($e, $j);";
            cmd.Parameters.AddWithValue("$e", idÉquipe);
            cmd.Parameters.AddWithValue("$j", idJoueur);
            cmd.ExecuteNonQuery();
            }

        public static IList<Joueur> ListerJoueursDansÉquipe(int idÉquipe)
            {
            List<Joueur> liste = new List<Joueur>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT j.id, j.nom, j.licence, j.club
                FROM joueurs j
                JOIN équipes_joueurs ej ON ej.id_joueur=j.id
                WHERE ej.id_équipe=$e;";
            cmd.Parameters.AddWithValue("$e", idÉquipe);

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Joueur
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    Licence = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Club = rd.IsDBNull(3) ? "" : rd.GetString(3)
                    });
                }
            return liste;
            }

        public static void RetirerJoueurDeÉquipe(int idÉquipe, int idJoueur)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM équipes_joueurs WHERE id_équipe=$e AND id_joueur=$j;";
            cmd.Parameters.AddWithValue("$e", idÉquipe);
            cmd.Parameters.AddWithValue("$j", idJoueur);
            cmd.ExecuteNonQuery();
            }
        }
    }
