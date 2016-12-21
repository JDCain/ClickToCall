using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickToCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security;
using System.Security.Cryptography;

namespace ClickToCallTests
{
    [TestClass()]
    public class ClickToCallTests
    {
        private static readonly string _username = "***REMOVED***";
        private static readonly string _password = "";
        private readonly NetworkCredential _credential = new NetworkCredential(_username, _password);
        private readonly IPAddress _ipAddress = IPAddress.Parse("***REMOVED***");
        private string ringXml = @"<CiscoIPPhoneExecute><ExecuteItem Priority='2' URL='Play:Classic1.raw' /></CiscoIPPhoneExecute>";

        [TestMethod]
        public void SecureStorageTest()
        {
            // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
            byte[] plaintext = Encoding.UTF8.GetBytes("TeSt");

            // Generate additional entropy (will be used as the Initialization vector)
            //byte[] entropy = new byte[20];
            //using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            //{
            //    rng.GetBytes(entropy);
            //}

            byte[] ciphertext = ProtectedData.Protect(plaintext, null,
                DataProtectionScope.CurrentUser);

            string encoded = System.Convert.ToBase64String(ciphertext);

            byte[] unencoded = System.Convert.FromBase64String(encoded);

            Assert.IsTrue(ciphertext.SequenceEqual(unencoded));

            byte[] plainbytetout = ProtectedData.Unprotect(unencoded, null, DataProtectionScope.CurrentUser);

            


            var ss = new SecureString();
            foreach (var c in Encoding.UTF8.GetChars(plainbytetout))
            {
                ss.AppendChar(c);
            }
        }

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