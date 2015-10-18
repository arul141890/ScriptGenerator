// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelValidationException.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The model validation exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ModelValidationException : Exception
    {

        public ModelValidationException()
        {
            this.ValidationErrors = new List<string>();
        }



        public List<string> ValidationErrors { get; set; }



        public override string ToString()
        {
            return string.Join("|", this.ValidationErrors);
        }

    }
}