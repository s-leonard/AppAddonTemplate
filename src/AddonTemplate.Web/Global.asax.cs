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
                var sb = new StringBuilder();
                sb.Append("<br/> Refer: " + Request.UrlReferrer);
                sb.Append("<br/> Url: " + Request.Url.AbsoluteUri);
                sb.Append("<br/> Query: " + Request.Url.Query);
                foreach (var header in Request.Headers)
                {
                    sb.Append("<br/> Header: " + header + " val: " + Request.Headers[header.ToString()]);
                }
                foreach (var key in Request.Form.AllKeys)
                {
                    sb.Append("<br/> Form Val: " + key + " val: " + Request.Form[key]);
                }
                
                sb.Append("<br/> Request Method" + Request.HttpMethod);
                sb.Append("<br/> Request Request Context" + Request.RequestContext);
                sb.Append("<br/> Request Request Type" + Request.RequestType);
                Emailer.SendEmail("Addon Debug Info", sb.ToString());
            }
            catch (Exception ex)
            {
              
            }
        }
	}
}
