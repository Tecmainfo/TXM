namespace TXM.Services.Licences   // ⚠️ Dans LicenceManager => namespace LicenceManager.Services
    {
    public static class Crypto_Service
        {
        // ⚠️ Ne pas modifier => clé/IV doivent être identiques dans les 2 projets
        private static readonly byte[] _clé = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF"); // 32 chars = 256 bits
        private static readonly byte[] _iv = Encoding.UTF8.GetBytes("ABCDEF0123456789");                  // 16 chars = 128 bits

        public static byte[] Chiffrer(byte[] clair)
            {
            // Exemple AES basique
            using var aes = Aes.Create();
            aes.Key = _clé; // clé secrète générée au lancement
            aes.IV = _iv;   // vecteur initialisation

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clair, 0, clair.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
            }

        public static byte[] Dechiffrer(byte[] chiffre)
            {
            using var aes = Aes.Create();
            aes.Key = _clé;
            aes.IV = _iv;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(chiffre, 0, chiffre.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
            }
        }
    }
