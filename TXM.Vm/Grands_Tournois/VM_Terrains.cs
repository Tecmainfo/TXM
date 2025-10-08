/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Vm
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : VM_Terrains.cs
*  Emplacement du fichier  : TXM/TXM.Vm/Grands_Tournois/VM_Terrains.cs
*  Description             : ViewModel pour la gestion des terrains d’un site de tournoi (CRUD complet)
* * * * * * * * * * * * *
*/

namespace TXM.Vm.Grands_Tournois
    {
    public class VM_Terrains : BaseVM
        {
        public ObservableCollection<Terrain> Terrains { get; } = new();

        private Terrain? _terrainSélectionné;
        public Terrain? TerrainSélectionné
            {
            get => _terrainSélectionné;
            set
                {
                if (_terrainSélectionné != value)
                    {
                    _terrainSélectionné = value;
                    OnPropertyChanged(nameof(TerrainSélectionné));
                    }
                }
            }

        private readonly int _idSite;

        public VM_Terrains(int idSite)
            {
            _idSite = idSite;
            Charger();
            }

        public void Charger()
            {
            Terrains.Clear();
            foreach (Terrain t in Service_Terrain.ListerPourSite(_idSite))
                Terrains.Add(t);

            TerrainSélectionné = Terrains.FirstOrDefault();
            }

        public void Ajouter(Terrain terrain)
            {
            Service_Terrain.Ajouter(terrain);
            Charger();
            }

        public void Supprimer(Terrain terrain)
            {
            if (terrain == null) return;
            Service_Terrain.Supprimer(terrain.Id);
            Charger();
            }
        }
    }
