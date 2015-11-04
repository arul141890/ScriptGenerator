namespace Core.Domain
{
    public class Dhcpinstallation : BaseModel
    {
        public string Staticip { get; set; }

        public string Joindomain { get; set; }

        public string Authorize { get; set; }

        public string Hostname { get; set; }

        public string Ipaddress { get; set; }
    }
}