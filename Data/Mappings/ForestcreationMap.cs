using System.Data.Entity.ModelConfiguration;
using Core.Domain.Activedirectory;

namespace Data.Mappings
{
    public class ForestcreationMap : EntityTypeConfiguration<Forestcreation>
    {
        #region Constructors and Destructors

        public ForestcreationMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}