using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickToCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ClickToCall.Tests
{
    [TestClass()]
    public class ClickToCallTests
    {
        static string _username = "Jerry.cain";
        static string _password = "Eial.Amtatm.";
        NetworkCredential credential = new NetworkCredential(_username, _password);
        IPAddress ipAddress = IPAddress.Parse("10.8.65.11");
        string ringXml = @"<CiscoIPPhoneExecute><ExecuteItem Priority='2' URL='Play:Classic1.raw' /></CiscoIPPhoneExecute>";

        [TestMethod()]
        public void SuccessTest()
        {
            var instance  = new Commands();
            var result = instance.SendCommand(credential, ipAddress, ringXml);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void BadUsernameTest()
        {
            var instance = new Commands();
            bool result =false;
            try
            {
                result = instance.SendCommand(new NetworkCredential("", ""), ipAddress, ringXml);
            }
            catch (Exception e)
            {
                if(e.Message.Contains("401"))
                {
                    result = true;
                }
            }                  
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DialTest()
        {
            var instance = new Commands();
            bool result = false;
            try
            {
                result = instance.MakeCall(credential, ipAddress, "***REMOVED***");
            }
            catch (Exception e)
            {

            }
            Assert.IsTrue(result);
        }
    }
}