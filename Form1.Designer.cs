namespace GTA_SA_Chaos
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonMainToggle = new System.Windows.Forms.Button();
            this.progressBarMain = new System.Windows.Forms.ProgressBar();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.buttonReset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxMainCooldown = new System.Windows.Forms.ComboBox();
            this.listLastEffectsMain = new System.Windows.Forms.ListBox();
            this.tabTwitch = new System.Windows.Forms.TabPage();
            this.labelTwitchCurrentMode = new System.Windows.Forms.Label();
            this.checkBoxTwitchAllowVoting = new System.Windows.Forms.CheckBox();
            this.buttonTwitchToggle = new System.Windows.Forms.Button();
            this.comboBoxVotingCooldown = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxVotingTime = new System.Windows.Forms.ComboBox();
            this.progressBarTwitch = new System.Windows.Forms.ProgressBar();
            this.checkBoxTwitchDontActivateEffects = new System.Windows.Forms.CheckBox();
            this.lastEffectsTwitch = new System.Windows.Forms.ListBox();
            this.checkBoxTwitchIsHost = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTwitchOAuth = new System.Windows.Forms.TextBox();
            this.textBoxTwitchUsername = new System.Windows.Forms.TextBox();
            this.buttonConnectTwitch = new System.Windows.Forms.Button();
            this.textBoxTwitchChannel = new System.Windows.Forms.TextBox();
            this.tabEffects = new System.Windows.Forms.TabPage();
            this.enabledEffectsView = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.presetComboBox = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxContinueTimer = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxSeed = new System.Windows.Forms.TextBox();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.buttonGenericTest = new System.Windows.Forms.Button();
            this.buttonTestSeed = new System.Windows.Forms.Button();
            this.labelTestSeed = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipHandler = new System.Windows.Forms.ToolTip(this.components);
            this.buttonAutoStart = new System.Windows.Forms.Button();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.buttonSwitchMode = new System.Windows.Forms.Button();
            this.checkBoxCrypticEffects = new System.Windows.Forms.CheckBox();
            this.tabSettings.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabTwitch.SuspendLayout();
            this.tabEffects.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMainToggle
            // 
            this.buttonMainToggle.Location = new System.Drawing.Point(6, 6);
            this.buttonMainToggle.Name = "buttonMainToggle";
            this.buttonMainToggle.Size = new System.Drawing.Size(75, 23);
            this.buttonMainToggle.TabIndex = 0;
            this.buttonMainToggle.Text = "Start";
            this.buttonMainToggle.UseVisualStyleBackColor = true;
            this.buttonMainToggle.Click += new System.EventHandler(this.ButtonMainToggle_Click);
            // 
            // progressBarMain
            // 
            this.progressBarMain.Location = new System.Drawing.Point(168, 6);
            this.progressBarMain.Maximum = 60;
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(278, 23);
            this.progressBarMain.Step = 1;
            this.progressBarMain.TabIndex = 1;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabMain);
            this.tabSettings.Controls.Add(this.tabTwitch);
            this.tabSettings.Controls.Add(this.tabEffects);
            this.tabSettings.Controls.Add(this.tabPage1);
            this.tabSettings.Controls.Add(this.tabDebug);
            this.tabSettings.Location = new System.Drawing.Point(12, 56);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(460, 257);
            this.tabSettings.TabIndex = 4;
            // 
            // tabMain
            // 
            this.tabMain.BackColor = System.Drawing.Color.Transparent;
            this.tabMain.Controls.Add(this.buttonReset);
            this.tabMain.Controls.Add(this.label2);
            this.tabMain.Controls.Add(this.comboBoxMainCooldown);
            this.tabMain.Controls.Add(this.buttonMainToggle);
            this.tabMain.Controls.Add(this.listLastEffectsMain);
            this.tabMain.Controls.Add(this.progressBarMain);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(452, 231);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            // 
            // buttonReset
            // 
            this.buttonReset.Enabled = false;
            this.buttonReset.Location = new System.Drawing.Point(87, 6);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 7;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(262, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Cooldown:";
            // 
            // comboBoxMainCooldown
            // 
            this.comboBoxMainCooldown.FormattingEnabled = true;
            this.comboBoxMainCooldown.Location = new System.Drawing.Point(325, 136);
            this.comboBoxMainCooldown.Name = "comboBoxMainCooldown";
            this.comboBoxMainCooldown.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMainCooldown.TabIndex = 5;
            this.comboBoxMainCooldown.SelectedIndexChanged += new System.EventHandler(this.MainCooldownComboBox_SelectedIndexChanged);
            // 
            // listLastEffectsMain
            // 
            this.listLastEffectsMain.FormattingEnabled = true;
            this.listLastEffectsMain.Location = new System.Drawing.Point(6, 35);
            this.listLastEffectsMain.Name = "listLastEffectsMain";
            this.listLastEffectsMain.Size = new System.Drawing.Size(440, 95);
            this.listLastEffectsMain.TabIndex = 4;
            // 
            // tabTwitch
            // 
            this.tabTwitch.BackColor = System.Drawing.Color.Transparent;
            this.tabTwitch.Controls.Add(this.labelTwitchCurrentMode);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchAllowVoting);
            this.tabTwitch.Controls.Add(this.buttonTwitchToggle);
            this.tabTwitch.Controls.Add(this.comboBoxVotingCooldown);
            this.tabTwitch.Controls.Add(this.label7);
            this.tabTwitch.Controls.Add(this.label6);
            this.tabTwitch.Controls.Add(this.comboBoxVotingTime);
            this.tabTwitch.Controls.Add(this.progressBarTwitch);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchDontActivateEffects);
            this.tabTwitch.Controls.Add(this.lastEffectsTwitch);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchIsHost);
            this.tabTwitch.Controls.Add(this.label5);
            this.tabTwitch.Controls.Add(this.label4);
            this.tabTwitch.Controls.Add(this.label3);
            this.tabTwitch.Controls.Add(this.textBoxTwitchOAuth);
            this.tabTwitch.Controls.Add(this.textBoxTwitchUsername);
            this.tabTwitch.Controls.Add(this.buttonConnectTwitch);
            this.tabTwitch.Controls.Add(this.textBoxTwitchChannel);
            this.tabTwitch.Location = new System.Drawing.Point(4, 22);
            this.tabTwitch.Name = "tabTwitch";
            this.tabTwitch.Size = new System.Drawing.Size(452, 231);
            this.tabTwitch.TabIndex = 2;
            this.tabTwitch.Text = "Twitch";
            // 
            // labelTwitchCurrentMode
            // 
            this.labelTwitchCurrentMode.AutoSize = true;
            this.labelTwitchCurrentMode.Location = new System.Drawing.Point(5, 208);
            this.labelTwitchCurrentMode.Name = "labelTwitchCurrentMode";
            this.labelTwitchCurrentMode.Size = new System.Drawing.Size(0, 13);
            this.labelTwitchCurrentMode.TabIndex = 17;
            // 
            // checkBoxTwitchAllowVoting
            // 
            this.checkBoxTwitchAllowVoting.AutoSize = true;
            this.checkBoxTwitchAllowVoting.Checked = true;
            this.checkBoxTwitchAllowVoting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTwitchAllowVoting.Location = new System.Drawing.Point(294, 119);
            this.checkBoxTwitchAllowVoting.Name = "checkBoxTwitchAllowVoting";
            this.checkBoxTwitchAllowVoting.Size = new System.Drawing.Size(109, 17);
            this.checkBoxTwitchAllowVoting.TabIndex = 16;
            this.checkBoxTwitchAllowVoting.Text = "Allow Chat Voting";
            this.toolTipHandler.SetToolTip(this.checkBoxTwitchAllowVoting, "When enabled the effects won\'t be sent to the game but instead only to announced " +
        "in chat.");
            this.checkBoxTwitchAllowVoting.UseVisualStyleBackColor = true;
            this.checkBoxTwitchAllowVoting.CheckedChanged += new System.EventHandler(this.CheckBoxAllowVoting_CheckedChanged);
            // 
            // buttonTwitchToggle
            // 
            this.buttonTwitchToggle.Enabled = false;
            this.buttonTwitchToggle.Location = new System.Drawing.Point(387, 28);
            this.buttonTwitchToggle.Name = "buttonTwitchToggle";
            this.buttonTwitchToggle.Size = new System.Drawing.Size(62, 23);
            this.buttonTwitchToggle.TabIndex = 15;
            this.buttonTwitchToggle.Text = "Start";
            this.buttonTwitchToggle.UseVisualStyleBackColor = true;
            this.buttonTwitchToggle.Click += new System.EventHandler(this.ButtonTwitchToggle_Click);
            // 
            // comboBoxVotingCooldown
            // 
            this.comboBoxVotingCooldown.FormattingEnabled = true;
            this.comboBoxVotingCooldown.Location = new System.Drawing.Point(294, 155);
            this.comboBoxVotingCooldown.Name = "comboBoxVotingCooldown";
            this.comboBoxVotingCooldown.Size = new System.Drawing.Size(155, 21);
            this.comboBoxVotingCooldown.TabIndex = 14;
            this.comboBoxVotingCooldown.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVotingCooldown_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(291, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Voting Cooldown:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(291, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Voting Time:";
            // 
            // comboBoxVotingTime
            // 
            this.comboBoxVotingTime.FormattingEnabled = true;
            this.comboBoxVotingTime.Location = new System.Drawing.Point(294, 97);
            this.comboBoxVotingTime.Name = "comboBoxVotingTime";
            this.comboBoxVotingTime.Size = new System.Drawing.Size(155, 21);
            this.comboBoxVotingTime.TabIndex = 11;
            this.comboBoxVotingTime.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVotingTime_SelectedIndexChanged);
            // 
            // progressBarTwitch
            // 
            this.progressBarTwitch.Location = new System.Drawing.Point(6, 182);
            this.progressBarTwitch.Name = "progressBarTwitch";
            this.progressBarTwitch.Size = new System.Drawing.Size(443, 23);
            this.progressBarTwitch.TabIndex = 10;
            // 
            // checkBoxTwitchDontActivateEffects
            // 
            this.checkBoxTwitchDontActivateEffects.AutoSize = true;
            this.checkBoxTwitchDontActivateEffects.Location = new System.Drawing.Point(294, 57);
            this.checkBoxTwitchDontActivateEffects.Name = "checkBoxTwitchDontActivateEffects";
            this.checkBoxTwitchDontActivateEffects.Size = new System.Drawing.Size(129, 17);
            this.checkBoxTwitchDontActivateEffects.TabIndex = 9;
            this.checkBoxTwitchDontActivateEffects.Text = "Don\'t Activate Effects";
            this.toolTipHandler.SetToolTip(this.checkBoxTwitchDontActivateEffects, "When enabled the effects won\'t be sent to the game but instead only to announced " +
        "in chat.");
            this.checkBoxTwitchDontActivateEffects.UseVisualStyleBackColor = true;
            this.checkBoxTwitchDontActivateEffects.CheckedChanged += new System.EventHandler(this.CheckBoxDontActivateEffects_CheckedChanged);
            // 
            // lastEffectsTwitch
            // 
            this.lastEffectsTwitch.FormattingEnabled = true;
            this.lastEffectsTwitch.Location = new System.Drawing.Point(6, 81);
            this.lastEffectsTwitch.Name = "lastEffectsTwitch";
            this.lastEffectsTwitch.Size = new System.Drawing.Size(282, 95);
            this.lastEffectsTwitch.TabIndex = 8;
            // 
            // checkBoxTwitchIsHost
            // 
            this.checkBoxTwitchIsHost.AutoSize = true;
            this.checkBoxTwitchIsHost.Location = new System.Drawing.Point(294, 31);
            this.checkBoxTwitchIsHost.Name = "checkBoxTwitchIsHost";
            this.checkBoxTwitchIsHost.Size = new System.Drawing.Size(59, 17);
            this.checkBoxTwitchIsHost.TabIndex = 7;
            this.checkBoxTwitchIsHost.Text = "Is Host";
            this.checkBoxTwitchIsHost.UseVisualStyleBackColor = true;
            this.checkBoxTwitchIsHost.CheckedChanged += new System.EventHandler(this.CheckBoxHost_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "OAuth Token:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Channel:";
            // 
            // textBoxTwitchOAuth
            // 
            this.textBoxTwitchOAuth.Enabled = false;
            this.textBoxTwitchOAuth.Location = new System.Drawing.Point(83, 55);
            this.textBoxTwitchOAuth.Name = "textBoxTwitchOAuth";
            this.textBoxTwitchOAuth.PasswordChar = '*';
            this.textBoxTwitchOAuth.Size = new System.Drawing.Size(205, 20);
            this.textBoxTwitchOAuth.TabIndex = 3;
            this.textBoxTwitchOAuth.TextChanged += new System.EventHandler(this.TextBoxOAuth_TextChanged);
            // 
            // textBoxTwitchUsername
            // 
            this.textBoxTwitchUsername.Enabled = false;
            this.textBoxTwitchUsername.Location = new System.Drawing.Point(83, 28);
            this.textBoxTwitchUsername.Name = "textBoxTwitchUsername";
            this.textBoxTwitchUsername.Size = new System.Drawing.Size(205, 20);
            this.textBoxTwitchUsername.TabIndex = 2;
            this.textBoxTwitchUsername.TextChanged += new System.EventHandler(this.TextBoxUsername_TextChanged);
            // 
            // buttonConnectTwitch
            // 
            this.buttonConnectTwitch.Enabled = false;
            this.buttonConnectTwitch.Location = new System.Drawing.Point(294, 2);
            this.buttonConnectTwitch.Name = "buttonConnectTwitch";
            this.buttonConnectTwitch.Size = new System.Drawing.Size(155, 23);
            this.buttonConnectTwitch.TabIndex = 1;
            this.buttonConnectTwitch.Text = "Connect to Twitch";
            this.buttonConnectTwitch.UseVisualStyleBackColor = true;
            this.buttonConnectTwitch.Click += new System.EventHandler(this.ButtonConnectTwitch_Click);
            // 
            // textBoxTwitchChannel
            // 
            this.textBoxTwitchChannel.Location = new System.Drawing.Point(83, 3);
            this.textBoxTwitchChannel.Name = "textBoxTwitchChannel";
            this.textBoxTwitchChannel.Size = new System.Drawing.Size(205, 20);
            this.textBoxTwitchChannel.TabIndex = 0;
            this.textBoxTwitchChannel.TextChanged += new System.EventHandler(this.TextBoxChannel_TextChanged);
            // 
            // tabEffects
            // 
            this.tabEffects.BackColor = System.Drawing.Color.Transparent;
            this.tabEffects.Controls.Add(this.enabledEffectsView);
            this.tabEffects.Controls.Add(this.label1);
            this.tabEffects.Controls.Add(this.presetComboBox);
            this.tabEffects.Location = new System.Drawing.Point(4, 22);
            this.tabEffects.Name = "tabEffects";
            this.tabEffects.Padding = new System.Windows.Forms.Padding(3);
            this.tabEffects.Size = new System.Drawing.Size(452, 231);
            this.tabEffects.TabIndex = 1;
            this.tabEffects.Text = "Effects";
            // 
            // enabledEffectsView
            // 
            this.enabledEffectsView.CheckBoxes = true;
            this.enabledEffectsView.Location = new System.Drawing.Point(6, 6);
            this.enabledEffectsView.Name = "enabledEffectsView";
            this.enabledEffectsView.Size = new System.Drawing.Size(440, 163);
            this.enabledEffectsView.TabIndex = 3;
            this.enabledEffectsView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.EnabledEffectsView_AfterCheck);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Presets:";
            // 
            // presetComboBox
            // 
            this.presetComboBox.FormattingEnabled = true;
            this.presetComboBox.Location = new System.Drawing.Point(57, 175);
            this.presetComboBox.Name = "presetComboBox";
            this.presetComboBox.Size = new System.Drawing.Size(389, 21);
            this.presetComboBox.TabIndex = 1;
            this.presetComboBox.SelectedIndexChanged += new System.EventHandler(this.PresetComboBox_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.checkBoxCrypticEffects);
            this.tabPage1.Controls.Add(this.checkBoxContinueTimer);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.textBoxSeed);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(452, 231);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Settings";
            // 
            // checkBoxContinueTimer
            // 
            this.checkBoxContinueTimer.AutoSize = true;
            this.checkBoxContinueTimer.Location = new System.Drawing.Point(6, 208);
            this.checkBoxContinueTimer.Name = "checkBoxContinueTimer";
            this.checkBoxContinueTimer.Size = new System.Drawing.Size(210, 17);
            this.checkBoxContinueTimer.TabIndex = 4;
            this.checkBoxContinueTimer.Text = "Continue Timer on Game Close / Crash";
            this.checkBoxContinueTimer.UseVisualStyleBackColor = true;
            this.checkBoxContinueTimer.CheckedChanged += new System.EventHandler(this.CheckBoxContinueTimer_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Seed:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxSeed
            // 
            this.textBoxSeed.Location = new System.Drawing.Point(47, 6);
            this.textBoxSeed.Name = "textBoxSeed";
            this.textBoxSeed.Size = new System.Drawing.Size(399, 20);
            this.textBoxSeed.TabIndex = 1;
            this.textBoxSeed.TextChanged += new System.EventHandler(this.TextBoxSeed_TextChanged);
            // 
            // tabDebug
            // 
            this.tabDebug.BackColor = System.Drawing.Color.Transparent;
            this.tabDebug.Controls.Add(this.buttonGenericTest);
            this.tabDebug.Controls.Add(this.buttonTestSeed);
            this.tabDebug.Controls.Add(this.labelTestSeed);
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(452, 231);
            this.tabDebug.TabIndex = 4;
            this.tabDebug.Text = "Debug";
            // 
            // buttonGenericTest
            // 
            this.buttonGenericTest.Location = new System.Drawing.Point(6, 202);
            this.buttonGenericTest.Name = "buttonGenericTest";
            this.buttonGenericTest.Size = new System.Drawing.Size(91, 23);
            this.buttonGenericTest.TabIndex = 10;
            this.buttonGenericTest.Text = "Test Something";
            this.buttonGenericTest.UseVisualStyleBackColor = true;
            this.buttonGenericTest.Click += new System.EventHandler(this.ButtonGenericTest_Click);
            // 
            // buttonTestSeed
            // 
            this.buttonTestSeed.Location = new System.Drawing.Point(3, 6);
            this.buttonTestSeed.Name = "buttonTestSeed";
            this.buttonTestSeed.Size = new System.Drawing.Size(75, 23);
            this.buttonTestSeed.TabIndex = 9;
            this.buttonTestSeed.Text = "Test Seed";
            this.buttonTestSeed.UseVisualStyleBackColor = true;
            this.buttonTestSeed.Click += new System.EventHandler(this.ButtonTestSeed_Click);
            // 
            // labelTestSeed
            // 
            this.labelTestSeed.AutoSize = true;
            this.labelTestSeed.Location = new System.Drawing.Point(84, 11);
            this.labelTestSeed.Name = "labelTestSeed";
            this.labelTestSeed.Size = new System.Drawing.Size(13, 13);
            this.labelTestSeed.TabIndex = 8;
            this.labelTestSeed.Text = "0";
            this.labelTestSeed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(482, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPresetToolStripMenuItem,
            this.savePresetToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadPresetToolStripMenuItem
            // 
            this.loadPresetToolStripMenuItem.Name = "loadPresetToolStripMenuItem";
            this.loadPresetToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.loadPresetToolStripMenuItem.Text = "Load Preset";
            this.loadPresetToolStripMenuItem.Click += new System.EventHandler(this.LoadPresetToolStripMenuItem_Click);
            // 
            // savePresetToolStripMenuItem
            // 
            this.savePresetToolStripMenuItem.Name = "savePresetToolStripMenuItem";
            this.savePresetToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.savePresetToolStripMenuItem.Text = "Save Preset";
            this.savePresetToolStripMenuItem.Click += new System.EventHandler(this.SavePresetToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // buttonAutoStart
            // 
            this.buttonAutoStart.Location = new System.Drawing.Point(12, 27);
            this.buttonAutoStart.Name = "buttonAutoStart";
            this.buttonAutoStart.Size = new System.Drawing.Size(75, 23);
            this.buttonAutoStart.TabIndex = 6;
            this.buttonAutoStart.Text = "Auto-Start";
            this.buttonAutoStart.UseVisualStyleBackColor = true;
            this.buttonAutoStart.Click += new System.EventHandler(this.ButtonAutoStart_Click);
            // 
            // timerMain
            // 
            this.timerMain.Enabled = true;
            this.timerMain.Interval = 10;
            this.timerMain.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // buttonSwitchMode
            // 
            this.buttonSwitchMode.Location = new System.Drawing.Point(393, 27);
            this.buttonSwitchMode.Name = "buttonSwitchMode";
            this.buttonSwitchMode.Size = new System.Drawing.Size(75, 23);
            this.buttonSwitchMode.TabIndex = 7;
            this.buttonSwitchMode.Text = "Twitch";
            this.buttonSwitchMode.UseVisualStyleBackColor = true;
            this.buttonSwitchMode.Click += new System.EventHandler(this.ButtonSwitchMode_Click);
            // 
            // checkBoxCrypticEffects
            // 
            this.checkBoxCrypticEffects.AutoSize = true;
            this.checkBoxCrypticEffects.Location = new System.Drawing.Point(6, 185);
            this.checkBoxCrypticEffects.Name = "checkBoxCrypticEffects";
            this.checkBoxCrypticEffects.Size = new System.Drawing.Size(94, 17);
            this.checkBoxCrypticEffects.TabIndex = 5;
            this.checkBoxCrypticEffects.Text = "Cryptic Effects";
            this.checkBoxCrypticEffects.UseVisualStyleBackColor = true;
            this.checkBoxCrypticEffects.CheckedChanged += new System.EventHandler(this.CheckBoxCrypticEffects_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 325);
            this.Controls.Add(this.buttonSwitchMode);
            this.Controls.Add(this.buttonAutoStart);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "GTA:SA Chaos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabSettings.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
            this.tabTwitch.ResumeLayout(false);
            this.tabTwitch.PerformLayout();
            this.tabEffects.ResumeLayout(false);
            this.tabEffects.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonMainToggle;
        private System.Windows.Forms.ProgressBar progressBarMain;
        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabEffects;
        private System.Windows.Forms.TreeView enabledEffectsView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox presetComboBox;
        private System.Windows.Forms.ListBox listLastEffectsMain;
        private System.Windows.Forms.ComboBox comboBoxMainCooldown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabTwitch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTwitchOAuth;
        private System.Windows.Forms.TextBox textBoxTwitchUsername;
        private System.Windows.Forms.Button buttonConnectTwitch;
        private System.Windows.Forms.TextBox textBoxTwitchChannel;
        private System.Windows.Forms.CheckBox checkBoxTwitchIsHost;
        private System.Windows.Forms.CheckBox checkBoxTwitchDontActivateEffects;
        private System.Windows.Forms.ListBox lastEffectsTwitch;
        private System.Windows.Forms.ToolTip toolTipHandler;
        private System.Windows.Forms.ComboBox comboBoxVotingCooldown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxVotingTime;
        private System.Windows.Forms.ProgressBar progressBarTwitch;
        private System.Windows.Forms.Button buttonTwitchToggle;
        private System.Windows.Forms.Button buttonAutoStart;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.Button buttonSwitchMode;
        private System.Windows.Forms.CheckBox checkBoxTwitchAllowVoting;
        private System.Windows.Forms.Label labelTwitchCurrentMode;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxSeed;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.Button buttonTestSeed;
        private System.Windows.Forms.Label labelTestSeed;
        private System.Windows.Forms.Button buttonGenericTest;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.CheckBox checkBoxContinueTimer;
        private System.Windows.Forms.CheckBox checkBoxCrypticEffects;
    }
}

