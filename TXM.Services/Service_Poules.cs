using TXM.Modèles.Joueurs;

namespace TXM.Services
    {
    public static class Service_Poules
        {
        // === Gestion des poules ===
        public static IList<Poule> Lister(int idConcours)
            {
            List<Poule> liste = new List<Poule>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nom, id_concours, heure_début
                                FROM poules WHERE id_concours=$c ORDER BY nom;";
            cmd.Parameters.AddWithValue("$c", idConcours);

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Poule
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    IdConcours = rd.GetInt32(2),
                    HeureDébut = DateTime.TryParse(rd.GetString(3), out DateTime h) ? h : DateTime.MinValue
                    });
                }
            return liste;
            }

        public static Poule Ajouter(string nom, int idConcours, DateTime heureDébut)
            {
            if (!Service_Restrictions.PeutAjouterPoule(idConcours))
                throw new InvalidOperationException("Limite atteinte : nombre de poules autorisées en mode Démo restreint.");

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO poules(nom, id_concours, heure_début)
                                VALUES($n, $c, $h);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$n", nom);
            cmd.Parameters.AddWithValue("$c", idConcours);
            cmd.Parameters.AddWithValue("$h", heureDébut.ToString("yyyy-MM-dd HH:mm"));
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Poule { Id = id, Nom = nom, IdConcours = idConcours, HeureDébut = heureDébut };
            }

        // === Équipes dans poules ===
        public static void AjouterÉquipeDansPoule(int idPoule, int idInscription)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT OR IGNORE INTO poules_inscriptions(id_poule, id_inscription)
                                VALUES($p, $i);";
            cmd.Parameters.AddWithValue("$p", idPoule);
            cmd.Parameters.AddWithValue("$i", idInscription);
            cmd.ExecuteNonQuery();
            }

        public static void RetirerÉquipeDePoule(int idPoule, int idInscription)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM poules_inscriptions 
                                WHERE id_poule=$p AND id_inscription=$i;";
            cmd.Parameters.AddWithValue("$p", idPoule);
            cmd.Parameters.AddWithValue("$i", idInscription);
            cmd.ExecuteNonQuery();
            }

        public static IList<Inscription> ListerÉquipesDansPoule(int idPoule)
            {
            List<Inscription> liste = new List<Inscription>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT i.id, i.nom_équipe, i.joueurs, i.id_concours
                FROM inscriptions i
                JOIN poules_inscriptions pi ON pi.id_inscription = i.id
                WHERE pi.id_poule=$p;";
            cmd.Parameters.AddWithValue("$p", idPoule);

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

        // === Joueurs dans poules ===
        public static void AjouterJoueurDansPoule(int idPoule, int idJoueur)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT OR IGNORE INTO poules_joueurs(id_poule, id_joueur)
                                VALUES($p, $j);";
            cmd.Parameters.AddWithValue("$p", idPoule);
            cmd.Parameters.AddWithValue("$j", idJoueur);
            cmd.ExecuteNonQuery();
            }

        public static void RetirerJoueurDePoule(int idPoule, int idJoueur)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM poules_joueurs 
                                WHERE id_poule=$p AND id_joueur=$j;";
            cmd.Parameters.AddWithValue("$p", idPoule);
            cmd.Parameters.AddWithValue("$j", idJoueur);
            cmd.ExecuteNonQuery();
            }

        public static IList<Joueur> ListerJoueursDansPoule(int idPoule)
            {
            List<Joueur> liste = new List<Joueur>();
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT j.id, j.nom, j.licence, j.club
                FROM joueurs j
                JOIN poules_joueurs pj ON pj.id_joueur = j.id
                WHERE pj.id_poule=$p;";
            cmd.Parameters.AddWithValue("$p", idPoule);

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

        // === Répartition automatique ===
        public static void RépartirÉquipesAléatoirement(int _, IList<Poule> poules, IList<Inscription> inscriptions)
            {
            if (poules.Count == 0 || inscriptions.Count == 0)
                return;

            Random rng = new Random();
            List<Inscription> shuffled = inscriptions.OrderBy(x => rng.Next()).ToList();

            int index = 0;
            foreach (Inscription? ins in shuffled)
                {
                Poule poule = poules[index % poules.Count]; // distribution circulaire
                AjouterÉquipeDansPoule(poule.Id, ins.Id);
                index++;
                }
            }

        public static void RépartirJoueursAléatoirement(IList<Poule> poules, IList<Joueur> joueurs)
            {
            if (poules.Count == 0 || joueurs.Count == 0)
                return;

            Random rng = new Random();
            List<Joueur> shuffled = joueurs.OrderBy(x => rng.Next()).ToList();

            int index = 0;
            foreach (Joueur? joueur in shuffled)
                {
                Poule poule = poules[index % poules.Count];
                AjouterJoueurDansPoule(poule.Id, joueur.Id);
                index++;
                }
            }
        }
    }
