// Copyright (c) 2019 Lordmau5
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;
using Newtonsoft.Json;

namespace GtaChaos.Forms
{
    public partial class Form1 : Form
    {
        private readonly string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.cfg");

        private readonly Stopwatch stopwatch;
        private readonly Dictionary<string, EffectTreeNode> idToEffectNodeMap = new Dictionary<string, EffectTreeNode>();
        private ITwitchConnection twitch;

        private int elapsedCount;
        private readonly System.Timers.Timer autoStartTimer;
        private int introState = 1;

        private int timesUntilRapidFire;

#if DEBUG
        private readonly bool debug = true;
#else

        private readonly bool debug = false;
#endif

        public Form1()
        {
            InitializeComponent();

            Text = $"GTA Trilogy Chaos Mod v{Shared.Version}";
            if (!debug)
            {
                gameToolStripMenuItem.Visible = false;
                tabs.TabPages.Remove(tabExperimental);
            }
            else
            {
                Text += " (DEBUG)";
                textBoxMultiplayerServer.Text = "ws://localhost:12312";
            }

            tabs.TabPages.Remove(tabPolls);

            stopwatch = new Stopwatch();
            autoStartTimer = new System.Timers.Timer()
            {
                Interval = 50,
                AutoReset = true
            };
            autoStartTimer.Elapsed += AutoStartTimer_Elapsed;

            EffectDatabase.PopulateEffects("san_andreas");
            PopulateEffectTreeList();

            PopulateMainCooldowns();
            PopulatePresets();

            tabs.TabPages.Remove(tabTwitch);

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

            if (Shared.TimerEnabled)
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
            bool found = false;
            foreach (MainCooldownComboBoxItem item in comboBoxMainCooldown.Items)
            {
                if (item.Time == Config.Instance().MainCooldown)
                {
                    comboBoxMainCooldown.SelectedItem = item;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                comboBoxMainCooldown.SelectedIndex = 3;
                Config.Instance().MainCooldown = 1000 * 60;
            }

            checkBoxTwitchAllowOnlyEnabledEffects.Checked = Config.Instance().TwitchAllowOnlyEnabledEffectsRapidFire;

            found = false;
            foreach (VotingTimeComboBoxItem item in comboBoxVotingTime.Items)
            {
                if (item.VotingTime == Config.Instance().TwitchVotingTime)
                {
                    comboBoxVotingTime.SelectedItem = item;
                    break;
                }
            }
            if (!found)
            {
                comboBoxVotingTime.SelectedIndex = 0;
                Config.Instance().TwitchVotingTime = 1000 * 30;
            }

            found = false;
            foreach (VotingCooldownComboBoxItem item in comboBoxVotingCooldown.Items)
            {
                if (item.VotingCooldown == Config.Instance().TwitchVotingCooldown)
                {
                    comboBoxVotingCooldown.SelectedItem = item;
                    break;
                }
            }
            if (!found)
            {
                comboBoxVotingCooldown.SelectedIndex = 1;
                Config.Instance().TwitchVotingCooldown = 1000 * 60;
            }

            textBoxTwitchChannel.Text = Config.Instance().TwitchChannel;
            textBoxTwitchUsername.Text = Config.Instance().TwitchUsername;
            textBoxTwitchOAuth.Text = Config.Instance().TwitchOAuthToken;

            checkBoxContinueTimer.Checked = Config.Instance().ContinueTimer;
            checkBoxPlayAudioForEffects.Checked = Config.Instance().PlayAudioForEffects;

            checkBoxShowLastEffectsMain.Checked = Config.Instance().MainShowLastEffects;
            checkBoxShowLastEffectsTwitch.Checked = Config.Instance().TwitchShowLastEffects;
            checkBoxTwitch3TimesCooldown.Checked = Config.Instance().Twitch3TimesCooldown;
            checkBoxTwitchCombineVotingMessages.Checked = Config.Instance().TwitchCombineChatMessages;
            checkBoxTwitchUsePolls.Checked = Config.Instance().TwitchUsePolls;
            checkBoxTwitchEnableMultipleEffects.Checked = Config.Instance().TwitchEnableMultipleEffects;

            checkBoxTwitchPollsPostMessages.Checked = Config.Instance().TwitchPollsPostMessages;
            checkBoxTwitchPollsSubcriberMultiplier.Checked = Config.Instance().TwitchPollsSubcriberMultiplier;
            checkBoxTwitchPollsSubscriberOnly.Checked = Config.Instance().TwitchPollsSubscriberOnly;
            numericUpDownTwitchPollsBitsCost.Value = Config.Instance().TwitchPollsBitsCost;
            textBoxTwitchPollsPassphrase.Text = Config.Instance().TwitchPollsPassphrase;
            checkBoxTwitchAppendFakePassCurrentMission.Checked = Config.Instance().TwitchAppendFakePassCurrentMission;

            checkBoxExperimental_EnableAllEffects.Checked = Config.Instance().Experimental_EnableAllEffects;
            checkBoxExperimental_EnableEffectOnAutoStart.Checked = Config.Instance().Experimental_RunEffectOnAutoStart;

            textBoxSeed.Text = Config.Instance().Seed;

            /*
             * Update selections
             */
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

            ListBox listBox = Shared.IsTwitchMode ? listLastEffectsTwitch : listLastEffectsMain;
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
                if (Config.Instance().Experimental_EnableAllEffects)
                {
                    foreach (AbstractEffect e in EffectDatabase.EnabledEffects)
                    {
                        EffectDatabase.RunEffect(e);
                        e?.ResetVoter();
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
                        effect?.ResetVoter();
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
                }
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

                buttonAutoStart.Enabled = Shared.IsTwitchMode && twitch?.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance().ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = twitch?.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
                }
                return;
            }

            ProcessHooker.AttachExitedMethod((sender, e) => buttonAutoStart.Invoke(new Action(() =>
            {
                buttonAutoStart.Enabled = Shared.IsTwitchMode && twitch?.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance().ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = twitch?.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
                }

                ProcessHooker.CloseProcess();
            })));

