namespace Core.Domain.Remotedesktopservices
{
    public class Rdsinstallation : BaseModel
    {
        public string Connectionbroker { get; set; }

        public string Webaccessserver { get; set; }

        public string Sessionhost { get; set; }

        public string Gatewayserver { get; set; }

        public string Gatewayfqdn { get; set; }

    }
}