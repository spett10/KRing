namespace KRingForm
{
    partial class PasswordList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.passwordListBox = new System.Windows.Forms.ListBox();
            this.listLabel = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.viewButton = new System.Windows.Forms.Button();
            this.deleteUserButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.buttonToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // passwordListBox
            // 
            this.passwordListBox.FormattingEnabled = true;
            this.passwordListBox.Location = new System.Drawing.Point(22, 48);
            this.passwordListBox.Name = "passwordListBox";
            this.passwordListBox.Size = new System.Drawing.Size(168, 316);
            this.passwordListBox.TabIndex = 0;
            this.passwordListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // listLabel
            // 
            this.listLabel.AutoSize = true;
            this.listLabel.Location = new System.Drawing.Point(19, 32);
            this.listLabel.Name = "listLabel";
            this.listLabel.Size = new System.Drawing.Size(92, 13);
            this.listLabel.TabIndex = 1;
            this.listLabel.Text = "Stored Passwords";
            this.buttonToolTips.SetToolTip(this.listLabel, "Select a stored password by clicking it in the below list, and manipulate it usin" +
        "g the buttons to the right.");
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(196, 48);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.buttonToolTips.SetToolTip(this.addButton, "Enables you to add a password, where you yourself can enter the password. We reco" +
        "mmend the \"New\" button for new passwords, whereas the \"Add\" button should be use" +
        "d to add existing passwords to storage.");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(196, 106);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 3;
            this.editButton.Text = "Edit";
            this.buttonToolTips.SetToolTip(this.editButton, "Enables you to edit the selected password and/or its domain.");
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(196, 135);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 4;
            this.deleteButton.Text = "Delete";
            this.buttonToolTips.SetToolTip(this.deleteButton, "Delete the selected password.");
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // viewButton
            // 
            this.viewButton.Location = new System.Drawing.Point(196, 77);
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new System.Drawing.Size(75, 23);
            this.viewButton.TabIndex = 0;
            this.viewButton.Text = "View";
            this.buttonToolTips.SetToolTip(this.viewButton, "View the details of the selected password. Enables you to copy the password to yo" +
        "ur clipboard, or reveal it to you on the screen.");
            this.viewButton.UseVisualStyleBackColor = true;
            this.viewButton.Click += new System.EventHandler(this.viewButton_Click);
            // 
            // deleteUserButton
            // 
            this.deleteUserButton.Enabled = false;
            this.deleteUserButton.Location = new System.Drawing.Point(196, 341);
            this.deleteUserButton.Name = "deleteUserButton";
            this.deleteUserButton.Size = new System.Drawing.Size(75, 23);
            this.deleteUserButton.TabIndex = 5;
            this.deleteUserButton.Text = "Delete User";
            this.buttonToolTips.SetToolTip(this.deleteUserButton, "Delete your user and all associated passwords. Recovery is not possible, so apply" +
        " with extreme care.");
            this.deleteUserButton.UseVisualStyleBackColor = true;
            this.deleteUserButton.Click += new System.EventHandler(this.deleteUserButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(196, 227);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.buttonToolTips.SetToolTip(this.saveButton, "Save your changes to the password list to the underlying encrypted storage.");
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // buttonToolTips
            // 
            this.buttonToolTips.ToolTipTitle = "New";
            // 
            // PasswordList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 412);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.deleteUserButton);
            this.Controls.Add(this.viewButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.listLabel);
            this.Controls.Add(this.passwordListBox);
            this.Name = "PasswordList";
            this.Text = "PasswordList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PasswordList_FormClosing);
            this.Load += new System.EventHandler(this.PasswordList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox passwordListBox;
        private System.Windows.Forms.Label listLabel;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button viewButton;
        private System.Windows.Forms.Button deleteUserButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ToolTip buttonToolTips;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}