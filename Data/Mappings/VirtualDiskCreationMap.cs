using System.Data.Entity.ModelConfiguration;
using Core.Domain.Hyperv;

namespace Data.Mappings
{
    public class VirtualDiskCreationMap : EntityTypeConfiguration<VirtualDiskCreation>
    {
        #region Constructors and Destructors

        public VirtualDiskCreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}