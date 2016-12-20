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
        private static readonly string _username = "***REMOVED***";
        private static readonly string _password = "";
        private readonly NetworkCredential _credential = new NetworkCredential(_username, _password);
        private readonly IPAddress _ipAddress = IPAddress.Parse("***REMOVED***");
        private string ringXml = @"<CiscoIPPhoneExecute><ExecuteItem Priority='2' URL='Play:Classic1.raw' /></CiscoIPPhoneExecute>";

        [TestMethod()]
        public void SuccessTest()
        {
            var instance  = new Commands();
            var result = instance.SendCommand(_credential, _ipAddress, ringXml);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void BadUsernameTest()
        {
            var instance = new Commands();
            bool result =false;
            try
            {
                result = instance.SendCommand(new NetworkCredential("", ""), _ipAddress, ringXml);
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
                result = instance.MakeCall(_credential, _ipAddress, "9***REMOVED***");
            }
            catch (Exception e)
            {
                result = false;
            }
            Assert.IsTrue(result);
        }
    }
}