namespace Core.Domain.Webserver
{
    public class Webserverinstallation : BaseModel
    {
        public string Ipaddress { get; set; }

        public string Hostname { get; set; }

        public string Installationtype { get; set; }
    }
}