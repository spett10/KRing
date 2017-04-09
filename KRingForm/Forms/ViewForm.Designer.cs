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
            this.domainLabel = new System.Windows.Forms.Label();
            this.domainBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.copyButton = new System.Windows.Forms.Button();
            this.warning = new System.Windows.Forms.Label();
            this.warningTimer = new System.Windows.Forms.Timer(this.components);
            this.revealButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // domainLabel
            // 
            this.domainLabel.AutoSize = true;
            this.domainLabel.Location = new System.Drawing.Point(105, 18);
            this.domainLabel.Name = "domainLabel";
            this.domainLabel.Size = new System.Drawing.Size(43, 13);
            this.domainLabel.TabIndex = 0;
            this.domainLabel.Text = "Domain";
            // 
            // domainBox
            // 
            this.domainBox.Location = new System.Drawing.Point(71, 34);
            this.domainBox.Name = "domainBox";
            this.domainBox.ReadOnly = true;
            this.domainBox.Size = new System.Drawing.Size(124, 20);
            this.domainBox.TabIndex = 1;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(105, 72);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "Password";
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(71, 88);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.ReadOnly = true;
            this.passwordBox.Size = new System.Drawing.Size(124, 20);
            this.passwordBox.TabIndex = 3;
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(95, 143);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 4;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // warning
            // 
            this.warning.AutoSize = true;
            this.warning.Location = new System.Drawing.Point(53, 179);
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
            this.revealButton.Location = new System.Drawing.Point(95, 114);
            this.revealButton.Name = "revealButton";
            this.revealButton.Size = new System.Drawing.Size(75, 23);
            this.revealButton.TabIndex = 6;
            this.revealButton.Text = "Reveal";
            this.revealButton.UseVisualStyleBackColor = true;
            this.revealButton.Click += new System.EventHandler(this.revealButton_Click);
            // 
            // ViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 220);
            this.Controls.Add(this.revealButton);
            this.Controls.Add(this.warning);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.domainBox);
            this.Controls.Add(this.domainLabel);
            this.Name = "ViewForm";
            this.Text = "ViewForm";
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
    }
}