
namespace ZyncAudio
{
    partial class HostForm
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
            this.PlayBtn = new System.Windows.Forms.Button();
            this.StopBtn = new System.Windows.Forms.Button();
            this.CloseEntryBtn = new System.Windows.Forms.Button();
            this.ClientListView = new System.Windows.Forms.ListView();
            this.PingChecker = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // PlayBtn
            // 
            this.PlayBtn.Enabled = false;
            this.PlayBtn.Location = new System.Drawing.Point(12, 93);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(112, 37);
            this.PlayBtn.TabIndex = 0;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // StopBtn
            // 
            this.StopBtn.Enabled = false;
            this.StopBtn.Location = new System.Drawing.Point(13, 136);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(112, 37);
            this.StopBtn.TabIndex = 1;
            this.StopBtn.Text = "Stop";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // CloseEntryBtn
            // 
            this.CloseEntryBtn.Enabled = false;
            this.CloseEntryBtn.Location = new System.Drawing.Point(13, 50);
            this.CloseEntryBtn.Name = "CloseEntryBtn";
            this.CloseEntryBtn.Size = new System.Drawing.Size(112, 37);
            this.CloseEntryBtn.TabIndex = 2;
            this.CloseEntryBtn.Text = "Close Entry";
            this.CloseEntryBtn.UseVisualStyleBackColor = true;
            this.CloseEntryBtn.Click += new System.EventHandler(this.CloseEntryBtn_Click);
            // 
            // ClientListView
            // 
            this.ClientListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClientListView.HideSelection = false;
            this.ClientListView.Location = new System.Drawing.Point(396, 12);
            this.ClientListView.Name = "ClientListView";
            this.ClientListView.Size = new System.Drawing.Size(184, 288);
            this.ClientListView.TabIndex = 4;
            this.ClientListView.UseCompatibleStateImageBehavior = false;
            // 
            // PingChecker
            // 
            this.PingChecker.Enabled = true;
            this.PingChecker.Interval = 250;
            this.PingChecker.Tick += new System.EventHandler(this.PingChecker_Tick);
            // 
            // HostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 312);
            this.Controls.Add(this.ClientListView);
            this.Controls.Add(this.CloseEntryBtn);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.PlayBtn);
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.Name = "HostForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Zync Audio Host";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HostForm_FormClosing);
            this.Load += new System.EventHandler(this.HostForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button PlayBtn;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.Button CloseEntryBtn;
        private System.Windows.Forms.ListView ClientListView;
        private System.Windows.Forms.Timer PingChecker;
    }
}