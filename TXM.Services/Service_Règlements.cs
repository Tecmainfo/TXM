namespace TXM.Services
    {
    public static class Service_Règlements
        {
        public static string Dossier_Racine =>
            Path.Combine(Configuration_Base_de_données.Dossier_App, "Reglements");

        public static IList<Règlement> Lister(int? année = null, string? filtreTexte = null)
            {
            List<Règlement> liste = [];
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();

            List<string> where = [];
            if (année.HasValue)
                {
                where.Add("annee = $a");
                }

            if (!string.IsNullOrWhiteSpace(filtreTexte))
                {
                where.Add("(LOWER(titre) LIKE $f OR LOWER(categorie) LIKE $f)");
                }

            cmd.CommandText = $@"
SELECT id, annee, titre, categorie, version, source_url, fichier_local, date_publication, date_ajout, hash
FROM reglements
{(where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "")}
ORDER BY annee DESC, categorie, titre;";

            if (année.HasValue)
                {
                _ = cmd.Parameters.AddWithValue("$a", année.Value);
                }

            if (!string.IsNullOrWhiteSpace(filtreTexte))
                {
                _ = cmd.Parameters.AddWithValue("$f", "%" + filtreTexte.Trim().ToLower() + "%");
                }

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Règlement
                    {
                    Id = rd.GetInt32(0),
                    Année = rd.GetInt32(1),
                    Titre = rd.GetString(2),
                    Catégorie = rd.GetString(3),
                    Version = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    Source_Url = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    Fichier_Local = rd.GetString(6),
                    Date_Publication = rd.IsDBNull(7) ? null : DateTime.TryParse(rd.GetString(7), out global::System.DateTime dp) ? dp : null,
                    Date_Ajout = DateTime.TryParse(rd.GetString(8), out DateTime da) ? da : DateTime.MinValue,
                    Hash = rd.IsDBNull(9) ? "" : rd.GetString(9)
                    });
                }
            return liste;
            }

        public static Règlement Ajouter_Depuis_Fichier(int année, string titre, string catégorie, string cheminSource, string version = "", DateTime? datePublication = null)
            {
            string dossier = Path.Combine(Dossier_Racine, année.ToString());
            _ = Directory.CreateDirectory(dossier);

            string nom = Path.GetFileName(cheminSource);
            if (string.IsNullOrWhiteSpace(Path.GetExtension(nom)))
                {
                nom += ".pdf";
                }

            string cible = Chemin_Unique(Path.Combine(dossier, nom));

            File.Copy(cheminSource, cible, overwrite: false);
            string hash = Hash_Fichier(cible);

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO reglements(annee, titre, categorie, version, source_url, fichier_local, date_publication, date_ajout, hash)
VALUES($a, $t, $c, $v, $u, $f, $dp, $daj, $h);
SELECT last_insert_rowid();";
            _ = cmd.Parameters.AddWithValue("$daj", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            _ = cmd.Parameters.AddWithValue("$a", année);
            _ = cmd.Parameters.AddWithValue("$t", titre);
            _ = cmd.Parameters.AddWithValue("$c", catégorie);
            _ = cmd.Parameters.AddWithValue("$v", version ?? "");
            _ = cmd.Parameters.AddWithValue("$u", "");
            _ = cmd.Parameters.AddWithValue("$f", cible);
            _ = cmd.Parameters.AddWithValue("$dp", datePublication?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            _ = cmd.Parameters.AddWithValue("$h", hash);

            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Règlement
                {
                Id = id,
                Année = année,
                Titre = titre,
                Catégorie = catégorie,
                Version = version ?? "",
                Source_Url = "",
                Fichier_Local = cible,
                Date_Publication = datePublication,
                Date_Ajout = DateTime.Now,
                Hash = hash
                };
            }

        public static async Task<Règlement> Ajouter_Depuis_Url(int année, string titre, string catégorie, string url, string version = "", DateTime? datePublication = null)
            {
            string dossier = Path.Combine(Dossier_Racine, année.ToString());
            _ = Directory.CreateDirectory(dossier);

            using HttpClient http = new();
            byte[] bytes = await http.GetByteArrayAsync(url);

            string nomBase = Slug(titre);
            string cible = Chemin_Unique(Path.Combine(dossier, nomBase + ".pdf"));
            await File.WriteAllBytesAsync(cible, bytes);

            string hash = Hash_Fichier(cible);

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO reglements(annee, titre, categorie, version, source_url, fichier_local, date_publication, date_ajout, hash)
VALUES($a, $t, $c, $v, $u, $f, $dp, $daj, $h);
SELECT last_insert_rowid();";
            _ = cmd.Parameters.AddWithValue("$daj", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            _ = cmd.Parameters.AddWithValue("$a", année);
            _ = cmd.Parameters.AddWithValue("$t", titre);
            _ = cmd.Parameters.AddWithValue("$c", catégorie);
            _ = cmd.Parameters.AddWithValue("$v", version ?? "");
            _ = cmd.Parameters.AddWithValue("$u", url);
            _ = cmd.Parameters.AddWithValue("$f", cible);
            _ = cmd.Parameters.AddWithValue("$dp", datePublication?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            _ = cmd.Parameters.AddWithValue("$h", hash);

            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return new Règlement
                {
                Id = id,
                Année = année,
                Titre = titre,
                Catégorie = catégorie,
                Version = version ?? "",
                Source_Url = url,
                Fichier_Local = cible,
                Date_Publication = datePublication,
                Date_Ajout = DateTime.Now,
                Hash = hash
                };
            }

        public static void MettreÀJour_Métadonnées(Règlement r)
            {
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE reglements
SET annee=$a, titre=$t, categorie=$c, version=$v, date_publication=$dp
WHERE id=$id;";
            _ = cmd.Parameters.AddWithValue("$id", r.Id);
            _ = cmd.Parameters.AddWithValue("$a", r.Année);
            _ = cmd.Parameters.AddWithValue("$t", r.Titre);
            _ = cmd.Parameters.AddWithValue("$c", r.Catégorie);
            _ = cmd.Parameters.AddWithValue("$v", r.Version ?? "");
            _ = cmd.Parameters.AddWithValue("$dp", r.Date_Publication?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            _ = cmd.ExecuteNonQuery();
            }

        public static void Supprimer(int id)
            {
            string? fichier = null;
            using (SqliteConnection conn = Service_SQLite.Ouvrir())
                {
                using SqliteCommand cmd1 = conn.CreateCommand();
                cmd1.CommandText = "SELECT fichier_local FROM reglements WHERE id=$id;";
                _ = cmd1.Parameters.AddWithValue("$id", id);
                fichier = cmd1.ExecuteScalar()?.ToString();
                }

            using (SqliteConnection conn = Service_SQLite.Ouvrir())
            using (SqliteCommand cmd = conn.CreateCommand())
                {
                cmd.CommandText = "DELETE FROM reglements WHERE id=$id;";
                _ = cmd.Parameters.AddWithValue("$id", id);
                _ = cmd.ExecuteNonQuery();
                }

            if (!string.IsNullOrWhiteSpace(fichier) && File.Exists(fichier))
                {
                try { File.Delete(fichier); } catch { /* ignore */ }
                }
            }

        public static void Ouvrir(Règlement r)
            {
            if (!File.Exists(r.Fichier_Local))
                {
                return;
                }

            ProcessStartInfo psi = new()
                {
                FileName = r.Fichier_Local,
                UseShellExecute = true
                };
            _ = System.Diagnostics.Process.Start(psi);
            }

        public static string Exporter_Csv(IList<Règlement> items)
            {
            string temp = Path.Combine(Path.GetTempPath(), $"reglements_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            using StreamWriter sw = new(temp, false, System.Text.Encoding.UTF8);
            sw.WriteLine("Id;Année;Catégorie;Titre;Version;Date_Publication;Date_Ajout;Fichier_Local;Source_Url;Hash");
            foreach (Règlement r in items)
                {
                string csvDatePub = r.Date_Publication?.ToString("yyyy-MM-dd") ?? "";
                sw.WriteLine($"{r.Id};{r.Année};{Esc(r.Catégorie)};{Esc(r.Titre)};{Esc(r.Version)};{csvDatePub};{r.Date_Ajout:yyyy-MM-dd HH:mm};{Esc(r.Fichier_Local)};{Esc(r.Source_Url)};{r.Hash}");
                }
            return temp;

            static string Esc(string s)
                {
                return s?.Replace(";", ",") ?? "";
                }
            }

        public static void Révéler_Dans_Explorateur(Règlement r)
            {
            if (!File.Exists(r.Fichier_Local))
                {
                return;
                }

            try
                {
                string arg = $"/select,\"{r.Fichier_Local}\"";
                _ = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                    FileName = "explorer.exe",
                    Arguments = arg,
                    UseShellExecute = true
                    });
                }
            catch { Ouvrir(r); }
            }

        public static IList<Règlement> ListerTous()
            {
            List<Règlement> liste = [];
            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, annee, titre, categorie, version, source_url, fichier_local, date_publication
                                FROM reglements
                                ORDER BY annee DESC, categorie;";

            using SqliteDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                liste.Add(new Règlement
                    {
                    Id = rd.GetInt32(0),
                    Année = rd.GetInt32(1),
                    Titre = rd.GetString(2),
                    Catégorie = rd.GetString(3),
                    Version = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    Source_Url = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    Fichier_Local = rd.GetString(6),
                    Date_Publication = rd.IsDBNull(7) ? null : DateTime.Parse(rd.GetString(7))
                    });
                }
            return liste;
            }

        public static bool Ajouter(Règlement reglement)
            {
            // Autorisation globale (documents)
            Service_Passerelle.VérifierOuThrow(ActionRestriction.AccéderDocuments);

            // Restriction numérique
            if (!Service_Restrictions.PeutAjouterRèglement())
                {
                throw new InvalidOperationException("Limite atteinte : maximum de règlements autorisés en mode Démo restreint.");
                }

            using SqliteConnection conn = Service_SQLite.Ouvrir();
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO reglements(annee, titre, categorie, version, source_url, fichier_local, date_publication)
                                VALUES($a, $t, $c, $v, $s, $f, $dp);";
            _ = cmd.Parameters.AddWithValue("$a", reglement.Année);
            _ = cmd.Parameters.AddWithValue("$t", reglement.Titre);
            _ = cmd.Parameters.AddWithValue("$c", reglement.Catégorie);
            _ = cmd.Parameters.AddWithValue("$v", reglement.Version ?? "");
            _ = cmd.Parameters.AddWithValue("$s", reglement.SourceUrl ?? "");
            _ = cmd.Parameters.AddWithValue("$f", reglement.Fichier_Local);
            _ = cmd.Parameters.AddWithValue("$dp", reglement.Date_Publication?.ToString("yyyy-MM-dd") ?? "");
            _ = cmd.ExecuteNonQuery();
            return true;
            }
        // utilitaires
        private static string Chemin_Unique(string chemin)
            {
            if (!File.Exists(chemin))
                {
                return chemin;
                }

            string dir = Path.GetDirectoryName(chemin)!;
            string nom = Path.GetFileNameWithoutExtension(chemin);
            string ext = Path.GetExtension(chemin);
            int i = 1;
            string c;
            do { c = Path.Combine(dir, $"{nom}_{i}{ext}"); i++; }
            while (File.Exists(c));
            return c;
            }

        private static string Hash_Fichier(string chemin)
            {
            using FileStream fs = File.OpenRead(chemin);
            using SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(fs);
            return Convert.ToHexStringLower(hash);
            }

        private static string Slug(string s)
            {
            char[] invalid = Path.GetInvalidFileNameChars();
            string clean = new([.. s.Select(ch => invalid.Contains(ch) ? '_' : ch)]);
            return clean.Replace(' ', '_');
            }
        }
    }
