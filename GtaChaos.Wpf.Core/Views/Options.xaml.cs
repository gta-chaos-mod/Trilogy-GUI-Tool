using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.IO;
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
using GtaChaos.Models.Presets;
using GtaChaos.Models.Utils;
using GtaChaos.Wpf.Core.Events;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace GtaChaos.Wpf.Core.Views
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();

            LoadPresets();
            SeedTextBox.Text = Config.Instance().Seed;

            GamesSelector.GameChanged += GameSelectorOnGameChanged;
        }

        private void GameSelectorOnGameChanged(object sender, GameChangeEventArgs e)
        {
            LoadPresets();
            EffectList.LoadList(new List<string>());
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.Instance().Seed = ((TextBox) e.Source).Text;
        }

        private void LoadPresets()
        {
            PresetComboBox.Items.Clear();

            var presets = Config.Instance().Presets?
                .Where(preset => preset.Game == Config.Instance().SelectedGame);

            if (presets == null)
            {
                return;
            }

            foreach (var presetsKey in presets)
            {
                PresetComboBox.Items.Add(new ComboBoxItem
                {
                    Content = presetsKey.Name
                });
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(new ExportSettings
            {
                enabledEffects = EffectList.GetEnabledEffects
            });
            var saveDialog = new SaveFileDialog
            {
                FileName = "ChaosSettings.json",
                Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt",
                Title = "Save settings file",
                DefaultExt = ".json"
            };

            var dialogResult = saveDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                File.WriteAllText(saveDialog.FileName, json);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                FileName = "ChaosSettings.json",
                Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt",
                Title = "Save settings file",
                DefaultExt = ".json"
            };

            var dialogResult = openDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                var json = File.ReadAllText(openDialog.FileName);
                var settings = JsonConvert.DeserializeObject<ExportSettings>(json);
                EffectList.LoadList(settings.enabledEffects);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var presetName = PresetName.Text;

            var effects = EffectList.GetEnabledEffects;
            var config = Config.Instance();

            var preset = new Preset
            {
                Name = presetName,
                EnabledEffects = effects,
                Game = config.SelectedGame
            };

            config.Presets.Add(preset);

            config.Save();

            LoadPresets();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var preset = Config.Instance().Presets
                .FirstOrDefault(presetObject =>
                    presetObject.Name == (string)((ComboBoxItem)PresetComboBox.SelectedValue).Content);

            if (preset == null)
            {
                return;
            }

            EffectList.LoadList(preset.EnabledEffects);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            EffectList.ToggleAll();
        }
    }

    public class ExportSettings
    {
        public List<string> enabledEffects;
    }
}
