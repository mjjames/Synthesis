using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml;
using System.Net;
/// <summary>
/// Generic mjjames Functions
/// </summary>
/// 
namespace mjjames
{
    /// <summary>
    /// generic Functions
    /// </summary>
    public class genericFunction
    {

        /// <summary>
        /// Function for extracting specific amount of content from an input. Strips all HTML Tags
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="wordNum">How Many Words Is Returned</param>
        /// <returns>String of extracted Words</returns>
        public string extractWords(string input, int wordNum)
        {
            string output = "";
            int i;
            int maxi;
            input = Regex.Replace(input, @"<[^>]*>", string.Empty);
            Array words = input.Split(new char[] { ' ' });
            if (words.Length < 20)
            {
                maxi = words.Length;
            }
            else
            {
                maxi = wordNum;
            }
            for (i = 0; i < maxi; i++)
            {
                output += words.GetValue(i) + " ";
            }

            output += "...";

            return output;
        }

        public DateTime GetHoursOffsetFromUtc(DateTime utcDateTime)
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;

            // Create a DaylightTime object for the specified year.
            DaylightTime daylight = localZone.GetDaylightChanges(DateTime.UtcNow.Year);

            int timeOffset = 0;

            if (localZone.IsDaylightSavingTime(DateTime.Now))
            {
                timeOffset += daylight.Delta.Hours;
            }

            utcDateTime = utcDateTime.ToUniversalTime().AddHours(timeOffset);
            return utcDateTime;
        }

        public string GetGeoCodedAddress(string address)
        {
            string geocodedAddress;

            // Create a new XmlDocument  
            XmlDocument doc = new XmlDocument();  
               
            // Load data  
            // need to web.config the api key
            doc.Load("http://local.yahooapis.com/MapsService/V1/geocode?appid=02p6cNjV34Fl7CkGOrYkTNvzgnLTC2N3sajX6471LGYhMSUXcDUCkLkaxCYNJ7mZoQ&street=" + address);  
               
            // Set up namespace manager for XPath  
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("urn:yahoo:maps", "http://local.yahooapis.com/MapsService/V1/GeocodeResponse.xsd");
            geocodedAddress = doc.InnerXml.ToString();
            // Get forecast with XPath  
            XmlNode latitude = doc.SelectSingleNode("//ResultSet/Result/Latitude", ns);  
            XmlNode longitude = doc.SelectSingleNode("//ResultSet/Result/Longitude", ns);

            geocodedAddress = latitude.Value.ToString() + "," + longitude.Value.ToString();
            return geocodedAddress;
        }

        public void Restart()
        {

            System.Web.HttpRuntime.UnloadAppDomain();
            WebClient myWebClient = null;
            try
            {
                string url = "~/Default.aspx";
                myWebClient = new WebClient();
                byte[] stuff = myWebClient.DownloadData(url);
            }
            catch
            {
            }
            finally
            {
                myWebClient.Dispose();
            }
        }
    }
}