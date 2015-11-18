using System.Data.Entity.ModelConfiguration;
using Core.Domain.dns;

namespace Data.Mappings
{
    public class DnsinstallationMap : EntityTypeConfiguration<Dnsinstallation>
    {
        #region Constructors and Destructors

        public DnsinstallationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}