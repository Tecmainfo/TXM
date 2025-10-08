/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Vm
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : VM_Sites.cs
*  Emplacement du fichier  : TXM/TXM.Vm/Grands_Tournois/VM_Sites.cs
*  Description             : ViewModel pour la gestion des sites d’un tournoi majeur (CRUD complet)
* * * * * * * * * * * * *
*/

namespace TXM.Vm.Grands_Tournois
    {
    public class VM_Sites : BaseVM
        {
        public ObservableCollection<Site> Sites { get; } = new();

        private Site? _siteSélectionné;
        public Site? SiteSélectionné
            {
            get => _siteSélectionné;
            set
                {
                if (_siteSélectionné != value)
                    {
                    _siteSélectionné = value;
                    OnPropertyChanged(nameof(SiteSélectionné));
                    }
                }
            }

        private readonly int _idTournoi;
        public VM_Sites(int idTournoi)
            {
            _idTournoi = idTournoi;
            Charger();
            }

        public void Charger()
            {
            Sites.Clear();
            foreach (Site s in Service_Site.ListerPourTournoi(_idTournoi))
                Sites.Add(s);

            SiteSélectionné = Sites.FirstOrDefault();
            }

        public void Ajouter(Site site)
            {
            Service_Site.Ajouter(site);
            Charger();
            }

        public void Supprimer(Site site)
            {
            if (site == null) return;
            Service_Site.Supprimer(site.Id);
            Charger();
            }
        }
    }
