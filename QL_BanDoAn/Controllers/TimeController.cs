using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QL_BanDoAn.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QL_BanDoAn.Controllers
{
    public class TimeController : Controller
    {

        // GET: Home
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "zBat9CHGhKLYcr9BcldeMq4i0dS2rMxFhd3w36h3",
            BasePath = "https://qlbandoan-6f252-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Time");
            var data = JsonConvert.DeserializeObject<List<Time>>(response.Body); // Đảm bảo dữ liệu được deserialize thành danh sách các đối tượng Time
            var list = new List<Time>();

            foreach (var item in data)
            {
                var time = JsonConvert.DeserializeObject<Time>(JsonConvert.SerializeObject(item));
                list.Add(time);
            }

            return View(list); // Trả về danh sách các đối tượng Time
        }

        public ActionResult Create()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Category");
            var data = JsonConvert.DeserializeObject<List<Category>>(response.Body);
            var list = new List<Category>();

            foreach (var item in data)
            {
                var cate = JsonConvert.DeserializeObject<Category>(JsonConvert.SerializeObject(item));
                list.Add(cate);
            }

            ViewBag.lstCate = list;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Time time)
        {
            try
            {
                int latestId = await GetLatestTimeId();
                time.Id = latestId + 1;
                await addTimeToFirebase(time);
                ModelState.AddModelError(string.Empty, "Added Successfully");
                return View(time);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(time);
            }

            return View();
        }
        private async Task<int> GetLatestTimeId()
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Time");
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.Body);
            var list = new List<Category>();
            var idMax = 0;
            if (response.Body == "null")
            {
                return 0;
            }
            else
            {

                foreach (var item in data)
                {
                    var cat = JsonConvert.DeserializeObject<Category>(JsonConvert.SerializeObject(item));
                    if (cat != null)
                    {
                        if (cat.Id > idMax)
                            idMax = cat.Id;
                    }
                }

            }

            return idMax;
        }
        private async Task addTimeToFirebase(Time time)
        {
            client = new FireSharp.FirebaseClient(config);
            await client.SetAsync($"Time/{time.Id}", time);
        }
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Time/" + id);
            Time data = response.ResultAs<Time>();

            return View(data);
        }

        [HttpPost]
        public ActionResult Delete(Time time)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("/Time/" + time.Id);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get($"Time/{id}");
            var time = JsonConvert.DeserializeObject<Time>(response.Body);

            if (time == null)
            {
                return HttpNotFound();
            }

            return View(time);
        }
        [HttpPost]
        public ActionResult Edit(Time time)
        {
            if (ModelState.IsValid)
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set($"Time/{time.Id}", time);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(time);
        }
    }
}
    




