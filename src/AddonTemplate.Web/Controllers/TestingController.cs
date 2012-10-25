using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AddonTemplate.Web.Models;
using Newtonsoft.Json;

namespace AddonTemplate.Web.Controllers
{
    public class TestingController : Controller
    {
        //
        // GET: /Testing/

        public ActionResult Index()
        {
            var provisionRequest = JsonConvert.DeserializeObject<ProvisioningRequest>("{ 'callback_url': 'https://appharbor.com/addons/appaddon/resources/21cdf174-5ec7-476b-af65-5eee466f6b80', 'region': 'amazon-web-services::eu-west-1', 'heroku_id': '21cdf174-5ec7-476b-af65-5eee466f6b80@apphb.com', 'plan': 'appharbor' } ");
            return View();
        }

    }
}
