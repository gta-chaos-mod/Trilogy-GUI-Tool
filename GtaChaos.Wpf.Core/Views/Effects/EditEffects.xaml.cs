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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;
using Brush = System.Drawing.Brush;

namespace GtaChaos.Wpf.Core.Views.Effects
{
    /// <summary>
    /// Interaction logic for EditEffects.xaml
    /// </summary>
    public partial class EditEffects : UserControl
    {
        public Dictionary<string, bool> EffectDictionary { get; }

        public List<string> GetEnabledEffects => 
            EffectDictionary
                .Where(keyValuePair => keyValuePair.Value)
                .Select(keyValuePair => keyValuePair.Key)
                .ToList();

        private Dictionary<string, TreeViewItem> _categoryDictionary;

        public EditEffects()
        {
            InitializeComponent();

            EffectDictionary = new Dictionary<string, bool>();

            var effectList = EffectDatabase.Effects.Select(effect => effect.Id).ToList();
            LoadList(effectList);
        }

        public void ToggleAll()
        {
            var initial = !EffectDictionary.First().Value;

            var dictionary = EffectDictionary.
                Where(keyValuePair => keyValuePair.Value)
                .Select(keyValuePair => keyValuePair.Key)
                .ToList();
            LoadList(dictionary);
        }

        public void LoadList(ICollection<string> effectDictionary)
        {
            // Clear both the already existing view list and the default enabled effects.
            EffectsView.Items.Clear();
            EffectDictionary.Clear();
            _categoryDictionary = new Dictionary<string, TreeViewItem>();

            if (effectDictionary == null)
            {
                throw new ArgumentNullException($"{nameof(effectDictionary)} can not be null");
            }

            // Group all effects by category.
            var effectCategory = EffectDatabase.Effects.GroupBy(effect => effect.Category);

            // Loop over each category.
            foreach (var categoryGroup in effectCategory)
            {
                // Make a root item for this category.
                var rootItem = new TreeViewItem
                {
                    Padding = new Thickness(0, 5, 0, 5)
                };

                rootItem.MouseLeftButtonUp += CategoryOnMouseClick;

                var enabledCounter = 0;
                // Loop over all effects within the category.
                foreach (var effect in categoryGroup)
                {
                    // Check what the prefix will be for the effect header.
                    // If it is enabled it should show the checkmark.
                    var isEnabled = effectDictionary.Contains(effect.Id);
                    var prefix =  isEnabled ? "✔ " : "❌ ";

                    if (isEnabled)
                    {
                        enabledCounter++;
                    }

                    // Create the new item with the right header and data context.
                    var item = new TreeViewItem
                    {
                        Header = prefix + effect.GetDescription(),
                        DataContext = effect,
                        Padding = new Thickness(0,2,0,2)
                    };

                    // Add a on click event for selecting or deselecting the event.
                    item.MouseLeftButtonUp += EffectOnMouseClick;

                    // Add the item for the effect to the category item.
                    rootItem.Items.Add(item);

                    // Save the effect to the dictionary that keeps the enabled effects.
                    if (!EffectDictionary.ContainsKey(effect.Id))
                    {
                        EffectDictionary.Add(effect.Id, isEnabled);
                    }

                    if (_categoryDictionary.ContainsKey(effect.Id))
                    {
                        continue;
                    }

                    _categoryDictionary.Add(effect.Id, rootItem);
                }

                // Create the category view model and save it as its data context.
                var categoryViewModel = new CategoryItemViewModel()
                {
                    CategoryName = categoryGroup.Key.Name,
                    EnabledEffects = enabledCounter,
                    TotalEffects = categoryGroup.Count()
                };

                rootItem.Header = categoryViewModel.GetHeader;
                rootItem.DataContext = categoryViewModel;

                // Add the list of categories to the main root.
                EffectsView.Items.Add(rootItem);
            }
        }

        private void CategoryOnMouseClick(object sender, MouseButtonEventArgs e)
        {
            var categoryItem = (CategoryItemViewModel) ((TreeViewItem) sender).DataContext;
            var effects = EffectDatabase.Effects.Where(effect => effect.Category.Name == categoryItem.CategoryName).Select(effect => effect.Id).ToList();

            var enabled = EffectDictionary[effects.First()];

            foreach (var effect in effects)
            {
                // Reverse all effects based on the first.
                EffectDictionary[effect] = !enabled;
            }

            // When updating the items, they will update the category themselves
            foreach (TreeViewItem item in ((TreeViewItem) sender).Items)
            {
                var effect = (AbstractEffect)item.DataContext;
                UpdateItemForEffect(item, effect.Id);
            }

            e.Handled = true;
        }

        private void EffectOnMouseClick(object sender, MouseButtonEventArgs e)
        {
            // Cast the source to the view item and the effect from the data context.
            var item = ((TreeViewItem)e.Source);
            var effect = (AbstractEffect)item.DataContext;

            EffectDictionary[effect.Id] = !EffectDictionary[effect.Id];
            UpdateItemForEffect(item, effect.Id);

            e.Handled = true;
        }

        private void UpdateItemForEffect(TreeViewItem effectItem, string effect)
        {
            // Check if the effect is currently enabled.
            if (EffectDictionary[effect])
            {
                // Enable the effect.
                effectItem.FontWeight = FontWeights.Normal;
                effectItem.Header = effectItem.Header.ToString().Replace("❌", "✔");
                UpdateCategory(_categoryDictionary[effect], true);
            }
            else
            {
                // Disable the effect.
                effectItem.FontWeight = FontWeights.ExtraLight;
                effectItem.Header = effectItem.Header.ToString().Replace("✔", "❌");
                UpdateCategory(_categoryDictionary[effect], false);
            }
        }

        private void UpdateCategory(TreeViewItem categoryItem, bool stateChange)
        {
            var viewModel = (CategoryItemViewModel) categoryItem.DataContext;
            viewModel.EnabledEffects += stateChange ? 1 : -1;
            categoryItem.DataContext = viewModel;
            categoryItem.Header = viewModel.GetHeader;
        }
    }

    public class CategoryItemViewModel
    {
        public string CategoryName { get; set; }
        
        public int EnabledEffects { get; set; }

        public int TotalEffects { get; set; }

        public string GetHeader => $"{CategoryName} ({EnabledEffects} out of {TotalEffects} enabled)";
    }
}
