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
            this.addButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.viewButton = new System.Windows.Forms.Button();
            this.deleteUserButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.buttonToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.inactiveTimer = new System.Windows.Forms.Timer(this.components);
            this.SearchBar = new System.Windows.Forms.TextBox();
            this.SearchBarLabel = new System.Windows.Forms.Label();
            this.SearchButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // passwordListBox
            // 
            this.passwordListBox.FormattingEnabled = true;
            this.passwordListBox.Location = new System.Drawing.Point(12, 48);
            this.passwordListBox.Name = "passwordListBox";
            this.passwordListBox.Size = new System.Drawing.Size(204, 316);
            this.passwordListBox.TabIndex = 0;
            this.passwordListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(221, 76);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Add";
            this.buttonToolTips.SetToolTip(this.addButton, "Enables you to add a password, where you yourself can enter the password, or have" +
        " the program generate a cryptographically random one. We recommend the latter.");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(222, 106);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 4;
            this.editButton.Text = "Edit";
            this.buttonToolTips.SetToolTip(this.editButton, "Enables you to edit the selected password and/or its domain.");
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(222, 135);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 5;
            this.deleteButton.Text = "Delete";
            this.buttonToolTips.SetToolTip(this.deleteButton, "Delete the selected password.");
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // viewButton
            // 
            this.viewButton.Location = new System.Drawing.Point(221, 48);
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new System.Drawing.Size(75, 23);
            this.viewButton.TabIndex = 2;
            this.viewButton.Text = "View";
            this.buttonToolTips.SetToolTip(this.viewButton, "View the details of the selected password. Enables you to copy the password to yo" +
        "ur clipboard, or reveal it to you on the screen.");
            this.viewButton.UseVisualStyleBackColor = true;
            this.viewButton.Click += new System.EventHandler(this.viewButton_Click);
            // 
            // deleteUserButton
            // 
            this.deleteUserButton.Enabled = false;
            this.deleteUserButton.Location = new System.Drawing.Point(222, 341);
            this.deleteUserButton.Name = "deleteUserButton";
            this.deleteUserButton.Size = new System.Drawing.Size(75, 23);
            this.deleteUserButton.TabIndex = 7;
            this.deleteUserButton.Text = "Delete User";
            this.buttonToolTips.SetToolTip(this.deleteUserButton, "Delete your user and all associated passwords. Recovery is not possible, so apply" +
        " with extreme care.");
            this.deleteUserButton.UseVisualStyleBackColor = true;
            this.deleteUserButton.Click += new System.EventHandler(this.deleteUserButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(222, 227);
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
            // inactiveTimer
            // 
            this.inactiveTimer.Tick += new System.EventHandler(this.inactiveTimer_Tick);
            // 
            // SearchBar
            // 
            this.SearchBar.Location = new System.Drawing.Point(12, 19);
            this.SearchBar.MaxLength = 80;
            this.SearchBar.Name = "SearchBar";
            this.SearchBar.Size = new System.Drawing.Size(204, 20);
            this.SearchBar.TabIndex = 0;
            // 
            // SearchBarLabel
            // 
            this.SearchBarLabel.AutoSize = true;
            this.SearchBarLabel.Location = new System.Drawing.Point(15, 6);
            this.SearchBarLabel.Name = "SearchBarLabel";
            this.SearchBarLabel.Size = new System.Drawing.Size(41, 13);
            this.SearchBarLabel.TabIndex = 8;
            this.SearchBarLabel.Text = "Search";
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(222, 18);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(75, 23);
            this.SearchButton.TabIndex = 1;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(222, 198);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 9;
            this.importButton.Text = "Import..";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // PasswordList
            // 
            this.AcceptButton = this.SearchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 381);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.SearchBarLabel);
            this.Controls.Add(this.SearchBar);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.deleteUserButton);
            this.Controls.Add(this.viewButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.addButton);
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
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button viewButton;
        private System.Windows.Forms.Button deleteUserButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ToolTip buttonToolTips;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer inactiveTimer;
        private System.Windows.Forms.TextBox SearchBar;
        private System.Windows.Forms.Label SearchBarLabel;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button importButton;
    }
}