// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterType.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The filter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Filtering
{
    public enum FilterType
    {
        Or, 

        And
    }

    public enum ValueType
    {
        Parameter = 0, 

        Sql
    }
}