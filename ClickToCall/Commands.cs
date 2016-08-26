using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace ClickToCall
{
    public class Commands
    {
        private bool IsNumbers(string number)
        {
            return Regex.Match(number, @"^[0-9]*$").Success;
        }
        
        /// <summary>
        /// send any raw xml command to phone
        /// </summary>
        /// <param name="credentials">NetworkCredential Type with username and password.</param>
        /// <param name="ip">IP address of device</param>
        /// <param name="command">command which would be after the XML= portion of the string</param>
        /// <returns></returns>
        public bool SendCommand(NetworkCredential credentials, IPAddress ip, string command, int timeout = 30 * 1000)
        {
            bool result;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                            string.Format("http://{0}/CGI/Execute", ip));
            request.Timeout = timeout;
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

        /// <summary>
        /// Send a command causing the phone to play a dial a number if phone is not in use
        /// </summary>
        /// <param name="credentials">NetworkCredential Type with username and password.</param>
        /// <param name="ip">IP address of device</param>
        /// <param name="phoneNumber">10 digit phone number</param>
        /// <returns></returns>
        public bool MakeCall(NetworkCredential credentials, IPAddress ip, string phoneNumber)
        {
            var result = false;
            if(!IsNumbers(phoneNumber))
            {
                throw new NotSupportedException();
            }
            else
            {
                result = SendCommand(credentials, ip, $@"<CiscoIPPhoneExecute><ExecuteItem Priority='3' URL='Dial: {phoneNumber}'/></ CiscoIPPhoneExecute>");
            }

            return result;
        }
        
        /// <summary>
        /// Send a command causing the phone to play a ringtone
        /// </summary>
        /// <param name="credentials">NetworkCredential Type with username and password.</param>
        /// <param name="ip">IP address of device</param>
        /// <returns></returns>
        public bool RingTest(NetworkCredential credentials, IPAddress ip)
        {
            return SendCommand(credentials, ip, @"<CiscoIPPhoneExecute><ExecuteItem Priority='2' URL='Play:Classic1.raw' /></CiscoIPPhoneExecute>");
        }
    }
}
