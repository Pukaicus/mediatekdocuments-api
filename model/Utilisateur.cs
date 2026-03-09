namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Pwd { get; set; }
        public string Service { get; set; }

        public Utilisateur(int id, string login, string pwd, string service)
        {
            this.Id = id;
            this.Login = login;
            this.Pwd = pwd;
            this.Service = service;
        }
    }
}