/*
* * * * * * * * * * * * *
* Nom du logiciel        : TripleX / Pétanque Maestro
* Projet Principal       : Interfaces
* Auteur                 : Kano
* Licence                : Licence Propriétaire
* Classification         : C2, Confidentiel
* Version                : 2025.09.2.0.0
* Type de version        : Alpha
* Nom du fichier         : Vue_Paramètres.xaml.cs
* Emplacement du fichier : Interfaces/Vues/Vue_Paramètres.xaml.cs
* Description            : Logique d’interaction pour la vue Paramètres (Skip_Splash + Langue)
* Copyright              : © 2025 GUILMET GAETAN – Tous droits réservés 
* * * * * * * * * * * * *
*/

namespace TXM.Interfaces.Vues
    {
    /// <summary>
    /// Logique d’interaction pour Vue_Paramètres.xaml
    /// </summary>
    public partial class Vue_Paramètres : UserControl
        {
        public Vue_Paramètres()
            {
            InitializeComponent();
            DataContext = new VM_Paramètres();
            }
        }
    }
