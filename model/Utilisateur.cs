namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        public int id { get; set; }
        public string login { get; set; }
        public string pwd { get; set; }
        public string idservice { get; set; }

        public Utilisateur(int id, string login, string pwd, string idservice)
        {
            this.id = id;
            this.login = login;
            this.pwd = pwd;
            this.idservice = idservice;
        }
    }
}