using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using QL_BanDoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QL_BanDoAn.Controllers
{
    public class PricesController : Controller
    {
        // GET: Prices
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "zBat9CHGhKLYcr9BcldeMq4i0dS2rMxFhd3w36h3",
            BasePath = "https://qlbandoan-6f252-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Price");
            var data = JsonConvert.DeserializeObject<List<Price>>(response.Body);
            var list = new List<Price>();

            foreach (var item in data)
            {
                var price = JsonConvert.DeserializeObject<Price>(JsonConvert.SerializeObject(item));
                list.Add(price);
            }

            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Price price)
        {
            try
            {
                int latestId = await GetLatestPriceId();
                price.Id = latestId + 1;
                await AddPriceToFirebase(price);
                ModelState.AddModelError(string.Empty, "Added Successfully");
                return View(price);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(price);
            }
        }

        private async Task<int> GetLatestPriceId()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Price");
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.Body);
            var idMax = 0;

            if (response.Body == "null")
            {
                return 0;
            }
            else
            {
                foreach (var item in data)
                {
                    var price = JsonConvert.DeserializeObject<Price>(JsonConvert.SerializeObject(item));
                    if (price != null)
                    {
                        if (price.Id > idMax)
                            idMax = price.Id;
                    }
                }
            }

            return idMax;
        }

        private async Task AddPriceToFirebase(Price price)
        {
            client = new FireSharp.FirebaseClient(config);
            await client.SetAsync($"Price/{price.Id}", price);
        }

        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Price/" + id);
            Price data = response.ResultAs<Price>();

            return View(data);
        }

        [HttpPost]
        public ActionResult Delete(Price price)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("/Price/" + price.Id);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get($"Price/{id}");
            var price = JsonConvert.DeserializeObject<Price>(response.Body);

            if (price == null)
            {
                return HttpNotFound();
            }

            return View(price);
        }

        [HttpPost]
        public ActionResult Edit(Price price)
        {
            if (ModelState.IsValid)
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set($"Price/{price.Id}", price);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(price);
        }
    }
}
