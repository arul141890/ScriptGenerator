// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientMap.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The client map.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Entity.ModelConfiguration;
using Core.Domain;

namespace Data.Mappings
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        #region Constructors and Destructors

        public UserMap()
        {
            this.HasKey(x => x.Id);
        }

        #endregion
    }
}