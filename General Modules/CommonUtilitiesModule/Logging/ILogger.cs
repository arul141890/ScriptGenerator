// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The Logger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.Logging
{
    using System;

    public interface ILogger
    {

        void Debug(Exception ex);

        void Debug(string format, params object[] objs);

        void Error(Exception ex);

        void Error(string format, params object[] objs);

        void Fatal(Exception ex);

        void Fatal(string format, params object[] objs);

        void Info(Exception ex);

        void Info(string format, params object[] objs);

        void Warn(Exception ex);

        void Warn(string format, params object[] objs);

    }
}