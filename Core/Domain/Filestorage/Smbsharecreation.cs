namespace Core.Domain.Filestorage
{
    public class Smbsharecreation : BaseModel
    {
        public string Directoryname { get; set; }

        public string Smbname { get; set; }

        public string Encyptdata { get; set; }

        public string Accessgroups { get; set; }
    }
}