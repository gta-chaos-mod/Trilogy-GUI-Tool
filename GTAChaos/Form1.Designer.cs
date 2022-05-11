namespace GTAChaos.Forms
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.checkBoxShowLastEffectsMain = new System.Windows.Forms.CheckBox();
            this.buttonResetMain = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxMainCooldown = new System.Windows.Forms.ComboBox();
            this.listLastEffectsMain = new System.Windows.Forms.ListBox();
            this.tabTwitch = new System.Windows.Forms.TabPage();
            this.labelTwitchAccessToken = new System.Windows.Forms.Label();
            this.linkLabelTwitchGetAccessToken = new System.Windows.Forms.LinkLabel();
            this.checkBoxTwitchCombineVotingMessages = new System.Windows.Forms.CheckBox();
            this.checkBoxTwitchUsePolls = new System.Windows.Forms.CheckBox();
            this.checkBoxTwitchMajorityVotes = new System.Windows.Forms.CheckBox();
            this.checkBoxTwitchEnableRapidFire = new System.Windows.Forms.CheckBox();
            this.checkBoxTwitchEnableMultipleEffects = new System.Windows.Forms.CheckBox();
            this.checkBoxTwitch3TimesCooldown = new System.Windows.Forms.CheckBox();
            this.buttonResetTwitch = new System.Windows.Forms.Button();
            this.checkBoxTwitchAllowOnlyEnabledEffects = new System.Windows.Forms.CheckBox();
            this.checkBoxShowLastEffectsTwitch = new System.Windows.Forms.CheckBox();
            this.labelTwitchCurrentMode = new System.Windows.Forms.Label();
            this.buttonTwitchToggle = new System.Windows.Forms.Button();
            this.comboBoxVotingCooldown = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxVotingTime = new System.Windows.Forms.ComboBox();
            this.progressBarTwitch = new System.Windows.Forms.ProgressBar();
            this.listLastEffectsTwitch = new System.Windows.Forms.ListBox();
            this.textBoxTwitchAccessToken = new System.Windows.Forms.TextBox();
            this.buttonConnectTwitch = new System.Windows.Forms.Button();
            this.tabPolls = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownTwitchPollsChannelPointsCost = new System.Windows.Forms.NumericUpDown();
            this.checkBoxTwitchPollsPostMessages = new System.Windows.Forms.CheckBox();
            this.labelTwitchPollsBitsCost = new System.Windows.Forms.Label();
            this.numericUpDownTwitchPollsBitsCost = new System.Windows.Forms.NumericUpDown();
            this.tabEffects = new System.Windows.Forms.TabPage();
            this.buttonEffectsToggleAll = new System.Windows.Forms.Button();
            this.enabledEffectsView = new System.Windows.Forms.TreeView();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.checkBoxPlayAudioForEffects = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxSeed = new System.Windows.Forms.TextBox();
            this.tabMultiplayer = new System.Windows.Forms.TabPage();
            this.buttonMultiplayerSend = new System.Windows.Forms.Button();
            this.textBoxMultiplayerChat = new System.Windows.Forms.TextBox();
            this.listBoxMultiplayerChat = new System.Windows.Forms.ListBox();
            this.labelMultiplayerHost = new System.Windows.Forms.Label();
            this.textBoxMultiplayerUsername = new System.Windows.Forms.TextBox();
            this.labelMultiplayerUsername = new System.Windows.Forms.Label();
            this.labelMultiplayerChannel = new System.Windows.Forms.Label();
            this.textBoxMultiplayerChannel = new System.Windows.Forms.TextBox();
            this.labelMultiplayerServer = new System.Windows.Forms.Label();
            this.textBoxMultiplayerServer = new System.Windows.Forms.TextBox();
            this.buttonMultiplayerConnect = new System.Windows.Forms.Button();
            this.tabExperimental = new System.Windows.Forms.TabPage();
            this.buttonExperimentalRunEffect = new System.Windows.Forms.Button();
            this.textBoxExperimentalEffectName = new System.Windows.Forms.TextBox();
            this.checkBoxExperimental_RunEffectOnAutoStart = new System.Windows.Forms.CheckBox();
            this.checkBoxExperimental_EnableAllEffects = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoStart = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viceCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sanAndreasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipHandler = new System.Windows.Forms.ToolTip(this.components);
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.buttonSwitchMode = new System.Windows.Forms.Button();
            this.tabs.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabTwitch.SuspendLayout();
            this.tabPolls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsChannelPointsCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsBitsCost)).BeginInit();
            this.tabEffects.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabMultiplayer.SuspendLayout();
            this.tabExperimental.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMainToggle
            // 
            this.buttonMainToggle.Location = new System.Drawing.Point(6, 6);
            this.buttonMainToggle.Name = "buttonMainToggle";
            this.buttonMainToggle.Size = new System.Drawing.Size(94, 23);
            this.buttonMainToggle.TabIndex = 0;
            this.buttonMainToggle.Text = "Start / Resume";
            this.buttonMainToggle.UseVisualStyleBackColor = true;
            this.buttonMainToggle.Click += new System.EventHandler(this.ButtonMainToggle_Click);
            // 
            // progressBarMain
            // 
            this.progressBarMain.Location = new System.Drawing.Point(206, 6);
            this.progressBarMain.Maximum = 60;
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(338, 23);
            this.progressBarMain.Step = 1;
            this.progressBarMain.TabIndex = 1;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabMain);
            this.tabs.Controls.Add(this.tabTwitch);
            this.tabs.Controls.Add(this.tabPolls);
            this.tabs.Controls.Add(this.tabEffects);
            this.tabs.Controls.Add(this.tabSettings);
            this.tabs.Controls.Add(this.tabMultiplayer);
            this.tabs.Controls.Add(this.tabExperimental);
            this.tabs.Location = new System.Drawing.Point(0, 41);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(560, 319);
            this.tabs.TabIndex = 4;
            // 
            // tabMain
            // 
            this.tabMain.BackColor = System.Drawing.Color.Transparent;
            this.tabMain.Controls.Add(this.checkBoxShowLastEffectsMain);
            this.tabMain.Controls.Add(this.buttonResetMain);
            this.tabMain.Controls.Add(this.label2);
            this.tabMain.Controls.Add(this.comboBoxMainCooldown);
            this.tabMain.Controls.Add(this.buttonMainToggle);
            this.tabMain.Controls.Add(this.listLastEffectsMain);
            this.tabMain.Controls.Add(this.progressBarMain);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(552, 293);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            // 
            // checkBoxShowLastEffectsMain
            // 
            this.checkBoxShowLastEffectsMain.AutoSize = true;
            this.checkBoxShowLastEffectsMain.Checked = true;
            this.checkBoxShowLastEffectsMain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowLastEffectsMain.Location = new System.Drawing.Point(6, 111);
            this.checkBoxShowLastEffectsMain.Name = "checkBoxShowLastEffectsMain";
            this.checkBoxShowLastEffectsMain.Size = new System.Drawing.Size(112, 17);
            this.checkBoxShowLastEffectsMain.TabIndex = 8;
            this.checkBoxShowLastEffectsMain.Text = "Show Last Effects";
            this.checkBoxShowLastEffectsMain.UseVisualStyleBackColor = true;
            this.checkBoxShowLastEffectsMain.CheckedChanged += new System.EventHandler(this.CheckBoxShowLastEffectsMain_CheckedChanged);
            // 
            // buttonResetMain
            // 
            this.buttonResetMain.Location = new System.Drawing.Point(106, 6);
            this.buttonResetMain.Name = "buttonResetMain";
            this.buttonResetMain.Size = new System.Drawing.Size(94, 23);
            this.buttonResetMain.TabIndex = 7;
            this.buttonResetMain.Text = "Reset";
            this.buttonResetMain.UseVisualStyleBackColor = true;
            this.buttonResetMain.Click += new System.EventHandler(this.ButtonResetMain_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(360, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Cooldown:";
            // 
            // comboBoxMainCooldown
            // 
            this.comboBoxMainCooldown.FormattingEnabled = true;
            this.comboBoxMainCooldown.Location = new System.Drawing.Point(423, 35);
            this.comboBoxMainCooldown.Name = "comboBoxMainCooldown";
            this.comboBoxMainCooldown.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMainCooldown.TabIndex = 5;
            this.comboBoxMainCooldown.SelectedIndexChanged += new System.EventHandler(this.MainCooldownComboBox_SelectedIndexChanged);
            // 
            // listLastEffectsMain
            // 
            this.listLastEffectsMain.FormattingEnabled = true;
            this.listLastEffectsMain.Location = new System.Drawing.Point(6, 134);
            this.listLastEffectsMain.Name = "listLastEffectsMain";
            this.listLastEffectsMain.Size = new System.Drawing.Size(538, 147);
            this.listLastEffectsMain.TabIndex = 4;
            // 
            // tabTwitch
            // 
            this.tabTwitch.BackColor = System.Drawing.Color.Transparent;
            this.tabTwitch.Controls.Add(this.labelTwitchAccessToken);
            this.tabTwitch.Controls.Add(this.linkLabelTwitchGetAccessToken);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchCombineVotingMessages);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchUsePolls);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchMajorityVotes);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchEnableRapidFire);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchEnableMultipleEffects);
            this.tabTwitch.Controls.Add(this.checkBoxTwitch3TimesCooldown);
            this.tabTwitch.Controls.Add(this.buttonResetTwitch);
            this.tabTwitch.Controls.Add(this.checkBoxTwitchAllowOnlyEnabledEffects);
            this.tabTwitch.Controls.Add(this.checkBoxShowLastEffectsTwitch);
            this.tabTwitch.Controls.Add(this.labelTwitchCurrentMode);
            this.tabTwitch.Controls.Add(this.buttonTwitchToggle);
            this.tabTwitch.Controls.Add(this.comboBoxVotingCooldown);
            this.tabTwitch.Controls.Add(this.label7);
            this.tabTwitch.Controls.Add(this.label6);
            this.tabTwitch.Controls.Add(this.comboBoxVotingTime);
            this.tabTwitch.Controls.Add(this.progressBarTwitch);
            this.tabTwitch.Controls.Add(this.listLastEffectsTwitch);
            this.tabTwitch.Controls.Add(this.textBoxTwitchAccessToken);
            this.tabTwitch.Controls.Add(this.buttonConnectTwitch);
            this.tabTwitch.Location = new System.Drawing.Point(4, 22);
            this.tabTwitch.Name = "tabTwitch";
            this.tabTwitch.Size = new System.Drawing.Size(550, 292);
            this.tabTwitch.TabIndex = 2;
            this.tabTwitch.Text = "Twitch";
            // 
            // labelTwitchAccessToken
            // 
            this.labelTwitchAccessToken.AutoSize = true;
            this.labelTwitchAccessToken.Location = new System.Drawing.Point(25, 11);
            this.labelTwitchAccessToken.Name = "labelTwitchAccessToken";
            this.labelTwitchAccessToken.Size = new System.Drawing.Size(79, 13);
            this.labelTwitchAccessToken.TabIndex = 31;
            this.labelTwitchAccessToken.Text = "Access Token:";
            // 
            // linkLabelTwitchGetAccessToken
            // 
            this.linkLabelTwitchGetAccessToken.AutoSize = true;
            this.linkLabelTwitchGetAccessToken.Location = new System.Drawing.Point(448, 35);
            this.linkLabelTwitchGetAccessToken.Name = "linkLabelTwitchGetAccessToken";
            this.linkLabelTwitchGetAccessToken.Size = new System.Drawing.Size(96, 13);
            this.linkLabelTwitchGetAccessToken.TabIndex = 30;
            this.linkLabelTwitchGetAccessToken.TabStop = true;
            this.linkLabelTwitchGetAccessToken.Text = "Get Access Token";
            this.linkLabelTwitchGetAccessToken.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelTwitchGetAccessToken_LinkClicked);
            // 
            // checkBoxTwitchCombineVotingMessages
            // 
            this.checkBoxTwitchCombineVotingMessages.AutoSize = true;
            this.checkBoxTwitchCombineVotingMessages.Location = new System.Drawing.Point(124, 121);
            this.checkBoxTwitchCombineVotingMessages.Name = "checkBoxTwitchCombineVotingMessages";
            this.checkBoxTwitchCombineVotingMessages.Size = new System.Drawing.Size(186, 17);
            this.checkBoxTwitchCombineVotingMessages.TabIndex = 29;
            this.checkBoxTwitchCombineVotingMessages.Text = "Combine Twitch Voting Messages";
            this.checkBoxTwitchCombineVotingMessages.UseVisualStyleBackColor = true;
            // 
            // checkBoxTwitchUsePolls
            // 
            this.checkBoxTwitchUsePolls.AutoSize = true;
            this.checkBoxTwitchUsePolls.Location = new System.Drawing.Point(391, 64);
            this.checkBoxTwitchUsePolls.Name = "checkBoxTwitchUsePolls";
            this.checkBoxTwitchUsePolls.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTwitchUsePolls.Size = new System.Drawing.Size(153, 17);
            this.checkBoxTwitchUsePolls.TabIndex = 28;
            this.checkBoxTwitchUsePolls.Text = "Use Twitch Polls For Votes";
            this.toolTipHandler.SetToolTip(this.checkBoxTwitchUsePolls, "This will force majority voting,\r\nno matter what the checkbox is.\r\nThere is no in" +
        "formation on\r\nwhich user voted for which vote.");
            this.checkBoxTwitchUsePolls.UseVisualStyleBackColor = true;
            this.checkBoxTwitchUsePolls.CheckedChanged += new System.EventHandler(this.CheckBoxTwitchUsePolls_CheckedChanged);
            // 
            // checkBoxTwitchMajorityVotes
            // 
            this.checkBoxTwitchMajorityVotes.AutoSize = true;
            this.checkBoxTwitchMajorityVotes.Checked = true;
            this.checkBoxTwitchMajorityVotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTwitchMajorityVotes.Location = new System.Drawing.Point(417, 116);
            this.checkBoxTwitchMajorityVotes.Name = "checkBoxTwitchMajorityVotes";
            this.checkBoxTwitchMajorityVotes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTwitchMajorityVotes.Size = new System.Drawing.Size(127, 17);
            this.checkBoxTwitchMajorityVotes.TabIndex = 27;
            this.checkBoxTwitchMajorityVotes.Text = "Twitch Majority Votes";
            this.toolTipHandler.SetToolTip(this.checkBoxTwitchMajorityVotes, "This will force majority voting,\r\nno matter what the checkbox is.\r\nThere is no in" +
        "formation on\r\nwhich user voted for which vote.");
            this.checkBoxTwitchMajorityVotes.UseVisualStyleBackColor = true;
            // 
            // checkBoxTwitchEnableRapidFire
            // 
            this.checkBoxTwitchEnableRapidFire.AutoSize = true;
            this.checkBoxTwitchEnableRapidFire.Checked = true;
            this.checkBoxTwitchEnableRapidFire.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTwitchEnableRapidFire.Location = new System.Drawing.Point(434, 185);
            this.checkBoxTwitchEnableRapidFire.Name = "checkBoxTwitchEnableRapidFire";
            this.checkBoxTwitchEnableRapidFire.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTwitchEnableRapidFire.Size = new System.Drawing.Size(110, 17);
            this.checkBoxTwitchEnableRapidFire.TabIndex = 26;
            this.checkBoxTwitchEnableRapidFire.Text = "Enable Rapid-Fire";
            this.checkBoxTwitchEnableRapidFire.UseVisualStyleBackColor = true;
            this.checkBoxTwitchEnableRapidFire.CheckedChanged += new System.EventHandler(this.CheckBoxTwitchEnableRapidFire_CheckedChanged);
            // 
            // checkBoxTwitchEnableMultipleEffects
            // 
            this.checkBoxTwitchEnableMultipleEffects.AutoSize = true;
            this.checkBoxTwitchEnableMultipleEffects.Location = new System.Drawing.Point(418, 139);
            this.checkBoxTwitchEnableMultipleEffects.Name = "checkBoxTwitchEnableMultipleEffects";
            this.checkBoxTwitchEnableMultipleEffects.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTwitchEnableMultipleEffects.Size = new System.Drawing.Size(126, 17);
            this.checkBoxTwitchEnableMultipleEffects.TabIndex = 25;
            this.checkBoxTwitchEnableMultipleEffects.Text = "Allow Multiple Effects";
            this.checkBoxTwitchEnableMultipleEffects.UseVisualStyleBackColor = true;
            this.checkBoxTwitchEnableMultipleEffects.CheckedChanged += new System.EventHandler(this.CheckBoxTwitchEnableMultipleEffects_CheckedChanged);
            // 
            // checkBoxTwitch3TimesCooldown
            // 
            this.checkBoxTwitch3TimesCooldown.AutoSize = true;
            this.checkBoxTwitch3TimesCooldown.Checked = true;
            this.checkBoxTwitch3TimesCooldown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTwitch3TimesCooldown.Location = new System.Drawing.Point(457, 162);
            this.checkBoxTwitch3TimesCooldown.Name = "checkBoxTwitch3TimesCooldown";
            this.checkBoxTwitch3TimesCooldown.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTwitch3TimesCooldown.Size = new System.Drawing.Size(87, 17);
            this.checkBoxTwitch3TimesCooldown.TabIndex = 24;
            this.checkBoxTwitch3TimesCooldown.Text = "3x Cooldown";
            this.toolTipHandler.SetToolTip(this.checkBoxTwitch3TimesCooldown, "When enabled effects will have 3x their cooldown.\r\n(Cooldown in this case is the " +
        "Voting Time + Voting Cooldown)");
            this.checkBoxTwitch3TimesCooldown.UseVisualStyleBackColor = true;
            this.checkBoxTwitch3TimesCooldown.CheckedChanged += new System.EventHandler(this.CheckBoxTwitch3TimesCooldown_CheckedChanged);
            // 
            // buttonResetTwitch
            // 
            this.buttonResetTwitch.Enabled = false;
            this.buttonResetTwitch.Location = new System.Drawing.Point(124, 35);
            this.buttonResetTwitch.Name = "buttonResetTwitch";
            this.buttonResetTwitch.Size = new System.Drawing.Size(93, 23);
            this.buttonResetTwitch.TabIndex = 21;
            this.buttonResetTwitch.Text = "Reset";
            this.buttonResetTwitch.UseVisualStyleBackColor = true;
            this.buttonResetTwitch.Click += new System.EventHandler(this.ButtonResetTwitch_Click);
            // 
            // checkBoxTwitchAllowOnlyEnabledEffects
            // 
            this.checkBoxTwitchAllowOnlyEnabledEffects.AutoSize = true;
            this.checkBoxTwitchAllowOnlyEnabledEffects.Location = new System.Drawing.Point(396, 207);
            this.checkBoxTwitchAllowOnlyEnabledEffects.Name = "checkBoxTwitchAllowOnlyEnabledEffects";
            this.checkBoxTwitchAllowOnlyEnabledEffects.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTwitchAllowOnlyEnabledEffects.Size = new System.Drawing.Size(148, 17);
            this.checkBoxTwitchAllowOnlyEnabledEffects.TabIndex = 19;
            this.checkBoxTwitchAllowOnlyEnabledEffects.Text = "Only Enabled Effects (RF)";
            this.toolTipHandler.SetToolTip(this.checkBoxTwitchAllowOnlyEnabledEffects, "Only allow effects that are enabled\r\nin the currently active preset during Rapid-" +
        "Fire.");
            this.checkBoxTwitchAllowOnlyEnabledEffects.UseVisualStyleBackColor = true;
            this.checkBoxTwitchAllowOnlyEnabledEffects.CheckedChanged += new System.EventHandler(this.CheckBoxTwitchAllowOnlyEnabledEffects_CheckedChanged);
            // 
            // checkBoxShowLastEffectsTwitch
            // 
            this.checkBoxShowLastEffectsTwitch.AutoSize = true;
            this.checkBoxShowLastEffectsTwitch.Checked = true;
            this.checkBoxShowLastEffectsTwitch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowLastEffectsTwitch.Location = new System.Drawing.Point(8, 121);
            this.checkBoxShowLastEffectsTwitch.Name = "checkBoxShowLastEffectsTwitch";
            this.checkBoxShowLastEffectsTwitch.Size = new System.Drawing.Size(112, 17);
            this.checkBoxShowLastEffectsTwitch.TabIndex = 18;
            this.checkBoxShowLastEffectsTwitch.Text = "Show Last Effects";
            this.checkBoxShowLastEffectsTwitch.UseVisualStyleBackColor = true;
            this.checkBoxShowLastEffectsTwitch.CheckedChanged += new System.EventHandler(this.CheckBoxShowLastEffectsTwitch_CheckedChanged);
            // 
            // labelTwitchCurrentMode
            // 
            this.labelTwitchCurrentMode.AutoSize = true;
            this.labelTwitchCurrentMode.Location = new System.Drawing.Point(8, 71);
            this.labelTwitchCurrentMode.Name = "labelTwitchCurrentMode";
            this.labelTwitchCurrentMode.Size = new System.Drawing.Size(124, 13);
            this.labelTwitchCurrentMode.TabIndex = 17;
            this.labelTwitchCurrentMode.Text = "Current Mode: Cooldown";
            // 
            // buttonTwitchToggle
            // 
            this.buttonTwitchToggle.Enabled = false;
            this.buttonTwitchToggle.Location = new System.Drawing.Point(8, 35);
            this.buttonTwitchToggle.Name = "buttonTwitchToggle";
            this.buttonTwitchToggle.Size = new System.Drawing.Size(110, 23);
            this.buttonTwitchToggle.TabIndex = 15;
            this.buttonTwitchToggle.Text = "Start / Resume";
            this.buttonTwitchToggle.UseVisualStyleBackColor = true;
            this.buttonTwitchToggle.Click += new System.EventHandler(this.ButtonTwitchToggle_Click);
            // 
            // comboBoxVotingCooldown
            // 
            this.comboBoxVotingCooldown.FormattingEnabled = true;
            this.comboBoxVotingCooldown.Location = new System.Drawing.Point(389, 257);
            this.comboBoxVotingCooldown.Name = "comboBoxVotingCooldown";
            this.comboBoxVotingCooldown.Size = new System.Drawing.Size(155, 21);
            this.comboBoxVotingCooldown.TabIndex = 14;
            this.comboBoxVotingCooldown.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVotingCooldown_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(293, 260);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Voting Cooldown:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(317, 233);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Voting Time:";
            // 
            // comboBoxVotingTime
            // 
            this.comboBoxVotingTime.FormattingEnabled = true;
            this.comboBoxVotingTime.Location = new System.Drawing.Point(389, 230);
            this.comboBoxVotingTime.Name = "comboBoxVotingTime";
            this.comboBoxVotingTime.Size = new System.Drawing.Size(155, 21);
            this.comboBoxVotingTime.TabIndex = 11;
            this.comboBoxVotingTime.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVotingTime_SelectedIndexChanged);
            // 
            // progressBarTwitch
            // 
            this.progressBarTwitch.Location = new System.Drawing.Point(8, 87);
            this.progressBarTwitch.Name = "progressBarTwitch";
            this.progressBarTwitch.Size = new System.Drawing.Size(536, 23);
            this.progressBarTwitch.TabIndex = 10;
            // 
            // listLastEffectsTwitch
            // 
            this.listLastEffectsTwitch.FormattingEnabled = true;
            this.listLastEffectsTwitch.Location = new System.Drawing.Point(8, 144);
            this.listLastEffectsTwitch.Name = "listLastEffectsTwitch";
            this.listLastEffectsTwitch.Size = new System.Drawing.Size(273, 134);
            this.listLastEffectsTwitch.TabIndex = 8;
            // 
            // textBoxTwitchAccessToken
            // 
            this.textBoxTwitchAccessToken.Location = new System.Drawing.Point(110, 7);
            this.textBoxTwitchAccessToken.Name = "textBoxTwitchAccessToken";
            this.textBoxTwitchAccessToken.PasswordChar = '*';
            this.textBoxTwitchAccessToken.Size = new System.Drawing.Size(307, 20);
            this.textBoxTwitchAccessToken.TabIndex = 3;
            this.textBoxTwitchAccessToken.TextChanged += new System.EventHandler(this.TextBoxOAuth_TextChanged);
            // 
            // buttonConnectTwitch
            // 
            this.buttonConnectTwitch.Enabled = false;
            this.buttonConnectTwitch.Location = new System.Drawing.Point(423, 6);
            this.buttonConnectTwitch.Name = "buttonConnectTwitch";
            this.buttonConnectTwitch.Size = new System.Drawing.Size(121, 22);
            this.buttonConnectTwitch.TabIndex = 1;
            this.buttonConnectTwitch.Text = "Connect to Twitch";
            this.buttonConnectTwitch.UseVisualStyleBackColor = true;
            this.buttonConnectTwitch.Click += new System.EventHandler(this.ButtonConnectTwitch_Click);
            // 
            // tabPolls
            // 
            this.tabPolls.BackColor = System.Drawing.Color.Transparent;
            this.tabPolls.Controls.Add(this.label1);
            this.tabPolls.Controls.Add(this.numericUpDownTwitchPollsChannelPointsCost);
            this.tabPolls.Controls.Add(this.checkBoxTwitchPollsPostMessages);
            this.tabPolls.Controls.Add(this.labelTwitchPollsBitsCost);
            this.tabPolls.Controls.Add(this.numericUpDownTwitchPollsBitsCost);
            this.tabPolls.Location = new System.Drawing.Point(4, 22);
            this.tabPolls.Name = "tabPolls";
            this.tabPolls.Padding = new System.Windows.Forms.Padding(3);
            this.tabPolls.Size = new System.Drawing.Size(550, 292);
            this.tabPolls.TabIndex = 5;
            this.tabPolls.Text = "Polls";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Channel Points Cost For Voting (0 = Disabled)";
            // 
            // numericUpDownTwitchPollsChannelPointsCost
            // 
            this.numericUpDownTwitchPollsChannelPointsCost.Location = new System.Drawing.Point(233, 56);
            this.numericUpDownTwitchPollsChannelPointsCost.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownTwitchPollsChannelPointsCost.Name = "numericUpDownTwitchPollsChannelPointsCost";
            this.numericUpDownTwitchPollsChannelPointsCost.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownTwitchPollsChannelPointsCost.TabIndex = 7;
            this.numericUpDownTwitchPollsChannelPointsCost.ValueChanged += new System.EventHandler(this.NumericUpDownTwitchPollsChannelPointsCost_ValueChanged);
            // 
            // checkBoxTwitchPollsPostMessages
            // 
            this.checkBoxTwitchPollsPostMessages.AutoSize = true;
            this.checkBoxTwitchPollsPostMessages.Location = new System.Drawing.Point(8, 6);
            this.checkBoxTwitchPollsPostMessages.Name = "checkBoxTwitchPollsPostMessages";
            this.checkBoxTwitchPollsPostMessages.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkBoxTwitchPollsPostMessages.Size = new System.Drawing.Size(148, 17);
            this.checkBoxTwitchPollsPostMessages.TabIndex = 6;
            this.checkBoxTwitchPollsPostMessages.Text = "Post Vote Options In Chat";
            this.checkBoxTwitchPollsPostMessages.UseVisualStyleBackColor = true;
            this.checkBoxTwitchPollsPostMessages.CheckedChanged += new System.EventHandler(this.CheckBoxTwitchPollsPostMessages_CheckedChanged);
            // 
            // labelTwitchPollsBitsCost
            // 
            this.labelTwitchPollsBitsCost.AutoSize = true;
            this.labelTwitchPollsBitsCost.Location = new System.Drawing.Point(6, 35);
            this.labelTwitchPollsBitsCost.Name = "labelTwitchPollsBitsCost";
            this.labelTwitchPollsBitsCost.Size = new System.Drawing.Size(167, 13);
            this.labelTwitchPollsBitsCost.TabIndex = 3;
            this.labelTwitchPollsBitsCost.Text = "Bits Cost For Voting (0 = Disabled)";
            // 
            // numericUpDownTwitchPollsBitsCost
            // 
            this.numericUpDownTwitchPollsBitsCost.Location = new System.Drawing.Point(233, 33);
            this.numericUpDownTwitchPollsBitsCost.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownTwitchPollsBitsCost.Name = "numericUpDownTwitchPollsBitsCost";
            this.numericUpDownTwitchPollsBitsCost.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownTwitchPollsBitsCost.TabIndex = 0;
            this.numericUpDownTwitchPollsBitsCost.ValueChanged += new System.EventHandler(this.NumericUpDownTwitchPollsBitsCost_ValueChanged);
            // 
            // tabEffects
            // 
            this.tabEffects.BackColor = System.Drawing.Color.Transparent;
            this.tabEffects.Controls.Add(this.buttonEffectsToggleAll);
            this.tabEffects.Controls.Add(this.enabledEffectsView);
            this.tabEffects.Location = new System.Drawing.Point(4, 22);
            this.tabEffects.Name = "tabEffects";
            this.tabEffects.Padding = new System.Windows.Forms.Padding(3);
            this.tabEffects.Size = new System.Drawing.Size(550, 292);
            this.tabEffects.TabIndex = 1;
            this.tabEffects.Text = "Effects";
            // 
            // buttonEffectsToggleAll
            // 
            this.buttonEffectsToggleAll.Location = new System.Drawing.Point(6, 259);
            this.buttonEffectsToggleAll.Name = "buttonEffectsToggleAll";
            this.buttonEffectsToggleAll.Size = new System.Drawing.Size(538, 23);
            this.buttonEffectsToggleAll.TabIndex = 7;
            this.buttonEffectsToggleAll.Text = "Toggle All";
            this.buttonEffectsToggleAll.UseVisualStyleBackColor = true;
            this.buttonEffectsToggleAll.Click += new System.EventHandler(this.ButtonEffectsToggleAll_Click);
            // 
            // enabledEffectsView
            // 
            this.enabledEffectsView.CheckBoxes = true;
            this.enabledEffectsView.Location = new System.Drawing.Point(6, 6);
            this.enabledEffectsView.Name = "enabledEffectsView";
            this.enabledEffectsView.Size = new System.Drawing.Size(538, 247);
            this.enabledEffectsView.TabIndex = 3;
            this.enabledEffectsView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.EnabledEffectsView_AfterCheck);
            this.enabledEffectsView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.EnabledEffectsView_NodeMouseDoubleClick);
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.Color.Transparent;
            this.tabSettings.Controls.Add(this.checkBoxPlayAudioForEffects);
            this.tabSettings.Controls.Add(this.label8);
            this.tabSettings.Controls.Add(this.textBoxSeed);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(550, 292);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            // 
            // checkBoxPlayAudioForEffects
            // 
            this.checkBoxPlayAudioForEffects.AutoSize = true;
            this.checkBoxPlayAudioForEffects.Checked = true;
            this.checkBoxPlayAudioForEffects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPlayAudioForEffects.Location = new System.Drawing.Point(9, 32);
            this.checkBoxPlayAudioForEffects.Name = "checkBoxPlayAudioForEffects";
            this.checkBoxPlayAudioForEffects.Size = new System.Drawing.Size(130, 17);
            this.checkBoxPlayAudioForEffects.TabIndex = 8;
            this.checkBoxPlayAudioForEffects.Text = "Play Audio For Effects";
            this.toolTipHandler.SetToolTip(this.checkBoxPlayAudioForEffects, "Some effects play a sound clip when\r\nthey get activated. Check this to have\r\nthem" +
        " play.");
            this.checkBoxPlayAudioForEffects.UseVisualStyleBackColor = true;
            this.checkBoxPlayAudioForEffects.CheckedChanged += new System.EventHandler(this.CheckBoxPlayAudioForEffects_CheckedChanged);
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
            this.textBoxSeed.Size = new System.Drawing.Size(497, 20);
            this.textBoxSeed.TabIndex = 1;
            this.textBoxSeed.TextChanged += new System.EventHandler(this.TextBoxSeed_TextChanged);
            // 
            // tabMultiplayer
            // 
            this.tabMultiplayer.BackColor = System.Drawing.Color.Transparent;
            this.tabMultiplayer.Controls.Add(this.buttonMultiplayerSend);
            this.tabMultiplayer.Controls.Add(this.textBoxMultiplayerChat);
            this.tabMultiplayer.Controls.Add(this.listBoxMultiplayerChat);
            this.tabMultiplayer.Controls.Add(this.labelMultiplayerHost);
            this.tabMultiplayer.Controls.Add(this.textBoxMultiplayerUsername);
            this.tabMultiplayer.Controls.Add(this.labelMultiplayerUsername);
            this.tabMultiplayer.Controls.Add(this.labelMultiplayerChannel);
            this.tabMultiplayer.Controls.Add(this.textBoxMultiplayerChannel);
            this.tabMultiplayer.Controls.Add(this.labelMultiplayerServer);
            this.tabMultiplayer.Controls.Add(this.textBoxMultiplayerServer);
            this.tabMultiplayer.Controls.Add(this.buttonMultiplayerConnect);
            this.tabMultiplayer.Location = new System.Drawing.Point(4, 22);
            this.tabMultiplayer.Name = "tabMultiplayer";
            this.tabMultiplayer.Padding = new System.Windows.Forms.Padding(3);
            this.tabMultiplayer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabMultiplayer.Size = new System.Drawing.Size(550, 292);
            this.tabMultiplayer.TabIndex = 6;
            this.tabMultiplayer.Text = "Multiplayer";
            // 
            // buttonMultiplayerSend
            // 
            this.buttonMultiplayerSend.Enabled = false;
            this.buttonMultiplayerSend.Location = new System.Drawing.Point(457, 260);
            this.buttonMultiplayerSend.Name = "buttonMultiplayerSend";
            this.buttonMultiplayerSend.Size = new System.Drawing.Size(87, 22);
            this.buttonMultiplayerSend.TabIndex = 10;
            this.buttonMultiplayerSend.Text = "Send";
            this.buttonMultiplayerSend.UseVisualStyleBackColor = true;
            this.buttonMultiplayerSend.Click += new System.EventHandler(this.ButtonMultiplayerSend_Click);
            // 
            // textBoxMultiplayerChat
            // 
            this.textBoxMultiplayerChat.Location = new System.Drawing.Point(6, 261);
            this.textBoxMultiplayerChat.MaxLength = 128;
            this.textBoxMultiplayerChat.Name = "textBoxMultiplayerChat";
            this.textBoxMultiplayerChat.Size = new System.Drawing.Size(445, 20);
            this.textBoxMultiplayerChat.TabIndex = 9;
            this.textBoxMultiplayerChat.TextChanged += new System.EventHandler(this.TextBoxMultiplayerChat_TextChanged);
            this.textBoxMultiplayerChat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxMultiplayerChat_KeyDown);
            // 
            // listBoxMultiplayerChat
            // 
            this.listBoxMultiplayerChat.FormattingEnabled = true;
            this.listBoxMultiplayerChat.Location = new System.Drawing.Point(6, 95);
            this.listBoxMultiplayerChat.Name = "listBoxMultiplayerChat";
            this.listBoxMultiplayerChat.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxMultiplayerChat.Size = new System.Drawing.Size(538, 160);
            this.listBoxMultiplayerChat.TabIndex = 8;
            // 
            // labelMultiplayerHost
            // 
            this.labelMultiplayerHost.AutoSize = true;
            this.labelMultiplayerHost.Location = new System.Drawing.Point(404, 9);
            this.labelMultiplayerHost.Name = "labelMultiplayerHost";
            this.labelMultiplayerHost.Size = new System.Drawing.Size(87, 13);
            this.labelMultiplayerHost.TabIndex = 7;
            this.labelMultiplayerHost.Text = "Not connected...";
            // 
            // textBoxMultiplayerUsername
            // 
            this.textBoxMultiplayerUsername.Location = new System.Drawing.Point(70, 32);
            this.textBoxMultiplayerUsername.MaxLength = 16;
            this.textBoxMultiplayerUsername.Name = "textBoxMultiplayerUsername";
            this.textBoxMultiplayerUsername.Size = new System.Drawing.Size(328, 20);
            this.textBoxMultiplayerUsername.TabIndex = 2;
            this.textBoxMultiplayerUsername.TextChanged += new System.EventHandler(this.TextBoxMultiplayerUsername_TextChanged);
            // 
            // labelMultiplayerUsername
            // 
            this.labelMultiplayerUsername.AutoSize = true;
            this.labelMultiplayerUsername.Location = new System.Drawing.Point(8, 35);
            this.labelMultiplayerUsername.Name = "labelMultiplayerUsername";
            this.labelMultiplayerUsername.Size = new System.Drawing.Size(58, 13);
            this.labelMultiplayerUsername.TabIndex = 5;
            this.labelMultiplayerUsername.Text = "Username:";
            // 
            // labelMultiplayerChannel
            // 
            this.labelMultiplayerChannel.AutoSize = true;
            this.labelMultiplayerChannel.Location = new System.Drawing.Point(17, 61);
            this.labelMultiplayerChannel.Name = "labelMultiplayerChannel";
            this.labelMultiplayerChannel.Size = new System.Drawing.Size(49, 13);
            this.labelMultiplayerChannel.TabIndex = 4;
            this.labelMultiplayerChannel.Text = "Channel:";
            // 
            // textBoxMultiplayerChannel
            // 
            this.textBoxMultiplayerChannel.Location = new System.Drawing.Point(70, 58);
            this.textBoxMultiplayerChannel.MaxLength = 16;
            this.textBoxMultiplayerChannel.Name = "textBoxMultiplayerChannel";
            this.textBoxMultiplayerChannel.Size = new System.Drawing.Size(328, 20);
            this.textBoxMultiplayerChannel.TabIndex = 3;
            this.textBoxMultiplayerChannel.TextChanged += new System.EventHandler(this.TextBoxMultiplayerChannel_TextChanged);
            // 
            // labelMultiplayerServer
            // 
            this.labelMultiplayerServer.AutoSize = true;
            this.labelMultiplayerServer.Location = new System.Drawing.Point(25, 9);
            this.labelMultiplayerServer.Name = "labelMultiplayerServer";
            this.labelMultiplayerServer.Size = new System.Drawing.Size(41, 13);
            this.labelMultiplayerServer.TabIndex = 2;
            this.labelMultiplayerServer.Text = "Server:";
            // 
            // textBoxMultiplayerServer
            // 
            this.textBoxMultiplayerServer.Location = new System.Drawing.Point(70, 6);
            this.textBoxMultiplayerServer.Name = "textBoxMultiplayerServer";
            this.textBoxMultiplayerServer.Size = new System.Drawing.Size(328, 20);
            this.textBoxMultiplayerServer.TabIndex = 1;
            this.textBoxMultiplayerServer.TextChanged += new System.EventHandler(this.TextBoxMultiplayerServer_TextChanged);
            // 
            // buttonMultiplayerConnect
            // 
            this.buttonMultiplayerConnect.Enabled = false;
            this.buttonMultiplayerConnect.Location = new System.Drawing.Point(404, 31);
            this.buttonMultiplayerConnect.Name = "buttonMultiplayerConnect";
            this.buttonMultiplayerConnect.Size = new System.Drawing.Size(140, 22);
            this.buttonMultiplayerConnect.TabIndex = 4;
            this.buttonMultiplayerConnect.Text = "Connect";
            this.buttonMultiplayerConnect.UseVisualStyleBackColor = true;
            this.buttonMultiplayerConnect.Click += new System.EventHandler(this.ButtonMultiplayerConnect_Click);
            // 
            // tabExperimental
            // 
            this.tabExperimental.BackColor = System.Drawing.Color.Transparent;
            this.tabExperimental.Controls.Add(this.buttonExperimentalRunEffect);
            this.tabExperimental.Controls.Add(this.textBoxExperimentalEffectName);
            this.tabExperimental.Controls.Add(this.checkBoxExperimental_RunEffectOnAutoStart);
            this.tabExperimental.Controls.Add(this.checkBoxExperimental_EnableAllEffects);
            this.tabExperimental.Location = new System.Drawing.Point(4, 22);
            this.tabExperimental.Name = "tabExperimental";
            this.tabExperimental.Padding = new System.Windows.Forms.Padding(3);
            this.tabExperimental.Size = new System.Drawing.Size(550, 292);
            this.tabExperimental.TabIndex = 7;
            this.tabExperimental.Text = "Experimental";
            // 
            // buttonExperimentalRunEffect
            // 
            this.buttonExperimentalRunEffect.Location = new System.Drawing.Point(469, 263);
            this.buttonExperimentalRunEffect.Name = "buttonExperimentalRunEffect";
            this.buttonExperimentalRunEffect.Size = new System.Drawing.Size(75, 22);
            this.buttonExperimentalRunEffect.TabIndex = 15;
            this.buttonExperimentalRunEffect.Text = "Run";
            this.buttonExperimentalRunEffect.UseVisualStyleBackColor = true;
            this.buttonExperimentalRunEffect.Click += new System.EventHandler(this.ButtonExperimentalRunEffect_Click);
            // 
            // textBoxExperimentalEffectName
            // 
            this.textBoxExperimentalEffectName.Location = new System.Drawing.Point(8, 264);
            this.textBoxExperimentalEffectName.Name = "textBoxExperimentalEffectName";
            this.textBoxExperimentalEffectName.Size = new System.Drawing.Size(455, 20);
            this.textBoxExperimentalEffectName.TabIndex = 14;
            this.textBoxExperimentalEffectName.TextChanged += new System.EventHandler(this.TextBoxExperimentalEffectName_TextChanged);
            // 
            // checkBoxExperimental_RunEffectOnAutoStart
            // 
            this.checkBoxExperimental_RunEffectOnAutoStart.AutoSize = true;
            this.checkBoxExperimental_RunEffectOnAutoStart.Location = new System.Drawing.Point(6, 29);
            this.checkBoxExperimental_RunEffectOnAutoStart.Name = "checkBoxExperimental_RunEffectOnAutoStart";
            this.checkBoxExperimental_RunEffectOnAutoStart.Size = new System.Drawing.Size(157, 17);
            this.checkBoxExperimental_RunEffectOnAutoStart.TabIndex = 12;
            this.checkBoxExperimental_RunEffectOnAutoStart.Text = "Enable Effect On Auto-Start";
            this.toolTipHandler.SetToolTip(this.checkBoxExperimental_RunEffectOnAutoStart, "When auto-start kicks in\r\nit will enable an effect immediately\r\ninstead of only s" +
        "tarting the\r\ntimer.\r\nDoesn\'t work for Twitch mode.");
            this.checkBoxExperimental_RunEffectOnAutoStart.UseVisualStyleBackColor = true;
            this.checkBoxExperimental_RunEffectOnAutoStart.Click += new System.EventHandler(this.CheckBoxExperimental_EnableEffectOnAutoStart_CheckedChanged);
            // 
            // checkBoxExperimental_EnableAllEffects
            // 
            this.checkBoxExperimental_EnableAllEffects.AutoSize = true;
            this.checkBoxExperimental_EnableAllEffects.Location = new System.Drawing.Point(6, 6);
            this.checkBoxExperimental_EnableAllEffects.Name = "checkBoxExperimental_EnableAllEffects";
            this.checkBoxExperimental_EnableAllEffects.Size = new System.Drawing.Size(182, 17);
            this.checkBoxExperimental_EnableAllEffects.TabIndex = 11;
            this.checkBoxExperimental_EnableAllEffects.Text = "Enable Selected Effects At Once";
            this.toolTipHandler.SetToolTip(this.checkBoxExperimental_EnableAllEffects, "When the cooldown runs out it\r\nwill enable all effects that were\r\nselected.\r\nUsef" +
        "ul for \"constant effect\" runs.");
            this.checkBoxExperimental_EnableAllEffects.UseVisualStyleBackColor = true;
            this.checkBoxExperimental_EnableAllEffects.Click += new System.EventHandler(this.CheckBoxExperimental_EnableAllEffects_CheckedChanged);
            // 
            // checkBoxAutoStart
            // 
            this.checkBoxAutoStart.AutoSize = true;
            this.checkBoxAutoStart.Location = new System.Drawing.Point(402, 16);
            this.checkBoxAutoStart.Name = "checkBoxAutoStart";
            this.checkBoxAutoStart.Size = new System.Drawing.Size(73, 17);
            this.checkBoxAutoStart.TabIndex = 8;
            this.checkBoxAutoStart.Text = "Auto-Start";
            this.checkBoxAutoStart.UseVisualStyleBackColor = true;
            this.checkBoxAutoStart.CheckedChanged += new System.EventHandler(this.CheckBoxAutoStart_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.gameToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(560, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPresetToolStripMenuItem,
            this.savePresetToolStripMenuItem,
            this.experimentalToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadPresetToolStripMenuItem
            // 
            this.loadPresetToolStripMenuItem.Name = "loadPresetToolStripMenuItem";
            this.loadPresetToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.loadPresetToolStripMenuItem.Text = "Load Preset";
            this.loadPresetToolStripMenuItem.Click += new System.EventHandler(this.LoadPresetToolStripMenuItem_Click);
            // 
            // savePresetToolStripMenuItem
            // 
            this.savePresetToolStripMenuItem.Name = "savePresetToolStripMenuItem";
            this.savePresetToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.savePresetToolStripMenuItem.Text = "Save Preset";
            this.savePresetToolStripMenuItem.Click += new System.EventHandler(this.SavePresetToolStripMenuItem_Click);
            // 
            // experimentalToolStripMenuItem
            // 
            this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
            this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.experimentalToolStripMenuItem.Text = "Experimental";
            this.experimentalToolStripMenuItem.Click += new System.EventHandler(this.ExperimentalToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viceCityToolStripMenuItem,
            this.sanAndreasToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // viceCityToolStripMenuItem
            // 
            this.viceCityToolStripMenuItem.Name = "viceCityToolStripMenuItem";
            this.viceCityToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.viceCityToolStripMenuItem.Text = "Vice City";
            this.viceCityToolStripMenuItem.Click += new System.EventHandler(this.ViceCityToolStripMenuItem_Click);
            // 
            // sanAndreasToolStripMenuItem
            // 
            this.sanAndreasToolStripMenuItem.Name = "sanAndreasToolStripMenuItem";
            this.sanAndreasToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.sanAndreasToolStripMenuItem.Text = "San Andreas";
            this.sanAndreasToolStripMenuItem.Click += new System.EventHandler(this.SanAndreasToolStripMenuItem_Click);
            // 
            // timerMain
            // 
            this.timerMain.Enabled = true;
            this.timerMain.Interval = 10;
            this.timerMain.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // buttonSwitchMode
            // 
            this.buttonSwitchMode.Location = new System.Drawing.Point(481, 12);
            this.buttonSwitchMode.Name = "buttonSwitchMode";
            this.buttonSwitchMode.Size = new System.Drawing.Size(73, 23);
            this.buttonSwitchMode.TabIndex = 7;
            this.buttonSwitchMode.Text = "Twitch";
            this.buttonSwitchMode.UseVisualStyleBackColor = true;
            this.buttonSwitchMode.Click += new System.EventHandler(this.ButtonSwitchMode_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 360);
            this.Controls.Add(this.checkBoxAutoStart);
            this.Controls.Add(this.buttonSwitchMode);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "GTA:SA Chaos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabs.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
            this.tabTwitch.ResumeLayout(false);
            this.tabTwitch.PerformLayout();
            this.tabPolls.ResumeLayout(false);
            this.tabPolls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsChannelPointsCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsBitsCost)).EndInit();
            this.tabEffects.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.tabMultiplayer.ResumeLayout(false);
            this.tabMultiplayer.PerformLayout();
            this.tabExperimental.ResumeLayout(false);
            this.tabExperimental.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonMainToggle;
        private System.Windows.Forms.ProgressBar progressBarMain;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabEffects;
        private System.Windows.Forms.TreeView enabledEffectsView;
        private System.Windows.Forms.ListBox listLastEffectsMain;
        private System.Windows.Forms.ComboBox comboBoxMainCooldown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabTwitch;
        private System.Windows.Forms.TextBox textBoxTwitchAccessToken;
        private System.Windows.Forms.Button buttonConnectTwitch;
        private System.Windows.Forms.ListBox listLastEffectsTwitch;
        private System.Windows.Forms.ToolTip toolTipHandler;
        private System.Windows.Forms.ComboBox comboBoxVotingCooldown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxVotingTime;
        private System.Windows.Forms.ProgressBar progressBarTwitch;
        private System.Windows.Forms.Button buttonTwitchToggle;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.Button buttonSwitchMode;
        private System.Windows.Forms.Label labelTwitchCurrentMode;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxSeed;
        private System.Windows.Forms.Button buttonResetMain;
        private System.Windows.Forms.CheckBox checkBoxShowLastEffectsMain;
        private System.Windows.Forms.CheckBox checkBoxShowLastEffectsTwitch;
        private System.Windows.Forms.CheckBox checkBoxTwitchAllowOnlyEnabledEffects;
        private System.Windows.Forms.Button buttonResetTwitch;
        private System.Windows.Forms.CheckBox checkBoxTwitch3TimesCooldown;
        private System.Windows.Forms.Button buttonEffectsToggleAll;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viceCityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sanAndreasToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxTwitchEnableMultipleEffects;
        private System.Windows.Forms.TabPage tabPolls;
        private System.Windows.Forms.Label labelTwitchPollsBitsCost;
        private System.Windows.Forms.NumericUpDown numericUpDownTwitchPollsBitsCost;
        private System.Windows.Forms.CheckBox checkBoxTwitchPollsPostMessages;
        private System.Windows.Forms.CheckBox checkBoxPlayAudioForEffects;
        private System.Windows.Forms.TabPage tabMultiplayer;
        private System.Windows.Forms.TextBox textBoxMultiplayerUsername;
        private System.Windows.Forms.Label labelMultiplayerUsername;
        private System.Windows.Forms.Label labelMultiplayerChannel;
        private System.Windows.Forms.TextBox textBoxMultiplayerChannel;
        private System.Windows.Forms.Label labelMultiplayerServer;
        private System.Windows.Forms.TextBox textBoxMultiplayerServer;
        private System.Windows.Forms.Button buttonMultiplayerConnect;
        private System.Windows.Forms.Label labelMultiplayerHost;
        private System.Windows.Forms.Button buttonMultiplayerSend;
        private System.Windows.Forms.TextBox textBoxMultiplayerChat;
        private System.Windows.Forms.ListBox listBoxMultiplayerChat;
        private System.Windows.Forms.TabPage tabExperimental;
        private System.Windows.Forms.ToolStripMenuItem experimentalToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxExperimental_RunEffectOnAutoStart;
        private System.Windows.Forms.CheckBox checkBoxExperimental_EnableAllEffects;
        private System.Windows.Forms.Button buttonExperimentalRunEffect;
        private System.Windows.Forms.TextBox textBoxExperimentalEffectName;
        private System.Windows.Forms.CheckBox checkBoxAutoStart;
        private System.Windows.Forms.CheckBox checkBoxTwitchEnableRapidFire;
        private System.Windows.Forms.CheckBox checkBoxTwitchCombineVotingMessages;
        private System.Windows.Forms.CheckBox checkBoxTwitchUsePolls;
        private System.Windows.Forms.CheckBox checkBoxTwitchMajorityVotes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownTwitchPollsChannelPointsCost;
        private System.Windows.Forms.LinkLabel linkLabelTwitchGetAccessToken;
        private System.Windows.Forms.Label labelTwitchAccessToken;
    }
}

