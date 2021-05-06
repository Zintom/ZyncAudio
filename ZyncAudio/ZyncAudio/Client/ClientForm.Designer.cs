
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
            this.components = new System.ComponentModel.Container();
            this._hostBtn = new ZyncAudio.TintableButton();
            this._connectBtn = new System.Windows.Forms.Button();
            this._ipAddressInputBox = new ZyncAudio.IPAddressTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this._nowPlayingBarText = new System.Windows.Forms.Label();
            this._nowPlayingBarAnimator = new System.Windows.Forms.Timer(this.components);
            this._offScreen = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _hostBtn
            // 
            this._hostBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._hostBtn.Checked = false;
            this._hostBtn.CheckedTint = System.Drawing.Color.Empty;
            this._hostBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this._hostBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._hostBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._hostBtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._hostBtn.ForeColor = System.Drawing.Color.Black;
            this._hostBtn.Location = new System.Drawing.Point(12, 49);
            this._hostBtn.Name = "_hostBtn";
            this._hostBtn.Size = new System.Drawing.Size(97, 31);
            this._hostBtn.TabIndex = 3;
            this._hostBtn.Text = "Host";
            this._hostBtn.Tint = System.Drawing.Color.Empty;
            this._hostBtn.UseVisualStyleBackColor = false;
            this._hostBtn.Click += new System.EventHandler(this.HostButton_Click);
            // 
            // _connectBtn
            // 
            this._connectBtn.BackColor = System.Drawing.Color.LightBlue;
            this._connectBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._connectBtn.ForeColor = System.Drawing.Color.Black;
            this._connectBtn.Location = new System.Drawing.Point(12, 12);
            this._connectBtn.Name = "_connectBtn";
            this._connectBtn.Size = new System.Drawing.Size(97, 31);
            this._connectBtn.TabIndex = 1;
            this._connectBtn.Text = "Connect";
            this._connectBtn.UseVisualStyleBackColor = false;
            this._connectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // _ipAddressInputBox
            // 
            this._ipAddressInputBox.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._ipAddressInputBox.Location = new System.Drawing.Point(115, 14);
            this._ipAddressInputBox.Name = "_ipAddressInputBox";
            this._ipAddressInputBox.Size = new System.Drawing.Size(146, 27);
            this._ipAddressInputBox.TabIndex = 2;
            this._ipAddressInputBox.Text = "255.255.255.255";
            this._ipAddressInputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._ipAddressInputBox.TextChanged += new System.EventHandler(this.IPAddressInputBox_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(100)))), ((int)(((byte)(190)))));
            this.panel1.Controls.Add(this._nowPlayingBarText);
            this.panel1.Location = new System.Drawing.Point(0, 111);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 24);
            this.panel1.TabIndex = 4;
            // 
            // _nowPlayingBarText
            // 
            this._nowPlayingBarText.AutoSize = true;
            this._nowPlayingBarText.ForeColor = System.Drawing.Color.White;
            this._nowPlayingBarText.Location = new System.Drawing.Point(3, 5);
            this._nowPlayingBarText.Name = "_nowPlayingBarText";
            this._nowPlayingBarText.Size = new System.Drawing.Size(0, 15);
            this._nowPlayingBarText.TabIndex = 5;
            // 
            // _nowPlayingBarAnimator
            // 
            this._nowPlayingBarAnimator.Interval = 32;
            this._nowPlayingBarAnimator.Tick += new System.EventHandler(this.NowPlayingBarTick);
            // 
            // _offScreen
            // 
            this._offScreen.Location = new System.Drawing.Point(-20, -20);
            this._offScreen.Name = "_offScreen";
            this._offScreen.Size = new System.Drawing.Size(10, 10);
            this._offScreen.TabIndex = 0;
            this._offScreen.Text = "button1";
            this._offScreen.UseVisualStyleBackColor = true;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 135);
            this.Controls.Add(this._offScreen);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._ipAddressInputBox);
            this.Controls.Add(this._connectBtn);
            this.Controls.Add(this._hostBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ClientForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zync Audio Client";
            this.Activated += new System.EventHandler(this.ClientForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TintableButton _hostBtn;
        private System.Windows.Forms.Button _connectBtn;
        private IPAddressTextBox _ipAddressInputBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label _nowPlayingBarText;
        private System.Windows.Forms.Timer _nowPlayingBarAnimator;
        private System.Windows.Forms.Button _offScreen;
    }
}

