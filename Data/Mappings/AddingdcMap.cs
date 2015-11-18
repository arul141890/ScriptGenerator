using System.Data.Entity.ModelConfiguration;
using Core.Domain.Activedirectory;

namespace Data.Mappings
{
    public class AddingdcMap : EntityTypeConfiguration<Addingdc>
    {
        #region Constructors and Destructors

        public AddingdcMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}