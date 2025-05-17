using System.Windows.Forms;

namespace OOBEMusicSetup
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fileSelectButton = new System.Windows.Forms.Button();
            this.filePathBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.superVerboseCheck = new System.Windows.Forms.CheckBox();
            this.OOBEHostAppCheck = new System.Windows.Forms.CheckBox();
            this.WWAHostCheck = new System.Windows.Forms.CheckBox();
            this.firstLogonCheck = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.threadTimeoutNumber = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.serviceStatusLabel = new System.Windows.Forms.Label();
            this.disableServiceButton = new System.Windows.Forms.Button();
            this.stopServiceButton = new System.Windows.Forms.Button();
            this.restartServiceButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadTimeoutNumber)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.fileSelectButton);
            this.groupBox1.Controls.Add(this.filePathBox);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(637, 67);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Music File";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(595, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select a music file here, supported : .mp3, .wav, .brstm, .at9 or any file suppor" +
    "ted by vgmstream or a playlist file (.m3u, .m3u8).";
            // 
            // fileSelectButton
            // 
            this.fileSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelectButton.Location = new System.Drawing.Point(556, 20);
            this.fileSelectButton.Name = "fileSelectButton";
            this.fileSelectButton.Size = new System.Drawing.Size(75, 23);
            this.fileSelectButton.TabIndex = 1;
            this.fileSelectButton.Text = "Select";
            this.fileSelectButton.UseVisualStyleBackColor = true;
            this.fileSelectButton.Click += new System.EventHandler(this.fileSelectButton_Click);
            // 
            // filePathBox
            // 
            this.filePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathBox.Location = new System.Drawing.Point(7, 20);
            this.filePathBox.Name = "filePathBox";
            this.filePathBox.ReadOnly = true;
            this.filePathBox.Size = new System.Drawing.Size(543, 20);
            this.filePathBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.superVerboseCheck);
            this.groupBox2.Controls.Add(this.OOBEHostAppCheck);
            this.groupBox2.Controls.Add(this.WWAHostCheck);
            this.groupBox2.Controls.Add(this.firstLogonCheck);
            this.groupBox2.Location = new System.Drawing.Point(13, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(637, 114);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Music Playback";
            // 
            // superVerboseCheck
            // 
            this.superVerboseCheck.AutoSize = true;
            this.superVerboseCheck.Location = new System.Drawing.Point(10, 90);
            this.superVerboseCheck.Name = "superVerboseCheck";
            this.superVerboseCheck.Size = new System.Drawing.Size(231, 17);
            this.superVerboseCheck.TabIndex = 3;
            this.superVerboseCheck.Text = "Enable \"Super Verbose Logs\" (deprecated)";
            this.superVerboseCheck.UseVisualStyleBackColor = true;
            this.superVerboseCheck.CheckedChanged += new System.EventHandler(this.superVerboseCheck_CheckedChanged);
            // 
            // OOBEHostAppCheck
            // 
            this.OOBEHostAppCheck.AutoSize = true;
            this.OOBEHostAppCheck.Location = new System.Drawing.Point(10, 67);
            this.OOBEHostAppCheck.Name = "OOBEHostAppCheck";
            this.OOBEHostAppCheck.Size = new System.Drawing.Size(333, 17);
            this.OOBEHostAppCheck.TabIndex = 2;
            this.OOBEHostAppCheck.Text = "Enable playback during OneDrive first setup screen during OOBE";
            this.OOBEHostAppCheck.UseVisualStyleBackColor = true;
            this.OOBEHostAppCheck.CheckedChanged += new System.EventHandler(this.OOBEHostAppCheck_CheckedChanged);
            // 
            // WWAHostCheck
            // 
            this.WWAHostCheck.AutoSize = true;
            this.WWAHostCheck.Location = new System.Drawing.Point(10, 43);
            this.WWAHostCheck.Name = "WWAHostCheck";
            this.WWAHostCheck.Size = new System.Drawing.Size(303, 17);
            this.WWAHostCheck.TabIndex = 1;
            this.WWAHostCheck.Text = "Enable playback during the OOBE screen (WWAHost.exe)";
            this.WWAHostCheck.UseVisualStyleBackColor = true;
            this.WWAHostCheck.CheckedChanged += new System.EventHandler(this.WWAHostCheck_CheckedChanged);
            // 
            // firstLogonCheck
            // 
            this.firstLogonCheck.AutoSize = true;
            this.firstLogonCheck.Location = new System.Drawing.Point(10, 20);
            this.firstLogonCheck.Name = "firstLogonCheck";
            this.firstLogonCheck.Size = new System.Drawing.Size(259, 17);
            this.firstLogonCheck.TabIndex = 0;
            this.firstLogonCheck.Text = "Enable playback during the First Logon Animation";
            this.firstLogonCheck.UseVisualStyleBackColor = true;
            this.firstLogonCheck.CheckedChanged += new System.EventHandler(this.firstLogonCheck_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.threadTimeoutNumber);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(13, 206);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(637, 65);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Thread Timeout";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(136, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "milliseconds";
            // 
            // threadTimeoutNumber
            // 
            this.threadTimeoutNumber.Location = new System.Drawing.Point(10, 33);
            this.threadTimeoutNumber.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.threadTimeoutNumber.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.threadTimeoutNumber.Name = "threadTimeoutNumber";
            this.threadTimeoutNumber.Size = new System.Drawing.Size(120, 20);
            this.threadTimeoutNumber.TabIndex = 1;
            this.threadTimeoutNumber.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.threadTimeoutNumber.ValueChanged += new System.EventHandler(this.threadTimeoutNumber_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(390, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "The thread timeout controls how frequently the service checks for any processes.";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Controls.Add(this.button4);
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.serviceStatusLabel);
            this.groupBox4.Controls.Add(this.disableServiceButton);
            this.groupBox4.Controls.Add(this.stopServiceButton);
            this.groupBox4.Controls.Add(this.restartServiceButton);
            this.groupBox4.Location = new System.Drawing.Point(13, 277);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(637, 75);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Service Control";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(397, 46);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(91, 23);
            this.button5.TabIndex = 8;
            this.button5.Text = "Reset Settings";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(494, 46);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(117, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "Test Application";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(494, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(117, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Open Event Viewer";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(106, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Start Service";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Location = new System.Drawing.Point(397, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Enable Service";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // serviceStatusLabel
            // 
            this.serviceStatusLabel.AutoSize = true;
            this.serviceStatusLabel.Location = new System.Drawing.Point(7, 46);
            this.serviceStatusLabel.Name = "serviceStatusLabel";
            this.serviceStatusLabel.Size = new System.Drawing.Size(85, 13);
            this.serviceStatusLabel.TabIndex = 3;
            this.serviceStatusLabel.Text = "Service Status : ";
            // 
            // disableServiceButton
            // 
            this.disableServiceButton.AutoSize = true;
            this.disableServiceButton.Location = new System.Drawing.Point(300, 19);
            this.disableServiceButton.Name = "disableServiceButton";
            this.disableServiceButton.Size = new System.Drawing.Size(91, 23);
            this.disableServiceButton.TabIndex = 2;
            this.disableServiceButton.Text = "Disable Service";
            this.disableServiceButton.UseVisualStyleBackColor = true;
            this.disableServiceButton.Click += new System.EventHandler(this.disableServiceButton_Click);
            // 
            // stopServiceButton
            // 
            this.stopServiceButton.AutoSize = true;
            this.stopServiceButton.Location = new System.Drawing.Point(205, 19);
            this.stopServiceButton.Name = "stopServiceButton";
            this.stopServiceButton.Size = new System.Drawing.Size(89, 23);
            this.stopServiceButton.TabIndex = 1;
            this.stopServiceButton.Text = "Stop Service";
            this.stopServiceButton.UseVisualStyleBackColor = true;
            this.stopServiceButton.Click += new System.EventHandler(this.stopServiceButton_Click);
            // 
            // restartServiceButton
            // 
            this.restartServiceButton.AutoSize = true;
            this.restartServiceButton.Location = new System.Drawing.Point(6, 20);
            this.restartServiceButton.Name = "restartServiceButton";
            this.restartServiceButton.Size = new System.Drawing.Size(94, 23);
            this.restartServiceButton.TabIndex = 0;
            this.restartServiceButton.Text = "Restart Service";
            this.restartServiceButton.UseVisualStyleBackColor = true;
            this.restartServiceButton.Click += new System.EventHandler(this.restartServiceButton_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 364);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(678, 403);
            this.Name = "Form1";
            this.Text = "OOBE Music Player Settings";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadTimeoutNumber)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button fileSelectButton;
        private System.Windows.Forms.TextBox filePathBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox firstLogonCheck;
        private System.Windows.Forms.CheckBox OOBEHostAppCheck;
        private System.Windows.Forms.CheckBox WWAHostCheck;
        private System.Windows.Forms.CheckBox superVerboseCheck;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown threadTimeoutNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label serviceStatusLabel;
        private System.Windows.Forms.Button disableServiceButton;
        private System.Windows.Forms.Button stopServiceButton;
        private System.Windows.Forms.Button restartServiceButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;

    }
}

