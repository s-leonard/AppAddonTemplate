﻿using System;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using AddonTemplate.Web.Models;
using AddonTemplate.Web.ViewModels;
using Newtonsoft.Json;
using PetaPoco;
using RestSharp;
using HttpCookie = System.Web.HttpCookie;

namespace AddonTemplate.Web.Controllers
{
	public class ResourceController : Controller
	{

        //public ActionResult Index()
        //{
        //    var output = new
        //        {
        //            id = Guid.NewGuid().ToString(),
        //            config = new
        //                {
        //                    APPADDON_URL = "http://appaddon.apphb.com?apikey=1234",
        //                }
        //        };
        //    Emailer.SendEmail("Addon Debug Response", output.ToString());
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

	    //[RequireBasicAuthentication("AppHarbor")]
        //public ActionResult Create(ProvisioningRequest provisionRequest)
        //{
        //    Plan plan;
        //    if (!Enum.TryParse<Plan>(provisionRequest.plan, true, out plan))
        //    {
        //        throw new ArgumentException(string.Format("Plan \"{0}\" is not a valid plan", provisionRequest.plan));
        //    }

        //    var resource = new Resource
        //    {
        //        CreatedBy = string.Format("{0};{1}", Request.GetForwardedHostAddress(), User.Identity.Name),
        //        Id = Guid.NewGuid(),
        //        Plan = plan,
        //        ProviderId = provisionRequest.heroku_id,
        //        ProvisionStatus = ProvisionStatus.Provisioning,
        //    };
			
        //    // TODO: Persist the resource

        //    // TODO: Provision the resource

        //    resource.ProvisionStatus = ProvisionStatus.Provisioned;
			
        //    // TODO: Persist the status change

        //    var output = new
        //    {
        //        id = resource.Id,
        //        config = new
        //        {
        //            CONFIG_VAR = "CONFIGURATION_VALUE",
        //        }
        //    };

        //    return Json(output);
        //}



        [RequireBasicAuthentication("AppHarbor")]
        [ActionName("Index")]
        [HttpPost]
        public ActionResult Create(ProvisioningRequest provisionRequest)
        {
            Emailer.SendEmail("Addon Action", "Create");
            //var output2 = new
            //{
            //    id = Guid.NewGuid().ToString(),
            //    config = new
            //    {
            //        APPADDON_URL = "http://appaddon.apphb.com?apikey=create",
            //    }
            //};
            //Emailer.SendEmail("Addon Debug Response", output2.ToString());
            //return Json(output2, JsonRequestBehavior.AllowGet);

            try
            {
                provisionRequest = JsonConvert.DeserializeObject<ProvisioningRequest>(Request.GetBody());
            }
            catch (Exception ex)
            {

                Emailer.SendEmail("Addon - Error Deserialize Body", ex.Message);
            }

            try
            {
                Plan plan;

                if (!Enum.TryParse<Plan>(provisionRequest.plan, true, out plan))
                {
                    Emailer.SendEmail("Addon Debug Response", "No plan exceptio");
                    throw new ArgumentException(string.Format("Plan \"{0}\" is not a valid plan", provisionRequest.plan));
                }
                
                var db = new Database("DefaultConnection");
                var purchase = new Purchase()
                    {
                        CreatedBy = string.Format("{0};{1}", Request.GetForwardedHostAddress(), User.Identity.Name),
                        UniqueId = Guid.NewGuid().ToString(),
                        Plan = plan,
                        ProviderId = provisionRequest.heroku_id,
                        ProvisionStatus = ProvisionStatus.Provisioning,
                        ApiKey = Guid.NewGuid().ToString(),
                        ApiSecretKey = Guid.NewGuid().ToString()
                    };
                // TODO: Provision the resource
                purchase.ProvisionStatus = ProvisionStatus.Provisioned;
                Purchase.Save(db, purchase);

                var output = new
                {
                    id = purchase.UniqueId,
                    config = new
                    {
                        CONFIG_ApiKey = purchase.ApiKey,
                        CONFIG_ApiSecretKey = purchase.ApiSecretKey
                    }
                };
                Emailer.SendEmail("Addon Debug Response", output.ToString());
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Emailer.SendEmail("Addon - Error Provisioning Body", ex.Message);
            }
            return Json("fail", JsonRequestBehavior.AllowGet);
        }

		[RequireBasicAuthentication("AppHarbor")]
        [ActionName("Index")]
        [HttpDelete]
		public ActionResult Destroy(Guid id)
		{
			// TODO: Fetch the resource from persistance store
			var resource = new Resource();

			resource.ProvisionStatus = ProvisionStatus.Deprovisioning;

			// TODO: Persist the status change

			// TODO: De-provision the resource

			resource.ProvisionStatus = ProvisionStatus.Deprovisioned;

			// TODO: Persist the status change

			return Json("ok");
		}


        [ActionName("Index")]
        [HttpGet]
        public ActionResult Show(int id)
        {
            return Json("ok" + id, JsonRequestBehavior.AllowGet);
        }

        [ActionName("Index")]
        [HttpGet]
        public ActionResult Show(string id)
        {
            string token = Request.QueryString["token"];
            string timeStamp = Request.QueryString["timeStamp"];
            AuthenticateToken(Guid.Parse(id), token, timeStamp);

            SetAddonCookie();

            var headerClient = new RestClient("http://appharbor.com/");
            var headerRequest = new RestRequest("header", Method.GET);
            var headerResponse = headerClient.Execute(headerRequest);

            // TODO: Fetch the resource from persistance store
            var resource = new Resource();

            // TODO: Populate the view model with the resource data
            var viewModel = new ResourceViewModel
            {
                Header = headerResponse.Content,
            };

            return View(viewModel);
        }

        [RequireBasicAuthentication("AppHarbor")]
        [ActionName("Index")]
        [HttpPut]
		public ActionResult Update(Guid id, PlatformRequest planUpdateRequest)
        {
            planUpdateRequest = JsonConvert.DeserializeObject<PlatformRequest>(Request.GetBody());
			Plan plan;
			if (!Enum.TryParse<Plan>(planUpdateRequest.plan, true, out plan))
			{
				throw new ArgumentException(string.Format("Plan \"{0}\" is not a valid plan", planUpdateRequest.plan));
			}

			// TODO: Fetch the resource from persistance store
			var resource = new Resource();

			resource.Plan = plan;

			// TODO: Update resource to reflect new plan

			// TODO: Persist the resource change

			var output = new
			{
				id = resource.Id,
				config = new
				{
					CONFIG_VAR = "CONFIGURATION_VALUE",
				}
			};
             
			return Json(output);
		}

		private void SetAddonCookie()
		{
			var navData = Request.QueryString["nav-data"];
			var cookie = new HttpCookie("appharbor-nav-data", navData);

			Response.SetCookie(cookie);
		}

		private void AuthenticateToken(Guid id, string token, string timeStamp)
		{
			var validToken = string.Join(":", id.ToString(), "MANIFEST_SSO_SALT", timeStamp);
			var hash = validToken.ToHash<SHA1Managed>();
			if (token != hash)
			{
				throw new HttpException(403, "Authentication failed");
			}

			var validTime = (DateTime.UtcNow.AddMinutes(-5) - new DateTime(1970, 1, 1)).TotalSeconds;
			if (Convert.ToInt32(timeStamp) < validTime)
			{
				throw new HttpException(403, "Timestamp too old");
			}
		}
	}
}
