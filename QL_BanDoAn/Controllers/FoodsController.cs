using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_BanDoAn.Models;
using Newtonsoft.Json.Linq;

namespace QL_BanDoAn.Controllers
{
    public class FoodsController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "zBat9CHGhKLYcr9BcldeMq4i0dS2rMxFhd3w36h3",
            BasePath = "https://qlbandoan-6f252-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        // GET: Foods
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Foods");
            JToken data = JToken.Parse(response.Body);
            var list = new List<Foods>();

            if (data.Type == JTokenType.Array)
            {
                foreach (JToken item in data.ToObject<JArray>())
                {
                    list.Add(item.ToObject<Foods>());
                }
            }
            else if (data.Type == JTokenType.Object)
            {
                foreach (JProperty item in ((JObject)data).Properties())
                {
                    list.Add(JsonConvert.DeserializeObject<Foods>(item.Value.ToString()));
                }
            }

            return View(list);
        }
    }
}