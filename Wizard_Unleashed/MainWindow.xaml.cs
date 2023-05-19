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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wizard_Unleashed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gw = new GameWindow();
            gw.Show();
            this.Hide();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LevelEditorWindow lvlw = new LevelEditorWindow();
            lvlw.Show();
            this.Hide();
            this.Close();

        }
    }
}
