namespace Core.Domain.dns
{
    public class Dnsinstallation : BaseModel
    {
        public string Staticip { get; set; }

        public string Hostname { get; set; }

        public string Ipaddress { get; set; }
    }
}