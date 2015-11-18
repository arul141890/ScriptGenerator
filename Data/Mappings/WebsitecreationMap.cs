using System.Data.Entity.ModelConfiguration;
using Core.Domain.Webserver;

namespace Data.Mappings
{
    public class WebsitecreationMap : EntityTypeConfiguration<Websitecreation>
    {
        #region Constructors and Destructors

        public WebsitecreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}