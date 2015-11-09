using System.Data.Entity.ModelConfiguration;
using Core.Domain.Hyperv;

namespace Data.Mappings
{
    public class VirtualSwitchCreationMap : EntityTypeConfiguration<VirtualSwitchCreation>
    {
        #region Constructors and Destructors

        public VirtualSwitchCreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}