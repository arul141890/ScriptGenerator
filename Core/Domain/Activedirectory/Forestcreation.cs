﻿namespace Core.Domain.Activedirectory
{
    public class Forestcreation : BaseModel
    {
        public string Hostname { get; set; }

        public string Ipaddress { get; set; }

        public string Databasepath { get; set; }

        public string Domainmode { get; set; }

        public string Domainname { get; set; }

        public string Domainnetbiosname { get; set; }

        public string Forestmode { get; set; }

        public string Logpath { get; set; }

        public string Sysvolpath { get; set; }
        
    }
}