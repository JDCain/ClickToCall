using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace OfficeCiscoDialer_ExcelAddIn
{
    public partial class Ribbon : Microsoft.Office.Core.IRibbonExtensibility
    {
        public string GetUsername(Microsoft.Office.Core.IRibbonControl control)
        {
            return Username;
        }

        public string GetPhoneIP(Microsoft.Office.Core.IRibbonControl control)
        {
            return PhoneIP;
        }

        public void Username_TextChanged(Microsoft.Office.Core.IRibbonControl control, string text)
        {
            Username = text;           
        }

        public void Password_TextChanged(Microsoft.Office.Core.IRibbonControl control, string text)
        {
            EncodedPassword = Encode(text);
            ribbon.InvalidateControl("passWord");
        }

        private string Encode(string input)
        {
            var plaintext = Encoding.UTF8.GetBytes(input);

            // Generate additional entropy (will be used as the Initialization vector)
            //var entropy = new byte[20];
            //using (var rng = new RNGCryptoServiceProvider())
            //{
            //    rng.GetBytes(entropy);
            //}

            var ciphertext = ProtectedData.Protect(plaintext, null,
                DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(ciphertext);
        }

        private SecureString Decode(string encoded)
        {
            var unencoded = System.Convert.FromBase64String(encoded);
            var ss = new SecureString();
            var x = ProtectedData.Unprotect(unencoded, null, DataProtectionScope.CurrentUser);
            foreach (var c in Encoding.UTF8.GetChars(x))
            {
                ss.AppendChar(c);
            }
            return ss;
        }
        public void PhoneIP_TextChanged(Microsoft.Office.Core.IRibbonControl control, string text)
        {
            PhoneIP = text;
        }

        public void TestSettings(Microsoft.Office.Core.IRibbonControl control)
        {
            Func<bool> func = () => _clickToCall.RingTest(_credential, IPAddress.Parse(PhoneIP));
            CheckAndCall(func);
        }

        public string GetPassword(Microsoft.Office.Core.IRibbonControl control)
        {
            return !string.IsNullOrWhiteSpace(EncodedPassword) ? "**********" : string.Empty;
        }

    }
}

