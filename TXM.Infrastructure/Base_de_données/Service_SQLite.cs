namespace TXM.Infrastructure.Base_de_données
{
    public static class Service_SQLite
    {
        public static SqliteConnection Ouvrir()
        {
            var c = new SqliteConnection(Configuration_Base_de_données.Chaine_Connexion);
            c.Open();
            using var pragma = c.CreateCommand();
            pragma.CommandText = "PRAGMA foreign_keys = ON;";
            pragma.ExecuteNonQuery();
            return c;
        }
    }
}
