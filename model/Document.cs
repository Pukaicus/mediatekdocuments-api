namespace MediaTekDocuments.model
{
    public class Document
    {
        public string Id { get; set; }
        public string Titre { get; set; }
        public string Image { get; set; }
        public string IdGenre { get; set; }
        public string Genre { get; set; }
        public string IdPublic { get; set; }
        public string Public { get; set; }
        public string IdRayon { get; set; }
        public string Rayon { get; set; }

        public Document(string id, string titre, string image, string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
        {
            this.Id = id;
            this.Titre = titre;
            this.Image = image;
            this.IdGenre = idGenre;
            this.Genre = genre;
            this.IdPublic = idPublic;
            this.Public = lePublic;
            this.IdRayon = idRayon;
            this.Rayon = rayon;
        }
    }
}