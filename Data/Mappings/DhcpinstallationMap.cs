using System.Data.Entity.ModelConfiguration;
using Core.Domain.dhcp;

namespace Data.Mappings
{
    public class DhcpinstallationMap : EntityTypeConfiguration<Dhcpinstallation>
    {
        #region Constructors and Destructors

        public DhcpinstallationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}