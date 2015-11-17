namespace Core.Domain.Remotedesktopservices
{
    public class Collectioncreation : BaseModel
    {
        public string Collectionname { get; set; }

        public string Sessionhost { get; set; }

        public string Collectiondescription { get; set; }

        public string Connectionbroker { get; set; }

    }
}