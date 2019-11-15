using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Utils;

namespace GtaChaos.Wpf.Core
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nl");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl");

            Config.Instance().Load();
            EffectDatabase.PopulateEffects(Games.SanAndreas);
        }
    }
}
