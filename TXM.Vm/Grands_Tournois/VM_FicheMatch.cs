/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Vm
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : VM_FicheMatch.cs
*  Emplacement du fichier  : TXM/TXM.Vm/Grands_Tournois/VM_FicheMatch.cs
*  Description             : ViewModel pour la génération des feuilles de match (PDF personnalisées)
* * * * * * * * * * * * *
*/

namespace TXM.Vm.Grands_Tournois
    {
    public class VM_FicheMatch : BaseVM
        {
        public Tournoi? TournoiActif { get; set; }

        private string _equipeA = "";
        private string _equipeB = "";
        private int _scoreA;
        private int _scoreB;
        private string _terrain = "";

        public string EquipeA
            {
            get => _equipeA;
            set { _equipeA = value; OnPropertyChanged(nameof(EquipeA)); }
            }

        public string EquipeB
            {
            get => _equipeB;
            set { _equipeB = value; OnPropertyChanged(nameof(EquipeB)); }
            }

        public int ScoreA
            {
            get => _scoreA;
            set { _scoreA = value; OnPropertyChanged(nameof(ScoreA)); }
            }

        public int ScoreB
            {
            get => _scoreB;
            set { _scoreB = value; OnPropertyChanged(nameof(ScoreB)); }
            }

        public string Terrain
            {
            get => _terrain;
            set { _terrain = value; OnPropertyChanged(nameof(Terrain)); }
            }

        public void Exporter()
            {
            if (TournoiActif == null) return;
            Service_Export_FicheMatch.Exporter(TournoiActif, EquipeA, EquipeB, ScoreA, ScoreB, Terrain);
            }
        }
    }
