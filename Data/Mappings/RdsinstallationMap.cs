using System.Data.Entity.ModelConfiguration;
using Core.Domain.Remotedesktopservices;

namespace Data.Mappings
{
    public class RdsinstallationMap : EntityTypeConfiguration<Rdsinstallation>
    {
        #region Constructors and Destructors

        public RdsinstallationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}