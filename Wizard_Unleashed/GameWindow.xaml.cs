using Logic;
using Models;
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
using System.Windows.Threading;

namespace Wizard_Unleashed
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {

        public IGameLogic logic;
        public Player player;
        private DispatcherTimer dT;

        public GameWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            player = new Player();

            logic = new GameLogic(player);
            //gameDisplay.SetupModel(new GameLogic(player));
            gameDisplay.SetupModel(logic);
            gameDisplay.SetupSizes(new Size(gameGrid.ActualWidth, gameGrid.ActualHeight));

            //időzítő:
            this.dT = new DispatcherTimer();
            // 20 ms -> 50 fps //eredetileg 200
            this.dT.Interval = TimeSpan.FromMilliseconds(1000);
            this.dT.Tick += this.DT_Tick;
            this.dT.Start();
        }

        private void DT_Tick(object? sender, EventArgs e)
        {
            logic.TimeStep();
            gameDisplay.InvalidateVisual();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (gameDisplay.logic != null)
            {
                gameDisplay.SetupSizes(new Size(gameGrid.ActualWidth, gameGrid.ActualHeight));
                //logic.SetupSizes(new System.Windows.Size(grid.ActualWidth, grid.ActualHeight));
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                gameDisplay.logic.Control(Direction.Left);
            }
            else if (e.Key == Key.Right)
            {
                gameDisplay.logic.Control(Direction.Right);
            }
            else if (e.Key == Key.Up)
            {
                gameDisplay.logic.Control(Direction.Up);
            }
            else if (e.Key == Key.Down)
            {
                gameDisplay.logic.Control(Direction.Down);
            }
            else if(e.Key == Key.Space)
            {
                gameDisplay.logic.CastSpell();
            }
            gameDisplay.InvalidateVisual();
        }
    }
}
