namespace Core.Domain.Filestorage
{
    public class Namespacecreation : BaseModel
    {
        public string Dfsservername { get; set; }

        public string Dfspath { get; set; }

        public string Fileservername { get; set; }

        public string Targetpath { get; set; }
    }
}