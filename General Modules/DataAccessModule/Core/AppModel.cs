// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppModel.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The app model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Core
{
    using System;

    public abstract class AppModel
    {

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Id { get; set; }

        public int LastChangedAuditTrailId { get; set; }

    }
}