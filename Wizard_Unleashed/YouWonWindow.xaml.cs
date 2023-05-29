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
    /// Interaction logic for YouWonWindow.xaml
    /// </summary>
    public partial class YouWonWindow : Window
    {
        public YouWonWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainMenu = new MainWindow();
            mainMenu.Show();
            this.Close();
        }
    }
}
