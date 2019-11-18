// Copyright (c) 2019 Lordmau5
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;
using Newtonsoft.Json;
using Serilog;

namespace GtaChaos.Forms
{
    public partial class Form1 : Form
    {
        private readonly string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.cfg");

        private readonly Stopwatch stopwatch;
        private readonly Dictionary<string, EffectTreeNode> idToEffectNodeMap = new Dictionary<string, EffectTreeNode>();
        private TwitchConnection twitch;

        private int elapsedCount;
        private readonly System.Timers.Timer autoStartTimer;
        private int introState = 1;

        private int timesUntilRapidFire;

        private readonly bool debug = false;

        public Form1()
        {
            InitializeComponent();

            Text = "GTA Trilogy Chaos Mod v2.0.2";
            if (!debug)
            {
                tabSettings.TabPages.Remove(tabDebug);
                gameToolStripMenuItem.Visible = false;
            }
            else
            {
                Text += " (DEBUG)";
            }

            stopwatch = new Stopwatch();
            autoStartTimer = new System.Timers.Timer()
            {
                Interval = 50,
                AutoReset = true
            };
            autoStartTimer.Elapsed += AutoStartTimer_Elapsed;

            EffectDatabase.PopulateEffects(GameIdentifiers.SanAndreas);
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

            if (Config.Instance().Enabled)
            {
                return;
            }

            MemoryHelper.Read((IntPtr)0xA4ED04, out int newIntroState);
            MemoryHelper.Read((IntPtr)0xB7CB84, out int playingTime);

            if (introState == 0 && newIntroState == 1 && playingTime < 1000 * 60)
            {
                buttonAutoStart.Invoke(new Action(SetAutostart));
            }

            introState = newIntroState;
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

                using (StreamReader streamReader = new StreamReader(configPath))
                using (JsonReader reader = new JsonTextReader(streamReader))
                {
                    Config.SetInstance(serializer.Deserialize<Config>(reader));
                    RandomHandler.SetSeed(Config.Instance().Seed);
                }
                LoadPreset(Config.Instance().EnabledEffects);

                UpdateInterface();
            }
            catch (Exception) { }
        }

