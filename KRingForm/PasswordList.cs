using KRing.Core.Model;
using KRing.Persistence.Interfaces;
using KRing.Persistence.Repositories;
using KRingForm.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KRingForm
{
    public partial class PasswordList : Form
    {
        public delegate void UpdateListCallback();

        private readonly User _user;
        private readonly IDbEntryRepository _passwordRep;


        public PasswordList(User user)
        {
            InitializeComponent();
            _user = user;

            _passwordRep = new DbEntryRepository(_user.Password);

            UpdateList();
        }

        /* TODO: make async? */
        public void UpdateList()
        {
            passwordListBox.Items.Clear();

            var passwords = _passwordRep.GetEntries();

            foreach (var pswd in passwords)
            {
                passwordListBox.Items.Add(pswd.Domain);
            }

            _passwordRep.WriteEntriesToDb();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var addForm = new AddPasswordForm(_passwordRep, UpdateList);
            addForm.Show();
        }


    }
}
