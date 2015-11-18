using System.Data.Entity.ModelConfiguration;
using Core.Domain.Filestorage;

namespace Data.Mappings
{
    public class SmbsharecreationMap : EntityTypeConfiguration<Smbsharecreation>
    {
        #region Constructors and Destructors

        public SmbsharecreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}