using System;

namespace MediaTekDocuments.model
{
    public class Abonnement : Commande
    {
        public DateTime DateFinAbonnement { get; set; }
        public string IdRevue { get; set; }
        
        public string IdIden => Id; 

        public Abonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue)
            : base(id, dateCommande, montant)
        {
            this.DateFinAbonnement = dateFinAbonnement;
            this.IdRevue = idRevue;
        }

        public static bool ParutionDansAbonnement(DateTime debut, DateTime fin, DateTime parution)
        {
            return (parution >= debut && parution <= fin);
        }
    }
}