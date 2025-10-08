namespace TXM.Services
    {
    public static class Service_Settings
        {
        // ✅ On garde camelCase pour compatibilité appsettings.json
        private record AppSettings(string Thème = "triplex", bool Skip_Splash = false);

        private static readonly string _dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TripleXMaestro");

        private static readonly string _file = Path.Combine(_dir, "appsettings.json");

        private static AppSettings _cache = new();

        private static readonly JsonSerializerOptions _jsonOptions =
            new()
                {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
                };

        public static void Load()
            {
            try
                {
                if (File.Exists(_file))
                    {
                    string json = File.ReadAllText(_file);
                    _cache = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new();
                    }
                }
            catch
                {
                _cache = new();
                }
            }

        public static void Save()
            {
            try
                {
                _ = Directory.CreateDirectory(_dir);
                string json = JsonSerializer.Serialize(_cache, _jsonOptions);
                File.WriteAllText(_file, json);
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[Settings] Sauvé: {_file}\n{json}");
#endif
                }
            catch
                {
                // noop
                }
            }

        /// <summary>
        /// Identifiant du thème en cours ("triplex", "maestro", "demo").
        /// </summary>
        public static string Thème
            {
            get => _cache.Thème.ToLowerInvariant();
            set
                {
                // On normalise la valeur
                string val = (value ?? "triplex").Trim().ToLowerInvariant();
                _cache = _cache with { Thème = val };
                Save();
                }
            }

        public static bool Skip_Splash
            {
            get => _cache.Skip_Splash;
            set { _cache = _cache with { Skip_Splash = value }; Save(); }
            }
        }
    }
