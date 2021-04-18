
namespace ZyncAudio
{
    partial class ClientForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HostBtn = new System.Windows.Forms.Button();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.IPAddressInputBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // HostBtn
            // 
            this.HostBtn.Location = new System.Drawing.Point(12, 12);
            this.HostBtn.Name = "HostBtn";
            this.HostBtn.Size = new System.Drawing.Size(97, 37);
            this.HostBtn.TabIndex = 1;
            this.HostBtn.Text = "Host Music";
            this.HostBtn.UseVisualStyleBackColor = true;
            this.HostBtn.Click += new System.EventHandler(this.HostButton_Click);
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Location = new System.Drawing.Point(12, 276);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(97, 37);
            this.ConnectBtn.TabIndex = 2;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // IPAddressInputBox
            // 
            this.IPAddressInputBox.Location = new System.Drawing.Point(115, 284);
            this.IPAddressInputBox.Name = "IPAddressInputBox";
            this.IPAddressInputBox.Size = new System.Drawing.Size(146, 23);
            this.IPAddressInputBox.TabIndex = 3;
            this.IPAddressInputBox.Text = "127.0.0.1";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 325);
            this.Controls.Add(this.IPAddressInputBox);
            this.Controls.Add(this.ConnectBtn);
            this.Controls.Add(this.HostBtn);
            this.Name = "ClientForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zync Audio Client";
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button HostBtn;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.TextBox IPAddressInputBox;
    }
}

