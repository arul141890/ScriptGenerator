namespace Core.Domain.Remotedesktopservices
{
    public class Apppublish : BaseModel
    {
        public string Alias { get; set; }

        public string Displayname { get; set; }

        public string Filepath { get; set; }

        public string collectionname { get; set; }

        public string Connectionbroker { get; set; }

    }
}