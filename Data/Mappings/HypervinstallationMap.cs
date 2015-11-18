using System.Data.Entity.ModelConfiguration;
using Core.Domain.Hyperv;

namespace Data.Mappings
{
    public class HypervinstallationMap : EntityTypeConfiguration<Hypervinstallation>
    {
        #region Constructors and Destructors

        public HypervinstallationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}