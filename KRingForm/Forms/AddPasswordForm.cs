using KRing.Persistence.Interfaces;
using KRing.Persistence.Model;
using KRing.Extensions;
using KRing.Persistence.Repositories;
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
    public partial class AddPasswordForm : Form
    {
        private readonly IDbEntryRepository _passwordRep;
        private readonly UpdateListCallback _callback;

        public AddPasswordForm(IDbEntryRepository repository, UpdateListCallback callback)
        {
            InitializeComponent();
            _passwordRep = repository;
            _callback = callback;

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var domainName = domainBox.Text;
            var plaintextPassword = passwordBox.Text;
            
            if(domainName == null || plaintextPassword == null)
            {
                MessageBox.Show("Please enter Domain and Password", "Error");
            }
            else
            {
                if(_passwordRep.ExistsEntry(domainName))
                {
                    MessageBox.Show("Domain already exists in stored passwords", "Error");
                }
                else
                {
                    var password = new SecureString();
                    password.PopulateWithString(plaintextPassword);
                    var dbEntry = new DBEntry(domainName, password);

                    _passwordRep.AddEntry(dbEntry);
                    
                    _callback();

                    this.Close();
                }                
            }
        }
    }
}
