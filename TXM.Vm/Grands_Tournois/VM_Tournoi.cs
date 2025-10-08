/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Vm
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : VM_Tournoi.cs
*  Emplacement du fichier  : TXM/TXM.Vm/Grands_Tournois/VM_Tournoi.cs
*  Description             : ViewModel principal pour la gestion des tournois majeurs (CRUD complet)
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

namespace TXM.Vm.Grands_Tournois
    {
    public class VM_Tournoi : BaseVM
        {
        public ObservableCollection<Tournoi> Tournois { get; } = new();

        private Tournoi? _tournoiSélectionné;
        public Tournoi? TournoiSélectionné
            {
            get => _tournoiSélectionné;
            set
                {
                if (_tournoiSélectionné != value)
                    {
                    _tournoiSélectionné = value;
                    OnPropertyChanged(nameof(TournoiSélectionné));
                    }
                }
            }

        public VM_Tournoi() => Charger();

        public void Charger()
            {
            Tournois.Clear();
            foreach (Tournoi t in Service_Tournoi.ListerTous())
                Tournois.Add(t);

            TournoiSélectionné = Tournois.FirstOrDefault();
            }

        public void Ajouter(Tournoi tournoi)
            {
            Service_Tournoi.Ajouter(tournoi);
            Charger();
            }

        public void Supprimer(Tournoi tournoi)
            {
            if (tournoi == null) return;
            Service_Tournoi.Supprimer(tournoi.Id);
            Charger();
            }
        }
    }
