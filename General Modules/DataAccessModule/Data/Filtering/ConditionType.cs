// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionType.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The condition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Filtering
{
    public enum ConditionType
    {
        EqualTo, 

        NotEqualTo, 

        LessThan, 

        GreaterThan, 

        Like, 

        NotLike, 

        LessThanOrEqualTo, 

        GreaterThanOrEqualTo, 

        In
    }
}