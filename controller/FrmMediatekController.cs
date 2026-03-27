using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using System.Linq;

namespace MediaTekDocuments.controller
{
    class FrmMediatekController
    {
        private readonly Access access;

        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }
        
        #region Sécurité
        /// <summary>
        /// Vérifie les identifiants et récupère l'utilisateur avec son service
        /// </summary>
        public Utilisateur Authentification(string login, string pwd)
        {
            return access.Authentification(login, pwd);
        }
        #endregion

        #region Référentiels
        public List<Categorie> GetAllGenres() => access.GetAllGenres();
        public List<Categorie> GetAllRayons() => access.GetAllRayons();
        public List<Categorie> GetAllPublics() => access.GetAllPublics();
        public List<Suivi> GetAllSuivi() => access.GetAllSuivi();
        #endregion

        #region Gestion des Documents
        // LIVRES
        public List<Livre> GetAllLivres() => access.GetAllLivres();
        public Livre GetLivre(string id) => access.GetLivre(id).FirstOrDefault(); 
        public bool CreerLivre(Livre livre) => access.CreerEntite("livre", livre);
        public bool ModifierLivre(Livre livre) => access.ModifierEntite("livre", livre);
        public bool SupprimerLivre(Livre livre) => access.SupprimerEntite("livre", livre);

        // DVD 
        public List<Dvd> GetAllDvd() => access.GetAllDvd();
        public Dvd GetDvd(string id) => access.GetDvd(id).FirstOrDefault();
        public bool CreerDvd(Dvd dvd) => access.CreerDvd(dvd);
        public bool ModifierDvd(Dvd dvd) => access.ModifierDvd(dvd);
        public bool SupprimerDvd(Dvd dvd) => access.SupprimerDvd(dvd);

        // REVUES
        public List<Revue> GetAllRevues() => access.GetAllRevues();
        public Revue GetRevue(string id) => access.GetRevue(id).FirstOrDefault(); 
        public List<Exemplaire> GetExemplairesRevue(string idDocument) => access.GetExemplairesRevue(idDocument);
        public bool CreerExemplaire(Exemplaire exemplaire) => access.CreerExemplaire(exemplaire);
        // EXEMPLAIRES
        public bool SupprimerExemplaire(Exemplaire exemplaire) => access.SupprimerEntite("exemplaire", exemplaire);
        public bool CreerRevue(Revue revue) => access.CreerRevue(revue);
        public bool ModifierRevue(Revue revue) => access.ModifierRevue(revue);
        public bool SupprimerRevue(Revue revue) => access.SupprimerRevue(revue);
        #endregion

        #region Gestion des Abonnements
        public List<Abonnement> GetAbonnements(string idRevue) => access.GetAbonnements(idRevue);
        public List<Abonnement> GetAbonnementsEcheance() => access.GetAbonnementsEcheance();
        public bool CreerAbonnement(Abonnement abonnement) => access.CreerEntite("abonnement", abonnement);
        public bool SupprimerAbonnement(Abonnement abonnement) => access.SupprimerEntite("abonnement", abonnement);
        #endregion

        #region Gestion des Commandes
        public List<CommandeDocument> GetCommandesDocument(string idDocument) => access.GetCommandesDocument(idDocument);
        public List<CommandeDocument> GetAllCommandesLivres() => access.GetAllCommandesLivres();
        public List<CommandeDocument> GetAllCommandesDvd() => access.GetAllCommandesDvd();
        public List<CommandeDocument> GetAllCommandesRevues() => access.GetAllCommandesRevues();
        public bool CreerCommandeDocument(CommandeDocument commandeDocument) => access.CreerEntite<CommandeDocument>("commandedocument", commandeDocument);
        public bool ModifierCommandeDocument(CommandeDocument commandeDocument) => access.ModifierEntite<CommandeDocument>("commandedocument", commandeDocument);
        public bool SupprimerCommandeDocument(CommandeDocument commandeDocument) => access.SupprimerEntite<CommandeDocument>("commandedocument", commandeDocument);
        #endregion
    }
}