
namespace ZyncAudio.Host
{
    partial class AudioRerouter
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
            this._audioDeviceSourceBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._refreshSourcesBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _audioDeviceSourceBox
            // 
            this._audioDeviceSourceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._audioDeviceSourceBox.FormattingEnabled = true;
            this._audioDeviceSourceBox.Location = new System.Drawing.Point(134, 19);
            this._audioDeviceSourceBox.Name = "_audioDeviceSourceBox";
            this._audioDeviceSourceBox.Size = new System.Drawing.Size(188, 23);
            this._audioDeviceSourceBox.TabIndex = 0;
            this._audioDeviceSourceBox.SelectedIndexChanged += new System.EventHandler(this.AudioDeviceSourceBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Audio Device Source";
            // 
            // _refreshSourcesBtn
            // 
            this._refreshSourcesBtn.Location = new System.Drawing.Point(328, 19);
            this._refreshSourcesBtn.Name = "_refreshSourcesBtn";
            this._refreshSourcesBtn.Size = new System.Drawing.Size(124, 32);
            this._refreshSourcesBtn.TabIndex = 2;
            this._refreshSourcesBtn.Text = "Refresh Sources";
            this._refreshSourcesBtn.UseVisualStyleBackColor = true;
            this._refreshSourcesBtn.Click += new System.EventHandler(this.RefreshSources);
            // 
            // AudioRerouter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 93);
            this.Controls.Add(this._refreshSourcesBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._audioDeviceSourceBox);
            this.Name = "AudioRerouter";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audio Rerouter";
            this.Load += new System.EventHandler(this.Audio_Rerouter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _audioDeviceSourceBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _refreshSourcesBtn;
    }
}