using KRing.Core;
using KRing.Persistence.Interfaces;
using KRing.Persistence.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class NewPasswordForm : Form
    {
        private readonly IDbEntryRepository _passwordRep;
        private readonly UpdateListCallback _callback;

        //TODO: add choices, some websites only allow certain sizes? Should be multiple of 3 so there is no padding (== looks ugly)
        private readonly int passwordSize = 12;

        public NewPasswordForm(IDbEntryRepository repository, UpdateListCallback callback)
        {
            InitializeComponent();
            _passwordRep = repository;
            _callback = callback;
            
            passwordBox.Text = Convert.ToBase64String(CryptoHashing.GenerateSalt(passwordSize));
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if(domainBox.Text == null || domainBox.Text == String.Empty)
            {
                Program._messageToUser("Please enter domain");
                return;
            }

            var password = passwordBox.Text.ToCharArray();

            var securePassword = new SecureString();
            foreach(var c in password)
            {
                securePassword.AppendChar(c);
            }

            var dbEntry = new DBEntry(domainBox.Text, securePassword);

            _passwordRep.AddEntry(dbEntry);

            _callback();

            this.Close();
        }

        private void domainBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
