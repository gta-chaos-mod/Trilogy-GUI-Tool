using System;
using System.Collections.Generic;
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
using GtaChaos.Models.Games;
using GtaChaos.Models.Utils;
using GtaChaos.Wpf.Core.Events;

namespace GtaChaos.Wpf.Core.Views.Components
{
    /// <summary>
    /// Interaction logic for GameSelector.xaml
    /// </summary>
    public partial class GameSelector : UserControl
    {
        public event EventHandler<GameChangeEventArgs> GameChanged;

        public GameSelector()
        {
            InitializeComponent();
            var games = GameCollection.Get.GetAll();
            var selectedGame = Config.Instance().SelectedGame;
            foreach (var game in games)
            {
                GameComboBox.Items.Add(new ComboBoxItem()
                {
                    DataContext = game,
                    Content = game.Name,
                    IsSelected = game.Id == selectedGame
                });
            }
        }

        private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            var game = (Game)((ComboBoxItem)e.AddedItems[0]).DataContext;

            EffectDatabase.PopulateEffects(game.Id);
            Config.Instance().SelectedGame = game.Id;

            GameChanged?.Invoke(this, new GameChangeEventArgs(game.Id));
        }
    }
}
