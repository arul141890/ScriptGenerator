using System.Data.Entity.ModelConfiguration;
using Core.Domain.Filestorage;

namespace Data.Mappings
{
    public class RoleinstallationMap : EntityTypeConfiguration<Roleinstallation>
    {
        #region Constructors and Destructors

        public RoleinstallationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}