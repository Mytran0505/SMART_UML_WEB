using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SMART_UML_WEB.Models;

namespace SMART_UML_WEB.Controllers
{
    public class TextToERDController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProcessTextToERD(string paragraph)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //POST Method
            var sampleValue = new InputText(paragraph: paragraph);
            var sampleValueJson = JsonSerializer.Serialize(sampleValue);

            var stringData = new StringContent(sampleValueJson, Encoding.UTF8, @"application/json");

            var postWithBodyResponse = await client.PostAsync("process-paragraph", stringData);

            string result = await postWithBodyResponse.Content.ReadAsStringAsync();
            //TempData["result"] = result;

            //return RedirectToAction("TextToERDResult");
            return Json(result);
        }

        public IActionResult TextToERDResult()
        {
            return View();
        }
    }
}
