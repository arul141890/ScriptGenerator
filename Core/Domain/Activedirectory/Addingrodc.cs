namespace Core.Domain.Activedirectory
{
    public class Addingrodc : BaseModel
    {
        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string AllowpasswordreplicationaccountName { get; set; }

        public string CriticalReplicationOnly { get; set; }

        public string Delegatedadministratoraccountname { get; set; }

        public string Denypasswordreplicationaccountname { get; set; }

        public string DomainName { get; set; }

        public string InstallDNS { get; set; }

        public string SiteName { get; set; }

     }
}