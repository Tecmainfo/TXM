namespace TXM.Infrastructure.Base_de_données
{
    public static class Schéma_SQLite
    {
        public static void Initialiser()
        {
            Configuration_Base_de_données.S_assurer_Dossier();
            using var conn = new SqliteConnection(Configuration_Base_de_données.Chaine_Connexion);
            conn.Open();

            using (var pragma = conn.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            // Création des tables de base
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS __métadonnées(
  clé TEXT PRIMARY KEY,
  valeur TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS joueurs(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nom TEXT NOT NULL,
  licence TEXT,
  club TEXT
);

CREATE TABLE IF NOT EXISTS équipes(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nom TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS incidents(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  date TEXT NOT NULL,
  description TEXT NOT NULL,
  gravité TEXT NOT NULL,
  id_équipe INTEGER NULL,
  id_joueur INTEGER NULL,
  FOREIGN KEY(id_équipe) REFERENCES équipes(id) ON DELETE SET NULL,
  FOREIGN KEY(id_joueur) REFERENCES joueurs(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS sanctions(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  date TEXT NOT NULL,
  type TEXT NOT NULL,
  motif TEXT NOT NULL,
  id_incident INTEGER NULL,
  FOREIGN KEY(id_incident) REFERENCES incidents(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS compétitions(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nom TEXT NOT NULL,
  lieu TEXT,
  date TEXT NOT NULL,
  statut TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS concours(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nom TEXT NOT NULL,
  date TEXT NOT NULL,
  type TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS concours_officiels(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nom TEXT NOT NULL,
  date TEXT NOT NULL,
  homologation TEXT,
  arbitre TEXT,
  statut TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS inscriptions(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nom_équipe TEXT NOT NULL,
  joueurs TEXT NOT NULL,
  id_concours INTEGER NOT NULL,
  FOREIGN KEY(id_concours) REFERENCES concours_officiels(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS matches(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  tour INTEGER NOT NULL,
  equipeA TEXT NOT NULL,
  equipeB TEXT NOT NULL,
  scoreA INTEGER NOT NULL DEFAULT 0,
  scoreB INTEGER NOT NULL DEFAULT 0,
  id_concours INTEGER NOT NULL,
  FOREIGN KEY(id_concours) REFERENCES concours_officiels(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS homologation_historique(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  id_concours INTEGER NOT NULL,
  date_action TEXT NOT NULL,
  décision TEXT NOT NULL,
  arbitre TEXT,
  commentaire TEXT,
  FOREIGN KEY(id_concours) REFERENCES concours_officiels(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS poules(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      nom TEXT NOT NULL,
      id_concours INTEGER NOT NULL,
      heure_début TEXT NOT NULL,
      FOREIGN KEY(id_concours) REFERENCES concours_officiels(id) ON DELETE CASCADE
    );

CREATE TABLE IF NOT EXISTS poules_inscriptions(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  id_poule INTEGER NOT NULL,
  id_inscription INTEGER NOT NULL,
  FOREIGN KEY(id_poule) REFERENCES poules(id) ON DELETE CASCADE,
  FOREIGN KEY(id_inscription) REFERENCES inscriptions(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS équipes_joueurs(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  id_équipe INTEGER NOT NULL,
  id_joueur INTEGER NOT NULL,
  FOREIGN KEY(id_équipe) REFERENCES équipes(id) ON DELETE CASCADE,
  FOREIGN KEY(id_joueur) REFERENCES joueurs(id) ON DELETE CASCADE
);

INSERT OR IGNORE INTO __métadonnées(clé, valeur) VALUES ('version_schéma', '1');
";
            cmd.ExecuteNonQuery();

            // Récupérer la version actuelle
            var versionActuelle = LireVersion(conn);

            // Appliquer migrations selon version
            if (versionActuelle < 2)
            {
                Migrer_V2(conn);
                MettreAJourVersion(conn, 2);
            }

            if (versionActuelle < 3)
            {
                Migrer_V3(conn);
                MettreAJourVersion(conn, 3);
            }

            if (versionActuelle < 4)
            {
                Migrer_V4(conn);
                MettreAJourVersion(conn, 4);
            }

            if (versionActuelle < 5)
            {
                Migrer_V5(conn);
                MettreAJourVersion(conn, 5);
            }

            if (versionActuelle < 6)
            {
                Migrer_V6(conn);
                MettreAJourVersion(conn, 6);
            }

            if (versionActuelle < 7)
            {
                Migrer_V7(conn);
                MettreAJourVersion(conn, 7);
            }
        }

        private static int LireVersion(SqliteConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT valeur FROM __métadonnées WHERE clé='version_schéma';";
            var valeur = cmd.ExecuteScalar()?.ToString();
            return int.TryParse(valeur, out var v) ? v : 1;
        }

        private static void MettreAJourVersion(SqliteConnection conn, int nouvelleVersion)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE __métadonnées SET valeur=$v WHERE clé='version_schéma';";
            cmd.Parameters.AddWithValue("$v", nouvelleVersion.ToString());
            cmd.ExecuteNonQuery();
        }

        private static void Migrer_V2(SqliteConnection conn)
        {
            // Ajout de colonnes incidents
            AjouterColonneSiManquante(conn, "incidents", "horodatage", "TEXT DEFAULT CURRENT_TIMESTAMP");
            AjouterColonneSiManquante(conn, "incidents", "arbitre", "TEXT");

            // Ajout de colonnes sanctions
            AjouterColonneSiManquante(conn, "sanctions", "article_règlement", "TEXT");
            AjouterColonneSiManquante(conn, "sanctions", "arbitre", "TEXT");
            AjouterColonneSiManquante(conn, "sanctions", "durée_minutes", "INTEGER DEFAULT 0");

            // Ajout de colonnes matches
            AjouterColonneSiManquante(conn, "matches", "date", "TEXT");
            AjouterColonneSiManquante(conn, "matches", "date_heure", "TEXT");
        }

        private static void Migrer_V3(SqliteConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS poules(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      nom TEXT NOT NULL,
      id_concours INTEGER NOT NULL,
      heure_début TEXT NOT NULL,
      FOREIGN KEY(id_concours) REFERENCES concours_officiels(id) ON DELETE CASCADE
    );";
            cmd.ExecuteNonQuery();
        }

        private static void Migrer_V4(SqliteConnection conn)
        {
            AjouterColonneSiManquante(conn, "matches", "id_poule", "INTEGER");
        }

        private static void Migrer_V5(SqliteConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS poules_joueurs(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  id_poule INTEGER NOT NULL,
  id_joueur INTEGER NOT NULL,
  FOREIGN KEY(id_poule) REFERENCES poules(id) ON DELETE CASCADE,
  FOREIGN KEY(id_joueur) REFERENCES joueurs(id) ON DELETE CASCADE
);";
            cmd.ExecuteNonQuery();
        }

        private static void Migrer_V6(SqliteConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS rapports(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      date TEXT NOT NULL,
      type TEXT NOT NULL,
      utilisateur TEXT NOT NULL,
      fichier TEXT NOT NULL
    );";
            cmd.ExecuteNonQuery();
        }

        private static void Migrer_V7(SqliteConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS reglements(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  annee INTEGER NOT NULL,
  titre TEXT NOT NULL,
  categorie TEXT NOT NULL,
  version TEXT,
  source_url TEXT,
  fichier_local TEXT NOT NULL,
  date_publication TEXT,
  date_ajout TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
  hash TEXT
);

CREATE INDEX IF NOT EXISTS idx_reglements_annee ON reglements(annee);
CREATE INDEX IF NOT EXISTS idx_reglements_categorie ON reglements(categorie);
";
            cmd.ExecuteNonQuery();
        }

        private static void AjouterColonneSiManquante(SqliteConnection conn, string table, string colonne, string typeDef)
        {
            using var check = conn.CreateCommand();
            check.CommandText = $"PRAGMA table_info({table});";
            using var reader = check.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetString(1).Equals(colonne, StringComparison.OrdinalIgnoreCase))
                    return;
            }

            using var alter = conn.CreateCommand();
            alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {colonne} {typeDef};";
            alter.ExecuteNonQuery();
        }
    }
}
