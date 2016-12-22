using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace OfficeCiscoDialer_ExcelAddIn
{
    public partial class Ribbon : Microsoft.Office.Core.IRibbonExtensibility
    {
        public void PasswordDialog(Microsoft.Office.Core.IRibbonControl control)
        {
            var passForm = new PasswordForm();
            var result = passForm.ShowDialog();
            
        }
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

        //public void Password_TextChanged(Microsoft.Office.Core.IRibbonControl control, string text)
        //{
        //    EncodedPassword = Encode(text);
        //    ribbon.InvalidateControl("passWord");
        //}

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

