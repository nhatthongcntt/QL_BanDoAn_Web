using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QL_BanDoAn.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QL_BanDoAn.Controllers
{
    public class CategoryController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "zBat9CHGhKLYcr9BcldeMq4i0dS2rMxFhd3w36h3",
            BasePath = "https://qlbandoan-6f252-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        // GET: Category
        public ActionResult Cat()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Category");
            JToken data = JToken.Parse(response.Body);
            var list = new List<Category>();

            if (data.Type == JTokenType.Array)
            {
                foreach (JToken item in data.ToObject<JArray>())
                {
                    list.Add(item.ToObject<Category>());
                }
            }
            else if (data.Type == JTokenType.Object)
            {
                foreach (JProperty item in ((JObject)data).Properties())
                {
                    list.Add(JsonConvert.DeserializeObject<Category>(item.Value.ToString()));
                }
            }

            return View(list);
        }
        public ActionResult Edit(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get($"Category/{id}");
            var category = JsonConvert.DeserializeObject<Category>(response.Body);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set($"Category/{category.Id}", category);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return RedirectToAction("Cat");
                }
            }

            return View(category);
        }



    }

}