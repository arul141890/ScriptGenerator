namespace Core.Domain
{
    public class VirtualDiskCreation : BaseModel
    {
        public string VHDPath { get; set; }

        public string VHDSize { get; set; }

        public string VHDType { get; set; }

        public string ParentPath { get; set; }
    }
}