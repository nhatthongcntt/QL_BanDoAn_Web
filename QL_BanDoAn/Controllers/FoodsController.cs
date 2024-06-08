using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using QL_BanDoAn.Models;
using System.IO;
using Firebase.Auth;
using System.Threading;
using Firebase.Storage;
using System.Threading.Tasks;


namespace QL_BanDoAn.Controllers
{
    public class FoodsController : Controller
    {
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
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
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.Body);
            var list = new List<Foods>();

            foreach (var item in data)
            {
                var food = JsonConvert.DeserializeObject<Foods>(JsonConvert.SerializeObject(item));
                list.Add(food);
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult Detail(String id = "0")
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Foods/" + id);
            Foods data = JsonConvert.DeserializeObject<Foods>(response.Body);

            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }

        private static string ApiKey = "AIzaSyDcxHeWsgIx4xehIBhNjGi6VfEhhZvXIA0";
        private static string Bucket = "qlbandoan-6f252.appspot.com";
        private static string AuthEmail = "kimgiau@gmail.com";
        private static string AuthPassword = "123456@";

        [HttpPost]
        public async Task<ActionResult> Create(Foods foods, HttpPostedFileBase file)
        {
            FileStream stream;
            if (file.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Content/images/"), file.FileName);
                file.SaveAs(path);
                stream = new FileStream(Path.Combine(path), FileMode.Open);
                foods.ImagePath = await Upload(stream, file.FileName);
            }

            try
            {
                int latestId = await GetLatestFoodId();
                foods.Id = latestId + 1;
                await addFoodToFirebase(foods);
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            
            return View();
        }

        private async Task<int> GetLatestFoodId()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Foods");
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.Body);
            var list = new List<Foods>();
            var idMax = 0;
            foreach (var item in data)
            {
                var food = JsonConvert.DeserializeObject<Foods>(JsonConvert.SerializeObject(item));
                if (food.Id > idMax)
                    idMax = food.Id;
            }
            return idMax;
        }


        private async Task addFoodToFirebase(Foods foods)
        {
            client = new FireSharp.FirebaseClient(config);
            await client.SetAsync($"Foods/{foods.Id}", foods);
        }

        public async Task<string> Upload(FileStream stream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            try
            {
                string link = await task;
                return link;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
            }
            return "";
        }

        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Foods/"+id);
            Foods data = response.ResultAs<Foods>();

            return View(data);
        }

        [HttpPost]
        public ActionResult Delete(Foods foods)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("/Foods/" + foods.Id);
            return RedirectToAction("Index");
        }
        public async Task UpdateFoodCategoryId(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Foods");
                var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.Body);
                var list = new List<Foods>();

                // Cập nhật các món ăn có CategoryId trùng với id thành 7
                foreach (var item in data)
                {
                    var food = JsonConvert.DeserializeObject<Foods>(JsonConvert.SerializeObject(item));
                    if (food.CategoryId.ToString() == id)
                    {
                        food.CategoryId = 7;
                        await client.UpdateAsync($"Foods/{food.Id}", food);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu cần
            }
        }


        public ActionResult GetFoodByCategoryId(int id)
        {
           
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Foods");
                var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.Body);
                var list = new List<Foods>();

                
                foreach (var item in data)
                {
                    var food = JsonConvert.DeserializeObject<Foods>(JsonConvert.SerializeObject(item));
                    if(food.CategoryId==id)
                        list.Add(food);
                }

            return View(list);
           
        }
    }
}