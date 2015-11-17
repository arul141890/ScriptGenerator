namespace Core.Domain.dns
{
    public class Dnsrecordcreation : BaseModel
    {
        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string Zonename { get; set; }
    }
}