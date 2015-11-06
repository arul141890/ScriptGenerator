namespace Core.Domain
{
    public class Websitecreation : BaseModel
    {
        public string Apppoolname { get; set; }

        public string Websitename { get; set; }

        public string Portnumber { get; set; }

        public string Websitednsname { get; set; }

        public string Physicalpath { get; set; }
    }
}