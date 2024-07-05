using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SMART_UML_WEB.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace SMART_UML_WEB.Controllers
{
    public class TextToERDController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProcessTextToUML(string paragraph)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //POST Method
            var sampleValue = new InputText(paragraph: paragraph);
            var sampleValueJson = JsonConvert.SerializeObject(sampleValue);

            var stringData = new StringContent(sampleValueJson, Encoding.UTF8, @"application/json");

            var postWithBodyResponse = await client.PostAsync("process-paragraph", stringData);

            string result = await postWithBodyResponse.Content.ReadAsStringAsync();

            //Deserialize with NewtonSoft.Json
            var umlResult = JsonConvert.DeserializeObject<TextToERDResult>(result);
            var uml = umlResult.uml;

            return Json(uml);
        }

        public async Task<IActionResult> ProcessTextToDDL(string paragraph)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //POST Method
            var sampleValue = new InputText(paragraph: paragraph);
            //Serialize with NewtonSoft.Json
            var sampleValueJson = JsonConvert.SerializeObject(sampleValue);

            var stringData = new StringContent(sampleValueJson, Encoding.UTF8, @"application/json");

            var postWithBodyResponse = await client.PostAsync("process-paragraph", stringData);

            string result = await postWithBodyResponse.Content.ReadAsStringAsync();

            //Deserialize with NewtonSoft.Json
            var umlResult = JsonConvert.DeserializeObject<TextToERDResult>(result);
            var sql = umlResult.sql;

            return Json(sql);
        }

        public async Task<IActionResult> ProcessUMLTextToImage(string paragraph)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //POST Method
            var sampleValue = new InputText(paragraph: paragraph);
            var sampleValueJson = JsonConvert.SerializeObject(sampleValue);

            var stringData = new StringContent(sampleValueJson, Encoding.UTF8, @"application/json");

            var postWithBodyResponse = await client.PostAsync("process-paragraph", stringData);

            string result = await postWithBodyResponse.Content.ReadAsStringAsync();

            //Deserialize with NewtonSoft.Json
            var umlResult = JsonConvert.DeserializeObject<TextToERDResult>(result);
            var umlText = umlResult.uml;

            byte[] bytes = Encoding.UTF8.GetBytes(umlText);

            string umlTextInHexString = Convert.ToHexString(bytes);

            return Json(umlTextInHexString);
        }

        public async Task<IActionResult> ProcessPlantUMLInputTextToImage(string umlInputText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(umlInputText);

            string umlTextInHexString = Convert.ToHexString(bytes);

            return Json(umlTextInHexString);
        }

        public IActionResult TextToERDResult()
        {
            return View();
        }
    }
}
