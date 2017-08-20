namespace KRingForm.Forms
{
    partial class ViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewForm));
            this.domainLabel = new System.Windows.Forms.Label();
            this.domainBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.copyButton = new System.Windows.Forms.Button();
            this.warning = new System.Windows.Forms.Label();
            this.warningTimer = new System.Windows.Forms.Timer(this.components);
            this.revealButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.clipboardTimer = new System.Windows.Forms.Timer(this.components);
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // domainLabel
            // 
            this.domainLabel.AutoSize = true;
            this.domainLabel.Location = new System.Drawing.Point(105, 9);
            this.domainLabel.Name = "domainLabel";
            this.domainLabel.Size = new System.Drawing.Size(43, 13);
            this.domainLabel.TabIndex = 0;
            this.domainLabel.Text = "Domain";
            // 
            // domainBox
            // 
            this.domainBox.Location = new System.Drawing.Point(71, 25);
            this.domainBox.Name = "domainBox";
            this.domainBox.ReadOnly = true;
            this.domainBox.Size = new System.Drawing.Size(124, 20);
            this.domainBox.TabIndex = 0;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(105, 87);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "Password";
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(56, 103);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.ReadOnly = true;
            this.passwordBox.Size = new System.Drawing.Size(154, 20);
            this.passwordBox.TabIndex = 2;
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(95, 158);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 4;
            this.copyButton.Text = "Copy";
            this.buttonToolTip.SetToolTip(this.copyButton, resources.GetString("copyButton.ToolTip"));
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // warning
            // 
            this.warning.AutoSize = true;
            this.warning.Location = new System.Drawing.Point(53, 194);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(142, 13);
            this.warning.TabIndex = 5;
            this.warning.Text = "Window closes in 5 seconds";
            // 
            // warningTimer
            // 
            this.warningTimer.Tick += new System.EventHandler(this.warningTimer_Tick);
            // 
            // revealButton
            // 
            this.revealButton.Location = new System.Drawing.Point(95, 129);
            this.revealButton.Name = "revealButton";
            this.revealButton.Size = new System.Drawing.Size(75, 23);
            this.revealButton.TabIndex = 3;
            this.revealButton.Text = "Reveal";
            this.buttonToolTip.SetToolTip(this.revealButton, "Reveals the password in plaintext for 5 seconds. Beware that people can be lookin" +
        "g over your shoulder etc.");
            this.revealButton.UseVisualStyleBackColor = true;
            this.revealButton.Click += new System.EventHandler(this.revealButton_Click);
            // 
            // clipboardTimer
            // 
            this.clipboardTimer.Tick += new System.EventHandler(this.clipboardTimer_Tick);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(105, 48);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(55, 13);
            this.UsernameLabel.TabIndex = 7;
            this.UsernameLabel.Text = "Username";
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(71, 64);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.ReadOnly = true;
            this.usernameBox.Size = new System.Drawing.Size(124, 20);
            this.usernameBox.TabIndex = 1;
            // 
            // ViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 215);
            this.Controls.Add(this.usernameBox);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.revealButton);
            this.Controls.Add(this.warning);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.domainBox);
            this.Controls.Add(this.domainLabel);
            this.Name = "ViewForm";
            this.Text = "ViewForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label domainLabel;
        private System.Windows.Forms.TextBox domainBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Label warning;
        private System.Windows.Forms.Timer warningTimer;
        private System.Windows.Forms.Button revealButton;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Timer clipboardTimer;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.TextBox usernameBox;
    }
}