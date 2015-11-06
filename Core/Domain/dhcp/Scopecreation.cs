namespace Core.Domain.dhcp
{
    public class Scopecreation : BaseModel
    {
        public string Name { get; set; }

        public string Startrange { get; set; }

        public string Endrange { get; set; }

        public string Subnetmask { get; set; }

        public string Makeactive { get; set; }

    }
}