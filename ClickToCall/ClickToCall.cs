using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace ClickToCall
{
    public class ClickToCall
    {

        public bool Send(NetworkCredential credentials, IPAddress ip, string command)
        {
            bool result;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                            string.Format("http://{0}/CGI/Execute", ip));
            request.Timeout = 30 * 1000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Credentials = credentials;

            //request.UseDefaultCredentials = true;
            request.PreAuthenticate = true;
            byte[] bytes = Encoding.UTF8.GetBytes(@"XML=" + HttpUtility.UrlEncode(command));

            var outStream = request.GetRequestStream();
            outStream.Write(bytes, 0, bytes.Length);
            outStream.Close();

            WebResponse response = request.GetResponse();

            var nodes = XElement.Load(response.GetResponseStream());

            if (nodes.Element("ResponseItem").Attribute("Status").Value != "0")
            {
                throw new CiscoException(nodes.Element("ResponseItem").Attribute("Data").Value,
                                        int.Parse(nodes.Element("ResponseItem").Attribute("Status").Value));
            }
            else
            {
                result = true;
            }

            response.Close();

            return result;

        }
    }
}
