namespace Core.Domain.Activedirectory
{
    public class Addingdc : BaseModel
    {
        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string Userdomain { get; set; }

        public string Databasepath { get; set; }

        public string Logpath { get; set; }

        public string Sysvolpath { get; set; }

        public string Safemodeadminpassword { get; set; }
    }
}