using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using OfficeCiscoDialer_ExcelAddIn.Properties;
using Office = Microsoft.Office.Core;
using System.Security.Cryptography;
using libphonenumber;
using Microsoft.Office.Interop.Excel;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace OfficeCiscoDialer_ExcelAddIn
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;
        private string _username;
        private string _passwordEncoded;
        private string _phoneIP;

        public Ribbon()
        {
            _phoneIP = Settings.Default.PhoneIP;
            _username = Settings.Default.Username;
            _passwordEncoded = Settings.Default.Password;
        }

        public string GetUsername(Office.IRibbonControl control)
        {
            return _username;
        }

        public string GetPhoneIP(Office.IRibbonControl control)
        {
            return _phoneIP;
        }

        public void Username_TextChanged(Office.IRibbonControl control, string text)
        {
            Settings.Default.Username = _username = text;
            Settings.Default.Save();
        }

        public void Password_TextChanged(Office.IRibbonControl control, string text)
        {
            Settings.Default.Password = Encode(text);
            ribbon.InvalidateControl("passWord");
            Settings.Default.Save();
            

            //var cred = new NetworkCredential(_username, _password);
            //var test = new ClickToCall.Commands();
            //test.RingTest(cred, IPAddress.Parse(_phoneIP));

            //Settings.Default.Password = text;
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
        public void PhoneIP_TextChanged(Office.IRibbonControl control, string text)
        {
            Settings.Default.PhoneIP = _phoneIP = text;
            Settings.Default.Save();
        }

        public void TestSettings(Office.IRibbonControl control)
        {
            if (!string.IsNullOrWhiteSpace(_passwordEncoded))
            {
                var cred = new NetworkCredential(_username, Decode(_passwordEncoded));
                var test = new ClickToCall.Commands();
                test.RingTest(cred, IPAddress.Parse(_phoneIP));
            }

        }

        public string GetPassword(Office.IRibbonControl control)
        {
            if (!string.IsNullOrWhiteSpace(_passwordEncoded))
            {
                return "******";
            }
            return "";

        }

        public bool IsVisible(Office.IRibbonControl control)
        {
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.Instance;
            var phoneNumberString = PhoneNumberUtil.NormalizeDigitsOnly(GetSelection());
            var nb = phoneUtil.Parse(phoneNumberString, "US");
            
            return nb.IsValidNumber;
        }

        public void DialNumber(Office.IRibbonControl control)
        {
            if (!string.IsNullOrWhiteSpace(_passwordEncoded))
            {
                var cred = new NetworkCredential(_username, Decode(_passwordEncoded));
                var test = new ClickToCall.Commands();
                test.MakeCall(cred, IPAddress.Parse(_phoneIP), GetNumber());
            }
        }

        private string GetNumber()
        {
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.Instance;
            var phoneNumberString = PhoneNumberUtil.NormalizeDigitsOnly(GetSelection());
            var nb = phoneUtil.Parse(phoneNumberString, "US");
            return $"91{nb.NationalNumber}";
        }

        private string GetSelection()
        {
            var range = Globals.ThisAddIn.Application.Selection as Range;
            if (range?.Cells.Count == 1
                && range?.Cells[1, 1].Value != null)
            {
                var x = range?.Cells[1, 1].Value;
                return x?.ToString();
            }
            return null;
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("OfficeCiscoDialer_ExcelAddIn.Ribbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
