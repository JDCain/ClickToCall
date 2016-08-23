﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickToCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickToCall.Tests
{
    [TestClass()]
    public class ClickToCallTests
    {
        string username = "";
        string password = "";
        string ipAddress = "10.8.65.11";
        string ringXml = @"<CiscoIPPhoneExecute><ExecuteItem Priority='2' URL='Play:Classic1.raw' /></CiscoIPPhoneExecute>";

        [TestMethod()]
        public void SuccessTest()
        {
            var instance  = new ClickToCall();
            var result = instance.Send(new System.Net.NetworkCredential(username, password), 
                            System.Net.IPAddress.Parse(ipAddress), 
                            ringXml);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void BadUsernameTest()
        {
            var instance = new ClickToCall();
            bool result =false;
            try
            {
                result = instance.Send(new System.Net.NetworkCredential("", ""), System.Net.IPAddress.Parse("10.8.65.11"), @"<CiscoIPPhoneExecute><ExecuteItem Priority='2' URL='Play:Classic1.raw' /></CiscoIPPhoneExecute>");
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
    }
}