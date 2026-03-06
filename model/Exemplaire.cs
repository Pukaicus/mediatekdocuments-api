using System;

namespace MediaTekDocuments.model
{
    public class Exemplaire
    {
        public int Numero { get; set; }
        public DateTime DateReception { get; set; }
        public string Photo { get; set; }
        public string IdEtat { get; set; }
        public string IdDocument { get; set; }
        public DateTime DateAchat { get; set; }

        public Exemplaire(int numero, DateTime dateReception, string photo, string idEtat, string idDocument, DateTime dateAchat)
        {
            this.Numero = numero;
            this.DateReception = dateReception;
            this.Photo = photo;
            this.IdEtat = idEtat;
            this.IdDocument = idDocument;
            this.DateAchat = dateAchat;
        }
    }
}