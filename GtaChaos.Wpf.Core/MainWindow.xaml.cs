using System;
using System.Collections.Generic;
using GtaChaos.Models.Utils;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GtaChaos.Wpf.Core.Timers;
using GtaChaos.Wpf.Core.ViewModels;

namespace GtaChaos.Wpf.Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainTimer _timer;
        private Stopwatch _stopWatch;
        private List<Control> _controlList;

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
        }

        private void ActivateEffect()
        {
            var milliseconds = _stopWatch.ElapsedMilliseconds;
            _stopWatch.Restart();
            TryExecuteWithDispatcher(() =>
            {
                EffectListView.Items.Insert(0, $"EFFECT GOES HERE, in {milliseconds.ToString()} Millis");
            });
        }

        private void ChangeProgressBar(double millisecondsPassed)
        {
            TryExecuteWithDispatcher(() =>
            {
                var percentage = 100 - (millisecondsPassed / (ViewModel.CooldownViewModel.SelectedCooldown * 1000) * 100);
                    ProgressBar.Value = percentage;
                    TimeLabel.Content = $"{100-Math.Round(percentage)}%";
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
            _stopWatch.Start();
            _timer.Start(ViewModel.CooldownViewModel.SelectedCooldown);
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            _timer.Reset();
            ProgressBar.Value = 100;
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

    }
}