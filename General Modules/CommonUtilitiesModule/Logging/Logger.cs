// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.Logging
{
    using System;
    using log4net;
    using log4net.Config;

    public class Logger : ILogger
    {

        private readonly ILog _logger;



        static Logger()
        {
            XmlConfigurator.Configure();
        }

        public Logger(Type type)
        {
            this._logger = LogManager.GetLogger(type);
        }



        public void Debug(Exception ex)
        {
            this._logger.Debug(ex);
        }

        public void Debug(string format, params object[] objs)
        {
            this._logger.DebugFormat(format, objs);
        }

        public void Error(Exception ex)
        {
            this._logger.Error(ex);
        }

        public void Error(string format, params object[] objs)
        {
            this._logger.ErrorFormat(format, objs);
        }

        public void Fatal(Exception ex)
        {
            this._logger.Fatal(ex);
        }

        public void Fatal(string format, params object[] objs)
        {
            this._logger.FatalFormat(format, objs);
        }

        public void Info(Exception ex)
        {
            this._logger.Info(ex);
        }

        public void Info(string format, params object[] objs)
        {
            this._logger.InfoFormat(format, objs);
        }

        public void Warn(Exception ex)
        {
            this._logger.Warn(ex);
        }

        public void Warn(string format, params object[] objs)
        {
            this._logger.WarnFormat(format, objs);
        }

    }
}