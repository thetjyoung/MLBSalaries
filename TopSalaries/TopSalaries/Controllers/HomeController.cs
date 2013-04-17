using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Xml;

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
            string locationsRequest = CreateRequest(Convert.ToString(Year).Trim());
            XmlDocument locationsResponse = MakeRequest(locationsRequest);
            //process response
            return View();
        }

        public static string CreateRequest(string year)
        {
            string UrlRequest = "http://api.usatoday.com/open/salaries/mlb?"
                                + " year=" + year.Trim()
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
