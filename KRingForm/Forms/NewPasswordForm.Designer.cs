﻿namespace KRingForm.Forms
{
    partial class NewPasswordForm
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
            this.generateButton = new System.Windows.Forms.Button();
            this.smallSizeButton = new System.Windows.Forms.RadioButton();
            this.mediumSizeButton = new System.Windows.Forms.RadioButton();
            this.largeSizeButton = new System.Windows.Forms.RadioButton();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // domainLabel
            // 
            this.domainLabel.AutoSize = true;
            this.domainLabel.Location = new System.Drawing.Point(102, 6);
            this.domainLabel.Name = "domainLabel";
            this.domainLabel.Size = new System.Drawing.Size(43, 13);
            this.domainLabel.TabIndex = 0;
            this.domainLabel.Text = "Domain";
            // 
            // domainBox
            // 
            this.domainBox.Location = new System.Drawing.Point(59, 22);
            this.domainBox.Name = "domainBox";
            this.domainBox.Size = new System.Drawing.Size(137, 20);
            this.domainBox.TabIndex = 1;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(75, 47);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(106, 13);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "Generated Password";
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(59, 63);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.ReadOnly = true;
            this.passwordBox.Size = new System.Drawing.Size(137, 20);
            this.passwordBox.TabIndex = 3;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(140, 136);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(56, 23);
            this.addButton.TabIndex = 4;
            this.addButton.Text = "Add";
            this.buttonToolTip.SetToolTip(this.addButton, "Adds the generated password to your password list.");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(59, 136);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(63, 23);
            this.generateButton.TabIndex = 5;
            this.generateButton.Text = "Generate";
            this.buttonToolTip.SetToolTip(this.generateButton, "Generates a cryptographically random password, with a length in characters as you" +
        " have chosen in the size above. ");
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // smallSizeButton
            // 
            this.smallSizeButton.AutoSize = true;
            this.smallSizeButton.Location = new System.Drawing.Point(71, 102);
            this.smallSizeButton.Name = "smallSizeButton";
            this.smallSizeButton.Size = new System.Drawing.Size(31, 17);
            this.smallSizeButton.TabIndex = 6;
            this.smallSizeButton.Text = "8";
            this.smallSizeButton.UseVisualStyleBackColor = true;
            this.smallSizeButton.CheckedChanged += new System.EventHandler(this.smallSizeButton_CheckedChanged);
            // 
            // mediumSizeButton
            // 
            this.mediumSizeButton.AutoSize = true;
            this.mediumSizeButton.Location = new System.Drawing.Point(108, 102);
            this.mediumSizeButton.Name = "mediumSizeButton";
            this.mediumSizeButton.Size = new System.Drawing.Size(37, 17);
            this.mediumSizeButton.TabIndex = 7;
            this.mediumSizeButton.Text = "12";
            this.mediumSizeButton.UseVisualStyleBackColor = true;
            this.mediumSizeButton.CheckedChanged += new System.EventHandler(this.mediumSizeButton_CheckedChanged);
            // 
            // largeSizeButton
            // 
            this.largeSizeButton.AutoSize = true;
            this.largeSizeButton.Checked = true;
            this.largeSizeButton.Location = new System.Drawing.Point(151, 102);
            this.largeSizeButton.Name = "largeSizeButton";
            this.largeSizeButton.Size = new System.Drawing.Size(37, 17);
            this.largeSizeButton.TabIndex = 8;
            this.largeSizeButton.TabStop = true;
            this.largeSizeButton.Text = "16";
            this.largeSizeButton.UseVisualStyleBackColor = true;
            this.largeSizeButton.CheckedChanged += new System.EventHandler(this.largeSizeButton_CheckedChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(118, 86);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(27, 13);
            this.sizeLabel.TabIndex = 9;
            this.sizeLabel.Text = "Size";
            this.buttonToolTip.SetToolTip(this.sizeLabel, "The size of your password, counted in characters.");
            // 
            // NewPasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 171);
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
            this.Name = "NewPasswordForm";
            this.Text = "NewPasswordForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label domainLabel;
        private System.Windows.Forms.TextBox domainBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.RadioButton smallSizeButton;
        private System.Windows.Forms.RadioButton mediumSizeButton;
        private System.Windows.Forms.RadioButton largeSizeButton;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.ToolTip buttonToolTip;
    }
}