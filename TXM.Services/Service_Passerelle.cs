namespace TXM.Services
    {
    /// <summary>
    /// Centralise les règles d’autorisations par action.
    /// S’appuie sur Service_Licence pour déterminer ce qui est autorisé
    /// en mode Démo, Démo restreint, TripleX ou Maestro.
    /// </summary>
    public static class Service_Passerelle
        {
        /// <summary>
        /// Vérifie si une action est autorisée. Lève une exception sinon.
        /// </summary>
        public static void VérifierOuThrow(ActionRestriction restriction)
            {
            if (!EstAutorisé(restriction))
                {
                throw new InvalidOperationException(
                    $"Action interdite en mode {Service_Licence.LicenceActuelle.Type} : {restriction}");
                }
            }

        /// <summary>
        /// Retourne vrai si l’action est autorisée, faux sinon.
        /// </summary>
        public static bool EstAutorisé(ActionRestriction restriction)
            {
            // En Maestro et TripleX → tout est autorisé
            if (Service_Licence.LicenceActuelle.Type == TypeLicence.Maestro)
                {
                return true;
                }

            if (Service_Licence.LicenceActuelle.Type == TypeLicence.TripleX)
                {
                return true;
                }

            // En Démo complète → autorisé sauf cas critiques
            if (!Service_Licence.EstEnModeRestreint())
                {
                return true;
                }

            // En Démo restreinte → limiter certaines fonctionnalités
            return restriction switch
                {
                    ActionRestriction.CréerConcours => false, // limité à 1 concours (géré par Service_Restrictions)
                    ActionRestriction.GérerHomologation => false,
                    ActionRestriction.GérerArbitrage => false,
                    ActionRestriction.AccéderDocuments => false,
                    ActionRestriction.CréerRapport => true,  // autorisé mais limité par Restrictions
                    ActionRestriction.CréerInscription => true,  // autorisé mais limité par Restrictions
                    _ => true
                    };
            }
        }

    /// <summary>
    /// Enum listant les actions qui peuvent être soumises à restrictions.
    /// </summary>
    public enum ActionRestriction
        {
        CréerConcours,
        CréerInscription,
        GérerHomologation,
        GérerArbitrage,
        AccéderDocuments,
        CréerRapport
        }
    }
