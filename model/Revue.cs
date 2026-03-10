using System;
using System.ComponentModel;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Revue hérite de Document
    /// </summary>
    public class Revue : Document
    {
        public string Periodicite { get; set; }
        public int DelaiMiseADispo { get; set; }
        public string Editeur { get; set; }
        
        public DateTime DateFinAbonnement { get; set; }

        public Revue(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon,
            string periodicite, int delaiMiseADispo, string editeur)
             : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.Periodicite = periodicite;
            this.DelaiMiseADispo = delaiMiseADispo;
            this.Editeur = editeur;
        }
    }
}