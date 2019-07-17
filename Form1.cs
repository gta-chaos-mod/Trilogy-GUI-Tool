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

        public Form1()
        {
            InitializeComponent();

            Text = "GTA:SA Chaos v0.993";
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
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(ConfigPath))
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, Config.Instance);
            }
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

            checkBoxTwitchAllowVoting.Checked = Config.Instance.TwitchAllowVoting;

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

            checkBoxTwitchIsHost.Checked = Config.Instance.TwitchIsHost;
            checkBoxTwitchDontActivateEffects.Checked = Config.Instance.TwitchDontActivateEffects;

            textBoxTwitchChannel.Text = Config.Instance.TwitchChannel;
            textBoxTwitchUsername.Text = Config.Instance.TwitchUsername;
            textBoxTwitchOAuth.Text = Config.Instance.TwitchOAuthToken;

            checkBoxContinueTimer.Checked = Config.Instance.ContinueTimer;

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

            ListBox listBox = Config.Instance.IsTwitchMode ? lastEffectsTwitch : listLastEffectsMain;
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
            }
            else
            {
                EffectDatabase.RunEffect(effect);
            }

            AddEffectToListBox(effect);
        }

        private void TrySetupAutostart()
        {
            if (ProcessHooker.HasExited()) // Make sure we are hookedtr
            {
                ProcessHooker.HookProcess();
            }

            if (ProcessHooker.HasExited())
            {
                buttonAutoStart.Enabled = true;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance.ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    Stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                }
                return;
            }

            ProcessHooker.AttachExitedMethod((sender, e) => buttonAutoStart.Invoke(new Action(() =>
            {
                buttonAutoStart.Enabled = true;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance.ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    Stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                }

                ProcessHooker.CloseProcess();
            })));

            buttonAutoStart.Enabled = false;
            buttonAutoStart.Text = "Waiting...";

            Config.Instance.Enabled = false;
            AutoStartTimer.Start();
            buttonMainToggle.Enabled = false;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Config.Instance.IsTwitchMode)
            {
                if (Config.Instance.TwitchIsHost)
                {
                    TickTwitchHost();
                }
                else
                {
                    TickTwitchListener();
                }
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

        private void TickTwitchHost()
        {
            if (!Config.Instance.Enabled) return;

            if (Config.Instance.IsTwitchVoting)
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

                    elapsedCount = (int)Stopwatch.ElapsedMilliseconds;
                }

                if (Stopwatch.ElapsedMilliseconds >= Config.Instance.TwitchVotingTime)
                {
                    ProcessHooker.SendEffectToGame("time", "0");
                    elapsedCount = 0;

                    progressBarTwitch.Value = 0;
                    progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;

                    Stopwatch.Restart();
                    Config.Instance.IsTwitchVoting = false;

                    labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                    VotingCooldownComboBoxItem item = (VotingCooldownComboBoxItem)comboBoxVotingCooldown.SelectedItem;

                    AbstractEffect effect = Twitch.GetRandomVotedEffect(out string username);

                    Twitch.SetVoting(false, item.VotingCooldown, item.Text, effect, username);
                    if (!Config.Instance.TwitchDontActivateEffects)
                    {
                        CallEffect(effect);
                    }
                }
            }
            else
            {
                if (progressBarTwitch.Maximum != Config.Instance.TwitchVotingCooldown)
                {
                    progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)Stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Min(value, progressBarTwitch.Maximum);
                progressBarTwitch.Value = Math.Min(value, progressBarTwitch.Maximum);

                if (!Config.Instance.TwitchAllowVoting && Stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, Config.Instance.TwitchVotingTime - Stopwatch.ElapsedMilliseconds);
                    int iRemaining = (int)((float)remaining / Config.Instance.TwitchVotingTime * 1000f);

                    ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                    elapsedCount = (int)Stopwatch.ElapsedMilliseconds;
                }

                if (Stopwatch.ElapsedMilliseconds >= Config.Instance.TwitchVotingCooldown)
                {
                    if (Config.Instance.TwitchAllowVoting)
                    {
                        progressBarTwitch.Value = progressBarTwitch.Maximum = Config.Instance.TwitchVotingTime;

                        Config.Instance.IsTwitchVoting = true;

                        labelTwitchCurrentMode.Text = "Current Mode: Voting";

                        VotingTimeComboBoxItem item = (VotingTimeComboBoxItem)comboBoxVotingTime.SelectedItem;

                        Twitch.SetVoting(true, item.VotingTime, item.Text);
                    }
                    else
                    {
                        ProcessHooker.SendEffectToGame("time", "0");
                        elapsedCount = 0;

                        progressBarTwitch.Value = 0;
                        progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;

                        labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                        VotingCooldownComboBoxItem item = (VotingCooldownComboBoxItem)comboBoxVotingCooldown.SelectedItem;

                        AbstractEffect effect = Twitch.GetRandomVotedEffect(out string username);

                        Twitch.SetVoting(false, item.VotingCooldown, item.Text, effect, username);
                        if (!Config.Instance.TwitchDontActivateEffects)
                        {
                            CallEffect(effect);
                        }
                    }
                    Stopwatch.Restart();
                }
            }
        }

        private void TickTwitchListener()
        {
            if (Config.Instance.IsTwitchVoting)
            {
                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)Stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value, 0);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value - 1, 0);

                if (!Config.Instance.TwitchAllowVoting && Stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, progressBarTwitch.Maximum - Stopwatch.ElapsedMilliseconds);
                    int iRemaining = (int)((float)remaining / progressBarTwitch.Maximum * 1000f);

                    ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                    elapsedCount = (int)Stopwatch.ElapsedMilliseconds;
                }
            }
            else
            {
                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)Stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Min(value, progressBarTwitch.Maximum);
                progressBarTwitch.Value = Math.Min(value - 1, progressBarTwitch.Maximum);
            }
        }

        private void SwitchTwitchListenerMode(bool isVoting, int duration)
        {
            if (Config.Instance.TwitchIsHost)
            {
                return;
            }

            Config.Instance.IsTwitchVoting = isVoting;
            if (isVoting)
            {
                progressBarTwitch.Value = progressBarTwitch.Maximum = duration;
                Config.Instance.TwitchVotingTime = duration;
                foreach (VotingTimeComboBoxItem item in comboBoxVotingTime.Items)
                {
                    if (item.VotingTime == duration)
                    {
                        comboBoxVotingTime.SelectedItem = item;
                        break;
                    }
                }

                labelTwitchCurrentMode.Text = "Current Mode: Voting";
            }
            else
            {
                progressBarTwitch.Value = 0;
                progressBarTwitch.Maximum = duration;
                Config.Instance.TwitchVotingCooldown = duration;
                foreach (VotingCooldownComboBoxItem item in comboBoxVotingCooldown.Items)
                {
                    if (item.VotingCooldown == duration)
                    {
                        comboBoxVotingCooldown.SelectedItem = item;
                        break;
                    }
                }

                labelTwitchCurrentMode.Text = "Current Mode: Cooldown";
            }
            Stopwatch.Restart();
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
            presetComboBox.Items.Add(new PresetComboBoxItem("Harmless", reversed: false, new string[]
            {
                "HE1", "HE2", "HE3", "HE5", "HE6",

                "WA2", "WA3",

                "WE1", "WE2",

                "VE2", "VE3", "VE4", "VE5", "VE8", "VE9",
                "VE12", "VE13", "VE14", "VE15", "VE16",

                "PE3", "PE5", "PE8", "PE10",
                "PE11", "PE12", "PE13", "PE14", "PE15", "PE16",

                "MO1", "MO2", "MO3", "MO4", "MO5", "MO6",

                "ST2", "ST4", "ST5", "ST6", "ST7", "ST8", "ST9",

                "CE11", "CE12",
                "CE22", "CE23",
                "CE30"
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Harmful", reversed: false, new string[]
            {
                "HE4",

                "WA1", "WA4",

                "WE3", "WE4", "WE5", "WE6", "WE7",

                "SP1", "SP2", "SP3", "SP4", "SP5", "SP6", "SP7", "SP8", "SP9", "SP10",
                "SP11", "SP12", "SP13", "SP14", "SP15", "SP16", "SP17", "SP18", "SP19",

                "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                "VE1", "VE6", "VE7", "VE10",
                "VE11",

                "PE1", "PE2", "PE4", "PE6", "PE7", "PE9",
                "PE17",

                "ST1", "ST3",

                "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                "CE11", "CE12", "CE13", "CE14", "CE15", "CE16", "CE17", "CE18", "CE19", "CE20",
                "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29",
                "CE30", "CE31", "CE32", "CE33",

                "TP1", "TP2", "TP3", "TP4", "TP5", "TP6", "TP7", "TP8", "TP9", "TP10",
                "TP11", "TP12",
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Good Luck", reversed: false, new string[]
            {
                "HE4",

                "WA4",

                "WE5", "WE6", "WE7",

                "SP10",
                "SP11", "SP15", "SP16", "SP17", "SP19",

                "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                "VE1", "VE4", "VE6", "VE7", "VE8", "VE10",
                "VE11", "VE15",

                "PE1", "PE2", "PE6", "PE7", "PE8", "PE9",
                "PE17",

                "ST1", "ST3",

                "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                "CE11", "CE12", "CE13", "CE14", "CE15", "CE16", "CE17", "CE18", "CE19", "CE20",
                "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29",
                "CE30", "CE31", "CE32", "CE33",

                "TP1", "TP2", "TP3", "TP4", "TP5", "TP6", "TP7", "TP8", "TP9", "TP10",
                "TP11", "TP12",
            }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Everything", reversed: true, new string[] { }));
            presetComboBox.Items.Add(new PresetComboBoxItem("Nothing", reversed: false, new string[] { }));

            presetComboBox.SelectedIndex = 3;
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
            buttonAutoStart.Enabled = true;
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
            (Config.Instance.IsTwitchMode ? buttonTwitchToggle : buttonMainToggle).Enabled = true;
            (Config.Instance.IsTwitchMode ? buttonTwitchToggle : buttonMainToggle).Text = Config.Instance.Enabled ? "Stop" : "Start";
            comboBoxMainCooldown.Enabled =
                buttonSwitchMode.Enabled =
                buttonReset.Enabled = !Config.Instance.Enabled;
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
        }

        private void ButtonConnectTwitch_Click(object sender, EventArgs e)
        {
            if (Twitch != null && Twitch.Client.IsConnected)
            {
                Twitch.Kill();
                Twitch = null;

                checkBoxTwitchDontActivateEffects.Enabled = true;
                checkBoxTwitchDontActivateEffects.Checked = false;
                checkBoxTwitchAllowVoting.Enabled = true;

                checkBoxTwitchIsHost.Enabled = true;
                checkBoxTwitchIsHost.Checked = false;

                comboBoxVotingTime.Enabled = true;
                comboBoxVotingCooldown.Enabled = true;

                textBoxTwitchChannel.Enabled = true;
                textBoxTwitchUsername.Enabled = Config.Instance.TwitchIsHost;
                textBoxTwitchOAuth.Enabled = Config.Instance.TwitchIsHost;

                buttonConnectTwitch.Text = "Connect to Twitch";

                Config.Instance.TwitchDontActivateEffects = false;

                if (!tabSettings.TabPages.Contains(tabEffects))
                {
                    tabSettings.TabPages.Insert(tabSettings.TabPages.IndexOf(tabTwitch), tabEffects);
                }

                return;
            }

            if (textBoxTwitchChannel.Text != "")
            {
                buttonConnectTwitch.Enabled = false;

                Twitch = new TwitchConnection(textBoxTwitchChannel.Text, textBoxTwitchUsername.Text, textBoxTwitchOAuth.Text);

                Twitch.OnVotingModeChange += (_sender, votingArgs) =>
                {
                    Invoke(new Action(() => SwitchTwitchListenerMode(votingArgs.IsVoting, votingArgs.Duration)));
                };

                Twitch.OnEffectActivated += (_sender, effectArgs) =>
                {
                    Invoke(new Action(() => AddEffectToListBox(EffectDatabase.RunEffect(effectArgs.Id, false))));
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
                    MessageBox.Show("Successfully connected to Twitch!", "Twitch Login Succeeded");
                    Invoke(new Action(() =>
                    {
                        buttonConnectTwitch.Enabled = true;
                        buttonTwitchToggle.Enabled = Config.Instance.TwitchIsHost;

                        checkBoxTwitchIsHost.Enabled = false;

                        buttonConnectTwitch.Text = "Disconnect";

                        textBoxTwitchChannel.Enabled = false;
                        textBoxTwitchUsername.Enabled = false;
                        textBoxTwitchOAuth.Enabled = false;

                        if (!Config.Instance.TwitchIsHost)
                        {
                            checkBoxTwitchDontActivateEffects.Enabled = false;
                            checkBoxTwitchDontActivateEffects.Checked = false;

                            checkBoxTwitchIsHost.Checked = false;

                            comboBoxVotingTime.Enabled = false;
                            comboBoxVotingCooldown.Enabled = false;
                            checkBoxTwitchAllowVoting.Enabled = false;

                            Config.Instance.TwitchDontActivateEffects = false;

                            tabSettings.TabPages.Remove(tabEffects);
                        }
                    }));
                };
            }
        }

        private void UpdateConnectTwitchState()
        {
            if (Config.Instance.TwitchIsHost)
            {
                buttonConnectTwitch.Enabled =
                    textBoxTwitchChannel.Text != "" &&
                    textBoxTwitchUsername.Text != "" &&
                    textBoxTwitchOAuth.Text != "";
            }
            else
            {
                buttonConnectTwitch.Enabled = textBoxTwitchChannel.Text != "";
            }
        }

        private void TextBoxChannel_TextChanged(object sender, EventArgs e)
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

        private void CheckBoxHost_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchIsHost = checkBoxTwitchIsHost.Checked;

            textBoxTwitchUsername.Enabled = textBoxTwitchOAuth.Enabled = Config.Instance.TwitchIsHost;
            UpdateConnectTwitchState();
        }

        private void ButtonSwitchMode_Click(object sender, EventArgs e)
        {
            Config.Instance.IsTwitchMode = true;

            buttonSwitchMode.Visible = false;
            buttonSwitchMode.Enabled = false;

            tabSettings.TabPages.Insert(0, tabTwitch);
            tabSettings.SelectedIndex = 0;
            tabSettings.TabPages.Remove(tabMain);

            listLastEffectsMain.Items.Clear();
            progressBarMain.Value = 0;

            elapsedCount = 0;

            Stopwatch.Reset();
            SetEnabled(false);
        }

        private void ButtonTwitchToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Config.Instance.Enabled);
        }

        private void CheckBoxDontActivateEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchDontActivateEffects = checkBoxTwitchDontActivateEffects.Checked;
        }

        private void CheckBoxAllowVoting_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchAllowVoting = comboBoxVotingTime.Enabled = checkBoxTwitchAllowVoting.Checked;
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
            //ProcessHooker.SendEffectToGame("timed_effect", "inverted_controls", 30000, "Inverted Controls");
            ProcessHooker.SendEffectToGame("timed_effect", "rainbow_cars", 30000, "Rainbow Cars");
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            Stopwatch.Reset();
            elapsedCount = 0;
            progressBarMain.Value = 0;
            buttonMainToggle.Enabled = false;
            buttonMainToggle.Text = "Start";
            buttonReset.Enabled = false;
        }

        private void CheckBoxContinueTimer_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.ContinueTimer = checkBoxContinueTimer.Checked;
        }
    }
}
