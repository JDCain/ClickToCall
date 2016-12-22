using libphonenumber;
using Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;

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
    public partial class Ribbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public bool IsVisible(Office.IRibbonControl control)
        {
            return IsValidNumber(GetSelection());          
        }

        

        public void DialNumber(Office.IRibbonControl control)
        {
            CheckAndCall(()=>_clickToCall.MakeCall(_credential, IPAddress.Parse(PhoneIP), GetNumber()));
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
