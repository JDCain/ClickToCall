using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfficeCiscoDialer_ExcelAddIn
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();
            this.passwordBox.KeyPress += CheckEnterKeyPress;
        }
        private void CheckEnterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)

            {
                Submit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Submit();
        }

        private void Submit()
        {
            Properties.Settings.Default.Password = Encode(passwordBox.Text);
            Properties.Settings.Default.Save();
            this.Close();
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
    }
}
