using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Utils;

namespace GtaChaos.Wpf.Core.Views.Effects
{
    /// <summary>
    /// Interaction logic for ViewEffects.xaml
    /// </summary>
    public partial class ViewEffects : Window
    {
        public ViewEffects()
        {
            InitializeComponent();

            var activeEffects = Config.Instance()?.EnabledEffects;

            if (activeEffects == null || activeEffects.Count == 0)
            {
                return;
            }

            var orderedEffects = activeEffects.OrderBy(value => value);
            var uniqueString = new StringBuilder();

            foreach (var effect in orderedEffects)
            {
                uniqueString.Append(effect);
                var effectImplementation = EffectDatabase.Effects.FirstOrDefault(abstractEffect =>
                    abstractEffect.Id == effect);

                if (effectImplementation == null)
                {
                    return;
                }

                EffectList.Items.Add(new ListBoxItem
                {
                    Content = $"{effectImplementation.Word} ({effectImplementation.Category})"
                });
            }

            UniqueStringTextBox.Text = uniqueString.ToString();
        }
    }
}
