using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QL_BanDoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using QL_BanDoAn.Controllers;
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
        private async Task<int> GetLatestCategoryId()
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Category");
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
                    if(cat!=null)
                    {    
                        if (cat.Id > idMax)
                            idMax = cat.Id;
                    }
                }

            }
           
            return idMax;
        }
        private async Task addCategoryToFirebase(Category cat)
        {
            client = new FireSharp.FirebaseClient(config);
            await client.SetAsync($"Category/{cat.Id}", cat);
        }
        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Category category)
        {
            try
            {
                int latestId = await GetLatestCategoryId();
                category.Id = latestId + 1;
                await addCategoryToFirebase(category);
                ModelState.AddModelError(string.Empty, "Added Successfully");
                return View(category);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(category);
            }

            return View();
        }

        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Category/" + id);
            Category data = response.ResultAs<Category>();

            return View(data);
        }
        // POST: Category/Delete
        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            
                client = new FireSharp.FirebaseClient(config);

            FoodsController foodsController = new FoodsController();
            await foodsController.UpdateFoodCategoryId(id);
            FirebaseResponse deleteResponse = await client.DeleteAsync("Category/" + id);

                return RedirectToAction("Cat");
           
        }
       







    }

}