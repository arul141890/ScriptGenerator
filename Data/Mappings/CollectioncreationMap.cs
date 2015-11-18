using System.Data.Entity.ModelConfiguration;
using Core.Domain.Remotedesktopservices;

namespace Data.Mappings
{
    public class CollectioncreationMap : EntityTypeConfiguration<Collectioncreation>
    {
        #region Constructors and Destructors

        public CollectioncreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}