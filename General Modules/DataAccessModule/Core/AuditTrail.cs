// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditTrail.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The audit trail.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Core
{
    using System;

    public class AuditTrail
    {

        public DateTime CreatedDate { get; set; }

        public int Id { get; set; }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public string UserId { get; set; }

    }
}