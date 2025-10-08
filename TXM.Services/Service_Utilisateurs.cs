namespace TXM.Services
    {
    public static class Service_Utilisateurs
        {
        public static Utilisateur? Obtenir_Par_Email(string email)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT id, nom, email, rôle, motdepasse
FROM users
WHERE email = $email;";
            _ = cmd.Parameters.AddWithValue("$email", email);
            using SqliteDataReader rd = cmd.ExecuteReader();
            return !rd.Read()
                ? null
                : new Utilisateur
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Email = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Rôle = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    MotDePasseHash = rd.IsDBNull(4) ? "" : rd.GetString(4)
                    };
            }

        public static IList<Utilisateur> Lister()
            {
            List<Utilisateur> liste = [];
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nom, email, role 
                                FROM utilisateurs
                                ORDER BY nom;";
            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Utilisateur
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    Email = rd.GetString(2),
                    Rôle = rd.GetString(3)
                    });
                }
            return liste;
            }

        public static Utilisateur? Ajouter(string nom, string email, string role)
            {
            // Optionnel : limite à 2 utilisateurs en mode Démo restreint
            if (Service_Licence.EstEnModeRestreint())
                {
                int count = Lister().Count;
                if (count >= 2)
                    {
                    throw new InvalidOperationException("Limite atteinte : 2 utilisateurs maximum en mode Démo restreint.");
                    }
                }

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO utilisateurs(nom, email, role) VALUES($n, $e, $r);
                                SELECT last_insert_rowid();";
            _ = cmd.Parameters.AddWithValue("$n", nom);
            _ = cmd.Parameters.AddWithValue("$e", email);
            _ = cmd.Parameters.AddWithValue("$r", role);
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Utilisateur
                {
                Id = id,
                Nom = nom,
                Email = email,
                Rôle = role
                };
            }

        public static void Supprimer(int id)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM utilisateurs WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", id);
            _ = cmd.ExecuteNonQuery();
            }
        }
    }
