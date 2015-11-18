using System.Data.Entity.ModelConfiguration;
using Core.Domain.Hyperv;

namespace Data.Mappings
{
    public class VMCreationMap : EntityTypeConfiguration<VMCreation>
    {
        #region Constructors and Destructors

        public VMCreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}