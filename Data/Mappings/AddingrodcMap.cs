using System.Data.Entity.ModelConfiguration;
using Core.Domain.Activedirectory;

namespace Data.Mappings
{
    public class AddingrodcMap : EntityTypeConfiguration<Addingrodc>
    {
        #region Constructors and Destructors

        public AddingrodcMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}