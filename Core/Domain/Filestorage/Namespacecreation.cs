namespace Core.Domain.Filestorage
{
    public class Namespacecreation : BaseModel
    {
        public string Dfsservername { get; set; }

        public string Smbsharename { get; set; }

        public string Fileservername { get; set; }

        public string Domainname { get; set; }
    }
}