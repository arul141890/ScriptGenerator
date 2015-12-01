namespace Core.Domain.Activedirectory
{
    public class Addingrodc : BaseModel
    {
        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string Allowreplicationaccount { get; set; }

        public string Delegatedadminiaccount { get; set; }

        public string Denyreplicationaccount { get; set; }

        public string DomainName { get; set; }

        public string SiteName { get; set; }

        public string Logpath { get; set; }

        public string Sysvolpath { get; set; }

        public string Databasepath { get; set; }
    }
}