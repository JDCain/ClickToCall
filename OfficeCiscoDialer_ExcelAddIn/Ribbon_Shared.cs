using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OfficeCiscoDialer_ExcelAddIn.Properties;

namespace OfficeCiscoDialer_ExcelAddIn
{
    public partial class Ribbon : Microsoft.Office.Core.IRibbonExtensibility
    {

        private string EncodedPassword
        {
            get { return Properties.Settings.Default.Password; }
            set
            {
                Settings.Default.Password = value;
                Settings.Default.Save();
            }
        }

        private string Username
        {
            get { return Properties.Settings.Default.Username; }
            set
            {
                Settings.Default.Username = value;
                Settings.Default.Save();
            }
        }

        private string PhoneIP
        {
            get { return Properties.Settings.Default.PhoneIP; }
            set
            {
                Settings.Default.PhoneIP = value;
                Settings.Default.Save();
            }
        }

        private ClickToCall.Commands _clickToCall = new ClickToCall.Commands();

        private NetworkCredential _credential;
        

    }
}
