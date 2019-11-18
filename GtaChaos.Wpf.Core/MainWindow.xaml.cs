using System;
using System.Collections.Generic;
using GtaChaos.Models.Utils;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GtaChaos.Models.Effects;
using GtaChaos.Wpf.Core.Helpers;
using GtaChaos.Wpf.Core.Timers;
using GtaChaos.Wpf.Core.ViewModels;
using GtaChaos.Wpf.Core.Views;
using GtaChaos.Wpf.Core.Views.Effects;

namespace GtaChaos.Wpf.Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainTimer _timer;
        private readonly Stopwatch _stopWatch;
        private readonly List<Control> _controlList;
        private const int MaxProgress = 100;

        public MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            _timer = new MainTimer(ActivateEffect, ChangeProgressBar);
            _stopWatch = new Stopwatch();

            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            InitializeComponent();
            _controlList = new List<Control>()
            {
                CooldownComboBox
            };

            LoadPresets();
        }

        private void ActivateEffect()
        {
            var milliseconds = _stopWatch.ElapsedMilliseconds;
            _stopWatch.Restart();
            var effect = EffectSelector.GetRandomEffect();
            effect.RunEffect();
            TryExecuteWithDispatcher(() =>
            {
                EffectListView.Items.Insert(0, $"{effect.GetDescription()} ({effect.Word})");
            });
        }

        private void ChangeProgressBar(double millisecondsPassed)
        {
            TryExecuteWithDispatcher(() =>
            {
                var percentage = MaxProgress - (millisecondsPassed / (ViewModel.CooldownViewModel.SelectedCooldown * 1000)
                                                 * MaxProgress);
                    ProgressBar.Value = percentage;
                    TimeLabel.Content = $"{MaxProgress - Math.Round(percentage)}%";
            });
        }

        private void SetEnabledState(bool state)
        {
            foreach (var control in _controlList)
            {
                control.IsEnabled = state;
            }
        }

        #region Hooks
        private void AutoStartButtonClick(object sender, RoutedEventArgs e)
        {
            ProcessHooker.HookProcess();
            SetEnabledState(false);
        }

        private void StartResumeClick(object sender, RoutedEventArgs e)
        {
            Config.Instance().MainCooldown = Convert.ToInt32(ViewModel.CooldownViewModel.SelectedCooldown) * 1000;
            ProcessHooker.HookProcess();
            _stopWatch.Start();
            _timer.Start(ViewModel.CooldownViewModel.SelectedCooldown);
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            _timer.Reset();
            ProgressBar.Value = MaxProgress;
            TimeLabel.Content = null;
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Tries to execute the <paramref name="action"/> and
        /// catches <see cref="TaskCanceledException"/>s.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        private void TryExecuteWithDispatcher(Action action)
        {
            Dispatcher?.Invoke(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (TaskCanceledException)
                {
                    // Ignored.
                }
            });
        }
        #endregion

        private void EditEffects_Click(object sender, RoutedEventArgs e)
        {
            var optionsView = new Options();
            optionsView.Closed += (o, args) =>
            {
                LoadPresets();
            };

            optionsView.Show();
        }

        private void SeedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.Instance().Seed = ((TextBox)e.Source).Text;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string identifier = null;
            try
            {
                identifier = (string) ((ComboBoxItem) e.AddedItems[0]).DataContext;
            }
            catch
            {
                identifier = null;
            }

            if (identifier == null)
            {
                return;
            }

            
            var config = Config.Instance();
            var presetObject = config.Presets.FirstOrDefault(preset => preset.Name == identifier && preset.Game == config.SelectedGame);

            if (presetObject == null)
            {
                return;
            }

            config.EnabledEffects = presetObject.EnabledEffects;

                EffectDatabase.EnabledEffects =
                    EffectDatabase.Effects.Where(effect =>
                        Config.Instance().EnabledEffects.Contains(effect.Id)).ToList();
            }

        private void LoadPresets()
        {
            PresetComboBox.Items.Clear();
            var config = Config.Instance();

            foreach (var name in config.Presets
                .Where(preset => preset.Game == config.SelectedGame)
                .Select(preset => preset.Name))
            {
                PresetComboBox.Items.Add(new ComboBoxItem {Content = name, DataContext = name });
            }
            
        }

        private void ActiveEffects_Click(object sender, RoutedEventArgs e)
        {
            var effectWindow = new ViewEffects();
            effectWindow.Show();
        }
    }
}