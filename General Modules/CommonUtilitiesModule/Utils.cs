using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilitiesModule
{
    public static class Utils
    {
        public static DateTime Ist
        {
            get
            {
                var indZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indZone);
            }
        }

        public static DateTime GetIstForUtc(DateTime utcDate)
        {
            var indZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, indZone);
        }

        public static DateTime GetUtcForIst(DateTime istDate)
        {
            var indZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            istDate = DateTime.SpecifyKind(istDate, DateTimeKind.Local);
            return TimeZoneInfo.ConvertTimeToUtc(istDate, indZone);
        }

        public static void SendEmail(string email, string subject, string body)
        {
            try
            {
                var m = new MailMessage("ess@tapmobi.in", email) { Subject = subject, Body = body, IsBodyHtml = true };

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("ess@tapmobi.in", "sms56060"),
                    EnableSsl = true
                };
                smtp.Send(m);
            }
            catch (Exception)
            {

            }
        }
    }
}
