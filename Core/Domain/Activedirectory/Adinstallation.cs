namespace Core.Domain.Activedirectory
{
    public class Adinstallation : BaseModel
    {
        public string Createorjoin { get; set; }

        public string Replicaornewdomain { get; set; }

        public string Replicadomaindnsname { get; set; }

        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string Userdomain { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Installdns { get; set; }

        public string Confirmgc { get; set; }

        public string Creatednsdelegation { get; set; }

        public string Safemodeadminpassword { get; set; }
    }
}