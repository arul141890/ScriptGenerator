// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataExtentions.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The data extentions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.Entity.Validation;

namespace Core.Data
{
    public static class DataExtentions
    {
        #region Public Methods and Operators

        public static Exception DbEntityValidationExceptionToException(this DbEntityValidationException dbEx)
        {
            string msg = string.Empty;

            foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                {
                    msg += Environment.NewLine
                           +
                           string.Format(
                               "Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                }
            }

            return new Exception(msg, dbEx);
        }

        #endregion
    }
}