using libphonenumber;
using OfficeCiscoDialer_ExcelAddIn.Properties;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace OfficeCiscoDialer_ExcelAddIn
{
    public partial class Ribbon : Microsoft.Office.Core.IRibbonExtensibility
    {
        public Ribbon()
        {
            if (CheckForSettings())
            {
                InitilizeCredential();
            }

            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Password")
            {
                InitilizeCredential();
            }
        }

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

        private bool CheckForSettings()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(EncodedPassword) &&
                !string.IsNullOrWhiteSpace(PhoneIP);            
        }

        private bool InitilizeCredential()
        {
            var result = CheckForSettings();
            if (result)
            {
                try
                {
                    _credential = new NetworkCredential(Username, Decode(EncodedPassword));                   
                }
                catch (Exception)
                {

                    result = false;
                }
            }
            return result;
        }

        private void CheckAndCall(Func<bool> func)
        {
            if (_credential != null)
            {
                var ping = new Ping();
                var pingReply = ping.Send(IPAddress.Parse(PhoneIP));
                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    if (!func.Invoke())
                    {
                        MessageBox.Show(@"Failed");
                    }
                }
                else
                {
                    MessageBox.Show($@"Unable to reach {PhoneIP}!");
                }
            }
            else
            {
                MessageBox.Show(@"Please make sure you have entered a 'Username', 'Password', and 'PhoneIP'.", @"Error");
            }
        }

        private bool IsValidNumber(string selection)
        {
            var result = false;
            if (selection != null)
            {
                var phoneUtil = PhoneNumberUtil.Instance;
                var phoneNumberString = PhoneNumberUtil.NormalizeDigitsOnly(selection);
                var nb = phoneUtil.Parse(phoneNumberString, "US");
                result = nb.IsValidNumber;
            }
            return result;
        }
    }
}
