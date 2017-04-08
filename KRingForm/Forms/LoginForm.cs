﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using KRing.Core;
using KRing.Extensions;
using KRing.Persistence.Repositories;
using KRing.Core.Model;
using static KRingForm.Program;

namespace KRingForm
{
    public partial class LoginForm : Form
    {
        private User savedUser;
        private readonly LoginCallback _callback;

        private const int maxLoginAttemps = 3;
        private int usedLoginAttempts = 0;

        public LoginForm(User user, LoginCallback callback)
        {
            savedUser = user;
            _callback = callback;
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            var userName = usernameBox.Text;
            var password = passwordBox.Text;

            if(userName == string.Empty || password == string.Empty)
            {
                MessageBox.Show("Please input username and password", "Error");
            }
            else
            {
                var securePassword = new SecureString();
                securePassword.PopulateWithString(password);

                var correctUsername = CheckUserName(userName);

                var correctPassword = CryptoHashing.ScryptCheckPassword(securePassword, savedUser.Cookie.HashSalt, savedUser.Cookie.HashedPassword);

                if (!(correctPassword && correctPassword))
                {
                    HandleFailedLogon();
                }
                else
                {
                    _callback(true);
                    this.Close();
                }
            }            
        }

        private bool CheckUserName(string username)
        {
            return String.Equals(savedUser.UserName, username, StringComparison.CurrentCultureIgnoreCase);
        }

        private void HandleFailedLogon()
        {
            usedLoginAttempts++;
            MessageBox.Show("Wrong username and/or password", "Error");


            if(usedLoginAttempts >= maxLoginAttemps)
            {
                MessageBox.Show("All login attempts used.");
                _callback(false);
                this.Close();
            }
        }
    }
}
