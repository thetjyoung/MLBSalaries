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

        /// <summary>
        /// Instigates the web service call with the passed in value and
        /// returns the results to the caller.
        /// </summary>
        /// <param name="Year">int representing desired year for web service query.</param>
        /// <returns>A serialized array of arrays each representing a table row to be added.</returns>
        public ActionResult CallWebService(int? Year)
        {
            string salariesRequest = CreateRequest(Convert.ToString(Year).Trim());
            XmlDocument salariesResponse = MakeRequest(salariesRequest);
            string[][] salaries = ProcessResponse(salariesResponse);
            JavaScriptSerializer serial = new JavaScriptSerializer();
            var output = serial.Serialize(salaries);
            return Json(output);
        }

        /// <summary>
        /// Parses the xml returned from the web service and extracts the 
        /// desired data. Data extracted here must match the schema of the
        /// datatable of the caller.
        /// </summary>
        /// <param name="salariesResponse">XML document returned from web service</param>
        /// <returns>An array of arrays one for each child node in the returned xml</returns>
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

        /// <summary>
        /// Creates an http request string with the desired query criteria
        /// and the user selected year.
        /// </summary>
        /// <param name="year">int representing desired year to query for</param>
        /// <returns>http request string</returns>
        public static string CreateRequest(string year)
        {
            string UrlRequest = "http://api.usatoday.com/open/salaries/mlb?"
                                + "players="
                                + "&seasons=" + year
                                + "&top=10"
                                + "&api_key=tpm3v8dne2t6g6km4x7tcwdn";
            return (UrlRequest);
        }

        /// <summary>
        /// Creates an http request object using the http request string and
        /// initiates the request. Packages the return value as an XMLDocument
        /// and returns to caller.
        /// </summary>
        /// <param name="requestUrl">http request string</param>
        /// <returns>XMLDocument containing the http response</returns>
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
