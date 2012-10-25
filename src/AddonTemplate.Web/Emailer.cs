using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace AddonTemplate.Web
{
    public class Emailer
    {
        public static void SendEmail(string subject, string body)
        {
            try
            {
                if(ConfigurationManager.AppSettings["DebugEmail"] == "1")
                {

                    var client = new SmtpClient("email-smtp.us-east-1.amazonaws.com", 587);

                    client.EnableSsl = true;

                    client.Credentials = new NetworkCredential("AKIAIETZDMTH5OPB3VMA",
                                                               "AnHlEOWHR6qAaeX+tO5ajSLj1yHS/R+QBEcAAhTJp46O");


                    var message = new MailMessage()
                        {
                            From = new MailAddress("sleonard84@msn.com", "Steve"),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        };
                    message.To.Add(new MailAddress("sleonard84@msn.com", "Steve"));

                    client.Send(message);
                }
            }
            catch (Exception ex)
            {

            }
        }
        
    }
}