
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostForm));
            this._closeEntryBtn = new System.Windows.Forms.Button();
            this.ClientListView = new System.Windows.Forms.ListView();
            this.PingChecker = new System.Windows.Forms.Timer(this.components);
            this._playListView = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this._loadFolderBtn = new ZyncAudio.TintableButton();
            this._unloadPlaylistBtn = new ZyncAudio.TintableButton();
            this._serverGroupBox = new System.Windows.Forms.GroupBox();
            this._mediaGroupBox = new System.Windows.Forms.GroupBox();
            this._addSingleTrackBtn = new ZyncAudio.TintableButton();
            this._rerouteAudioBtn = new ZyncAudio.TintableButton();
            this._searchSubFoldersBtn = new ZyncAudio.TintableButton();
            this._audioLevelsImg = new ZyncAudio.TintableButton();
            this._playBtn = new ZyncAudio.TintableButton();
            this._volumeControlBar = new System.Windows.Forms.TrackBar();
            this._shuffleBtn = new ZyncAudio.TintableButton();
            this._previousBtn = new ZyncAudio.TintableButton();
            this._stopBtn = new ZyncAudio.TintableButton();
            this._nextBtn = new ZyncAudio.TintableButton();
            this._toolTipProvider = new System.Windows.Forms.ToolTip(this.components);
            this._serverGroupBox.SuspendLayout();
            this._mediaGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._volumeControlBar)).BeginInit();
            this.SuspendLayout();
            // 
            // _closeEntryBtn
            // 
            this._closeEntryBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._closeEntryBtn.Location = new System.Drawing.Point(13, 414);
            this._closeEntryBtn.Name = "_closeEntryBtn";
            this._closeEntryBtn.Size = new System.Drawing.Size(112, 37);
            this._closeEntryBtn.TabIndex = 2;
            this._closeEntryBtn.Text = "Close Entry";
            this._closeEntryBtn.UseVisualStyleBackColor = true;
            this._closeEntryBtn.Click += new System.EventHandler(this.CloseEntryBtn_Click);
            // 
            // ClientListView
            // 
            this.ClientListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ClientListView.HideSelection = false;
            this.ClientListView.Location = new System.Drawing.Point(13, 21);
            this.ClientListView.Name = "ClientListView";
            this.ClientListView.Size = new System.Drawing.Size(178, 382);
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
            this._playListView.FormattingEnabled = true;
            this._playListView.IntegralHeight = false;
            this._playListView.ItemHeight = 15;
            this._playListView.Location = new System.Drawing.Point(12, 39);
            this._playListView.Name = "_playListView";
            this._playListView.Size = new System.Drawing.Size(364, 278);
            this._playListView.TabIndex = 5;
            this._playListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PlayQueue_MouseDown);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(156, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Playing Queue";
            // 
            // _loadFolderBtn
            // 
            this._loadFolderBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._loadFolderBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.folder_8x;
            this._loadFolderBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._loadFolderBtn.Checked = false;
            this._loadFolderBtn.CheckedTint = System.Drawing.Color.Empty;
            this._loadFolderBtn.FlatAppearance.BorderSize = 0;
            this._loadFolderBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._loadFolderBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._loadFolderBtn.Location = new System.Drawing.Point(247, 326);
            this._loadFolderBtn.Name = "_loadFolderBtn";
            this._loadFolderBtn.Size = new System.Drawing.Size(25, 25);
            this._loadFolderBtn.TabIndex = 7;
            this._loadFolderBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this._toolTipProvider.SetToolTip(this._loadFolderBtn, "Add a folder of tracks to the current playing queue.");
            this._loadFolderBtn.UseVisualStyleBackColor = true;
            this._loadFolderBtn.Click += new System.EventHandler(this.LoadFolderBtn_Click);
            // 
            // _unloadPlaylistBtn
            // 
            this._unloadPlaylistBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._unloadPlaylistBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.trash_8x;
            this._unloadPlaylistBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._unloadPlaylistBtn.Checked = false;
            this._unloadPlaylistBtn.CheckedTint = System.Drawing.Color.Empty;
            this._unloadPlaylistBtn.FlatAppearance.BorderSize = 0;
            this._unloadPlaylistBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._unloadPlaylistBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._unloadPlaylistBtn.Location = new System.Drawing.Point(313, 326);
            this._unloadPlaylistBtn.Name = "_unloadPlaylistBtn";
            this._unloadPlaylistBtn.Size = new System.Drawing.Size(25, 25);
            this._unloadPlaylistBtn.TabIndex = 10;
            this._unloadPlaylistBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this._toolTipProvider.SetToolTip(this._unloadPlaylistBtn, "Remove all tracks from the playing queue.");
            this._unloadPlaylistBtn.UseVisualStyleBackColor = true;
            this._unloadPlaylistBtn.Click += new System.EventHandler(this.UnloadItems_Click);
            // 
            // _serverGroupBox
            // 
            this._serverGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._serverGroupBox.Controls.Add(this.ClientListView);
            this._serverGroupBox.Controls.Add(this._closeEntryBtn);
            this._serverGroupBox.Location = new System.Drawing.Point(430, 12);
            this._serverGroupBox.Name = "_serverGroupBox";
            this._serverGroupBox.Size = new System.Drawing.Size(203, 466);
            this._serverGroupBox.TabIndex = 12;
            this._serverGroupBox.TabStop = false;
            this._serverGroupBox.Text = "Server";
            // 
            // _mediaGroupBox
            // 
            this._mediaGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mediaGroupBox.Controls.Add(this._addSingleTrackBtn);
            this._mediaGroupBox.Controls.Add(this._rerouteAudioBtn);
            this._mediaGroupBox.Controls.Add(this._searchSubFoldersBtn);
            this._mediaGroupBox.Controls.Add(this._audioLevelsImg);
            this._mediaGroupBox.Controls.Add(this._playBtn);
            this._mediaGroupBox.Controls.Add(this._volumeControlBar);
            this._mediaGroupBox.Controls.Add(this._shuffleBtn);
            this._mediaGroupBox.Controls.Add(this._playListView);
            this._mediaGroupBox.Controls.Add(this.label1);
            this._mediaGroupBox.Controls.Add(this._unloadPlaylistBtn);
            this._mediaGroupBox.Controls.Add(this._previousBtn);
            this._mediaGroupBox.Controls.Add(this._stopBtn);
            this._mediaGroupBox.Controls.Add(this._nextBtn);
            this._mediaGroupBox.Controls.Add(this._loadFolderBtn);
            this._mediaGroupBox.Location = new System.Drawing.Point(12, 12);
            this._mediaGroupBox.Name = "_mediaGroupBox";
            this._mediaGroupBox.Size = new System.Drawing.Size(388, 466);
            this._mediaGroupBox.TabIndex = 13;
            this._mediaGroupBox.TabStop = false;
            this._mediaGroupBox.Text = "Media";
            // 
            // _addSingleTrackBtn
            // 
            this._addSingleTrackBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._addSingleTrackBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.file_8x;
            this._addSingleTrackBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._addSingleTrackBtn.Checked = false;
            this._addSingleTrackBtn.CheckedTint = System.Drawing.Color.Empty;
            this._addSingleTrackBtn.FlatAppearance.BorderSize = 0;
            this._addSingleTrackBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._addSingleTrackBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._addSingleTrackBtn.Location = new System.Drawing.Point(282, 326);
            this._addSingleTrackBtn.Name = "_addSingleTrackBtn";
            this._addSingleTrackBtn.Size = new System.Drawing.Size(25, 25);
            this._addSingleTrackBtn.TabIndex = 19;
            this._addSingleTrackBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this._toolTipProvider.SetToolTip(this._addSingleTrackBtn, "Add a track to the current playing queue.");
            this._addSingleTrackBtn.UseVisualStyleBackColor = true;
            this._addSingleTrackBtn.Click += new System.EventHandler(this.AddSingleTrackBtn_Clicked);
            // 
            // _rerouteAudioBtn
            // 
            this._rerouteAudioBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rerouteAudioBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_rerouteAudioBtn.BackgroundImage")));
            this._rerouteAudioBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._rerouteAudioBtn.Checked = false;
            this._rerouteAudioBtn.CheckedTint = System.Drawing.Color.Empty;
            this._rerouteAudioBtn.FlatAppearance.BorderSize = 0;
            this._rerouteAudioBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._rerouteAudioBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._rerouteAudioBtn.Location = new System.Drawing.Point(15, 323);
            this._rerouteAudioBtn.Name = "_rerouteAudioBtn";
            this._rerouteAudioBtn.Size = new System.Drawing.Size(25, 25);
            this._rerouteAudioBtn.TabIndex = 18;
            this._rerouteAudioBtn.Tint = System.Drawing.Color.Empty;
            this._toolTipProvider.SetToolTip(this._rerouteAudioBtn, resources.GetString("_rerouteAudioBtn.ToolTip"));
            this._rerouteAudioBtn.UseVisualStyleBackColor = true;
            this._rerouteAudioBtn.Click += new System.EventHandler(this.RerouteAudioBtn_Click);
            // 
            // _searchSubFoldersBtn
            // 
            this._searchSubFoldersBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._searchSubFoldersBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.fork_8x;
            this._searchSubFoldersBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._searchSubFoldersBtn.Checked = false;
            this._searchSubFoldersBtn.CheckedTint = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this._searchSubFoldersBtn.FlatAppearance.BorderSize = 0;
            this._searchSubFoldersBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._searchSubFoldersBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipY;
            this._searchSubFoldersBtn.Location = new System.Drawing.Point(215, 326);
            this._searchSubFoldersBtn.Name = "_searchSubFoldersBtn";
            this._searchSubFoldersBtn.Size = new System.Drawing.Size(25, 25);
            this._searchSubFoldersBtn.TabIndex = 17;
            this._searchSubFoldersBtn.Tint = System.Drawing.Color.Silver;
            this._toolTipProvider.SetToolTip(this._searchSubFoldersBtn, "Load sub-folders when adding a folder. (On/Off)");
            this._searchSubFoldersBtn.UseVisualStyleBackColor = true;
            this._searchSubFoldersBtn.Click += new System.EventHandler(this.SearchSubFoldersBtn_Click);
            // 
            // _audioLevelsImg
            // 
            this._audioLevelsImg.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._audioLevelsImg.BackgroundImage = global::ZyncAudio.Properties.Resources.volume_high_8x;
            this._audioLevelsImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._audioLevelsImg.Checked = false;
            this._audioLevelsImg.CheckedTint = System.Drawing.Color.Empty;
            this._audioLevelsImg.FlatAppearance.BorderSize = 0;
            this._audioLevelsImg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._audioLevelsImg.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._audioLevelsImg.Location = new System.Drawing.Point(66, 421);
            this._audioLevelsImg.Name = "_audioLevelsImg";
            this._audioLevelsImg.Size = new System.Drawing.Size(25, 25);
            this._audioLevelsImg.TabIndex = 16;
            this._audioLevelsImg.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(217)))));
            this._audioLevelsImg.UseVisualStyleBackColor = true;
            // 
            // _playBtn
            // 
            this._playBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._playBtn.BackColor = System.Drawing.Color.Transparent;
            this._playBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.media_play_8x;
            this._playBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._playBtn.Checked = false;
            this._playBtn.CheckedTint = System.Drawing.Color.Empty;
            this._playBtn.FlatAppearance.BorderSize = 0;
            this._playBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._playBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._playBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._playBtn.Location = new System.Drawing.Point(161, 370);
            this._playBtn.Name = "_playBtn";
            this._playBtn.Size = new System.Drawing.Size(30, 30);
            this._playBtn.TabIndex = 15;
            this._playBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this._toolTipProvider.SetToolTip(this._playBtn, "Play the currently selected track or the first available track in the playing que" +
        "ue.");
            this._playBtn.UseVisualStyleBackColor = false;
            this._playBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // _volumeControlBar
            // 
            this._volumeControlBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._volumeControlBar.Location = new System.Drawing.Point(97, 414);
            this._volumeControlBar.Maximum = 100;
            this._volumeControlBar.Name = "_volumeControlBar";
            this._volumeControlBar.Size = new System.Drawing.Size(225, 45);
            this._volumeControlBar.TabIndex = 14;
            this._volumeControlBar.TickFrequency = 10;
            this._volumeControlBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this._toolTipProvider.SetToolTip(this._volumeControlBar, "Adjust the volume for all clients.");
            this._volumeControlBar.Value = 100;
            this._volumeControlBar.Scroll += new System.EventHandler(this.VolumeControlBar_Scroll);
            // 
            // _shuffleBtn
            // 
            this._shuffleBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._shuffleBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.random_8x;
            this._shuffleBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._shuffleBtn.Checked = false;
            this._shuffleBtn.CheckedTint = System.Drawing.Color.Empty;
            this._shuffleBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this._shuffleBtn.FlatAppearance.BorderSize = 0;
            this._shuffleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._shuffleBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._shuffleBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._shuffleBtn.Location = new System.Drawing.Point(346, 326);
            this._shuffleBtn.Name = "_shuffleBtn";
            this._shuffleBtn.Size = new System.Drawing.Size(25, 25);
            this._shuffleBtn.TabIndex = 12;
            this._shuffleBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this._toolTipProvider.SetToolTip(this._shuffleBtn, "Shuffle the tracks in the playing queue.");
            this._shuffleBtn.UseVisualStyleBackColor = true;
            this._shuffleBtn.Click += new System.EventHandler(this.ShuffleBtnClicked);
            // 
            // _previousBtn
            // 
            this._previousBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._previousBtn.BackColor = System.Drawing.Color.Transparent;
            this._previousBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.media_skip_backward_8x;
            this._previousBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._previousBtn.Checked = false;
            this._previousBtn.CheckedTint = System.Drawing.Color.Empty;
            this._previousBtn.FlatAppearance.BorderSize = 0;
            this._previousBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._previousBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._previousBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._previousBtn.ForeColor = System.Drawing.Color.White;
            this._previousBtn.Location = new System.Drawing.Point(116, 370);
            this._previousBtn.Name = "_previousBtn";
            this._previousBtn.Size = new System.Drawing.Size(30, 30);
            this._previousBtn.TabIndex = 9;
            this._previousBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._previousBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this._toolTipProvider.SetToolTip(this._previousBtn, "Play the previous track in the playing queue.");
            this._previousBtn.UseVisualStyleBackColor = false;
            this._previousBtn.Click += new System.EventHandler(this.PreviousBtn_Click);
            // 
            // _stopBtn
            // 
            this._stopBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._stopBtn.BackColor = System.Drawing.Color.Transparent;
            this._stopBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.media_stop_8x;
            this._stopBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._stopBtn.Checked = false;
            this._stopBtn.CheckedTint = System.Drawing.Color.Empty;
            this._stopBtn.FlatAppearance.BorderSize = 0;
            this._stopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._stopBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._stopBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._stopBtn.ForeColor = System.Drawing.Color.White;
            this._stopBtn.Location = new System.Drawing.Point(199, 370);
            this._stopBtn.Name = "_stopBtn";
            this._stopBtn.Size = new System.Drawing.Size(30, 30);
            this._stopBtn.TabIndex = 1;
            this._stopBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this._toolTipProvider.SetToolTip(this._stopBtn, "Stop playback across all clients");
            this._stopBtn.UseVisualStyleBackColor = false;
            this._stopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // _nextBtn
            // 
            this._nextBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._nextBtn.BackColor = System.Drawing.Color.Transparent;
            this._nextBtn.BackgroundImage = global::ZyncAudio.Properties.Resources.media_skip_forward_8x;
            this._nextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._nextBtn.Checked = false;
            this._nextBtn.CheckedTint = System.Drawing.Color.Empty;
            this._nextBtn.FlatAppearance.BorderSize = 0;
            this._nextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._nextBtn.FlipEffect = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this._nextBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._nextBtn.ForeColor = System.Drawing.Color.White;
            this._nextBtn.Location = new System.Drawing.Point(243, 370);
            this._nextBtn.Name = "_nextBtn";
            this._nextBtn.Size = new System.Drawing.Size(30, 30);
            this._nextBtn.TabIndex = 8;
            this._nextBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._nextBtn.Tint = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this._toolTipProvider.SetToolTip(this._nextBtn, "Play the next track in the playing queue.");
            this._nextBtn.UseVisualStyleBackColor = false;
            this._nextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // _toolTipProvider
            // 
            this._toolTipProvider.AutoPopDelay = 8000;
            this._toolTipProvider.InitialDelay = 500;
            this._toolTipProvider.ReshowDelay = 100;
            // 
            // HostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 490);
            this.Controls.Add(this._mediaGroupBox);
            this.Controls.Add(this._serverGroupBox);
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(661, 300);
            this.Name = "HostForm";
            this.ShowIcon = false;
            this.Text = "Zync Audio Host";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HostForm_FormClosing);
            this.Load += new System.EventHandler(this.HostForm_Load);
            this._serverGroupBox.ResumeLayout(false);
            this._mediaGroupBox.ResumeLayout(false);
            this._mediaGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._volumeControlBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button _closeEntryBtn;
        private System.Windows.Forms.ListView ClientListView;
        private System.Windows.Forms.Timer PingChecker;
        private System.Windows.Forms.ListBox _playListView;
        private System.Windows.Forms.Label label1;
        private TintableButton _loadFolderBtn;
        private TintableButton _unloadPlaylistBtn;
        private System.Windows.Forms.GroupBox _serverGroupBox;
        private System.Windows.Forms.GroupBox _mediaGroupBox;
        private TintableButton _shuffleBtn;
        private System.Windows.Forms.TrackBar _volumeControlBar;
        private TintableButton _playBtn;
        private TintableButton _stopBtn;
        private TintableButton _nextBtn;
        private TintableButton _previousBtn;
        private System.Windows.Forms.ToolTip _toolTipProvider;
        private TintableButton _audioLevelsImg;
        private TintableButton _searchSubFoldersBtn;
        private TintableButton _rerouteAudioBtn;
        private TintableButton _addSingleTrackBtn;
    }
}