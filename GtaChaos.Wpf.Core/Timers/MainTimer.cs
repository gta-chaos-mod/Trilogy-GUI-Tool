using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using GtaChaos.Models.Utils;

namespace GtaChaos.Wpf.Core.Timers
{
    /// <summary>
    /// Timer that is used to trigger effects.
    /// </summary>
    public class MainTimer
    {
        private Timer _effectTimer;
        private Timer _progressTimer;
        private readonly Action _timerEffectCallback;
        private readonly Action<double> _progressCallback;

        // The progress timer might not actually run every 10 ms because
        // of a delay with the UI or general processing stuff.
        // This stopwatch however will be in sync.
        private readonly Stopwatch _progressStopwatch;

        private double _elapsedMillis;

        // 10 Millisecond timer interval.
        private const long PROGRESS_INTERVAL = 10;

        /// <summary>
        /// Creates a new instance of the <see cref="MainTimer"/> class.
        /// Prepares all stopwatches and timers to be activated.
        /// </summary>
        /// <param name="effectCallback">
        /// The callback to be ran when an effect should be triggered.
        /// </param>
        /// <param name="progressCallback">
        /// The callback to be ran when updating the progress bar.
        /// </param>
        public MainTimer(Action effectCallback, Action<double> progressCallback)
        {
            _effectTimer = new Timer();
            _effectTimer.Elapsed += EffectTimerOnElapsed;

            _progressTimer = new Timer(PROGRESS_INTERVAL);
            _progressTimer.Elapsed += ProgressTimerOnElapsed;

            _timerEffectCallback = effectCallback;
            _progressCallback = progressCallback;
            _progressStopwatch = new Stopwatch();
        }

        /// <summary>
        /// Starts the timers and triggers an effect every
        /// amount of seconds specified in <paramref name="seconds"/>.
        /// </summary>
        /// <param name="seconds">
        /// The amount of seconds between effects.
        /// </param>
        public void Start(double seconds)
        {
            // Set the interval to be in milliseconds and auto reset.
            _effectTimer.Interval = seconds * 1000;

            // Start all the timers needed.
            _effectTimer.Start();
            _progressTimer.Start();
            _progressStopwatch.Start();
        }

        /// <summary>
        /// Stops and resets all timers.
        /// </summary>
        public void Reset()
        {
            _effectTimer.Stop();
            _progressTimer.Stop();
            _effectTimer = new Timer();
            _progressTimer = new Timer(PROGRESS_INTERVAL);
            _progressStopwatch.Reset();
        }

        private void ProgressTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _elapsedMillis += _progressStopwatch.ElapsedMilliseconds;

            if (_elapsedMillis > 100)
            {
                var remaining = Math.Max(0, Config.Instance().MainCooldown - _progressStopwatch.ElapsedMilliseconds);

                ProcessHooker.SendEffectToGame("time", $"{remaining},{Config.Instance().MainCooldown}");
                _elapsedMillis = 0;
            }

            _progressCallback.Invoke(_progressStopwatch.ElapsedMilliseconds);
        }

        private void EffectTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _progressStopwatch.Restart();
            _timerEffectCallback.Invoke();
        }

        //private void TickMain()
        //{
        //    if (!Config.Instance.Enabled) return;

        //    int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);

        //    // Hack to fix Windows' broken-ass progress bar handling
        //    progressBarMain.Value = Math.Min(value, progressBarMain.Maximum);
        //    progressBarMain.Value = Math.Min(value - 1, progressBarMain.Maximum);

        //    if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
        //    {
        //        long remaining = Math.Max(0, Config.Instance.MainCooldown - stopwatch.ElapsedMilliseconds);
        //        int iRemaining = (int)((float)remaining / Config.Instance.MainCooldown * 1000f);

        //        ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

        //        elapsedCount = (int)stopwatch.ElapsedMilliseconds;
        //    }

        //    if (stopwatch.ElapsedMilliseconds >= Config.Instance.MainCooldown)
        //    {
        //        progressBarMain.Value = 0;
        //        CallEffect();
        //        elapsedCount = 0;
        //        stopwatch.Restart();
        //    }
        //}
    }
}
