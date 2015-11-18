using System.Data.Entity.ModelConfiguration;
using Core.Domain.Filestorage;

namespace Data.Mappings
{
    public class NamespacecreationMap : EntityTypeConfiguration<Namespacecreation>
    {
        #region Constructors and Destructors

        public NamespacecreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}