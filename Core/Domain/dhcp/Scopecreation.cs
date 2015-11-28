namespace Core.Domain.dhcp
{
    public class Scopecreation : BaseModel
    {
        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string Csvfilename { get; set; }

    }
}