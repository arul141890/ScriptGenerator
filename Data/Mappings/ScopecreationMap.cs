using System.Data.Entity.ModelConfiguration;
using Core.Domain.dhcp;

namespace Data.Mappings
{
    public class ScopecreationMap : EntityTypeConfiguration<Scopecreation>
    {
        #region Constructors and Destructors

        public ScopecreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}