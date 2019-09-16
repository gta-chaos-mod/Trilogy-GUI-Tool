// Copyright (c) 2019 Lordmau5
using GTA_SA_Chaos.effects;
using GTA_SA_Chaos.util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GTA_SA_Chaos
{
    public partial class Form1 : Form
    {
        private readonly string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config.cfg");

        private readonly Stopwatch Stopwatch;
        private readonly Dictionary<string, EffectTreeNode> IdToEffectNodeMap = new Dictionary<string, EffectTreeNode>();
        private TwitchConnection Twitch;

        private int elapsedCount;
        private readonly System.Timers.Timer AutoStartTimer;
        private int introState = 1;

        private int timesUntilRapidFire;

        public Form1()
        {
            InitializeComponent();

            Text = "GTA:SA Chaos v1.1.1";
            tabSettings.TabPages.Remove(tabDebug);

            Stopwatch = new Stopwatch();
            AutoStartTimer = new System.Timers.Timer()
            {
                Interval = 50,
                AutoReset = true
            };
            AutoStartTimer.Elapsed += AutoStartTimer_Elapsed;

            PopulateEffectTreeList();

            PopulateMainCooldowns();
            PopulatePresets();

            tabSettings.TabPages.Remove(tabTwitch);

            PopulateVotingTimes();
            PopulateVotingCooldowns();

            TryLoadConfig();

            timesUntilRapidFire = new Random().Next(10, 15);
        }

        private void AutoStartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ProcessHooker.HasExited())
            {
                return;
            }

            if (Config.Instance.Enabled)
            {
                return;
            }

            MemoryHelper.Read((IntPtr)0xA4ED04, out int new_introState);
            MemoryHelper.Read((IntPtr)0xB7CB84, out int playingTime);

            if (introState == 0 && new_introState == 1 && playingTime < 1000 * 60)
            {
                buttonAutoStart.Invoke(new Action(() => SetAutostart()));
            }

            introState = new_introState;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
            ProcessHooker.CloseProcess();
        }

        private void TryLoadConfig()
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();

                using (StreamReader sr = new StreamReader(ConfigPath))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Config.Instance = serializer.Deserialize<Config>(reader);

                    RandomHandler.SetSeed(Config.Instance.Seed);

                    UpdateInterface();
                }
            }
            catch (Exception) { }
        }

        private void SaveConfig()
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();

                using (StreamWriter sw = new StreamWriter(ConfigPath))
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, Config.Instance);
                }
            }
            catch (Exception) { }
        }

        private void UpdateInterface()
        {
            foreach (MainCooldownComboBoxItem item in comboBoxMainCooldown.Items)
            {
                if (item.Time == Config.Instance.MainCooldown)
                {
                    comboBoxMainCooldown.SelectedItem = item;
                    break;
                }
            }

            checkBoxTwitchAllowOnlyEnabledEffects.Checked = Config.Instance.TwitchAllowOnlyEnabledEffectsRapidFire;

            foreach (VotingTimeComboBoxItem item in comboBoxVotingTime.Items)
            {
                if (item.VotingTime == Config.Instance.TwitchVotingTime)
                {
                    comboBoxVotingTime.SelectedItem = item;
                    break;
                }
            }

            foreach (VotingCooldownComboBoxItem item in comboBoxVotingCooldown.Items)
            {
                if (item.VotingCooldown == Config.Instance.TwitchVotingCooldown)
                {
                    comboBoxVotingCooldown.SelectedItem = item;
                    break;
                }
            }

            textBoxTwitchChannel.Text = Config.Instance.TwitchChannel;
            textBoxTwitchUsername.Text = Config.Instance.TwitchUsername;
            textBoxTwitchOAuth.Text = Config.Instance.TwitchOAuthToken;

            checkBoxContinueTimer.Checked = Config.Instance.ContinueTimer;
            checkBoxCrypticEffects.Checked = Config.Instance.CrypticEffects;

            checkBoxShowLastEffectsMain.Checked = Config.Instance.MainShowLastEffects;
            checkBoxShowLastEffectsTwitch.Checked = Config.Instance.TwitchShowLastEffects;
            checkBoxTwitchMajorityVoting.Checked = Config.Instance.TwitchMajorityVoting;

            textBoxSeed.Text = Config.Instance.Seed;
        }

        public void AddEffectToListBox(AbstractEffect effect)
        {
            string Description = "Invalid";
            if (effect != null)
            {
                Description = effect.GetDescription();
                if (!string.IsNullOrEmpty(effect.Word))
                {
                    Description += " (" + effect.Word + ")";
                }
            }

            ListBox listBox = Config.Instance.IsTwitchMode ? listLastEffectsTwitch : listLastEffectsMain;
            listBox.Items.Insert(0, Description);
            if (listBox.Items.Count > 7)
            {
                listBox.Items.RemoveAt(7);
            }
        }

        private void ButtonAutoStart_Click(object sender, EventArgs e)
        {
            TrySetupAutostart();
        }

        private void CallEffect(AbstractEffect effect = null)
        {
            if (effect == null)
            {
                effect = EffectDatabase.RunEffect(EffectDatabase.GetRandomEffect(true));
                effect.ResetVoter();
            }
            else
            {
                EffectDatabase.RunEffect(effect);
            }

            AddEffectToListBox(effect);
        }

        private void TrySetupAutostart()
        {
            if (ProcessHooker.HasExited()) // Make sure we are hooked
            {
                ProcessHooker.HookProcess();
            }

            if (ProcessHooker.HasExited())
            {
                MessageBox.Show("The game needs to be running!", "Error");

                buttonAutoStart.Enabled = Config.Instance.IsTwitchMode && Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance.ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    Stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
                }
                return;
            }

            ProcessHooker.AttachExitedMethod((sender, e) => buttonAutoStart.Invoke(new Action(() =>
            {
                buttonAutoStart.Enabled = Config.Instance.IsTwitchMode && Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance.ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    Stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
                }

                ProcessHooker.CloseProcess();
            })));

            buttonAutoStart.Enabled = false;
            buttonAutoStart.Text = "Waiting...";

            Config.Instance.Enabled = false;
            AutoStartTimer.Start();
            buttonMainToggle.Enabled = false;
            buttonTwitchToggle.Enabled = Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Config.Instance.IsTwitchMode)
            {
                TickTwitch();
            }
            else
            {
                TickMain();
            }
        }

        private void TickMain()
        {
            if (!Config.Instance.Enabled) return;

            int value = Math.Max(1, (int)Stopwatch.ElapsedMilliseconds);

            // Hack to fix Windows' broken-ass progress bar handling
            progressBarMain.Value = Math.Min(value, progressBarMain.Maximum);
            progressBarMain.Value = Math.Min(value - 1, progressBarMain.Maximum);

            if (Stopwatch.ElapsedMilliseconds - elapsedCount > 100)
            {
                long remaining = Math.Max(0, Config.Instance.MainCooldown - Stopwatch.ElapsedMilliseconds);
                int iRemaining = (int)((float)remaining / Config.Instance.MainCooldown * 1000f);

                ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                elapsedCount = (int)Stopwatch.ElapsedMilliseconds;
            }

            if (Stopwatch.ElapsedMilliseconds >= Config.Instance.MainCooldown)
            {
                progressBarMain.Value = 0;
                CallEffect();
                elapsedCount = 0;
                Stopwatch.Restart();
            }
        }

        private void TickTwitch()
        {
            if (!Config.Instance.Enabled) return;

            if (Config.Instance.TwitchVotingMode == 1)
            {
                if (progressBarTwitch.Maximum != Config.Instance.TwitchVotingTime)
                {
                    progressBarTwitch.Maximum = Config.Instance.TwitchVotingTime;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)Stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value, 0);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value - 1, 0);

                if (Stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, Config.Instance.TwitchVotingTime - Stopwatch.ElapsedMilliseconds);
                    int iRemaining = (int)((float)remaining / Config.Instance.TwitchVotingTime * 1000f);

                    ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                    Twitch?.SendEffectVotingToGame();

                    elapsedCount = (int)Stopwatch.ElapsedMilliseconds;
                }

                if (Stopwatch.ElapsedMilliseconds >= Config.Instance.TwitchVotingTime)
                {
                    ProcessHooker.SendEffectToGame("time", "0");
                    elapsedCount = 0;

                    progressBarTwitch.Value = 0;
                    progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;

                    Stopwatch.Restart();
                    Config.Instance.TwitchVotingMode = 0;

                    labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                    if (Twitch != null)
                    {
                        TwitchConnection.VotingElement element = Twitch.GetRandomVotedEffect(out string username);

                        Twitch.SetVoting(0, timesUntilRapidFire, element, username);
                        CallEffect(element.Effect);
                    }
                }
            }
            else if (Config.Instance.TwitchVotingMode == 2)
            {
                if (progressBarTwitch.Maximum != 1000 * 10)
                {
                    progressBarTwitch.Maximum = 1000 * 10;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)Stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value, 0);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value - 1, 0);

                if (Stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, (1000 * 10) - Stopwatch.ElapsedMilliseconds);
                    int iRemaining = (int)((float)remaining / (1000 * 10) * 1000f);

                    ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                    elapsedCount = (int)Stopwatch.ElapsedMilliseconds;
                }

                if (Stopwatch.ElapsedMilliseconds >= 1000 * 10) // Set 10 seconds
                {
                    ProcessHooker.SendEffectToGame("time", "0");
                    elapsedCount = 0;

                    progressBarTwitch.Value = 0;
                    progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;

                    Stopwatch.Restart();
                    Config.Instance.TwitchVotingMode = 0;

                    labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                    Twitch?.SetVoting(0, timesUntilRapidFire);
                }
            }
            else if (Config.Instance.TwitchVotingMode == 0)
            {
                if (progressBarTwitch.Maximum != Config.Instance.TwitchVotingCooldown)
                {
                    progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)Stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Min(value + 1, progressBarTwitch.Maximum);
                progressBarTwitch.Value = Math.Min(value, progressBarTwitch.Maximum);

                if (Stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, Config.Instance.TwitchVotingCooldown - Stopwatch.ElapsedMilliseconds);
                    int iRemaining = Math.Min(1000, 1000 - (int)((float)remaining / Config.Instance.TwitchVotingCooldown * 1000f));

                    ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                    elapsedCount = (int)Stopwatch.ElapsedMilliseconds;
                }

                if (Stopwatch.ElapsedMilliseconds >= Config.Instance.TwitchVotingCooldown)
                {
                    elapsedCount = 0;

                    if (--timesUntilRapidFire == 0)
                    {
                        progressBarTwitch.Value = progressBarTwitch.Maximum = 1000 * 10;

                        timesUntilRapidFire = new Random().Next(10, 15);

                        Config.Instance.TwitchVotingMode = 2;
                        labelTwitchCurrentMode.Text = "Current Mode: Rapid-Fire";

                        Twitch?.SetVoting(2, timesUntilRapidFire);
                    }
                    else
                    {
                        progressBarTwitch.Value = progressBarTwitch.Maximum = Config.Instance.TwitchVotingTime;

                        Config.Instance.TwitchVotingMode = 1;
                        labelTwitchCurrentMode.Text = "Current Mode: Voting";

                        Twitch?.SetVoting(1, timesUntilRapidFire);
                    }
                    Stopwatch.Restart();
                }
            }
        }

        private void PopulateEffectTreeList()
        {
            // Add Categories
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.WeaponsAndHealth));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.WantedLevel));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.Weather));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.Spawning));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.Time));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.VehiclesTraffic));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.PedsAndCo));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.PlayerModifications));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.Stats));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.CustomEffects));
            enabledEffectsView.Nodes.Add(new CategoryTreeNode(Category.Teleportation));

            // Add Effects
            foreach (AbstractEffect effect in EffectDatabase.Effects)
            {
                TreeNode node = enabledEffectsView.Nodes.Find(effect.Category.Name, false).FirstOrDefault();
                EffectTreeNode addedNode = new EffectTreeNode(effect)
                {
                    Checked = true
                };
                node.Nodes.Add(addedNode);
                IdToEffectNodeMap.Add(effect.Id, addedNode);
            }
        }

        private void PopulatePresets()
        {
            presetComboBox.Items.Add(new PresetComboBoxItem("Speedrun", reversed: false, new string[]
            {
                "HE1", "HE2", "HE3", "HE4", "HE5", "HE7",

                "WA1", "WA2", "WA3", "WA4",

                "WE1", "WE2", "WE3", "WE4", "WE5", "WE6", "WE7",

                "SP1", "SP2", "SP19",

                "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                "VE1", "VE2", "VE3", "VE4", "VE5", "VE6", "VE7", "VE8", "VE9", "VE10",
                "VE11", "VE12", "VE13", "VE14",

                "PE1", "PE2", "PE3", "PE4", "PE5", "PE6", "PE7", "PE8", "PE9", "PE10",
                "PE11", "PE12", "PE14", "PE15", "PE16", "PE17", "PE18",

                "MO1", "MO2", "MO3", "MO4", "MO5",

                "ST1", "ST2", "ST3", "ST4", "ST5", "ST6", "ST7", "ST8", "ST9", "ST10",
                "ST11", "ST12",

                "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                "CE11", "CE12", "CE13", "CE14", "CE16", "CE17", "CE18", "CE19",
                "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29", "CE30",
                "CE31", "CE32", "CE33", "CE34", "CE35", "CE36", "CE37", "CE38", "CE39", "CE40",
                "CE41", "CE43", "CE44", "CE45", "CE46", "CE47", "CE48", "CE49", "CE50",
                "CE51", "CE52", "CE53",

                "TP1"
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Harmless", reversed: false, new string[]
            {
                "HE1", "HE2", "HE3", "HE4", "HE5", "HE7",

                "WA2", "WA3",

                "WE1", "WE2",

                "VE2", "VE3", "VE4", "VE5", "VE7", "VE8",
                "VE11", "VE12", "VE13", "VE14", "VE15",

                "PE3", "PE5", "PE8", "PE10",
                "PE11", "PE12", "PE13", "PE14", "PE15", "PE16", "PE17",

                "MO1", "MO2", "MO3", "MO4", "MO5",

                "ST2", "ST4", "ST6", "ST8", "ST10",
                "ST11", "ST12",

                "CE11", "CE12",
                "CE22", "CE23", "CE30",
                "CE40",
                "CE46", "CE47", "CE50",
                "CE52", "CE53"
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Harmful", reversed: false, new string[]
            {
                "HE6",

                "WA1", "WA4",

                "WE3", "WE4", "WE5", "WE6", "WE7",

                "SP1", "SP2", "SP3", "SP4", "SP5", "SP6", "SP7", "SP8", "SP9", "SP10",
                "SP11", "SP12", "SP13", "SP14", "SP15", "SP16", "SP17", "SP18", "SP19",

                "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                "VE1", "VE6", "VE9", "VE10",

                "PE1", "PE2", "PE4", "PE6", "PE7", "PE9",
                "PE18",

                "ST1", "ST3", "ST5", "ST7", "ST9",

                "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                "CE11", "CE12", "CE13", "CE14", "CE15", "CE16", "CE17", "CE18", "CE19", "CE20",
                "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29", "CE30",
                "CE31", "CE32", "CE33", "CE34", "CE35", "CE36", "CE37", "CE38", "CE39",
                "CE41", "CE43", "CE44", "CE45", "CE48", "CE49",
                "CE51",

                "TP1", "TP2", "TP3", "TP4", "TP5", "TP6", "TP7", "TP8", "TP9", "TP10",
                "TP11", "TP12"
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Good Luck", reversed: false, new string[]
            {
                "HE6",

                "WA4",

                "WE5", "WE6", "WE7",

                "SP10",
                "SP11", "SP15", "SP16", "SP17", "SP19",

                "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                "VE1", "VE4", "VE6", "VE7", "VE9", "VE10",
                "VE14",

                "PE1", "PE2", "PE6", "PE7", "PE8", "PE9",
                "PE18",

                "ST1", "ST3", "ST5", "ST7", "ST9",

                "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                "CE11", "CE12", "CE13", "CE14", "CE15", "CE16", "CE17", "CE18", "CE19", "CE20",
                "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29", "CE30",
                "CE31", "CE32", "CE33", "CE34", "CE35", "CE36", "CE37", "CE38", "CE39",
                "CE41", "CE42", "CE43", "CE44", "CE45", "CE48", "CE49",
                "CE51",

                "TP1", "TP2", "TP3", "TP4", "TP5", "TP6", "TP7", "TP8", "TP9", "TP10",
                "TP11", "TP12"
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Everything", reversed: true, new string[] { }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Twitch Voting", reversed: true, new string[]
            {
                "CE41"
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Nothing", reversed: false, new string[] { }));

            presetComboBox.SelectedIndex = 0;
        }

        private class PresetComboBoxItem
        {
            public readonly string Text;
            public readonly bool Reversed;
            public readonly string[] EnabledEffects;

            public PresetComboBoxItem(string text, bool reversed, string[] enabledEffects)
            {
                Text = text;
                Reversed = reversed;
                EnabledEffects = enabledEffects;
            }

            override public string ToString()
            {
                return Text;
            }
        }

        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PresetComboBoxItem item = (PresetComboBoxItem)presetComboBox.SelectedItem;

            LoadPreset(item.Reversed, item.EnabledEffects);
        }

        private void PopulateMainCooldowns()
        {
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("10 seconds", 1000 * 10));
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("20 seconds", 1000 * 20));
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("30 seconds", 1000 * 30));
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("1 minute", 1000 * 60));
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("2 minutes", 1000 * 60 * 2));
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("5 minutes", 1000 * 60 * 5));
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("10 minutes", 1000 * 60 * 10));
            //comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("DEBUG - 1 second", 1000));

            comboBoxMainCooldown.SelectedIndex = 3;

            Config.Instance.MainCooldown = 1000 * 60;
        }

        private class MainCooldownComboBoxItem
        {
            public readonly string Text;
            public readonly int Time;

            public MainCooldownComboBoxItem(string text, int time)
            {
                Text = text;
                Time = time;
            }

            override public string ToString()
            {
                return Text;
            }
        }

        private void MainCooldownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainCooldownComboBoxItem item = (MainCooldownComboBoxItem)comboBoxMainCooldown.SelectedItem;
            Config.Instance.MainCooldown = item.Time;

            if (!Config.Instance.Enabled)
            {
                progressBarMain.Value = 0;
                progressBarMain.Maximum = Config.Instance.MainCooldown;
                elapsedCount = 0;
                Stopwatch.Reset();
            }
        }

        private void PopulateVotingTimes()
        {
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("5 seconds", 1000 * 5));
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("10 seconds", 1000 * 10));
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("15 seconds", 1000 * 15));
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("20 seconds", 1000 * 20));
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("30 seconds", 1000 * 30));
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("1 minute", 1000 * 60));

            comboBoxVotingTime.SelectedIndex = 2;

            Config.Instance.TwitchVotingTime = 1000 * 15;
        }

        private class VotingTimeComboBoxItem
        {
            public readonly int VotingTime;
            public readonly string Text;

            public VotingTimeComboBoxItem(string text, int votingTime)
            {
                Text = text;
                VotingTime = votingTime;
            }

            override public string ToString()
            {
                return Text;
            }
        }

        private void ComboBoxVotingTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingTimeComboBoxItem item = (VotingTimeComboBoxItem)comboBoxVotingTime.SelectedItem;
            Config.Instance.TwitchVotingTime = item.VotingTime;
        }

        private void PopulateVotingCooldowns()
        {
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("10 seconds", 1000 * 10));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("30 seconds", 1000 * 30));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("1 minute", 1000 * 60));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("2 minutes", 1000 * 60 * 2));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("5 minutes", 1000 * 60 * 5));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("10 minutes", 1000 * 60 * 10));

            comboBoxVotingCooldown.SelectedIndex = 2;

            Config.Instance.TwitchVotingCooldown = 1000 * 60 * 2;
        }

        private class VotingCooldownComboBoxItem
        {
            public readonly int VotingCooldown;
            public readonly string Text;

            public VotingCooldownComboBoxItem(string text, int votingCooldown)
            {
                Text = text;
                VotingCooldown = votingCooldown;
            }

            override public string ToString()
            {
                return Text;
            }
        }

        private void ComboBoxVotingCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingCooldownComboBoxItem item = (VotingCooldownComboBoxItem)comboBoxVotingCooldown.SelectedItem;
            Config.Instance.TwitchVotingCooldown = item.VotingCooldown;
        }

        private void SetAutostart()
        {
            buttonAutoStart.Enabled = Config.Instance.IsTwitchMode && Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
            buttonAutoStart.Text = "Auto-Start";
            Stopwatch.Reset();
            SetEnabled(true);
        }

        private void SetEnabled(bool enabled)
        {
            Config.Instance.Enabled = enabled;
            if (Config.Instance.Enabled)
            {
                Stopwatch.Start();
            }
            else
            {
                Stopwatch.Stop();
            }
            AutoStartTimer.Stop();
            buttonMainToggle.Enabled = true;
            (Config.Instance.IsTwitchMode ? buttonTwitchToggle : buttonMainToggle).Text = Config.Instance.Enabled ? "Stop / Pause" : "Start / Resume";
            comboBoxMainCooldown.Enabled =
                buttonSwitchMode.Enabled =
                buttonResetMain.Enabled =
                buttonResetTwitch.Enabled = !Config.Instance.Enabled;

            comboBoxVotingTime.Enabled =
                comboBoxVotingCooldown.Enabled = !Config.Instance.Enabled;
        }

        private void ButtonMainToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Config.Instance.Enabled);
        }

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node is EffectTreeNode etn)
                {
                    EffectDatabase.SetEffectEnabled(etn.Effect, etn.Checked);
                }

                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void EnabledEffectsView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node is EffectTreeNode etn)
                {
                    EffectDatabase.SetEffectEnabled(etn.Effect, etn.Checked);
                }

                if (e.Node.Nodes.Count > 0)
                {
                    CheckAllChildNodes(e.Node, e.Node.Checked);
                }

                foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
                {
                    node.UpdateCategory();
                }
            }
        }

        private void LoadPreset(bool reversed, string[] enabledEffects)
        {
            foreach (TreeNode node in enabledEffectsView.Nodes)
            {
                node.Checked = !reversed;
                CheckAllChildNodes(node, reversed);
            }

            foreach (string effect in enabledEffects)
            {
                if (IdToEffectNodeMap.TryGetValue(effect, out EffectTreeNode node))
                {
                    node.Checked = !reversed;
                    EffectDatabase.SetEffectEnabled(node.Effect, !reversed);
                }
            }

            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.UpdateCategory();
            }
        }

        private class CategoryTreeNode : TreeNode
        {
            private readonly Category category;

            public CategoryTreeNode(Category _category)
            {
                category = _category;
                Name = Text = category.Name;
            }

            public void UpdateCategory()
            {
                bool newChecked = true;
                int enabled = 0;
                foreach (TreeNode node in Nodes)
                {
                    if (node.Checked)
                    {
                        enabled++;
                    }
                    else
                    {
                        newChecked = false;
                    }
                }
                Checked = newChecked;
                Text = Name + " (" + enabled + "/" + Nodes.Count + ")";
            }
        }

        private class EffectTreeNode : TreeNode
        {
            public readonly AbstractEffect Effect;

            public EffectTreeNode(AbstractEffect effect)
            {
                Effect = effect;

                Name = Text = effect.GetDescription();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadPresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Preset File|*.cfg",
                Title = "Save Preset"
            };
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                string content = System.IO.File.ReadAllText(dialog.FileName);
                string[] enabledEffects = content.Split(',');
                List<string> enabledEffectList = new List<string>();
                foreach (string effect in enabledEffects)
                {
                    enabledEffectList.Add(effect);
                }
                LoadPreset(false, enabledEffectList.ToArray());
            }

            dialog.Dispose();
        }

        private void SavePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> enabledEffects = new List<string>();
            foreach (EffectTreeNode node in IdToEffectNodeMap.Values)
            {
                if (node.Checked)
                {
                    enabledEffects.Add(node.Effect.Id);
                }
            }
            string joined = string.Join(",", enabledEffects);

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Preset File|*.cfg",
                Title = "Save Preset"
            };
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                System.IO.File.WriteAllText(dialog.FileName, joined);
            }

            dialog.Dispose();
        }

        private void ButtonConnectTwitch_Click(object sender, EventArgs e)
        {
            if (Twitch != null && Twitch.Client.IsConnected)
            {
                Twitch?.Kill();
                Twitch = null;

                comboBoxVotingTime.Enabled = true;
                comboBoxVotingCooldown.Enabled = true;

                textBoxTwitchChannel.Enabled = true;
                textBoxTwitchUsername.Enabled = true;
                textBoxTwitchOAuth.Enabled = true;

                buttonConnectTwitch.Text = "Connect to Twitch";

                if (!tabSettings.TabPages.Contains(tabEffects))
                {
                    tabSettings.TabPages.Insert(tabSettings.TabPages.IndexOf(tabTwitch), tabEffects);
                }

                return;
            }

            if (Config.Instance.TwitchChannel != "" && Config.Instance.TwitchUsername != "" && Config.Instance.TwitchOAuthToken != "")
            {
                buttonConnectTwitch.Enabled = false;

                Twitch = new TwitchConnection();

                Twitch.OnRapidFireEffect += (_sender, rapidFireArgs) =>
                {
                    Invoke(new Action(() =>
                    {
                        if (Config.Instance.TwitchVotingMode == 2)
                        {
                            rapidFireArgs.Effect.RunEffect();
                            AddEffectToListBox(rapidFireArgs.Effect);
                        }
                    }));
                };

                Twitch.Client.OnIncorrectLogin += (_sender, _e) =>
                {
                    MessageBox.Show("There was an error trying to log in to the account. Wrong username / OAuth token?", "Twitch Login Error");
                    Invoke(new Action(() =>
                    {
                        buttonConnectTwitch.Enabled = true;
                    }));
                    Twitch.Kill();
                };

                Twitch.Client.OnConnected += (_sender, _e) =>
                {
                    Invoke(new Action(() =>
                    {
                        buttonConnectTwitch.Enabled = true;
                        buttonTwitchToggle.Enabled = true;

                        buttonAutoStart.Enabled = true;

                        buttonConnectTwitch.Text = "Disconnect";

                        textBoxTwitchChannel.Enabled = false;
                        textBoxTwitchUsername.Enabled = false;
                        textBoxTwitchOAuth.Enabled = false;
                    }));
                };
            }
        }

        private void UpdateConnectTwitchState()
        {
            buttonConnectTwitch.Enabled =
                textBoxTwitchChannel.Text != "" &&
                textBoxTwitchUsername.Text != "" &&
                textBoxTwitchOAuth.Text != "";

            textBoxTwitchChannel.Enabled = textBoxTwitchUsername.Enabled = textBoxTwitchOAuth.Enabled = true;
        }

        private void TextBoxTwitchChannel_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchChannel = textBoxTwitchChannel.Text;
            UpdateConnectTwitchState();
        }

        private void TextBoxUsername_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchUsername = textBoxTwitchUsername.Text;
            UpdateConnectTwitchState();
        }

        private void TextBoxOAuth_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchOAuthToken = textBoxTwitchOAuth.Text;
            UpdateConnectTwitchState();
        }

        private void ButtonSwitchMode_Click(object sender, EventArgs e)
        {
            if (Config.Instance.IsTwitchMode)
            {
                Config.Instance.IsTwitchMode = false;

                buttonSwitchMode.Text = "Twitch";

                tabSettings.TabPages.Insert(0, tabMain);
                tabSettings.SelectedIndex = 0;
                tabSettings.TabPages.Remove(tabTwitch);

                listLastEffectsMain.Items.Clear();
                progressBarMain.Value = 0;

                elapsedCount = 0;

                Stopwatch.Reset();
                SetEnabled(false);
            }
            else
            {
                Config.Instance.IsTwitchMode = true;

                buttonSwitchMode.Text = "Main";
                buttonAutoStart.Enabled = Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;

                tabSettings.TabPages.Insert(0, tabTwitch);
                tabSettings.SelectedIndex = 0;
                tabSettings.TabPages.Remove(tabMain);

                listLastEffectsTwitch.Items.Clear();
                progressBarTwitch.Value = 0;

                elapsedCount = 0;

                Stopwatch.Reset();
                SetEnabled(false);
            }
        }

        private void ButtonTwitchToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Config.Instance.Enabled);
        }

        private void TextBoxSeed_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.Seed = textBoxSeed.Text;
            RandomHandler.SetSeed(Config.Instance.Seed);
        }

        private void ButtonTestSeed_Click(object sender, EventArgs e)
        {
            labelTestSeed.Text = $"{RandomHandler.Next(100, 999)}";
        }

        private void ButtonGenericTest_Click(object sender, EventArgs e)
        {
            ProcessHooker.SendEffectToGame("effect", "set_vehicle_on_fire", 60000, "Set Vehicle On Fire");
            ProcessHooker.SendEffectToGame("timed_effect", "one_hit_ko", 60000, "One Hit K.O.", "25characterusernamehanice");
            //ProcessHooker.SendEffectToGame("timed_effect", "fail_mission", 60000, "Fail Current Mission", "lordmau5");
        }

        private void ButtonResetMain_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            Stopwatch.Reset();
            elapsedCount = 0;
            progressBarMain.Value = 0;
            buttonMainToggle.Enabled = true;
            buttonMainToggle.Text = "Start / Resume";
            buttonAutoStart.Enabled = true;
            buttonAutoStart.Text = "Auto-Start";
        }

        private void CheckBoxContinueTimer_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.ContinueTimer = checkBoxContinueTimer.Checked;
        }

        private void CheckBoxCrypticEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.CrypticEffects = checkBoxCrypticEffects.Checked;
        }

        private void CheckBoxShowLastEffectsMain_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.MainShowLastEffects
                = listLastEffectsMain.Visible
                = checkBoxShowLastEffectsMain.Checked;
        }

        private void CheckBoxShowLastEffectsTwitch_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchShowLastEffects
                = listLastEffectsTwitch.Visible
                = checkBoxShowLastEffectsTwitch.Checked;
        }

        private void CheckBoxTwitchAllowOnlyEnabledEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchAllowOnlyEnabledEffectsRapidFire = checkBoxTwitchAllowOnlyEnabledEffects.Checked;
        }

        private void CheckBoxTwitchMajorityVoting_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchMajorityVoting = checkBoxTwitchMajorityVoting.Checked;
        }

        private void ButtonResetTwitch_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            Stopwatch.Reset();
            elapsedCount = 0;
            progressBarTwitch.Value = 0;
            buttonTwitchToggle.Enabled = Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
            buttonTwitchToggle.Text = "Start / Resume";
            buttonAutoStart.Enabled = Twitch != null && Twitch.Client != null && Twitch.Client.IsConnected;
            buttonAutoStart.Text = "Auto-Start";
        }
    }
}
