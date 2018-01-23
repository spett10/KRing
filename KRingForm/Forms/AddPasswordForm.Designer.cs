namespace KRingForm.Forms
{
    partial class AddPasswordForm
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
            this.addButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.generateButton = new System.Windows.Forms.Button();
            this.smallSizeButton = new System.Windows.Forms.RadioButton();
            this.mediumSizeButton = new System.Windows.Forms.RadioButton();
            this.largeSizeButton = new System.Windows.Forms.RadioButton();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.largestSizeButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // domainLabel
            // 
            this.domainLabel.AutoSize = true;
            this.domainLabel.Location = new System.Drawing.Point(103, 15);
            this.domainLabel.Name = "domainLabel";
            this.domainLabel.Size = new System.Drawing.Size(43, 13);
            this.domainLabel.TabIndex = 0;
            this.domainLabel.Text = "Domain";
            // 
            // domainBox
            // 
            this.domainBox.Location = new System.Drawing.Point(63, 31);
            this.domainBox.MaxLength = 80;
            this.domainBox.Name = "domainBox";
            this.domainBox.Size = new System.Drawing.Size(124, 20);
            this.domainBox.TabIndex = 0;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(96, 96);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 1;
            this.passwordLabel.Text = "Password";
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(47, 112);
            this.passwordBox.MaxLength = 80;
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(154, 20);
            this.passwordBox.TabIndex = 2;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(131, 184);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 7;
            this.addButton.Text = "Add";
            this.buttonToolTip.SetToolTip(this.addButton, "Adds the password and domain to encrypted memory.");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(50, 184);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(75, 23);
            this.generateButton.TabIndex = 6;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // smallSizeButton
            // 
            this.smallSizeButton.AutoSize = true;
            this.smallSizeButton.Location = new System.Drawing.Point(47, 152);
            this.smallSizeButton.Name = "smallSizeButton";
            this.smallSizeButton.Size = new System.Drawing.Size(31, 17);
            this.smallSizeButton.TabIndex = 3;
            this.smallSizeButton.Text = "8";
            this.smallSizeButton.UseVisualStyleBackColor = true;
            this.smallSizeButton.CheckedChanged += new System.EventHandler(this.smallSizeButton_CheckedChanged);
            // 
            // mediumSizeButton
            // 
            this.mediumSizeButton.AutoSize = true;
            this.mediumSizeButton.Location = new System.Drawing.Point(84, 152);
            this.mediumSizeButton.Name = "mediumSizeButton";
            this.mediumSizeButton.Size = new System.Drawing.Size(37, 17);
            this.mediumSizeButton.TabIndex = 4;
            this.mediumSizeButton.Text = "12";
            this.mediumSizeButton.UseVisualStyleBackColor = true;
            this.mediumSizeButton.CheckedChanged += new System.EventHandler(this.mediumSizeButton_CheckedChanged);
            // 
            // largeSizeButton
            // 
            this.largeSizeButton.AutoSize = true;
            this.largeSizeButton.Location = new System.Drawing.Point(128, 152);
            this.largeSizeButton.Name = "largeSizeButton";
            this.largeSizeButton.Size = new System.Drawing.Size(37, 17);
            this.largeSizeButton.TabIndex = 5;
            this.largeSizeButton.Text = "16";
            this.largeSizeButton.UseVisualStyleBackColor = true;
            this.largeSizeButton.CheckedChanged += new System.EventHandler(this.largeSizeButton_CheckedChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(111, 136);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(27, 13);
            this.sizeLabel.TabIndex = 7;
            this.sizeLabel.Text = "Size";
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(100, 54);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(55, 13);
            this.usernameLabel.TabIndex = 8;
            this.usernameLabel.Text = "Username";
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(63, 70);
            this.usernameBox.MaxLength = 80;
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(124, 20);
            this.usernameBox.TabIndex = 1;
            // 
            // largestSizeButton
            // 
            this.largestSizeButton.AutoSize = true;
            this.largestSizeButton.Checked = true;
            this.largestSizeButton.Location = new System.Drawing.Point(172, 152);
            this.largestSizeButton.Name = "largestSizeButton";
            this.largestSizeButton.Size = new System.Drawing.Size(37, 17);
            this.largestSizeButton.TabIndex = 9;
            this.largestSizeButton.TabStop = true;
            this.largestSizeButton.Text = "32";
            this.largestSizeButton.UseVisualStyleBackColor = true;
            this.largestSizeButton.CheckedChanged += new System.EventHandler(this.largestSizeButton_CheckedChanged);
            // 
            // AddPasswordForm
            // 
            this.AcceptButton = this.addButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 217);
            this.Controls.Add(this.largestSizeButton);
            this.Controls.Add(this.usernameBox);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.largeSizeButton);
            this.Controls.Add(this.mediumSizeButton);
            this.Controls.Add(this.smallSizeButton);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.domainBox);
            this.Controls.Add(this.domainLabel);
            this.Name = "AddPasswordForm";
            this.Text = "AddPasswordForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label domainLabel;
        private System.Windows.Forms.TextBox domainBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.RadioButton smallSizeButton;
        private System.Windows.Forms.RadioButton mediumSizeButton;
        private System.Windows.Forms.RadioButton largeSizeButton;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.RadioButton largestSizeButton;
    }
}