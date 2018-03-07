namespace KRingForm.Forms
{
    partial class EditPasswordForm
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
            this.domainLabel = new System.Windows.Forms.Label();
            this.domainBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.editButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.generateButton = new System.Windows.Forms.Button();
            this.smallSizeButton = new System.Windows.Forms.RadioButton();
            this.mediumSizeButton = new System.Windows.Forms.RadioButton();
            this.largeSizeButton = new System.Windows.Forms.RadioButton();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.largestSizeButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // domainLabel
            // 
            this.domainLabel.AutoSize = true;
            this.domainLabel.Location = new System.Drawing.Point(89, 11);
            this.domainLabel.Name = "domainLabel";
            this.domainLabel.Size = new System.Drawing.Size(43, 13);
            this.domainLabel.TabIndex = 0;
            this.domainLabel.Text = "Domain";
            // 
            // domainBox
            // 
            this.domainBox.Location = new System.Drawing.Point(52, 27);
            this.domainBox.Name = "domainBox";
            this.domainBox.ReadOnly = true;
            this.domainBox.Size = new System.Drawing.Size(124, 20);
            this.domainBox.TabIndex = 99;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(74, 95);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(78, 13);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "New Password";
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(35, 111);
            this.passwordBox.MaxLength = 80;
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(154, 20);
            this.passwordBox.TabIndex = 0;
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(122, 175);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 3;
            this.editButton.Text = "Edit";
            this.buttonToolTip.SetToolTip(this.editButton, "Saves your changes to the password to the encrypted memory..");
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(41, 175);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(75, 23);
            this.generateButton.TabIndex = 100;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // smallSizeButton
            // 
            this.smallSizeButton.AutoSize = true;
            this.smallSizeButton.Location = new System.Drawing.Point(35, 152);
            this.smallSizeButton.Name = "smallSizeButton";
            this.smallSizeButton.Size = new System.Drawing.Size(31, 17);
            this.smallSizeButton.TabIndex = 101;
            this.smallSizeButton.Text = "8";
            this.smallSizeButton.UseVisualStyleBackColor = true;
            this.smallSizeButton.CheckedChanged += new System.EventHandler(this.smallSizeButton_CheckedChanged);
            // 
            // mediumSizeButton
            // 
            this.mediumSizeButton.AutoSize = true;
            this.mediumSizeButton.Location = new System.Drawing.Point(70, 152);
            this.mediumSizeButton.Name = "mediumSizeButton";
            this.mediumSizeButton.Size = new System.Drawing.Size(37, 17);
            this.mediumSizeButton.TabIndex = 102;
            this.mediumSizeButton.Text = "12";
            this.mediumSizeButton.UseVisualStyleBackColor = true;
            this.mediumSizeButton.CheckedChanged += new System.EventHandler(this.mediumSizeButton_CheckedChanged);
            // 
            // largeSizeButton
            // 
            this.largeSizeButton.AutoSize = true;
            this.largeSizeButton.Location = new System.Drawing.Point(106, 153);
            this.largeSizeButton.Name = "largeSizeButton";
            this.largeSizeButton.Size = new System.Drawing.Size(37, 17);
            this.largeSizeButton.TabIndex = 103;
            this.largeSizeButton.Text = "16";
            this.largeSizeButton.UseVisualStyleBackColor = true;
            this.largeSizeButton.CheckedChanged += new System.EventHandler(this.largeSizeButton_CheckedChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(99, 138);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(27, 13);
            this.sizeLabel.TabIndex = 104;
            this.sizeLabel.Text = "Size";
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(84, 54);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(55, 13);
            this.usernameLabel.TabIndex = 105;
            this.usernameLabel.Text = "Username";
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(51, 70);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(124, 20);
            this.usernameBox.TabIndex = 106;
            this.usernameBox.TextChanged += new System.EventHandler(this.usernameBox_TextChanged);
            // 
            // largestSizeButton
            // 
            this.largestSizeButton.AutoSize = true;
            this.largestSizeButton.Checked = true;
            this.largestSizeButton.Location = new System.Drawing.Point(150, 152);
            this.largestSizeButton.Name = "largestSizeButton";
            this.largestSizeButton.Size = new System.Drawing.Size(37, 17);
            this.largestSizeButton.TabIndex = 107;
            this.largestSizeButton.TabStop = true;
            this.largestSizeButton.Text = "32";
            this.largestSizeButton.UseVisualStyleBackColor = true;
            this.largestSizeButton.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // EditPasswordForm
            // 
            this.AcceptButton = this.editButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 209);
            this.Controls.Add(this.largestSizeButton);
            this.Controls.Add(this.usernameBox);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.largeSizeButton);
            this.Controls.Add(this.mediumSizeButton);
            this.Controls.Add(this.smallSizeButton);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.domainBox);
            this.Controls.Add(this.domainLabel);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewForm_FormClosing);
            this.Name = "EditPasswordForm";
            this.Text = "EditPasswordForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label domainLabel;
        private System.Windows.Forms.TextBox domainBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.RadioButton smallSizeButton;
        private System.Windows.Forms.RadioButton mediumSizeButton;
        private System.Windows.Forms.RadioButton largeSizeButton;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.RadioButton largestSizeButton;
    }
}