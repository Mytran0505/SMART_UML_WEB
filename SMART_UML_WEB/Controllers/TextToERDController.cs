using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SMART_UML_WEB.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

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

        public IActionResult ProcessPlantUMLInputTextToDDL(string umlInputText)
        {
            DDLSupport _ddlSupport = new DDLSupport();
            // Split the input into lines
            string[] lines = umlInputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Dictionaries to store entities, primary keys, and foreign keys
            var entities = new Dictionary<string, List<string>>();
            var primaryKeys = new Dictionary<string, string>();
            var foreignKeys = new List<Tuple<string, string, string, string>>();

            string currentEntity = null;

            // Process each line of the UML script
            foreach (var scriptLine in lines)
            {
                var entityMatch = Regex.Match(scriptLine, @"entity\s+""(.+)""\s+as\s+(\w+)\s*\{");
                var attributeMatch = Regex.Match(scriptLine, @"^\s*[\*\+]\s*(\w+)");
                var relationMatch = Regex.Match(scriptLine, @"(\w+)\s*\|\w*\{\s*(\w+)");

                if (entityMatch.Success)
                {
                    currentEntity = entityMatch.Groups[2].Value;
                    entities[currentEntity] = new List<string>();
                }
                else if (attributeMatch.Success && currentEntity != null)
                {
                    entities[currentEntity].Add(attributeMatch.Groups[1].Value);
                    if (attributeMatch.Groups[0].ToString().StartsWith("*"))
                    {
                        primaryKeys[currentEntity] = attributeMatch.Groups[1].Value;
                    }
                }
                else if (relationMatch.Success)
                {
                    string parentEntity = relationMatch.Groups[1].Value;
                    string childEntity = relationMatch.Groups[2].Value;

                    if (entities.ContainsKey(parentEntity) && entities.ContainsKey(childEntity))
                    {
                        string parentKey = primaryKeys[parentEntity];
                        foreignKeys.Add(Tuple.Create(childEntity, parentKey, parentEntity, parentKey));
                    }
                }
            }

            // StringBuilder to construct the DDL statements
            var ddl = new StringBuilder();

            // Generate DDL for each entity
            foreach (var entity in entities)
            {
                ddl.AppendLine($"CREATE TABLE {entity.Key} (");
                foreach (var attribute in entity.Value)
                {
                    var dataType = _ddlSupport.GetSqlDataType(attribute);
                    ddl.AppendLine($"    {attribute} {dataType},");
                }

                if (primaryKeys.ContainsKey(entity.Key))
                {
                    ddl.AppendLine($"    CONSTRAINT PK_{entity.Key} PRIMARY KEY({primaryKeys[entity.Key]})");
                }

                ddl.AppendLine(");");
                ddl.AppendLine();
            }

            var ddlResult = ddl.ToString();

            return Json(ddlResult);
        }

        public IActionResult TextToERDResult()
        {
            return View();
        }
    }
}
