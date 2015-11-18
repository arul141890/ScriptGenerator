using System.Data.Entity.ModelConfiguration;
using Core.Domain.Remotedesktopservices;

namespace Data.Mappings
{
    public class ApppublishMap : EntityTypeConfiguration<Apppublish>
    {
        #region Constructors and Destructors

        public ApppublishMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}