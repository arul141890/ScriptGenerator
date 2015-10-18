// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredMessageConverter.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The colored message converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.Logging
{
    using System.IO;
    using log4net.Core;
    using log4net.Layout.Pattern;

    public class ColoredMessageConverter : PatternLayoutConverter
    {

        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var color = string.Empty;
            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                    color = "#FFC40D";
                    break;
                case "WARN":
                case "INFO":
                    color = "#00ABA9";
                    break;
                case "ERROR":
                    color = "#EE1111";
                    break;
                case "FATAL":
                    color = "#FF0097";
                    break;
            }

            var logToRender = string.Format(" <p style=\"color:{0};\">{1}</p>", color, loggingEvent.RenderedMessage);

            writer.Write(logToRender);
        }

    }
}