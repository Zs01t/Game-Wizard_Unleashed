using Logic;
using Models;
using Models.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public DispatcherTimer AnimationTimer;

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
            this.dT.Interval = TimeSpan.FromMilliseconds(500);
            this.dT.Tick += this.DT_Tick;
            this.dT.Start();


            //külön timere van az animációknak
            AnimationTimer = new DispatcherTimer();
            AnimationTimer.Interval = TimeSpan.FromMilliseconds(60);
            AnimationTimer.Tick += AnimationTimer_Tick;
            AnimationTimer.Start();

            logic.PlayerDead += this.PlayerDead;
            logic.NoMoreLevel += NoMoreLevel;
 
            
        }

        private void NoMoreLevel(object? sender, EventArgs e)
        {
            dT.Stop();
            
            YouWonWindow youWonWindow = new YouWonWindow();
            youWonWindow.Show();
            this.Close();


        }

        
        int walkFrameChangeTick = 12;
        private int deathAnimationtick = 7;
        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (!gameDisplay.playerObject.IsDead)
            {
                if (walkFrameChangeTick > 0 && gameDisplay.playerObject.IsWalking)
                {
                    gameDisplay.playerObject.ChangeCurrentWalkImage();
                    walkFrameChangeTick--;
                }
                else
                {
                    gameDisplay.playerObject.ChangeCurrentIdleImage();
                    gameDisplay.playerObject.IsWalking = false;
                }
                foreach (var enemy in gameDisplay.enemyObjects)
                {
                    enemy.ChangeCurrentWalkImage();
                }
                if (gameDisplay.spellObjects.Count > 0)
                {
                    foreach (var spell in gameDisplay.spellObjects)
                    {
                        spell.ChangeCurrentProjectileImage();
                    }

                }
            }
            else
            {
                
                if (deathAnimationtick > 0)
                {
                    gameDisplay.playerObject.ChangeCurrentDeathImage();
                    deathAnimationtick--;
                }
                else
                {
                    dT.Stop();
                    AnimationTimer.Stop();
                    GameOverWindow gameOverWindow = new GameOverWindow();
                    gameOverWindow.Show();
                    this.Close();
                }
                
            }
           
 
            





            //ebben nem vagyok biztos, hogy ez jót tesz -Zs
            gameDisplay.InvalidateVisual();
        }

        private void DT_Tick(object? sender, EventArgs e)
        {
            logic.TimeStep();
            foreach (NormalEnemy enemy in logic.Enemies) //enemy dodgeolásához kell
            {
                enemy.timeSinceLastDodge += dT.Interval.TotalSeconds;
            }
            logic.Player.timeSinceLastSpell += dT.Interval.TotalSeconds; //mérjük mennyi idő telt el az utolsó lövés óta
            gameDisplay.InvalidateVisual();
            //timeSinceLastMove += dT.Interval.TotalSeconds;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (gameDisplay.logic != null)
            {
                gameDisplay.SetupSizes(new Size(gameGrid.ActualWidth, gameGrid.ActualHeight));
                //logic.SetupSizes(new System.Windows.Size(grid.ActualWidth, grid.ActualHeight));
            }
        }

        //// Define a cooldown time for movement
        //private const double moveCooldown = 0.0001; // in seconds

        //// Define a variable to store the time since the last movement
        //private double timeSinceLastMove = 0;


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Left)
            {
                gameDisplay.logic.Control(Direction.Left);
                //teszt jelleggel van itt egyenlőre
                gameDisplay.playerObject.IsWalking = true;
                walkFrameChangeTick = 12;
            }
            else if (e.Key == Key.Right)
            {
                gameDisplay.logic.Control(Direction.Right);
                //teszt jelleggel van itt egyenlőre
                gameDisplay.playerObject.IsWalking = true;
                walkFrameChangeTick = 12;
            }
            else if (e.Key == Key.Up)
            {
                gameDisplay.logic.Control(Direction.Up);
                //teszt jelleggel van itt egyenlőre
                gameDisplay.playerObject.IsWalking = true;
                walkFrameChangeTick = 12;
            }
            else if (e.Key == Key.Down)
            {
                gameDisplay.logic.Control(Direction.Down);
                //teszt jelleggel van itt egyenlőre
                gameDisplay.playerObject.IsWalking = true;
                walkFrameChangeTick = 12;
            }
            else if (e.Key == Key.Space)
            {
                gameDisplay.logic.CastSpell();
            }

            gameDisplay.InvalidateVisual();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainw = new MainWindow();
            mainw.Show();
            this.Close();
        }

        
        private void PlayerDead(object sender, EventArgs e)
        {
            //Az AnimationTImer_Tick-ben ennek hatására mág lemegy a halál animáció, majd ugrik a game over ablakra

            gameDisplay.playerObject.IsDead = true;
            //ez csak a látvány miatt van itt
            AnimationTimer.Interval = TimeSpan.FromMilliseconds(200);

        }
    }
}
