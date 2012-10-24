using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RestfulRouting;

namespace AddonTemplate.Web
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
             
			RegisterRoutes(RouteTable.Routes);
		}

		private static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
			routes.MapRoutes<Routes>();
		}

        public void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                var client = new SmtpClient("email-smtp.us-east-1.amazonaws.com", 587);

               client.EnableSsl = true;

               client.Credentials = new NetworkCredential("AKIAIETZDMTH5OPB3VMA", "AnHlEOWHR6qAaeX+tO5ajSLj1yHS/R+QBEcAAhTJp46O");

                var sb = new StringBuilder();
                sb.Append("<br/> Refer: " + Request.UrlReferrer);
                sb.Append("<br/> Url: " + Request.Url.AbsoluteUri);
                sb.Append("<br/> Query: " + Request.Url.Query);
                foreach (var header in Request.Headers)
                {
                    sb.Append("<br/> Header: " + header + " val: " + Request.Headers[header.ToString()]);
                }

                var message = new MailMessage()
                    {
                        From = new MailAddress("sleonard84@msn.com", "Steve"),
                        Subject = "Addon Debug Info",
                        Body = sb.ToString(),
                        IsBodyHtml=true
                    };
                message.To.Add(new MailAddress("sleonard84@msn.com", "Steve"));

                client.Send(message);
            }
            catch (Exception ex)
            {
              
            }
        }
	}
}
