using System.Data.Entity.ModelConfiguration;
using Core.Domain.Webserver;

namespace Data.Mappings
{
    public class WebserverinstallationMap : EntityTypeConfiguration<Webserverinstallation>
    {
        #region Constructors and Destructors

        public WebserverinstallationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}