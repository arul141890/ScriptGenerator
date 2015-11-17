namespace Core.Domain.Hyperv
{
    public class Hypervinstallation : BaseModel
    {
        public string Isphysicalmachine { get; set; }

        public string Isvtenabled { get; set; }

        public string IPAddress { get; set; }

        public string Hostname { get; set; }
    }
}