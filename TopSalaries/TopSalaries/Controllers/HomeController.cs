using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Web.Script.Serialization;

namespace TopSalaries.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CallWebService(int? Year)
        {
            string salariesRequest = CreateRequest(Convert.ToString(Year).Trim());
            XmlDocument salariesResponse = MakeRequest(salariesRequest);
            string[][] salaries = ProcessResponse(salariesResponse);
            JavaScriptSerializer serial = new JavaScriptSerializer();
            var output = serial.Serialize(salaries);
            return Json(output);
        }

        private string[][] ProcessResponse(XmlDocument salariesResponse)
        {
            string[][] salaries = new string[10][];
            XmlNodeList nodes = salariesResponse.SelectNodes("root/salaries/salary");
            int index = 0;
            foreach (XmlNode node in nodes)
            {
                string[] tmp = new string[]{
                    node.Attributes["rank"].Value.ToString(),
                    node.Attributes["player"].Value.ToString(),
                    node.Attributes["position_desc"].Value.ToString(),
                    node.Attributes["salary"].Value.ToString()
                };
                salaries[index] = tmp;
                index++;
            }
            return salaries;
        }

        public static string CreateRequest(string year)
        {
            string UrlRequest = "http://api.usatoday.com/open/salaries/mlb?"
                                + "players="
                                + "&seasons=" + year
                                + "&top=10"
                                + "&api_key=tpm3v8dne2t6g6km4x7tcwdn";
            return (UrlRequest);
        }

        public static XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                Console.Read();
                return null;
            }
        }
    }
}
