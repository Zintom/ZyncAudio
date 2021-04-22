
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
            this._playListView = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this._loadFolderBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.PreviousBtn = new System.Windows.Forms.Button();
            this._unloadPlaylistBtn = new System.Windows.Forms.Button();
            this._searchSubFoldersChkBox = new System.Windows.Forms.CheckBox();
            this._serverGroupBox = new System.Windows.Forms.GroupBox();
            this._mediaGroupBox = new System.Windows.Forms.GroupBox();
            this._shuffleBtn = new System.Windows.Forms.Button();
            this._serverGroupBox.SuspendLayout();
            this._mediaGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PlayBtn
            // 
            this.PlayBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.PlayBtn.Enabled = false;
            this.PlayBtn.Location = new System.Drawing.Point(142, 353);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(53, 37);
            this.PlayBtn.TabIndex = 0;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // StopBtn
            // 
            this.StopBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.StopBtn.Enabled = false;
            this.StopBtn.Location = new System.Drawing.Point(277, 353);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(45, 37);
            this.StopBtn.TabIndex = 1;
            this.StopBtn.Text = "Stop";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // CloseEntryBtn
            // 
            this.CloseEntryBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CloseEntryBtn.Enabled = false;
            this.CloseEntryBtn.Location = new System.Drawing.Point(13, 352);
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
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ClientListView.HideSelection = false;
            this.ClientListView.Location = new System.Drawing.Point(13, 21);
            this.ClientListView.Name = "ClientListView";
            this.ClientListView.Size = new System.Drawing.Size(178, 320);
            this.ClientListView.TabIndex = 4;
            this.ClientListView.UseCompatibleStateImageBehavior = false;
            // 
            // PingChecker
            // 
            this.PingChecker.Enabled = true;
            this.PingChecker.Interval = 250;
            this.PingChecker.Tick += new System.EventHandler(this.PingChecker_Tick);
            // 
            // _playListView
            // 
            this._playListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._playListView.Enabled = false;
            this._playListView.FormattingEnabled = true;
            this._playListView.IntegralHeight = false;
            this._playListView.ItemHeight = 15;
            this._playListView.Location = new System.Drawing.Point(12, 39);
            this._playListView.Name = "_playListView";
            this._playListView.Size = new System.Drawing.Size(364, 267);
            this._playListView.TabIndex = 5;
            this._playListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PlayQueue_MouseDown);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(172, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Playlist";
            // 
            // _loadFolderBtn
            // 
            this._loadFolderBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._loadFolderBtn.Enabled = false;
            this._loadFolderBtn.Location = new System.Drawing.Point(144, 313);
            this._loadFolderBtn.Name = "_loadFolderBtn";
            this._loadFolderBtn.Size = new System.Drawing.Size(92, 29);
            this._loadFolderBtn.TabIndex = 7;
            this._loadFolderBtn.Text = "Load folder";
            this._loadFolderBtn.UseVisualStyleBackColor = true;
            this._loadFolderBtn.Click += new System.EventHandler(this.LoadFolderBtn_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.NextBtn.Enabled = false;
            this.NextBtn.Location = new System.Drawing.Point(201, 353);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(70, 37);
            this.NextBtn.TabIndex = 8;
            this.NextBtn.Text = "Next";
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // PreviousBtn
            // 
            this.PreviousBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.PreviousBtn.Enabled = false;
            this.PreviousBtn.Location = new System.Drawing.Point(66, 353);
            this.PreviousBtn.Name = "PreviousBtn";
            this.PreviousBtn.Size = new System.Drawing.Size(70, 37);
            this.PreviousBtn.TabIndex = 9;
            this.PreviousBtn.Text = "Previous";
            this.PreviousBtn.UseVisualStyleBackColor = true;
            this.PreviousBtn.Click += new System.EventHandler(this.PreviousBtn_Click);
            // 
            // _unloadPlaylistBtn
            // 
            this._unloadPlaylistBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._unloadPlaylistBtn.Enabled = false;
            this._unloadPlaylistBtn.Location = new System.Drawing.Point(242, 313);
            this._unloadPlaylistBtn.Name = "_unloadPlaylistBtn";
            this._unloadPlaylistBtn.Size = new System.Drawing.Size(92, 29);
            this._unloadPlaylistBtn.TabIndex = 10;
            this._unloadPlaylistBtn.Text = "Unload All";
            this._unloadPlaylistBtn.UseVisualStyleBackColor = true;
            this._unloadPlaylistBtn.Click += new System.EventHandler(this.UnloadItems_Click);
            // 
            // _searchSubFoldersChkBox
            // 
            this._searchSubFoldersChkBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._searchSubFoldersChkBox.AutoSize = true;
            this._searchSubFoldersChkBox.Location = new System.Drawing.Point(13, 319);
            this._searchSubFoldersChkBox.Name = "_searchSubFoldersChkBox";
            this._searchSubFoldersChkBox.Size = new System.Drawing.Size(125, 19);
            this._searchSubFoldersChkBox.TabIndex = 11;
            this._searchSubFoldersChkBox.Text = "Search Sub-folders";
            this._searchSubFoldersChkBox.UseVisualStyleBackColor = true;
            // 
            // _serverGroupBox
            // 
            this._serverGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._serverGroupBox.Controls.Add(this.ClientListView);
            this._serverGroupBox.Controls.Add(this.CloseEntryBtn);
            this._serverGroupBox.Location = new System.Drawing.Point(430, 12);
            this._serverGroupBox.Name = "_serverGroupBox";
            this._serverGroupBox.Size = new System.Drawing.Size(203, 404);
            this._serverGroupBox.TabIndex = 12;
            this._serverGroupBox.TabStop = false;
            this._serverGroupBox.Text = "Server";
            // 
            // _mediaGroupBox
            // 
            this._mediaGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mediaGroupBox.Controls.Add(this._shuffleBtn);
            this._mediaGroupBox.Controls.Add(this._playListView);
            this._mediaGroupBox.Controls.Add(this.label1);
            this._mediaGroupBox.Controls.Add(this._searchSubFoldersChkBox);
            this._mediaGroupBox.Controls.Add(this._unloadPlaylistBtn);
            this._mediaGroupBox.Controls.Add(this.PlayBtn);
            this._mediaGroupBox.Controls.Add(this.PreviousBtn);
            this._mediaGroupBox.Controls.Add(this.StopBtn);
            this._mediaGroupBox.Controls.Add(this.NextBtn);
            this._mediaGroupBox.Controls.Add(this._loadFolderBtn);
            this._mediaGroupBox.Location = new System.Drawing.Point(12, 12);
            this._mediaGroupBox.Name = "_mediaGroupBox";
            this._mediaGroupBox.Size = new System.Drawing.Size(388, 404);
            this._mediaGroupBox.TabIndex = 13;
            this._mediaGroupBox.TabStop = false;
            this._mediaGroupBox.Text = "Media";
            // 
            // _shuffleBtn
            // 
            this._shuffleBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._shuffleBtn.Enabled = false;
            this._shuffleBtn.Font = new System.Drawing.Font("Segoe UI Emoji", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._shuffleBtn.Location = new System.Drawing.Point(336, 313);
            this._shuffleBtn.Name = "_shuffleBtn";
            this._shuffleBtn.Size = new System.Drawing.Size(40, 40);
            this._shuffleBtn.TabIndex = 12;
            this._shuffleBtn.Text = "🔀";
            this._shuffleBtn.UseVisualStyleBackColor = true;
            this._shuffleBtn.Click += new System.EventHandler(this.ShuffleBtnClicked);
            // 
            // HostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 428);
            this.Controls.Add(this._mediaGroupBox);
            this.Controls.Add(this._serverGroupBox);
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 300);
            this.Name = "HostForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Zync Audio Host";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HostForm_FormClosing);
            this.Load += new System.EventHandler(this.HostForm_Load);
            this._serverGroupBox.ResumeLayout(false);
            this._mediaGroupBox.ResumeLayout(false);
            this._mediaGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button PlayBtn;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.Button CloseEntryBtn;
        private System.Windows.Forms.ListView ClientListView;
        private System.Windows.Forms.Timer PingChecker;
        private System.Windows.Forms.ListBox _playListView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _loadFolderBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.Button PreviousBtn;
        private System.Windows.Forms.Button _unloadPlaylistBtn;
        private System.Windows.Forms.CheckBox _searchSubFoldersChkBox;
        private System.Windows.Forms.GroupBox _serverGroupBox;
        private System.Windows.Forms.GroupBox _mediaGroupBox;
        private System.Windows.Forms.Button _shuffleBtn;
    }
}