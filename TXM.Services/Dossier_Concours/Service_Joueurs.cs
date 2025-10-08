namespace TXM.Services.Dossier_Concours
    {
    /// <summary>
    /// Service central de gestion des joueurs – compatible TripleX et Pétanque Maestro.
    /// </summary>
    public static class Service_Joueurs
        {
        /// <summary>
        /// Retourne la liste complète des joueurs selon le mode actif.
        /// </summary>
        public static IList<Joueur> ListerTous()
            {
            List<Joueur> liste = [];

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();

            // Requête adaptée au type de licence
            cmd.CommandText = Licence_Helper.Est_Maestro
                ? @"SELECT id, nom, licence, club, sexe, categorie, nationalite, 
                          club_code_ffpjp, date_naissance, points_ffpjp, statut 
                   FROM joueurs ORDER BY nom;"
                : @"SELECT id, nom, licence, club FROM joueurs ORDER BY nom;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                Joueur j = new()
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    Licence = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Club = rd.IsDBNull(3) ? "" : rd.GetString(3)
                    };

                // Lecture des champs supplémentaires si Maestro
                if (Licence_Helper.Est_Maestro && rd.FieldCount > 4)
                    {
                    j.Sexe = rd.IsDBNull(4) ? "" : rd.GetString(4);
                    j.Catégorie = rd.IsDBNull(5) ? "" : rd.GetString(5);
                    j.Nationalité = rd.IsDBNull(6) ? "" : rd.GetString(6);
                    j.Club_Code_FFPJP = rd.IsDBNull(7) ? "" : rd.GetString(7);
                    j.Date_Naissance = rd.IsDBNull(8) ? null : DateTime.TryParse(rd.GetString(8), out var d) ? d : null;
                    j.Points_FFPJP = rd.IsDBNull(9) ? 0 : rd.GetInt32(9);
                    j.Statut = rd.IsDBNull(10) ? "Actif" : rd.GetString(10);
                    }

                liste.Add(j);
                }

            return liste;
            }

        /// <summary>
        /// Retourne le nombre total de joueurs enregistrés.
        /// </summary>
        public static int Compter()
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM joueurs;";
            return Convert.ToInt32(cmd.ExecuteScalar());
            }

        /// <summary>
        /// Ajoute un nouveau joueur à la base, avec champs fédéraux si activés.
        /// </summary>
        public static void Ajouter(Joueur joueur)
            {
            if (!Service_Restrictions.PeutAjouterJoueur())
                {
                throw new InvalidOperationException(
                    "Limite atteinte : maximum de joueurs autorisés en mode Démo restreint.");
                }

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();

            if (Licence_Helper.Est_Maestro)
                {
                cmd.CommandText = @"
                    INSERT INTO joueurs(nom, licence, club, sexe, categorie, nationalite, 
                                        club_code_ffpjp, date_naissance, points_ffpjp, statut)
                    VALUES($n, $l, $c, $s, $cat, $nat, $code, $dn, $pts, $statut);";

                _ = cmd.Parameters.AddWithValue("$n", joueur.Nom);
                _ = cmd.Parameters.AddWithValue("$l", joueur.Licence);
                _ = cmd.Parameters.AddWithValue("$c", joueur.Club);
                _ = cmd.Parameters.AddWithValue("$s", joueur.Sexe);
                _ = cmd.Parameters.AddWithValue("$cat", joueur.Catégorie);
                _ = cmd.Parameters.AddWithValue("$nat", joueur.Nationalité);
                _ = cmd.Parameters.AddWithValue("$code", joueur.Club_Code_FFPJP);
                _ = cmd.Parameters.AddWithValue("$dn", joueur.Date_Naissance?.ToString("yyyy-MM-dd") ?? "");
                _ = cmd.Parameters.AddWithValue("$pts", joueur.Points_FFPJP);
                _ = cmd.Parameters.AddWithValue("$statut", joueur.Statut);
                }
            else
                {
                cmd.CommandText = @"INSERT INTO joueurs(nom, licence, club) VALUES($n, $l, $c);";
                _ = cmd.Parameters.AddWithValue("$n", joueur.Nom);
                _ = cmd.Parameters.AddWithValue("$l", joueur.Licence);
                _ = cmd.Parameters.AddWithValue("$c", joueur.Club);
                }

            _ = cmd.ExecuteNonQuery();
            }

        /// <summary>
        /// Supprime un joueur selon son ID.
        /// </summary>
        public static void Supprimer(int id)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM joueurs WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", id);
            _ = cmd.ExecuteNonQuery();
            }

        /// <summary>
        /// Met à jour les informations d’un joueur existant.
        /// </summary>
        public static void MettreÀJour(Joueur joueur)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();

            if (Licence_Helper.Est_Maestro)
                {
                cmd.CommandText = @"
                    UPDATE joueurs SET 
                        nom=$n, licence=$l, club=$c, sexe=$s, categorie=$cat, 
                        nationalite=$nat, club_code_ffpjp=$code, date_naissance=$dn, 
                        points_ffpjp=$pts, statut=$statut
                    WHERE id=$id;";

                _ = cmd.Parameters.AddWithValue("$id", joueur.Id);
                _ = cmd.Parameters.AddWithValue("$n", joueur.Nom);
                _ = cmd.Parameters.AddWithValue("$l", joueur.Licence);
                _ = cmd.Parameters.AddWithValue("$c", joueur.Club);
                _ = cmd.Parameters.AddWithValue("$s", joueur.Sexe);
                _ = cmd.Parameters.AddWithValue("$cat", joueur.Catégorie);
                _ = cmd.Parameters.AddWithValue("$nat", joueur.Nationalité);
                _ = cmd.Parameters.AddWithValue("$code", joueur.Club_Code_FFPJP);
                _ = cmd.Parameters.AddWithValue("$dn", joueur.Date_Naissance?.ToString("yyyy-MM-dd") ?? "");
                _ = cmd.Parameters.AddWithValue("$pts", joueur.Points_FFPJP);
                _ = cmd.Parameters.AddWithValue("$statut", joueur.Statut);
                }
            else
                {
                cmd.CommandText = @"UPDATE joueurs 
                                    SET nom=$n, licence=$l, club=$c 
                                    WHERE id=$id;";
                _ = cmd.Parameters.AddWithValue("$id", joueur.Id);
                _ = cmd.Parameters.AddWithValue("$n", joueur.Nom);
                _ = cmd.Parameters.AddWithValue("$l", joueur.Licence);
                _ = cmd.Parameters.AddWithValue("$c", joueur.Club);
                }

            _ = cmd.ExecuteNonQuery();
            }
        }
    }
