using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wizard_Unleashed.ViewModels;

namespace Wizard_Unleashed
{
    /// <summary>
    /// Interaction logic for LevelEditor.xaml
    /// </summary>
    public partial class LevelEditorWindow : Window
    {
        public LevelEditorWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                ILevelEditorLogic logic = new LevelEditorLogic();

                levelEditorDisplay.SetUpLogic(logic);
                levelEditorDisplay.SetupSizes(new Size(editorGrid.ActualWidth, editorGrid.ActualHeight));
                levelEditorDisplay.InvalidateVisual();
                //ezzel elekrültem a Dependency-s problémákat? -Zs
                (this.DataContext as LevelEditorViewModel).SetUpLogic(logic);
        }

        //sender: az az elem, amelyi kezeli az eseményt, nem a kiváltó
        //e.Source az esemény kiváltója (logikai fán)
        //e.OriginalSource: az esemény kiváltója, de úgy hogy pl. Egy buttonban lévő Textblockra kattintottunk
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == levelEditorDisplay && (this.DataContext as LevelEditorViewModel).SelectedTile != null)
            {
                double mousePosX = e.GetPosition(levelEditorDisplay).X;
                double mousePosY = e.GetPosition(levelEditorDisplay).Y;
                levelEditorDisplay.PlaceItemIntoGrid(mousePosX, mousePosY, (this.DataContext as LevelEditorViewModel).SelectedTile);
                levelEditorDisplay.InvalidateVisual();
            }
            
        }

    }
}
