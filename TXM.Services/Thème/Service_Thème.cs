using TXM.Modèles.Thèmes;

namespace TXM.Services.Thème
    {
    /// <summary>
    /// Service central de gestion du thème (mutualisé TripleX / Pétanque Maestro / Démo)
    /// </summary>
    public sealed class Service_Thème
        {
        private ResourceDictionary? _courant;

        private readonly string _path_TripleX =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/TripleX_Thème.xaml";
        private readonly string _path_Maestro =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/Maestro_Thème.xaml";
        private readonly string _path_Démo =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/Démo_Thème.xaml";
        private readonly string _path_GrandsTournois =
            "pack://application:,,,/TXM.Interfaces;component/Ressources/Thèmes/GrandsTournois_Thème.xaml";

        public Thème_Id ThèmeActif { get; private set; } = Thème_Id.TripleX;

        private const string _settingsFile = "appsettings.json";

        public void Appliquer_Thème(Thème_Id id)
            {
            if (Application.Current is not Application app)
                return;

            string path = id switch
                {
                    Thème_Id.TripleX => _path_TripleX,
                    Thème_Id.Maestro => _path_Maestro,
                    Thème_Id.Démo => _path_Démo,
                    Thème_Id.GrandsTournois => _path_GrandsTournois,
                    _ => _path_TripleX
                    };

            ResourceDictionary nouveau = new ResourceDictionary { Source = new Uri(path, UriKind.Absolute) };

            if (_courant != null)
                app.Resources.MergedDictionaries.Remove(_courant);

            app.Resources.MergedDictionaries.Add(nouveau);
            _courant = nouveau;
            ThèmeActif = id;

            Sauver_Thème(id);
            }

        private static void Sauver_Thème(Thème_Id id)
            {
            try
                {
                var json = JsonSerializer.Serialize(new ParamètresThème { Thème = id.ToString() },
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFile, json);
                }
            catch
                {
                // rien de bloquant ici
                }
            }

        private sealed class ParamètresThème
            {
            public string Thème { get; set; } = "TripleX";
            }
        }
    }