            buttonAutoStart.Enabled = false;
            buttonAutoStart.Text = "Waiting...";

            Shared.TimerEnabled = false;
            autoStartTimer.Start();
            buttonMainToggle.Enabled = false;
            buttonTwitchToggle.Enabled = twitch?.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Shared.IsTwitchMode)
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
            if (!Shared.TimerEnabled) return;

            int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);

            // Hack to fix Windows' broken-ass progress bar handling
            progressBarMain.Value = Math.Min(value, progressBarMain.Maximum);
            progressBarMain.Value = Math.Min(value - 1, progressBarMain.Maximum);

            if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
            {
                long remaining = Math.Max(0, Config.Instance().MainCooldown - stopwatch.ElapsedMilliseconds);

                ProcessHooker.SendEffectToGame("time", $"{remaining},{Config.Instance().MainCooldown}");
                Shared.Multiplayer?.SendTimeUpdate((int)remaining, Config.Instance().MainCooldown);

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
            if (!Shared.TimerEnabled) return;

            if (Shared.TwitchVotingMode == 1)
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
                    Shared.Multiplayer?.SendTimeUpdate((int)remaining, Config.Instance().TwitchVotingTime);

                    twitch?.SendEffectVotingToGame();

                    elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                }

                bool didFinish = false;

                if (Config.Instance().TwitchUsePolls && twitch != null)
                {
                    didFinish = twitch.GetRemaining() == 0;

                    if (stopwatch.ElapsedMilliseconds >= Config.Instance().TwitchVotingTime)
                    {
                        long millisecondsOver = stopwatch.ElapsedMilliseconds - Config.Instance().TwitchVotingTime;
                        int waitLeft = Math.Max(0, 15000 - decimal.ToInt32(millisecondsOver));
                        labelTwitchCurrentMode.Text = $"Current Mode: Waiting For Poll... ({Math.Ceiling((float)waitLeft / 1000)}s left)";

                        if (waitLeft == 0)
                        {
                            labelTwitchCurrentMode.Text = "Current Mode: Cooldown (Poll Failed)";

                            ProcessHooker.SendEffectToGame("time", "0");
                            Shared.Multiplayer?.SendTimeUpdate(0, Config.Instance().TwitchVotingCooldown);
                            elapsedCount = 0;

                            progressBarTwitch.Value = 0;
                            progressBarTwitch.Maximum = Config.Instance().TwitchVotingCooldown;

                            stopwatch.Restart();
                            Shared.TwitchVotingMode = 0;

                            if (twitch != null)
                            {
                                twitch.SetVoting(3, timesUntilRapidFire, null);
                            }

                            return;
                        }
                    }
                }
                else
                {
                    didFinish = stopwatch.ElapsedMilliseconds >= Config.Instance().TwitchVotingTime;
                }

                if (didFinish)
                {
                    ProcessHooker.SendEffectToGame("time", "0");
                    Shared.Multiplayer?.SendTimeUpdate(0, Config.Instance().TwitchVotingCooldown);
                    elapsedCount = 0;

                    progressBarTwitch.Value = 0;
                    progressBarTwitch.Maximum = Config.Instance().TwitchVotingCooldown;

                    stopwatch.Restart();
                    Shared.TwitchVotingMode = 0;

                    labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                    if (twitch != null)
                    {
                        List<IVotingElement> elements = twitch.GetMajorityVotes();

                        twitch.SetVoting(0, timesUntilRapidFire, elements);
                        elements.ForEach(e =>
                        {
                            float multiplier = e.GetEffect().GetMultiplier();
                            e.GetEffect().SetMultiplier(multiplier / elements.Count);
                            CallEffect(e.GetEffect());
                            e.GetEffect().SetMultiplier(multiplier);
                        });
                    }
                }
            }
            else if (Shared.TwitchVotingMode == 2)
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
                    Shared.Multiplayer?.SendTimeUpdate((int)remaining, 10000);

                    elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                }

                if (stopwatch.ElapsedMilliseconds >= 1000 * 10) // Set 10 seconds
                {
                    ProcessHooker.SendEffectToGame("time", "0");
                    Shared.Multiplayer?.SendTimeUpdate(0, Config.Instance().TwitchVotingCooldown);
                    elapsedCount = 0;

                    progressBarTwitch.Value = 0;
                    progressBarTwitch.Maximum = Config.Instance().TwitchVotingCooldown;

                    stopwatch.Restart();
                    Shared.TwitchVotingMode = 0;

                    labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                    twitch?.SetVoting(0, timesUntilRapidFire);
                }
            }
            else if (Shared.TwitchVotingMode == 0)
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
                    Shared.Multiplayer?.SendTimeUpdate((int)remaining, Config.Instance().TwitchVotingCooldown);

                    elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                }

                if (stopwatch.ElapsedMilliseconds >= Config.Instance().TwitchVotingCooldown)
                {
                    elapsedCount = 0;

                    if (--timesUntilRapidFire == 0)
                    {
                        progressBarTwitch.Value = progressBarTwitch.Maximum = 1000 * 10;

                        timesUntilRapidFire = new Random().Next(10, 15);

                        Shared.TwitchVotingMode = 2;
                        labelTwitchCurrentMode.Text = "Current Mode: Rapid-Fire";

                        twitch?.SetVoting(2, timesUntilRapidFire);
                    }
                    else
                    {
                        progressBarTwitch.Value = progressBarTwitch.Maximum = Config.Instance().TwitchVotingTime;

                        Shared.TwitchVotingMode = 1;
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

                string Description = effect.GetDescription();

                if (effect.Word.Equals("IWontTakeAFreePass"))
                {
                    Description += " (Fake)";
                }

                EffectTreeNode addedNode = new EffectTreeNode(effect, Description)
                {
                    Checked = true,
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
            comboBoxMainCooldown.Items.Add(new MainCooldownComboBoxItem("3 minutes", 1000 * 60 * 3));
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

            if (!Shared.TimerEnabled)
            {
                progressBarMain.Value = 0;
                progressBarMain.Maximum = Config.Instance().MainCooldown;
                elapsedCount = 0;
                stopwatch.Reset();
            }
        }

        private void PopulateVotingTimes()
        {
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("30 seconds", 1000 * 30));
            comboBoxVotingTime.Items.Add(new VotingTimeComboBoxItem("1 minute", 1000 * 60));

            comboBoxVotingTime.SelectedIndex = 0;

            Config.Instance().TwitchVotingTime = 1000 * 30;
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
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("30 seconds", 1000 * 30));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("1 minute", 1000 * 60));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("2 minutes", 1000 * 60 * 2));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("3 minutes", 1000 * 60 * 3));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("5 minutes", 1000 * 60 * 5));
            comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("10 minutes", 1000 * 60 * 10));

            if (debug)
            {
                comboBoxVotingCooldown.Items.Add(new VotingCooldownComboBoxItem("5 seconds", 1000 * 5));
            }

            comboBoxVotingCooldown.SelectedIndex = 1;

            Config.Instance().TwitchVotingCooldown = 1000 * 60;
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
            buttonAutoStart.Enabled = Shared.IsTwitchMode && twitch != null && twitch.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
            buttonAutoStart.Text = "Auto-Start";
            stopwatch.Reset();
            SetEnabled(true);

            if (Config.Instance().Experimental_RunEffectOnAutoStart && !Shared.IsTwitchMode)
            {
                CallEffect();
            }
        }

        private void SetEnabled(bool enabled)
        {
            Shared.TimerEnabled = enabled;
            if (Shared.TimerEnabled)
            {
                stopwatch.Start();
            }
            else
            {
                stopwatch.Stop();
            }
            autoStartTimer.Stop();
            buttonMainToggle.Enabled = true;
            (Shared.IsTwitchMode ? buttonTwitchToggle : buttonMainToggle).Text = Shared.TimerEnabled ? "Stop / Pause" : "Start / Resume";
            comboBoxMainCooldown.Enabled =
                buttonSwitchMode.Enabled =
                buttonResetMain.Enabled =
                buttonResetTwitch.Enabled = !Shared.TimerEnabled;

            comboBoxVotingTime.Enabled =
                comboBoxVotingCooldown.Enabled =
                textBoxSeed.Enabled = !Shared.TimerEnabled;
        }

        private void ButtonMainToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Shared.TimerEnabled);
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

            public EffectTreeNode(AbstractEffect effect, string description)
            {
                Effect = effect;

                Name = Text = description;
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
            if (twitch != null && twitch.GetTwitchClient().IsConnected)
            {
                twitch?.Kill();
                twitch = null;

                comboBoxVotingTime.Enabled = true;
                comboBoxVotingCooldown.Enabled = true;

                textBoxTwitchChannel.Enabled = true;
                textBoxTwitchUsername.Enabled = true;
                textBoxTwitchOAuth.Enabled = true;

                buttonTwitchToggle.Enabled = false;

                checkBoxTwitchUsePolls.Enabled = true;

                buttonConnectTwitch.Text = "Connect to Twitch";

                if (!tabs.TabPages.Contains(tabEffects))
                {
                    tabs.TabPages.Insert(tabs.TabPages.IndexOf(tabTwitch), tabEffects);
                }

                return;
            }

            if (Config.Instance().TwitchChannel != "" && Config.Instance().TwitchUsername != "" && Config.Instance().TwitchOAuthToken != "")
            {
                buttonConnectTwitch.Enabled = false;

                if (Config.Instance().TwitchUsePolls)
                {
                    twitch = new TwitchConnection_Poll();
                }
                else
                {
                    twitch = new TwitchConnection_Chat();
                }

                twitch.OnRapidFireEffect += (_sender, rapidFireArgs) =>
                {
                    Invoke(new Action(() =>
                    {
                        if (Shared.TwitchVotingMode == 2)
                        {
                            if (Shared.Multiplayer != null)
                            {
                                Shared.Multiplayer.SendEffect(rapidFireArgs.Effect);
                            }
                            else
                            {
                                rapidFireArgs.Effect.RunEffect();
                                AddEffectToListBox(rapidFireArgs.Effect);
                            }
                        }
                    }));
                };

                twitch.GetTwitchClient().OnIncorrectLogin += (_sender, _e) =>
                {
                    MessageBox.Show("There was an error trying to log in to the account. Wrong username / OAuth token?", "Twitch Login Error");
                    Invoke(new Action(() =>
                    {
                        buttonConnectTwitch.Enabled = true;
                    }));
                    twitch.Kill();
                };

                twitch.GetTwitchClient().OnConnected += (_sender, _e) =>
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

                        checkBoxTwitchUsePolls.Enabled = false;
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
            if (Shared.IsTwitchMode)
            {
                Shared.IsTwitchMode = false;

                buttonSwitchMode.Text = "Twitch";

                tabs.TabPages.Insert(0, tabMain);
                tabs.SelectedIndex = 0;
                tabs.TabPages.Remove(tabTwitch);

                tabs.TabPages.Remove(tabPolls);

                listLastEffectsMain.Items.Clear();
                progressBarMain.Value = 0;

                elapsedCount = 0;

                stopwatch.Reset();
                SetEnabled(false);
            }
            else
            {
                Shared.IsTwitchMode = true;

                buttonSwitchMode.Text = "Main";
                buttonAutoStart.Enabled = twitch != null && twitch.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;

                tabs.TabPages.Insert(0, tabTwitch);
                tabs.SelectedIndex = 0;
                tabs.TabPages.Remove(tabMain);

                if (Config.Instance().TwitchUsePolls)
                {
                    tabs.TabPages.Insert(2, tabPolls);
                }

                listLastEffectsTwitch.Items.Clear();
                progressBarTwitch.Value = 0;

                elapsedCount = 0;

                stopwatch.Reset();
                SetEnabled(false);
            }
        }

        private void ButtonTwitchToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Shared.TimerEnabled);
        }

        private void TextBoxSeed_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().Seed = textBoxSeed.Text;
            RandomHandler.SetSeed(Config.Instance().Seed);
        }

        private void ButtonResetMain_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            RandomHandler.SetSeed(Config.Instance().Seed);
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

        private void ButtonResetTwitch_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            RandomHandler.SetSeed(Config.Instance().Seed);
            stopwatch.Reset();
            elapsedCount = 0;
            labelTwitchCurrentMode.Text = "Current Mode: Cooldown";
            Shared.TwitchVotingMode = 0;
            timesUntilRapidFire = new Random().Next(10, 15);
            progressBarTwitch.Value = 0;
            buttonTwitchToggle.Enabled = twitch?.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
            buttonTwitchToggle.Text = "Start / Resume";
            buttonAutoStart.Enabled = twitch?.GetTwitchClient() != null && twitch.GetTwitchClient().IsConnected;
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

        private void ViceCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shared.SelectedGame = "vice_city";
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects("vice_city");
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

        private void SanAndreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shared.SelectedGame = "san_andreas";
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects("san_andreas");
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

        private void CheckBoxTwitchCombineVotingMessages_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchCombineChatMessages = checkBoxTwitchCombineVotingMessages.Checked;
        }

        private void UpdatePollTabVisibility()
        {
            if (checkBoxTwitchUsePolls.Checked)
            {
                if (!tabs.TabPages.Contains(tabPolls) && Shared.IsTwitchMode)
                {
                    tabs.TabPages.Insert(2, tabPolls);
                }
            }
            else
            {
                tabs.TabPages.Remove(tabPolls);
            }
        }

        private void CheckBoxTwitchUsePolls_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchUsePolls = checkBoxTwitchUsePolls.Checked;
            UpdatePollTabVisibility();
        }

        private void CheckBoxTwitchEnableMultipleEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchEnableMultipleEffects = checkBoxTwitchEnableMultipleEffects.Checked;
        }

        private void CheckBoxTwitchPollsSubscriberOnly_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchPollsSubscriberOnly = checkBoxTwitchPollsSubscriberOnly.Checked;
        }

        private void CheckBoxTwitchPollsSubcriberBonus_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchPollsSubcriberMultiplier = checkBoxTwitchPollsSubcriberMultiplier.Checked;
        }

        private void NumericUpDownTwitchPollsBitsCost_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchPollsBitsCost = decimal.ToInt32(numericUpDownTwitchPollsBitsCost.Value);
        }

        private void TextBoxTwitchPollsPassphrase_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchPollsPassphrase = textBoxTwitchPollsPassphrase.Text;
        }

        private void CheckBoxTwitchPollsPostMessages_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchPollsPostMessages = checkBoxTwitchPollsPostMessages.Checked;
        }

        private void CheckBoxTwitchAppendFakePassCurrentMission_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchAppendFakePassCurrentMission = checkBoxTwitchAppendFakePassCurrentMission.Checked;
        }

        private void EnabledEffectsView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is EffectTreeNode && debug)
            {
                EffectTreeNode node = (EffectTreeNode)e.Node;
                node.Effect.RunEffect();
            }
        }

        private void CheckBoxPlayAudioForEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().PlayAudioForEffects = checkBoxPlayAudioForEffects.Checked;
        }

        private string FilterMultiplayerCharacters(string text)
        {
            return Regex.Replace(text, "[^A-Za-z0-9]", "");
        }

        private void UpdateButtonState()
        {
            buttonMultiplayerConnect.Enabled
                = !string.IsNullOrEmpty(textBoxMultiplayerServer.Text)
                && !string.IsNullOrEmpty(textBoxMultiplayerChannel.Text)
                && !string.IsNullOrEmpty(textBoxMultiplayerUsername.Text);
        }

        private void TextBoxMultiplayerServer_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        private void TextBoxMultiplayerChannel_TextChanged(object sender, EventArgs e)
        {
            textBoxMultiplayerChannel.Text = FilterMultiplayerCharacters(textBoxMultiplayerChannel.Text);
            UpdateButtonState();
        }

        private void TextBoxMultiplayerUsername_TextChanged(object sender, EventArgs e)
        {
            textBoxMultiplayerUsername.Text = FilterMultiplayerCharacters(textBoxMultiplayerUsername.Text);
            UpdateButtonState();
        }

        private void UpdateMultiplayerConnectionState(int state)
        {
            Invoke(new Action(() =>
            {
                if (state == 0) // Not connected
                {
                    textBoxMultiplayerServer.Enabled
                        = textBoxMultiplayerChannel.Enabled
                        = textBoxMultiplayerUsername.Enabled
                        = buttonMultiplayerConnect.Enabled
                        = true;

                    buttonMultiplayerConnect.Text = "Connect";

                    buttonSwitchMode.Enabled = true;
                    buttonMainToggle.Enabled = true;
                    buttonResetMain.Enabled = true;
                    buttonAutoStart.Enabled = true;
                    comboBoxMainCooldown.Enabled = true;
                }
                else if (state == 1) // Connecting...
                {
                    textBoxMultiplayerServer.Enabled
                        = textBoxMultiplayerChannel.Enabled
                        = textBoxMultiplayerUsername.Enabled
                        = buttonMultiplayerConnect.Enabled
                        = false;

                    buttonMultiplayerConnect.Text = "Connecting...";
                }
                else if (state == 2) // Connected
                {
                    textBoxMultiplayerServer.Enabled
                        = textBoxMultiplayerChannel.Enabled
                        = textBoxMultiplayerUsername.Enabled
                        = false;

                    buttonMultiplayerConnect.Enabled = true;
                    buttonMultiplayerConnect.Text = "Disconnect";
                }
            }));
        }

        private void ShowMessageBox(string text, string caption)
        {
            Invoke(new Action(() =>
            {
                MessageBoxEx.Show(this, text, caption);
            }));
        }

        private void AddToMultiplayerChatHistory(string message)
        {
            Invoke(new Action(() =>
            {
                listBoxMultiplayerChat.Items.Add(message);
                listBoxMultiplayerChat.TopIndex = listBoxMultiplayerChat.Items.Count - 1;
            }));
        }

        private void ClearMultiplayerChatHistory()
        {
            Invoke(new Action(() =>
            {
                listBoxMultiplayerChat.Items.Clear();
            }));
        }

        private void ButtonMultiplayerConnect_Click(object sender, EventArgs e)
        {
            Shared.Multiplayer?.Disconnect();

            if (buttonMultiplayerConnect.Text == "Disconnect")
            {
                UpdateMultiplayerConnectionState(0);
                return;
            }

            ClearMultiplayerChatHistory();

            Shared.Multiplayer = new Multiplayer(
                textBoxMultiplayerServer.Text,
                textBoxMultiplayerChannel.Text,
                textBoxMultiplayerUsername.Text
            );

            UpdateMultiplayerConnectionState(1);

            Shared.Multiplayer.OnConnectionFailed += (_sender, args) =>
            {
                ShowMessageBox("Connection failed - is the server running?", "Error");
                UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnUsernameInUse += (_sender, args) =>
            {
                ShowMessageBox("Username already in use!", "Error");
                UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnConnectionSuccessful += (_sender, args) =>
            {
                ShowMessageBox("Successfully connected!", "Connected");
                AddToMultiplayerChatHistory($"Successfully connected to channel: {textBoxMultiplayerChannel.Text}");
                UpdateMultiplayerConnectionState(2);

                Invoke(new Action(() =>
                {
                    if (!args.IsHost)
                    {
                        buttonSwitchMode.Enabled = false;
                        buttonMainToggle.Enabled = false;
                        buttonResetMain.Enabled = false;
                        buttonAutoStart.Enabled = false;
                        comboBoxMainCooldown.Enabled = false;
                    }

                    labelMultiplayerHost.Text = $"Host: {args.HostUsername}";
                    if (args.IsHost)
                    {
                        labelMultiplayerHost.Text += " (You!)";
                    }
                }));
            };

            Shared.Multiplayer.OnHostLeftChannel += (_sender, args) =>
            {
                ShowMessageBox("Host has left the channel; Disconnected.", "Host Left");
                AddToMultiplayerChatHistory("Host has left the channel; Disconnected.");
                UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnVersionMismatch += (_sender, args) =>
            {
                ShowMessageBox($"Channel is v{args.Version} but you have v{Shared.Version}; Disconnected.", "Version Mismatch");
                AddToMultiplayerChatHistory($"Channel is v{args.Version} but you have v{Shared.Version}; Disconnected.");
                UpdateMultiplayerConnectionState(0);
            };

            Shared.Multiplayer.OnUserJoined += (_sender, args) =>
            {
                AddToMultiplayerChatHistory($"{args.Username} joined!");
            };

            Shared.Multiplayer.OnUserLeft += (_sender, args) =>
            {
                AddToMultiplayerChatHistory($"{args.Username} left!");
            };

            Shared.Multiplayer.OnChatMessage += (_sender, args) =>
            {
                AddToMultiplayerChatHistory($"{args.Username}: {args.Message}");
            };

            Shared.Multiplayer.OnTimeUpdate += (_sender, args) =>
            {
                if (!Shared.Multiplayer.IsHost)
                {
                    ProcessHooker.SendEffectToGame("time", $"{args.Remaining},{args.Total}");
                }
            };

            Shared.Multiplayer.OnEffect += (_sender, args) =>
            {
                var effect = EffectDatabase.GetByWord(args.Word);
                if (effect != null)
                {
                    if (string.IsNullOrEmpty(args.Voter) || args.Voter == "N/A")
                    {
                        effect.ResetVoter();
                    }
                    else
                    {
                        effect.SetVoter(args.Voter);
                    }
                    EffectDatabase.RunEffect(effect, args.Seed, args.Duration);
                }
            };

            Shared.Multiplayer.OnVotes += (_sender, args) =>
            {
                string[] effects = args.Effects;
                int[] votes = args.Votes;

                string voteString = $"votes:{effects[0]};{votes[0]};;{effects[1]};{votes[1]};;{effects[2]};{votes[2]};;{args.LastChoice}";
                ProcessHooker.SendPipeMessage(voteString);
            };

            Shared.Multiplayer.Connect();
        }

        private void TextBoxMultiplayerChat_TextChanged(object sender, EventArgs e)
        {
            buttonMultiplayerSend.Enabled = Shared.Multiplayer != null && !string.IsNullOrEmpty(textBoxMultiplayerChat.Text);
        }

        private void ButtonMultiplayerSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxMultiplayerChat.Text))
            {
                Shared.Multiplayer?.SendChatMessage(textBoxMultiplayerChat.Text);
                textBoxMultiplayerChat.Text = "";
            }
        }

        private void TextBoxMultiplayerChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(textBoxMultiplayerChat.Text))
            {
                Shared.Multiplayer?.SendChatMessage(textBoxMultiplayerChat.Text);
                textBoxMultiplayerChat.Text = "";
            }
        }

        private void CheckBoxExperimental_EnableAllEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().Experimental_EnableAllEffects = checkBoxExperimental_EnableAllEffects.Checked;
        }

        private void CheckBoxExperimental_EnableEffectOnAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().Experimental_RunEffectOnAutoStart = checkBoxExperimental_EnableEffectOnAutoStart.Checked;
        }

        private void ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!tabs.TabPages.Contains(tabExperimental))
            {
                tabs.TabPages.Add(tabExperimental);
            }
            experimentalToolStripMenuItem.Visible = false;
        }
    }
}
