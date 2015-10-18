// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumHelper.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The enum helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumHelper
    {

        public static object GetEnumFrmoString(Type enumType, string name)
        {
            return Enum.Parse(enumType, name, true);
        }

        public static List<string> GetEnumNames(this Type enumType)
        {
            return Enum.GetNames(enumType).ToList();
        }

    }
}