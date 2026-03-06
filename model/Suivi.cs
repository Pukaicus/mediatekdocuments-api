namespace MediaTekDocuments.model
{
    public class Suivi
    {
        public int Id { get; set; }
        public string Libelle { get; set; }

        public Suivi(int id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        public override string ToString() => Libelle;
    }
}