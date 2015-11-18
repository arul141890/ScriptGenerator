using System.Data.Entity.ModelConfiguration;
using Core.Domain.dns;

namespace Data.Mappings
{
    public class DnsrecordcreationMap : EntityTypeConfiguration<Dnsrecordcreation>
    {
        #region Constructors and Destructors

        public DnsrecordcreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}