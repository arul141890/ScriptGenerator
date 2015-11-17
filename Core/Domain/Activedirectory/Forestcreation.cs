namespace Core.Domain.Activedirectory
{
    public class Forestcreation : BaseModel
    {
        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string Creatednsdelegation { get; set; }

        public string Domainmode { get; set; }

        public string Domainname { get; set; }

        public string Domainnetbiosname { get; set; }

        public string Forestmode { get; set; }

        public string Installdns { get; set; }

        public string safemodeadministratorpassword { get; set; }
        
    }
}