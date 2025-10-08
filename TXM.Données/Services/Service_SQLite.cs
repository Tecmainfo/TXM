using Microsoft.Data.Sqlite;

namespace TXM.Données.Services
    {
    public static class Service_SQLite
        {
        public static SqliteConnection Ouvrir()
            {
            SqliteConnection c = new(Configuration_Base_de_données.Chaine_Connexion);
            c.Open();
            using SqliteCommand pragma = c.CreateCommand();
            pragma.CommandText = "PRAGMA foreign_keys = ON;";
            pragma.ExecuteNonQuery();
            return c;
            }
        }
    }
