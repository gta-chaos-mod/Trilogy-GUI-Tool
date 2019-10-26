using System;
using System.Collections.Generic;
using System.Text;

namespace GtaChaos.Wpf.Core.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            CooldownViewModel = new CooldownViewModel();
        }

        public CooldownViewModel CooldownViewModel { get; set; }
    }
}