        private void SaveConfig()
        {
            try
            {
                Config.Instance().EnabledEffects.Clear();
                foreach (var effect in EffectDatabase.EnabledEffects)
                {
                    Config.Instance().EnabledEffects.Add(effect.Id);
                }

                JsonSerializer serializer = new JsonSerializer();

                using (StreamWriter sw = new StreamWriter(configPath))
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, Config.Instance());
                }
            }
            catch (Exception) { }
        }

        private void UpdateInterface()
        {
            foreach (MainCooldownComboBoxItem item in comboBoxMainCooldown.Items)
            {
                if (item.Time == Config.Instance().MainCooldown)
                {
                    comboBoxMainCooldown.SelectedItem = item;
                    break;
                }
            }

            checkBoxTwitchAllowOnlyEnabledEffects.Checked = Config.Instance().TwitchAllowOnlyEnabledEffectsRapidFire;

            foreach (VotingTimeComboBoxItem item in comboBoxVotingTime.Items)
            {
                if (item.VotingTime == Config.Instance().TwitchVotingTime)
                {
                    comboBoxVotingTime.SelectedItem = item;
                    break;
                }
            }

            foreach (VotingCooldownComboBoxItem item in comboBoxVotingCooldown.Items)
            {
                if (item.VotingCooldown == Config.Instance().TwitchVotingCooldown)
                {
                    comboBoxVotingCooldown.SelectedItem = item;
                    break;
                }
            }

            textBoxTwitchChannel.Text = Config.Instance().TwitchChannel;
            textBoxTwitchUsername.Text = Config.Instance().TwitchUsername;
            textBoxTwitchOAuth.Text = Config.Instance().TwitchOAuthToken;

            checkBoxContinueTimer.Checked = Config.Instance().ContinueTimer;

            checkBoxShowLastEffectsMain.Checked = Config.Instance().MainShowLastEffects;
            checkBoxShowLastEffectsTwitch.Checked = Config.Instance().TwitchShowLastEffects;
            checkBoxTwitchMajorityVoting.Checked = Config.Instance().TwitchMajorityVoting;
            checkBoxTwitch3TimesCooldown.Checked = Config.Instance().Twitch3TimesCooldown;

            textBoxSeed.Text = Config.Instance().Seed;
        }

        public void AddEffectToListBox(AbstractEffect effect)
        {
            string description = "Invalid";
            if (effect != null)
            {
                description = effect.GetDescription();
                if (!string.IsNullOrEmpty(effect.Word))
                {
                    description += $" ({effect.Word})";
                }
            }

            ListBox listBox = Config.Instance().IsTwitchMode ? listLastEffectsTwitch : listLastEffectsMain;
            listBox.Items.Insert(0, description);
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
                effect?.ResetVoter();
            }
            else
            {
                EffectDatabase.RunEffect(effect);
            }

            if (effect != null)
            {
                AddEffectToListBox(effect);
            }
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

                buttonAutoStart.Enabled = Config.Instance().IsTwitchMode && twitch?.Client != null && twitch.Client.IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance().ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
                }
                return;
            }

            ProcessHooker.AttachExitedMethod((sender, e) => buttonAutoStart.Invoke(new Action(() =>
            {
                buttonAutoStart.Enabled = Config.Instance().IsTwitchMode && twitch?.Client != null && twitch.Client.IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance().ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
                }

                ProcessHooker.CloseProcess();
            })));

            buttonAutoStart.Enabled = false;
            buttonAutoStart.Text = "Waiting...";

            Config.Instance().Enabled = false;
            autoStartTimer.Start();
            buttonMainToggle.Enabled = false;
            buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Config.Instance().IsTwitchMode)
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
            if (!Config.Instance().Enabled) return;

            int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);

            // Hack to fix Windows' broken-ass progress bar handling
            progressBarMain.Value = Math.Min(value, progressBarMain.Maximum);
            progressBarMain.Value = Math.Min(value - 1, progressBarMain.Maximum);

            if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
            {
                long remaining = Math.Max(0, Config.Instance().MainCooldown - stopwatch.ElapsedMilliseconds);

                ProcessHooker.SendEffectToGame("time", $"{remaining},{Config.Instance().MainCooldown}");

                elapsedCount = (int)stopwatch.ElapsedMilliseconds;
            }

            if (stopwatch.ElapsedMilliseconds >= Config.Instance().MainCooldown)
            {
                progressBarMain.Value = 0;
                CallEffect();
                elapsedCount = 0;
                stopwatch.Restart();
            }
        }

        private void TickTwitch()
        {
            if (!Config.Instance().Enabled) return;

            if (Config.Instance().TwitchVotingMode == 1)
            {
                if (progressBarTwitch.Maximum != Config.Instance().TwitchVotingTime)
                {
                    progressBarTwitch.Maximum = Config.Instance().TwitchVotingTime;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value, 0);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value - 1, 0);

                if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, Config.Instance().TwitchVotingTime - stopwatch.ElapsedMilliseconds);

                    ProcessHooker.SendEffectToGame("time", $"{remaining},{Config.Instance().TwitchVotingTime}");

                    twitch?.SendEffectVotingToGame();

                    elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                }

                if (stopwatch.ElapsedMilliseconds >= Config.Instance().TwitchVotingTime)
                {
                    ProcessHooker.SendEffectToGame("time", "0");
                    elapsedCount = 0;

                    progressBarTwitch.Value = 0;
                    progressBarTwitch.Maximum = Config.Instance().TwitchVotingCooldown;

                    stopwatch.Restart();
                    Config.Instance().TwitchVotingMode = 0;

                    labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                    if (twitch != null)
                    {
                        TwitchConnection.VotingElement element = twitch.GetRandomVotedEffect(out string username);

                        twitch.SetVoting(0, timesUntilRapidFire, element, username);
                        CallEffect(element.Effect);
                    }
                }
            }
            else if (Config.Instance().TwitchVotingMode == 2)
            {
                if (progressBarTwitch.Maximum != 1000 * 10)
                {
                    progressBarTwitch.Maximum = 1000 * 10;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value, 0);
                progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value - 1, 0);

                if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, (1000 * 10) - stopwatch.ElapsedMilliseconds);

                    ProcessHooker.SendEffectToGame("time", $"{remaining},10000");

                    elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                }

                if (stopwatch.ElapsedMilliseconds >= 1000 * 10) // Set 10 seconds
                {
                    ProcessHooker.SendEffectToGame("time", "0");
                    elapsedCount = 0;

                    progressBarTwitch.Value = 0;
                    progressBarTwitch.Maximum = Config.Instance().TwitchVotingCooldown;

                    stopwatch.Restart();
                    Config.Instance().TwitchVotingMode = 0;

                    labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                    twitch?.SetVoting(0, timesUntilRapidFire);
                }
            }
            else if (Config.Instance().TwitchVotingMode == 0)
            {
                if (progressBarTwitch.Maximum != Config.Instance().TwitchVotingCooldown)
                {
                    progressBarTwitch.Maximum = Config.Instance().TwitchVotingCooldown;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);
                progressBarTwitch.Value = Math.Min(value + 1, progressBarTwitch.Maximum);
                progressBarTwitch.Value = Math.Min(value, progressBarTwitch.Maximum);

                if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                {
                    long remaining = Math.Max(0, Config.Instance().TwitchVotingCooldown - stopwatch.ElapsedMilliseconds);

                    ProcessHooker.SendEffectToGame("time", $"{remaining},{Config.Instance().TwitchVotingCooldown}");

                    elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                }

                if (stopwatch.ElapsedMilliseconds >= Config.Instance().TwitchVotingCooldown)
                {
                    elapsedCount = 0;

                    if (--timesUntilRapidFire == 0)
                    {
                        progressBarTwitch.Value = progressBarTwitch.Maximum = 1000 * 10;

                        timesUntilRapidFire = new Random().Next(10, 15);

                        Config.Instance().TwitchVotingMode = 2;
                        labelTwitchCurrentMode.Text = "Current Mode: Rapid-Fire";

                        twitch?.SetVoting(2, timesUntilRapidFire);
                    }
                    else
                    {
                        progressBarTwitch.Value = progressBarTwitch.Maximum = Config.Instance().TwitchVotingTime;

                        Config.Instance().TwitchVotingMode = 1;
                        labelTwitchCurrentMode.Text = "Current Mode: Voting";

                        twitch?.SetVoting(1, timesUntilRapidFire);
                    }
                    stopwatch.Restart();
                }
            }
        }

        private void PopulateEffectTreeList()
        {
            enabledEffectsView.Nodes.Clear();
            idToEffectNodeMap.Clear();

            // Add Categories
            foreach (Category cat in Category.Categories)
            {
                if (cat.GetEffectCount() > 0)
                {
                    enabledEffectsView.Nodes.Add(new CategoryTreeNode(cat));
                }
            }

            // Add Effects
            foreach (AbstractEffect effect in EffectDatabase.Effects)
            {
                TreeNode node = enabledEffectsView.Nodes.Find(effect.Category.Name, false).FirstOrDefault();
                EffectTreeNode addedNode = new EffectTreeNode(effect)
                {
                    Checked = true
                };
                node.Nodes.Add(addedNode);
                idToEffectNodeMap.Add(effect.Id, addedNode);
            }
        }

        private void PopulatePresets()
        {
            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.Checked = false;
                CheckAllChildNodes(node, false);
                node.UpdateCategory();
            }
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

            if (debug)
            {
                comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("DEBUG - 1 second", 1000));
                comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("DEBUG - 10ms", 10));
            }

            comboBoxMainCooldown.SelectedIndex = 3;

            Config.Instance().MainCooldown = 1000 * 60;
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

            public override string ToString()
            {
                return Text;
            }
        }

        private void MainCooldownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainCooldownComboBoxItem item = (MainCooldownComboBoxItem)comboBoxMainCooldown.SelectedItem;
            Config.Instance().MainCooldown = item.Time;

            if (!Config.Instance().Enabled)
            {
                progressBarMain.Value = 0;
                progressBarMain.Maximum = Config.Instance().MainCooldown;
                elapsedCount = 0;
                stopwatch.Reset();
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

            Config.Instance().TwitchVotingTime = 1000 * 15;
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

            public override string ToString()
            {
                return Text;
            }
        }

        private void ComboBoxVotingTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingTimeComboBoxItem item = (VotingTimeComboBoxItem)comboBoxVotingTime.SelectedItem;
            Config.Instance().TwitchVotingTime = item.VotingTime;
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

            Config.Instance().TwitchVotingCooldown = 1000 * 60 * 2;
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

            public override string ToString()
            {
                return Text;
            }
        }

        private void ComboBoxVotingCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingCooldownComboBoxItem item = (VotingCooldownComboBoxItem)comboBoxVotingCooldown.SelectedItem;
            Config.Instance().TwitchVotingCooldown = item.VotingCooldown;
        }

        private void SetAutostart()
        {
            buttonAutoStart.Enabled = Config.Instance().IsTwitchMode && twitch != null && twitch.Client != null && twitch.Client.IsConnected;
            buttonAutoStart.Text = "Auto-Start";
            stopwatch.Reset();
            SetEnabled(true);
        }

        private void SetEnabled(bool enabled)
        {
            Config.Instance().Enabled = enabled;
            if (Config.Instance().Enabled)
            {
                stopwatch.Start();
            }
            else
            {
                stopwatch.Stop();
            }
            autoStartTimer.Stop();
            buttonMainToggle.Enabled = true;
            (Config.Instance().IsTwitchMode ? buttonTwitchToggle : buttonMainToggle).Text = Config.Instance().Enabled ? "Stop / Pause" : "Start / Resume";
            comboBoxMainCooldown.Enabled =
                buttonSwitchMode.Enabled =
                buttonResetMain.Enabled =
                buttonResetTwitch.Enabled = !Config.Instance().Enabled;

            comboBoxVotingTime.Enabled =
                comboBoxVotingCooldown.Enabled =
                textBoxSeed.Enabled = !Config.Instance().Enabled;
        }

        private void ButtonMainToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Config.Instance().Enabled);
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

        private void LoadPreset(List<string> enabledEffects)
        {
            PopulatePresets();

            foreach (string effect in enabledEffects)
            {
                if (idToEffectNodeMap.TryGetValue(effect, out EffectTreeNode node))
                {
                    node.Checked = true;
                    EffectDatabase.SetEffectEnabled(node.Effect, true);
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
                Text = Name + $" ({enabled}/{Nodes.Count})";
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
                Title = "Load Preset"
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
                LoadPreset(enabledEffectList);
            }

            dialog.Dispose();
        }

        private void SavePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> enabledEffects = new List<string>();
            foreach (EffectTreeNode node in idToEffectNodeMap.Values)
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
            if (twitch != null && twitch.Client.IsConnected)
            {
                twitch?.Kill();
                twitch = null;

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

            if (Config.Instance().TwitchChannel != "" && Config.Instance().TwitchUsername != "" && Config.Instance().TwitchOAuthToken != "")
            {
                buttonConnectTwitch.Enabled = false;

                twitch = new TwitchConnection();

                twitch.OnRapidFireEffect += (_sender, rapidFireArgs) =>
                {
                    Invoke(new Action(() =>
                    {
                        if (Config.Instance().TwitchVotingMode == 2)
                        {
                            rapidFireArgs.Effect.RunEffect();
                            AddEffectToListBox(rapidFireArgs.Effect);
                        }
                    }));
                };

                twitch.Client.OnIncorrectLogin += (_sender, _e) =>
                {
                    MessageBox.Show("There was an error trying to log in to the account. Wrong username / OAuth token?", "Twitch Login Error");
                    Invoke(new Action(() =>
                    {
                        buttonConnectTwitch.Enabled = true;
                    }));
                    twitch.Kill();
                };

                twitch.Client.OnConnected += (_sender, _e) =>
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
            Config.Instance().TwitchChannel = textBoxTwitchChannel.Text;
            UpdateConnectTwitchState();
        }

        private void TextBoxUsername_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchUsername = textBoxTwitchUsername.Text;
            UpdateConnectTwitchState();
        }

        private void TextBoxOAuth_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchOAuthToken = textBoxTwitchOAuth.Text;
            UpdateConnectTwitchState();
        }

        private void ButtonSwitchMode_Click(object sender, EventArgs e)
        {
            if (Config.Instance().IsTwitchMode)
            {
                Config.Instance().IsTwitchMode = false;

                buttonSwitchMode.Text = "Twitch";

                tabSettings.TabPages.Insert(0, tabMain);
                tabSettings.SelectedIndex = 0;
                tabSettings.TabPages.Remove(tabTwitch);

                listLastEffectsMain.Items.Clear();
                progressBarMain.Value = 0;

                elapsedCount = 0;

                stopwatch.Reset();
                SetEnabled(false);
            }
            else
            {
                Config.Instance().IsTwitchMode = true;

                buttonSwitchMode.Text = "Main";
                buttonAutoStart.Enabled = twitch != null && twitch.Client != null && twitch.Client.IsConnected;

                tabSettings.TabPages.Insert(0, tabTwitch);
                tabSettings.SelectedIndex = 0;
                tabSettings.TabPages.Remove(tabMain);

                listLastEffectsTwitch.Items.Clear();
                progressBarTwitch.Value = 0;

                elapsedCount = 0;

                stopwatch.Reset();
                SetEnabled(false);
            }
        }

        private void ButtonTwitchToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Config.Instance().Enabled);
        }

        private void TextBoxSeed_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().Seed = textBoxSeed.Text;
            RandomHandler.SetSeed(Config.Instance().Seed);
        }

        private void ButtonTestSeed_Click(object sender, EventArgs e)
        {
            labelTestSeed.Text = $"{RandomHandler.Next(100, 999)}";
        }

        private void ButtonGenericTest_Click(object sender, EventArgs e)
        {
            ProcessHooker.SendEffectToGame("effect", "never_wanted", 5000, "Never Wanted", "lordmau5");
            ProcessHooker.SendEffectToGame("effect", "weapon_set_1", 5000, "Weapon Set 1", "senor stendec");
            ProcessHooker.SendEffectToGame("effect", "one_hit_ko", 5000, "One Hit K.O.", "daniel salvation");
            //ProcessHooker.SendEffectToGame("timed_effect", "fail_mission", 60000, "Fail Current Mission", "lordmau5");
        }

        private void ButtonResetMain_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            stopwatch.Reset();
            elapsedCount = 0;
            progressBarMain.Value = 0;
            buttonMainToggle.Enabled = true;
            buttonMainToggle.Text = "Start / Resume";
            buttonAutoStart.Enabled = true;
            buttonAutoStart.Text = "Auto-Start";
        }

        private void CheckBoxContinueTimer_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().ContinueTimer = checkBoxContinueTimer.Checked;
        }

        private void CheckBoxShowLastEffectsMain_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().MainShowLastEffects
                = listLastEffectsMain.Visible
                = checkBoxShowLastEffectsMain.Checked;
        }

        private void CheckBoxShowLastEffectsTwitch_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchShowLastEffects
                = listLastEffectsTwitch.Visible
                = checkBoxShowLastEffectsTwitch.Checked;
        }

        private void CheckBoxTwitchAllowOnlyEnabledEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchAllowOnlyEnabledEffectsRapidFire = checkBoxTwitchAllowOnlyEnabledEffects.Checked;
        }

        private void CheckBoxTwitchMajorityVoting_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchMajorityVoting = checkBoxTwitchMajorityVoting.Checked;
        }

        private void ButtonResetTwitch_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            stopwatch.Reset();
            elapsedCount = 0;
            timesUntilRapidFire = new Random().Next(10, 15);
            progressBarTwitch.Value = 0;
            buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
            buttonTwitchToggle.Text = "Start / Resume";
            buttonAutoStart.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
            buttonAutoStart.Text = "Auto-Start";
        }

        private void CheckBoxTwitch3TimesCooldown_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().Twitch3TimesCooldown = checkBoxTwitch3TimesCooldown.Checked;
        }

        private void ButtonEffectsToggleAll_Click(object sender, EventArgs e)
        {
            bool oneEnabled = false;
            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                if (node.Checked)
                {
                    oneEnabled = true;
                    break;
                }

                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Checked)
                    {
                        oneEnabled = true;
                        break;
                    }
                }
            }

            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.Checked = !oneEnabled;
                CheckAllChildNodes(node, !oneEnabled);
                node.UpdateCategory();
            }
        }

        private void viceCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Instance().SelectedGame = GameIdentifiers.ViceCity;
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects(GameIdentifiers.ViceCity);
            PopulateEffectTreeList();

            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.Checked = false;
                CheckAllChildNodes(node, false);
                node.UpdateCategory();
            }

            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.UpdateCategory();
            }
        }

        private void sanAndreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Instance().SelectedGame = GameIdentifiers.SanAndreas;
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects(GameIdentifiers.SanAndreas);
            PopulateEffectTreeList();

            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.Checked = false;
                CheckAllChildNodes(node, false);
                node.UpdateCategory();
            }

            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.UpdateCategory();
            }
        }
    }
}
