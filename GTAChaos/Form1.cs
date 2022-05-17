// Copyright (c) 2019 Lordmau5
using GTAChaos.Effects;
using GTAChaos.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GTAChaos.Forms
{
    public partial class Form1 : Form
    {
        private readonly string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.cfg");

        private readonly Stopwatch stopwatch;
        private readonly Dictionary<string, EffectTreeNode> idToEffectNodeMap = new();
        private IStreamConnection stream;

        private readonly System.Timers.Timer websocketReconnectionTimer;
        private int elapsedCount;
        private int timesUntilRapidFire;

#if DEBUG
        private readonly bool debug = true;
#else

        private readonly bool debug = false;
#endif

        public Form1()
        {
            this.InitializeComponent();

            this.Text = $"GTA Trilogy Chaos Mod v{Shared.Version}";
            if (!this.debug)
            {
                this.gameToolStripMenuItem.Visible = false;
                this.tabs.TabPages.Remove(this.tabExperimental);
                this.textBoxExperimentalEffectName.Visible = false;
                this.buttonExperimentalRunEffect.Visible = false;
            }
            else
            {
                this.Text += " (DEBUG)";
                this.textBoxMultiplayerServer.Text = "ws://localhost:12312";
            }

            this.tabs.TabPages.Remove(this.tabPolls);

            this.stopwatch = new Stopwatch();

            EffectDatabase.PopulateEffects("san_andreas");
            this.PopulateEffectTreeList();

            this.PopulateMainCooldowns();
            this.PopulatePresets();

            this.tabs.TabPages.Remove(this.tabStream);

            this.PopulateVotingTimes();
            this.PopulateVotingCooldowns();

            this.TryLoadConfig();

            this.timesUntilRapidFire = new Random().Next(10, 15);

            WebsocketHandler.INSTANCE.ConnectWebsocket();
            WebsocketHandler.INSTANCE.OnSocketMessage += this.OnSocketMessage;

            this.websocketReconnectionTimer = new System.Timers.Timer()
            {
                Interval = 1000,
                AutoReset = true
            };
            this.websocketReconnectionTimer.Elapsed += this.WebsocketReconnectionTimer_Elapsed;
            this.websocketReconnectionTimer.Start();
        }

        private void WebsocketReconnectionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) =>
            // This is hacky but it works
            WebsocketHandler.INSTANCE.ConnectWebsocket();

        private void OnSocketMessage(object sender, SocketMessageEventArgs e)
        {
            try
            {
                JObject json = JObject.Parse(e.Data);

                string type = Convert.ToString(json["type"]);
                string state = Convert.ToString(json["state"]);

                if (type == "ChaosMod" && state == "auto_start")
                {
                    this.Invoke(new Action(() => this.DoAutostart()));
                }
            }
            catch (Exception) { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => this.SaveConfig();

        private void TryLoadConfig()
        {
            try
            {
                JsonSerializer serializer = new();

                using StreamReader streamReader = new(this.configPath);
                using JsonReader reader = new JsonTextReader(streamReader);

                Config.SetInstance(serializer.Deserialize<Config>(reader));
                RandomHandler.SetSeed(Config.Instance().Seed);
            }
            catch (Exception) { }

            this.LoadPreset(Config.Instance().EnabledEffects);
            this.UpdateInterface();
        }

        private void SaveConfig()
        {
            try
            {
                Config.Instance().EnabledEffects.Clear();
                foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.EnabledEffects.Get())
                {
                    Config.Instance().EnabledEffects.Add(entry.item.GetID());
                }

                JsonSerializer serializer = new();

                using StreamWriter sw = new(this.configPath);
                using JsonTextWriter writer = new(sw);

                serializer.Serialize(writer, Config.Instance());
            }
            catch (Exception) { }
        }

        private void UpdateInterface()
        {
            bool found = false;
            foreach (MainCooldownComboBoxItem item in this.comboBoxMainCooldown.Items)
            {
                if (item.Time == Config.Instance().MainCooldown)
                {
                    this.comboBoxMainCooldown.SelectedItem = item;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                this.comboBoxMainCooldown.SelectedIndex = 3;
                Config.Instance().MainCooldown = 1000 * 60;
            }

            this.checkBoxAutoStart.Checked = Config.Instance().AutoStart;

            this.checkBoxStreamAllowOnlyEnabledEffects.Checked = Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire;

            found = false;
            foreach (VotingTimeComboBoxItem item in this.comboBoxVotingTime.Items)
            {
                if (item.VotingTime == Config.Instance().StreamVotingTime)
                {
                    this.comboBoxVotingTime.SelectedItem = item;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                this.comboBoxVotingTime.SelectedIndex = 0;
                Config.Instance().StreamVotingTime = 1000 * 30;
            }

            found = false;
            foreach (VotingCooldownComboBoxItem item in this.comboBoxVotingCooldown.Items)
            {
                if (item.VotingCooldown == Config.Instance().StreamVotingCooldown)
                {
                    this.comboBoxVotingCooldown.SelectedItem = item;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                this.comboBoxVotingCooldown.SelectedIndex = 1;
                Config.Instance().StreamVotingCooldown = 1000 * 60;
            }

            this.textBoxStreamAccessToken.Text = Config.Instance().StreamAccessToken;
            this.textBoxStreamClientID.Text = Config.Instance().StreamClientID;

            this.checkBoxPlayAudioForEffects.Checked = Config.Instance().PlayAudioForEffects;

            this.checkBoxShowLastEffectsMain.Checked = Config.Instance().MainShowLastEffects;
            this.checkBoxShowLastEffectsStream.Checked = Config.Instance().StreamShowLastEffects;
            this.checkBoxStream3TimesCooldown.Checked = Config.Instance().Stream3TimesCooldown;
            this.checkBoxStreamCombineVotingMessages.Checked = Config.Instance().StreamCombineChatMessages;
            this.checkBoxStreamEnableMultipleEffects.Checked = Config.Instance().StreamEnableMultipleEffects;
            this.checkBoxStreamEnableRapidFire.Checked = Config.Instance().StreamEnableRapidFire;

            this.checkBoxStreamMajorityVotes.Checked = Config.Instance().StreamMajorityVotes;
            this.checkBoxStreamEnableMultipleEffects.Enabled = Config.Instance().StreamMajorityVotes;

            this.checkBoxTwitchUsePolls.Checked = Config.Instance().TwitchUsePolls;
            this.checkBoxTwitchPollsPostMessages.Checked = Config.Instance().TwitchPollsPostMessages;
            this.numericUpDownTwitchPollsBitsCost.Value = Config.Instance().TwitchPollsBitsCost;
            this.numericUpDownTwitchPollsChannelPointsCost.Value = Config.Instance().TwitchPollsChannelPointsCost;

            this.checkBoxExperimental_EnableAllEffects.Checked = Config.Instance().Experimental_EnableAllEffects;
            this.checkBoxExperimental_RunEffectOnAutoStart.Checked = Config.Instance().Experimental_RunEffectOnAutoStart;
            this.textBoxExperimentalEffectName.Text = Config.Instance().Experimental_EffectName;
            this.checkBoxExperimentalYouTubeConnection.Checked = Config.Instance().Experimental_YouTubeConnection;

            this.textBoxSeed.Text = Config.Instance().Seed;

            /*
             * Update selections
             */
        }

        public void AddEffectToListBox(AbstractEffect effect)
        {
            string description = "Invalid";
            if (effect != null)
            {
                description = effect.GetDisplayName(DisplayNameType.UI);
                if (!string.IsNullOrEmpty(effect.Word))
                {
                    description += $" ({effect.Word})";
                }
            }

            ListBox listBox = Shared.IsStreamMode ? this.listLastEffectsStream : this.listLastEffectsMain;
            listBox.Items.Insert(0, description);
            if (listBox.Items.Count > 7)
            {
                listBox.Items.RemoveAt(7);
            }
        }

        private void CallEffect(AbstractEffect effect = null)
        {
            if (effect == null)
            {
                if (Config.Instance().Experimental_EnableAllEffects)
                {
                    foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.EnabledEffects.Get())
                    {
                        EffectDatabase.RunEffect(entry.item);
                        entry.item?.ResetStreamVoter();
                    }
                }
                else
                {
                    effect = EffectDatabase.GetRandomEffect(true);
                    if (Shared.Multiplayer != null)
                    {
                        Shared.Multiplayer.SendEffect(effect);
                    }
                    else
                    {
                        effect = EffectDatabase.RunEffect(effect);
                        effect?.ResetStreamVoter();

                        if (effect != null)
                        {
                            this.AddEffectToListBox(effect);
                        }
                    }
                }
            }
            else
            {
                if (Shared.Multiplayer != null)
                {
                    Shared.Multiplayer.SendEffect(effect);
                }
                else
                {
                    EffectDatabase.RunEffect(effect);
                    this.AddEffectToListBox(effect);
                }
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Shared.IsStreamMode)
            {
                this.TickStream();
            }
            else
            {
                this.TickMain();
            }
        }

        private void TickMain()
        {
            if (!Shared.TimerEnabled)
            {
                return;
            }

            int value = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);

            // Hack to fix Windows' broken-ass progress bar handling
            this.progressBarMain.Value = Math.Min(value, this.progressBarMain.Maximum);
            this.progressBarMain.Value = Math.Min(value - 1, this.progressBarMain.Maximum);

            int remaining = (int)Math.Max(0, Config.Instance().MainCooldown - this.stopwatch.ElapsedMilliseconds);

            WebsocketHandler.INSTANCE.SendTimeToGame(remaining, Config.Instance().MainCooldown);

            if (this.stopwatch.ElapsedMilliseconds - this.elapsedCount > 100)
            {
                Shared.Multiplayer?.SendTimeUpdate(remaining, Config.Instance().MainCooldown);

                this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
            }

            if (this.stopwatch.ElapsedMilliseconds >= Config.Instance().MainCooldown)
            {
                this.progressBarMain.Value = 0;
                this.CallEffect();
                this.elapsedCount = 0;
                this.stopwatch.Restart();
            }
        }

        private void TickStream()
        {
            if (!Shared.TimerEnabled)
            {
                return;
            }

            if (Shared.StreamVotingMode == 1)
            {
                if (this.progressBarStream.Maximum != Config.Instance().StreamVotingTime)
                {
                    this.progressBarStream.Maximum = Config.Instance().StreamVotingTime;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);
                this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - value, 0);
                this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - value - 1, 0);

                int remaining = (int)Math.Max(0, Config.Instance().StreamVotingTime - this.stopwatch.ElapsedMilliseconds);

                WebsocketHandler.INSTANCE.SendTimeToGame(remaining, Config.Instance().StreamVotingTime, "Voting");

                if (this.stopwatch.ElapsedMilliseconds - this.elapsedCount > 100)
                {
                    Shared.Multiplayer?.SendTimeUpdate(remaining, Config.Instance().StreamVotingTime);

                    this.stream?.SendEffectVotingToGame();

                    this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
                }

                bool didFinish;

                if (Config.Instance().TwitchUsePolls && this.stream != null && !Config.Instance().Experimental_YouTubeConnection)
                {
                    didFinish = this.stream.GetRemaining() == 0;

                    if (this.stopwatch.ElapsedMilliseconds >= Config.Instance().StreamVotingTime)
                    {
                        long millisecondsOver = this.stopwatch.ElapsedMilliseconds - Config.Instance().StreamVotingTime;
                        int waitLeft = Math.Max(0, 15000 - decimal.ToInt32(millisecondsOver));
                        this.labelStreamCurrentMode.Text = $"Current Mode: Waiting For Poll... ({Math.Ceiling((float)waitLeft / 1000)}s left)";

                        if (waitLeft == 0)
                        {
                            this.labelStreamCurrentMode.Text = "Current Mode: Cooldown (Poll Failed)";

                            WebsocketHandler.INSTANCE.SendTimeToGame(0);
                            Shared.Multiplayer?.SendTimeUpdate(0, Config.Instance().StreamVotingCooldown);
                            this.elapsedCount = 0;

                            this.progressBarStream.Value = 0;
                            this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;

                            this.stopwatch.Restart();
                            Shared.StreamVotingMode = 0;

                            this.stream?.SetVoting(3, this.timesUntilRapidFire, null);

                            return;
                        }
                    }
                }
                else
                {
                    didFinish = this.stopwatch.ElapsedMilliseconds >= Config.Instance().StreamVotingTime;
                }

                if (didFinish)
                {
                    WebsocketHandler.INSTANCE.SendTimeToGame(0);
                    Shared.Multiplayer?.SendTimeUpdate(0, Config.Instance().StreamVotingCooldown);
                    this.elapsedCount = 0;

                    this.progressBarStream.Value = 0;
                    this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;

                    this.stopwatch.Restart();
                    Shared.StreamVotingMode = 0;

                    this.labelStreamCurrentMode.Text = "Current Mode: Cooldown";

                    if (this.stream != null)
                    {
                        List<IVotingElement> elements = this.stream.GetVotedEffects();

                        bool zeroVotes = true;
                        foreach (IVotingElement e in elements)
                        {
                            if (e.GetVotes() > 0)
                            {
                                zeroVotes = false;
                            }
                        }

                        if (!zeroVotes)
                        {
                            foreach (IVotingElement e in elements)
                            {
                                float multiplier = e.GetEffect().GetMultiplier();
                                e.GetEffect().SetMultiplier(multiplier / elements.Count);
                                this.CallEffect(e.GetEffect());
                                e.GetEffect().SetMultiplier(multiplier);
                            }
                        }

                        this.stream.SetVoting(0, this.timesUntilRapidFire, zeroVotes ? null : elements);
                    }
                }
            }
            else if (Shared.StreamVotingMode == 2)
            {
                if (this.progressBarStream.Maximum != 1000 * 10)
                {
                    this.progressBarStream.Maximum = 1000 * 10;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);
                this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - value, 0);
                this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - value - 1, 0);

                int remaining = (int)Math.Max(0, (1000 * 10) - this.stopwatch.ElapsedMilliseconds);

                WebsocketHandler.INSTANCE.SendTimeToGame(remaining, 10000, "Rapid-Fire");

                if (this.stopwatch.ElapsedMilliseconds - this.elapsedCount > 100)
                {
                    Shared.Multiplayer?.SendTimeUpdate(remaining, 10000);

                    this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
                }

                if (this.stopwatch.ElapsedMilliseconds >= 1000 * 10) // Set 10 seconds
                {
                    WebsocketHandler.INSTANCE.SendTimeToGame(0);
                    Shared.Multiplayer?.SendTimeUpdate(0, Config.Instance().StreamVotingCooldown);
                    this.elapsedCount = 0;

                    this.progressBarStream.Value = 0;
                    this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;

                    this.stopwatch.Restart();
                    Shared.StreamVotingMode = 0;

                    this.labelStreamCurrentMode.Text = "Current Mode: Cooldown";

                    this.stream?.SetVoting(0, this.timesUntilRapidFire);
                }
            }
            else if (Shared.StreamVotingMode == 0)
            {
                if (this.progressBarStream.Maximum != Config.Instance().StreamVotingCooldown)
                {
                    this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;
                }

                // Hack to fix Windows' broken-ass progress bar handling
                int value = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);
                this.progressBarStream.Value = Math.Min(value + 1, this.progressBarStream.Maximum);
                this.progressBarStream.Value = Math.Min(value, this.progressBarStream.Maximum);

                int remaining = (int)Math.Max(0, Config.Instance().StreamVotingCooldown - this.stopwatch.ElapsedMilliseconds);

                WebsocketHandler.INSTANCE.SendTimeToGame(remaining, Config.Instance().StreamVotingCooldown, "Cooldown");

                if (this.stopwatch.ElapsedMilliseconds - this.elapsedCount > 100)
                {
                    Shared.Multiplayer?.SendTimeUpdate(remaining, Config.Instance().StreamVotingCooldown);

                    this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
                }

                if (this.stopwatch.ElapsedMilliseconds >= Config.Instance().StreamVotingCooldown)
                {
                    this.elapsedCount = 0;

                    if (Config.Instance().StreamEnableRapidFire)
                    {
                        this.timesUntilRapidFire--;
                    }

                    if (this.timesUntilRapidFire == 0 && Config.Instance().StreamEnableRapidFire)
                    {
                        this.progressBarStream.Value = this.progressBarStream.Maximum = 1000 * 10;

                        this.timesUntilRapidFire = new Random().Next(10, 15);

                        Shared.StreamVotingMode = 2;
                        this.labelStreamCurrentMode.Text = "Current Mode: Rapid-Fire";

                        this.stream?.SetVoting(2, this.timesUntilRapidFire);
                    }
                    else
                    {
                        this.progressBarStream.Value = this.progressBarStream.Maximum = Config.Instance().StreamVotingTime;

                        Shared.StreamVotingMode = 1;
                        this.labelStreamCurrentMode.Text = "Current Mode: Voting";

                        this.stream?.SetVoting(1, this.timesUntilRapidFire);
                    }

                    this.stopwatch.Restart();
                }
            }
        }

        private void PopulateEffectTreeList()
        {
            this.enabledEffectsView.Nodes.Clear();
            this.idToEffectNodeMap.Clear();

            // Add Categories
            foreach (Category cat in Category.Categories)
            {
                if (cat.GetEffectCount() > 0)
                {
                    this.enabledEffectsView.Nodes.Add(new CategoryTreeNode(cat));
                }
            }

            // Add Effects
            foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
            {
                AbstractEffect effect = entry.item;

                if (this.idToEffectNodeMap.ContainsKey(effect.GetID()))
                {
                    MessageBox.Show($"Tried adding effect with ID that was already present: '{effect.GetID()}'");
                }

                TreeNode node = this.enabledEffectsView.Nodes.Find(effect.Category.Name, false).FirstOrDefault();

                string Description = effect.GetDisplayName(DisplayNameType.UI);

                EffectTreeNode addedNode = new(effect, Description)
                {
                    Checked = true,
                };
                node.Nodes.Add(addedNode);
                this.idToEffectNodeMap[effect.GetID()] = addedNode;
            }
        }

        private void PopulatePresets()
        {
            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = false;
                this.CheckAllChildNodes(node, false);
                node.UpdateCategory();
            }
        }

        private void PopulateMainCooldowns()
        {
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("10 seconds", 1000 * 10));
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("20 seconds", 1000 * 20));
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("30 seconds", 1000 * 30));
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("1 minute", 1000 * 60));
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("2 minutes", 1000 * 60 * 2));
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("3 minutes", 1000 * 60 * 3));
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("5 minutes", 1000 * 60 * 5));
            this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("10 minutes", 1000 * 60 * 10));

            if (this.debug)
            {
                this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("DEBUG - 1 second", 1000));
                this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("DEBUG - 500ms", 500));
                this.comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("DEBUG - 10ms", 10));
            }

            this.comboBoxMainCooldown.SelectedIndex = 3;

            Config.Instance().MainCooldown = 1000 * 60;
        }

        private class MainCooldownComboBoxItem
        {
            public readonly string Text;
            public readonly int Time;

            public MainCooldownComboBoxItem(string text, int time)
            {
                this.Text = text;
                this.Time = time;
            }

            public override string ToString() => this.Text;
        }

        private void MainCooldownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainCooldownComboBoxItem item = (MainCooldownComboBoxItem)this.comboBoxMainCooldown.SelectedItem;
            Config.Instance().MainCooldown = item.Time;

            if (!Shared.TimerEnabled)
            {
                this.progressBarMain.Value = 0;
                this.progressBarMain.Maximum = Config.Instance().MainCooldown;
                this.elapsedCount = 0;
                this.stopwatch.Reset();
            }
        }

        private void PopulateVotingTimes()
        {
            this.comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("30 seconds", 1000 * 30));
            this.comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("1 minute", 1000 * 60));

            this.comboBoxVotingTime.SelectedIndex = 0;

            Config.Instance().StreamVotingTime = 1000 * 30;
        }

        private class VotingTimeComboBoxItem
        {
            public readonly int VotingTime;
            public readonly string Text;

            public VotingTimeComboBoxItem(string text, int votingTime)
            {
                this.Text = text;
                this.VotingTime = votingTime;
            }

            public override string ToString() => this.Text;
        }

        private void ComboBoxVotingTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingTimeComboBoxItem item = (VotingTimeComboBoxItem)this.comboBoxVotingTime.SelectedItem;
            Config.Instance().StreamVotingTime = item.VotingTime;
        }

        private void PopulateVotingCooldowns()
        {
            this.comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("30 seconds", 1000 * 30));
            this.comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("1 minute", 1000 * 60));
            this.comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("2 minutes", 1000 * 60 * 2));
            this.comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("3 minutes", 1000 * 60 * 3));
            this.comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("5 minutes", 1000 * 60 * 5));
            this.comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("10 minutes", 1000 * 60 * 10));

            if (this.debug)
            {
                this.comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("5 seconds", 1000 * 5));
            }

            this.comboBoxVotingCooldown.SelectedIndex = 1;

            Config.Instance().StreamVotingCooldown = 1000 * 60;
        }

        private class VotingCooldownComboBoxItem
        {
            public readonly int VotingCooldown;
            public readonly string Text;

            public VotingCooldownComboBoxItem(string text, int votingCooldown)
            {
                this.Text = text;
                this.VotingCooldown = votingCooldown;
            }

            public override string ToString() => this.Text;
        }

        private void ComboBoxVotingCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingCooldownComboBoxItem item = (VotingCooldownComboBoxItem)this.comboBoxVotingCooldown.SelectedItem;
            Config.Instance().StreamVotingCooldown = item.VotingCooldown;
        }

        private void DoAutostart()
        {
            if (!this.checkBoxAutoStart.Checked)
            {
                return;
            }

            this.elapsedCount = 0;
            this.stopwatch.Reset();
            this.SetEnabled(true);

            if (Config.Instance().Experimental_RunEffectOnAutoStart && !Shared.IsStreamMode)
            {
                this.CallEffect();
            }
        }

        private void SetEnabled(bool enabled)
        {
            Shared.TimerEnabled = enabled;
            if (Shared.TimerEnabled)
            {
                this.stopwatch.Start();
            }
            else
            {
                this.stopwatch.Stop();
            }

            this.buttonMainToggle.Enabled = true;
            (Shared.IsStreamMode ? this.buttonStreamToggle : this.buttonMainToggle).Text = Shared.TimerEnabled ? "Stop / Pause" : "Start / Resume";
            this.comboBoxMainCooldown.Enabled =
                this.buttonSwitchMode.Enabled =
                this.buttonResetMain.Enabled =
                this.buttonResetStream.Enabled = !Shared.TimerEnabled;

            this.comboBoxVotingTime.Enabled =
                this.comboBoxVotingCooldown.Enabled =
                this.textBoxSeed.Enabled = !Shared.TimerEnabled;
        }

        private void ButtonMainToggle_Click(object sender, EventArgs e) => this.SetEnabled(!Shared.TimerEnabled);

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
                    this.CheckAllChildNodes(node, nodeChecked);
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
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }

                foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
                {
                    node.UpdateCategory();
                }
            }
        }

        private void LoadPreset(List<string> enabledEffects)
        {
            this.PopulatePresets();

            foreach (string effect in enabledEffects)
            {
                if (this.idToEffectNodeMap.TryGetValue(effect, out EffectTreeNode node))
                {
                    node.Checked = true;
                    EffectDatabase.SetEffectEnabled(node.Effect, true);
                }
            }

            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.UpdateCategory();
            }
        }

        private class CategoryTreeNode : TreeNode
        {
            private readonly Category category;

            public CategoryTreeNode(Category _category)
            {
                this.category = _category;
                this.Name = this.Text = this.category.Name;
            }

            public void UpdateCategory()
            {
                bool newChecked = true;
                int enabled = 0;
                foreach (TreeNode node in this.Nodes)
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

                this.Checked = newChecked;
                this.Text = this.Name + $" ({enabled}/{this.Nodes.Count})";
            }
        }

        private class EffectTreeNode : TreeNode
        {
            public readonly AbstractEffect Effect;

            public EffectTreeNode(AbstractEffect effect, string description)
            {
                this.Effect = effect;

                this.Name = this.Text = description;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => this.Close();

        private void LoadPresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Preset File|*.cfg",
                Title = "Load Preset"
            };
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                string content = System.IO.File.ReadAllText(dialog.FileName);
                string[] enabledEffects = content.Split(',');

                List<string> enabledEffectList = new();
                foreach (string effect in enabledEffects)
                {
                    enabledEffectList.Add(effect);
                }

                this.LoadPreset(enabledEffectList);
            }

            dialog.Dispose();
        }

        private void SavePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> enabledEffects = new();
            foreach (EffectTreeNode node in this.idToEffectNodeMap.Values)
            {
                if (node.Checked)
                {
                    enabledEffects.Add(node.Effect.GetID());
                }
            }

            string joined = string.Join(",", enabledEffects);

            SaveFileDialog dialog = new()
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

        private async void ButtonConnectStream_Click(object sender, EventArgs e)
        {
            if (this.stream?.IsConnected() == true || this.buttonConnectStream.Text == "Disconnect")
            {
                this.stream?.Kill();
                this.stream = null;

                this.buttonSwitchMode.Enabled = true;

                this.comboBoxVotingTime.Enabled = true;
                this.comboBoxVotingCooldown.Enabled = true;

                this.textBoxStreamAccessToken.Enabled = true;
                this.textBoxStreamClientID.Enabled = true;

                this.buttonStreamToggle.Enabled = false;

                this.checkBoxTwitchUsePolls.Enabled = true;

                this.buttonConnectStream.Text = "Connect to Stream";

                if (!this.tabs.TabPages.Contains(this.tabEffects))
                {
                    this.tabs.TabPages.Insert(this.tabs.TabPages.IndexOf(this.tabStream), this.tabEffects);
                }

                this.SetEnabled(false);

                return;
            }

            if (!string.IsNullOrEmpty(Config.Instance().StreamAccessToken) && !string.IsNullOrEmpty(Config.Instance().StreamClientID))
            {
                this.buttonSwitchMode.Enabled = false;

                this.buttonConnectStream.Enabled = false;
                this.textBoxStreamAccessToken.Enabled = false;
                this.textBoxStreamClientID.Enabled = false;

                this.stream = Config.Instance().Experimental_YouTubeConnection
                    ? new YouTubeChatConnection()
                    : Config.Instance().TwitchUsePolls ? new TwitchPollConnection() : new TwitchChatConnection();

                this.stream.OnRapidFireEffect += (_sender, rapidFireArgs) => this.Invoke(new Action(() =>
                {
                    if (Shared.StreamVotingMode == 2)
                    {
                        if (Shared.Multiplayer != null)
                        {
                            Shared.Multiplayer.SendEffect(rapidFireArgs.Effect, 1000 * 15);
                        }
                        else
                        {
                            rapidFireArgs.Effect.RunEffect(-1, 1000 * 15);
                            this.AddEffectToListBox(rapidFireArgs.Effect);
                        }
                    }
                }));

                this.stream.OnLoginError += (_sender, _e) =>
                {
                    MessageBox.Show("There was an error trying to log in to the account. Invalid Access Token?", "Stream Login Error");
                    this.Invoke(new Action(() =>
                    {
                        this.buttonSwitchMode.Enabled = true;

                        this.buttonConnectStream.Enabled = true;
                        this.textBoxStreamAccessToken.Enabled = true;
                        this.textBoxStreamClientID.Enabled = true;
                    }));
                    this.stream?.Kill();
                    this.stream = null;
                };

                this.stream.OnConnected += (_sender, _e) => this.Invoke(new Action(() =>
                {
                    this.buttonConnectStream.Enabled = true;
                    this.buttonStreamToggle.Enabled = true;

                    this.buttonConnectStream.Text = "Disconnect";

                    this.textBoxStreamAccessToken.Enabled = false;
                    this.textBoxStreamClientID.Enabled = false;

                    this.checkBoxTwitchUsePolls.Enabled = false;
                }));

                this.stream.OnDisconnected += (_sender, _e) => this.Invoke(new Action(() =>
                {
                    this.stream = null;

                    this.buttonSwitchMode.Enabled = true;

                    this.comboBoxVotingTime.Enabled = true;
                    this.comboBoxVotingCooldown.Enabled = true;

                    this.textBoxStreamAccessToken.Enabled = true;
                    this.textBoxStreamClientID.Enabled = true;

                    this.buttonStreamToggle.Enabled = false;

                    this.checkBoxTwitchUsePolls.Enabled = true;

                    this.buttonConnectStream.Text = "Connect to Stream";

                    if (!this.tabs.TabPages.Contains(this.tabEffects))
                    {
                        this.tabs.TabPages.Insert(this.tabs.TabPages.IndexOf(this.tabStream), this.tabEffects);
                    }

                    this.SetEnabled(false);
                }));

                await this.stream.TryConnect();
            }
        }

        private void UpdateStreamConnectButtonState()
        {
            this.buttonConnectStream.Enabled = !string.IsNullOrEmpty(Config.Instance().StreamAccessToken)
                && !string.IsNullOrEmpty(Config.Instance().StreamClientID);
        }

        private void TextBoxOAuth_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().StreamAccessToken = this.textBoxStreamAccessToken.Text;

            this.UpdateStreamConnectButtonState();
        }

        private void SwitchMode(bool isStreamMode)
        {
            if (!isStreamMode)
            {
                this.buttonSwitchMode.Text = "Stream";

                if (!this.tabs.TabPages.Contains(this.tabMain))
                {
                    this.tabs.TabPages.Insert(0, this.tabMain);
                }

                this.tabs.SelectedIndex = 0;
                this.tabs.TabPages.Remove(this.tabStream);

                this.tabs.TabPages.Remove(this.tabPolls);

                this.listLastEffectsMain.Items.Clear();
                this.progressBarMain.Value = 0;

                this.elapsedCount = 0;

                this.stopwatch.Reset();
                this.SetEnabled(false);
            }
            else
            {
                this.buttonSwitchMode.Text = "Main";

                if (!this.tabs.TabPages.Contains(this.tabStream))
                {
                    this.tabs.TabPages.Insert(0, this.tabStream);
                }

                this.tabs.SelectedIndex = 0;
                this.tabs.TabPages.Remove(this.tabMain);

                if (Config.Instance().TwitchUsePolls && !this.tabs.TabPages.Contains(this.tabPolls))
                {
                    this.tabs.TabPages.Insert(2, this.tabPolls);
                }

                this.listLastEffectsStream.Items.Clear();
                this.progressBarStream.Value = 0;

                this.elapsedCount = 0;

                this.stopwatch.Reset();
                this.SetEnabled(false);
            }
        }

        private void ButtonSwitchMode_Click(object sender, EventArgs e)
        {
            Shared.IsStreamMode = !Shared.IsStreamMode;
            this.SwitchMode(Shared.IsStreamMode);
        }

        private void ButtonStreamToggle_Click(object sender, EventArgs e) => this.SetEnabled(!Shared.TimerEnabled);

        private void TextBoxSeed_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().Seed = this.textBoxSeed.Text;
            RandomHandler.SetSeed(Config.Instance().Seed);
        }

        private void ButtonResetMain_Click(object sender, EventArgs e)
        {
            this.SetEnabled(false);
            RandomHandler.SetSeed(Config.Instance().Seed);
            this.stopwatch.Reset();
            this.elapsedCount = 0;
            this.progressBarMain.Value = 0;
            this.buttonMainToggle.Enabled = true;
            this.buttonMainToggle.Text = "Start / Resume";
        }

        private void CheckBoxShowLastEffectsMain_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().MainShowLastEffects
                = this.listLastEffectsMain.Visible
                = this.checkBoxShowLastEffectsMain.Checked;
        }

        private void CheckBoxShowLastEffectsStream_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().StreamShowLastEffects
                = this.listLastEffectsStream.Visible
                = this.checkBoxShowLastEffectsStream.Checked;
        }

        private void CheckBoxStreamAllowOnlyEnabledEffects_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire = this.checkBoxStreamAllowOnlyEnabledEffects.Checked;

        private void ButtonResetStream_Click(object sender, EventArgs e)
        {
            this.SetEnabled(false);
            RandomHandler.SetSeed(Config.Instance().Seed);
            this.stopwatch.Reset();
            this.elapsedCount = 0;
            this.labelStreamCurrentMode.Text = "Current Mode: Cooldown";
            Shared.StreamVotingMode = 0;
            this.timesUntilRapidFire = new Random().Next(10, 15);
            this.progressBarStream.Value = 0;
            this.buttonStreamToggle.Enabled = this.stream?.IsConnected() == true;
            this.buttonStreamToggle.Text = "Start / Resume";
        }

        private void CheckBoxStream3TimesCooldown_CheckedChanged(object sender, EventArgs e) => Config.Instance().Stream3TimesCooldown = this.checkBoxStream3TimesCooldown.Checked;

        private void ButtonEffectsToggleAll_Click(object sender, EventArgs e)
        {
            bool oneEnabled = false;
            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
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

            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = !oneEnabled;
                this.CheckAllChildNodes(node, !oneEnabled);
                node.UpdateCategory();
            }
        }

        private void ViceCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shared.SelectedGame = "vice_city";
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects("vice_city");
            this.PopulateEffectTreeList();

            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = false;
                this.CheckAllChildNodes(node, false);
                node.UpdateCategory();
            }

            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.UpdateCategory();
            }
        }

        private void SanAndreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shared.SelectedGame = "san_andreas";
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects("san_andreas");
            this.PopulateEffectTreeList();

            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = false;
                this.CheckAllChildNodes(node, false);
                node.UpdateCategory();
            }

            foreach (CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.UpdateCategory();
            }
        }

        private void CheckBoxStreamCombineVotingMessages_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamCombineChatMessages = this.checkBoxStreamCombineVotingMessages.Checked;

        private void UpdatePollTabVisibility()
        {
            if (Config.Instance().TwitchUsePolls)
            {
                if (!this.tabs.TabPages.Contains(this.tabPolls) && Shared.IsStreamMode)
                {
                    this.tabs.TabPages.Insert(2, this.tabPolls);
                }
            }
            else
            {
                this.tabs.TabPages.Remove(this.tabPolls);
            }
        }

        private void CheckBoxTwitchUsePolls_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchUsePolls = this.checkBoxTwitchUsePolls.Checked;
            this.UpdatePollTabVisibility();
        }

        private void CheckBoxStreamEnableMultipleEffects_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamEnableMultipleEffects = this.checkBoxStreamEnableMultipleEffects.Checked;

        private void NumericUpDownTwitchPollsBitsCost_ValueChanged(object sender, EventArgs e) => Config.Instance().TwitchPollsBitsCost = decimal.ToInt32(this.numericUpDownTwitchPollsBitsCost.Value);

        private void NumericUpDownTwitchPollsChannelPointsCost_ValueChanged(object sender, EventArgs e) => Config.Instance().TwitchPollsChannelPointsCost = decimal.ToInt32(this.numericUpDownTwitchPollsChannelPointsCost.Value);

        private void CheckBoxTwitchPollsPostMessages_CheckedChanged(object sender, EventArgs e) => Config.Instance().TwitchPollsPostMessages = this.checkBoxTwitchPollsPostMessages.Checked;

        private void EnabledEffectsView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is EffectTreeNode node && this.debug)
            {
                node.Effect.RunEffect();
            }
        }

        private void CheckBoxPlayAudioForEffects_CheckedChanged(object sender, EventArgs e) => Config.Instance().PlayAudioForEffects = this.checkBoxPlayAudioForEffects.Checked;

        private string FilterMultiplayerCharacters(string text) => Regex.Replace(text, "[^A-Za-z0-9]", "");

        private void UpdateButtonState()
        {
            this.buttonMultiplayerConnect.Enabled
                = !string.IsNullOrEmpty(this.textBoxMultiplayerServer.Text)
                && !string.IsNullOrEmpty(this.textBoxMultiplayerChannel.Text)
                && !string.IsNullOrEmpty(this.textBoxMultiplayerUsername.Text);
        }

        private void TextBoxMultiplayerServer_TextChanged(object sender, EventArgs e) => this.UpdateButtonState();

        private void TextBoxMultiplayerChannel_TextChanged(object sender, EventArgs e)
        {
            this.textBoxMultiplayerChannel.Text = this.FilterMultiplayerCharacters(this.textBoxMultiplayerChannel.Text);
            this.UpdateButtonState();
        }

        private void TextBoxMultiplayerUsername_TextChanged(object sender, EventArgs e)
        {
            this.textBoxMultiplayerUsername.Text = this.FilterMultiplayerCharacters(this.textBoxMultiplayerUsername.Text);
            this.UpdateButtonState();
        }

        private void UpdateMultiplayerConnectionState(int state)
        {
            this.Invoke(new Action(() =>
            {
                if (state == 0) // Not connected
                {
                    this.textBoxMultiplayerServer.Enabled
                        = this.textBoxMultiplayerChannel.Enabled
                        = this.textBoxMultiplayerUsername.Enabled
                        = this.buttonMultiplayerConnect.Enabled
                        = true;

                    this.buttonMultiplayerConnect.Text = "Connect";

                    this.buttonSwitchMode.Enabled = true;
                    this.buttonMainToggle.Enabled = true;
                    this.buttonResetMain.Enabled = true;
                    this.comboBoxMainCooldown.Enabled = true;
                    this.enabledEffectsView.Enabled = true;
                    this.textBoxSeed.Enabled = true;
                }
                else if (state == 1) // Connecting...
                {
                    this.textBoxMultiplayerServer.Enabled
                        = this.textBoxMultiplayerChannel.Enabled
                        = this.textBoxMultiplayerUsername.Enabled
                        = this.buttonMultiplayerConnect.Enabled
                        = false;

                    this.buttonMultiplayerConnect.Text = "Connecting...";
                }
                else if (state == 2) // Connected
                {
                    this.textBoxMultiplayerServer.Enabled
                        = this.textBoxMultiplayerChannel.Enabled
                        = this.textBoxMultiplayerUsername.Enabled
                        = false;

                    this.buttonMultiplayerConnect.Enabled = true;
                    this.buttonMultiplayerConnect.Text = "Disconnect";
                }
            }));
        }

        private void ShowMessageBox(string text, string caption) => this.Invoke(new Action(() => MessageBox.Show(this, text, caption)));

        private void AddToMultiplayerChatHistory(string message)
        {
            this.Invoke(new Action(() =>
            {
                this.listBoxMultiplayerChat.Items.Add(message);
                this.listBoxMultiplayerChat.TopIndex = this.listBoxMultiplayerChat.Items.Count - 1;
            }));
        }

        private void ClearMultiplayerChatHistory() => this.Invoke(new Action(() => this.listBoxMultiplayerChat.Items.Clear()));

        private void ButtonMultiplayerConnect_Click(object sender, EventArgs e)
        {
            Shared.Multiplayer?.Disconnect();

            if (this.buttonMultiplayerConnect.Text == "Disconnect")
            {
                this.UpdateMultiplayerConnectionState(0);
                return;
            }

            this.ClearMultiplayerChatHistory();

            Shared.Multiplayer = new Multiplayer(
                this.textBoxMultiplayerServer.Text,
                this.textBoxMultiplayerChannel.Text,
                this.textBoxMultiplayerUsername.Text
            );

            this.UpdateMultiplayerConnectionState(1);

            Shared.Multiplayer.OnConnectionFailed += (_sender, args) =>
            {
                this.ShowMessageBox("Connection failed - is the server running?", "Error");
                this.UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnUsernameInUse += (_sender, args) =>
            {
                this.ShowMessageBox("Username already in use!", "Error");
                this.UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnConnectionSuccessful += (_sender, args) =>
            {
                this.ShowMessageBox("Successfully connected!", "Connected");
                this.AddToMultiplayerChatHistory($"Successfully connected to channel: {this.textBoxMultiplayerChannel.Text}");
                this.UpdateMultiplayerConnectionState(2);

                this.Invoke(new Action(() =>
                {
                    if (!args.IsHost)
                    {
                        this.SwitchMode(false);

                        this.buttonSwitchMode.Enabled = false;
                        this.buttonMainToggle.Enabled = false;
                        this.buttonResetMain.Enabled = false;
                        this.comboBoxMainCooldown.Enabled = false;
                        this.enabledEffectsView.Enabled = false;
                        this.buttonResetMain.Enabled = false;
                        this.textBoxSeed.Enabled = false;
                    }

                    this.labelMultiplayerHost.Text = $"Host: {args.HostUsername}";
                    if (args.IsHost)
                    {
                        this.labelMultiplayerHost.Text += " (You!)";
                    }
                }));
            };

            Shared.Multiplayer.OnHostLeftChannel += (_sender, args) =>
            {
                this.ShowMessageBox("Host has left the channel; Disconnected.", "Host Left");
                this.AddToMultiplayerChatHistory("Host has left the channel; Disconnected.");
                this.UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnVersionMismatch += (_sender, args) =>
            {
                this.ShowMessageBox($"Channel is v{args.Version} but you have v{Shared.Version}; Disconnected.", "Version Mismatch");
                this.AddToMultiplayerChatHistory($"Channel is v{args.Version} but you have v{Shared.Version}; Disconnected.");
                this.UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnUserJoined += (_sender, args) => this.AddToMultiplayerChatHistory($"{args.Username} joined!");

            Shared.Multiplayer.OnUserLeft += (_sender, args) => this.AddToMultiplayerChatHistory($"{args.Username} left!");

            Shared.Multiplayer.OnChatMessage += (_sender, args) => this.AddToMultiplayerChatHistory($"{args.Username}: {args.Message}");

            Shared.Multiplayer.OnTimeUpdate += (_sender, args) =>
            {
                if (!Shared.Multiplayer.IsHost)
                {
                    WebsocketHandler.INSTANCE.SendTimeToGame(args.Remaining, args.Total);
                }
            };

            Shared.Multiplayer.OnEffect += (_sender, args) =>
            {
                AbstractEffect effect = EffectDatabase.GetByWord(args.Word);
                if (effect != null)
                {
                    if (string.IsNullOrEmpty(args.Voter) || args.Voter == "N/A")
                    {
                        effect.ResetStreamVoter();
                    }
                    else
                    {
                        effect.SetTreamVoter(args.Voter);
                    }

                    EffectDatabase.RunEffect(effect, args.Seed, args.Duration);

                    this.AddEffectToListBox(effect);
                }
            };

            Shared.Multiplayer.OnVotes += (_sender, args) =>
            {
                string[] effects = args.Effects;
                int[] votes = args.Votes;

                WebsocketHandler.INSTANCE.SendVotes(effects, votes, args.LastChoice);
            };

            Shared.Multiplayer.Connect();
        }

        private void TextBoxMultiplayerChat_TextChanged(object sender, EventArgs e) => this.buttonMultiplayerSend.Enabled = Shared.Multiplayer != null && !string.IsNullOrEmpty(this.textBoxMultiplayerChat.Text);

        private void ButtonMultiplayerSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBoxMultiplayerChat.Text))
            {
                Shared.Multiplayer?.SendChatMessage(this.textBoxMultiplayerChat.Text);
                this.textBoxMultiplayerChat.Text = "";
            }
        }

        private void TextBoxMultiplayerChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(this.textBoxMultiplayerChat.Text))
            {
                Shared.Multiplayer?.SendChatMessage(this.textBoxMultiplayerChat.Text);
                this.textBoxMultiplayerChat.Text = "";
            }
        }

        private void CheckBoxExperimental_EnableAllEffects_CheckedChanged(object sender, EventArgs e) => Config.Instance().Experimental_EnableAllEffects = this.checkBoxExperimental_EnableAllEffects.Checked;

        private void CheckBoxExperimental_EnableEffectOnAutoStart_CheckedChanged(object sender, EventArgs e) => Config.Instance().Experimental_RunEffectOnAutoStart = this.checkBoxExperimental_RunEffectOnAutoStart.Checked;

        private void ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.tabs.TabPages.Contains(this.tabExperimental))
            {
                this.tabs.TabPages.Add(this.tabExperimental);
            }

            this.experimentalToolStripMenuItem.Visible = false;
        }

        private void ButtonExperimentalRunEffect_Click(object sender, EventArgs e)
        {
            string textInput = this.textBoxExperimentalEffectName.Text;
            if (string.IsNullOrEmpty(textInput))
            {
                return;
            }

            // Try and get an effect by it's ID
            AbstractEffect effect = EffectDatabase.GetByID(textInput);
            // Try and get an effect by it's ID with "effect_" at the start
            if (effect == null)
            {
                effect = EffectDatabase.GetByID($"effect_{textInput}");
            }
            // Try and get an effect by it's "cheat" word
            if (effect == null)
            {
                effect = EffectDatabase.GetByWord(textInput);
            }

            if (effect != null)
            {
                effect.RunEffect();
                return;
            }

            int duration = Config.GetEffectDuration();
            WebsocketHandler.INSTANCE.SendEffectToGame(this.textBoxExperimentalEffectName.Text, new
            {
                seed = RandomHandler.Next(9999999)
            }, duration);
        }

        private void TextBoxExperimentalEffectName_TextChanged(object sender, EventArgs e) => Config.Instance().Experimental_EffectName = this.textBoxExperimentalEffectName.Text;

        private void CheckBoxAutoStart_CheckedChanged(object sender, EventArgs e) => Config.Instance().AutoStart = this.checkBoxAutoStart.Checked;

        private void CheckBoxStreamEnableRapidFire_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamEnableRapidFire = this.checkBoxStreamEnableRapidFire.Checked;

        private void CheckBoxStreamMajorityVotes_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().StreamMajorityVotes = this.checkBoxStreamMajorityVotes.Checked;

            this.checkBoxStreamEnableMultipleEffects.Enabled = Config.Instance().StreamMajorityVotes;
        }

        private void LinkLabelTwitchGetAccessToken_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start("https://chaos.lord.moe/");

        private void CheckBoxExperimentalYouTubeConnection_CheckedChanged(object sender, EventArgs e) => Config.Instance().Experimental_YouTubeConnection = this.checkBoxExperimentalYouTubeConnection.Checked;

        private void TextBoxStreamClientID_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().StreamClientID = this.textBoxStreamClientID.Text;

            this.UpdateStreamConnectButtonState();
        }
    }
}
