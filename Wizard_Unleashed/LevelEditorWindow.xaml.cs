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
            levelEditorDisplay.SetupSizes(new Size(editorGrid.ActualWidth, editorGrid.ActualHeight));
            levelEditorDisplay.InvalidateVisual();
        }
    }
}
