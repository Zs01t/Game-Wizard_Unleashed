using Logic;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wizard_Unleashed
{
    public class GameDisplay : FrameworkElement
    {
        private Size size;
        public IGameLogic logic;
        public PlayerObject playerObject;

        public void SetupSizes(Size size)
        {
            //if (this.size.Width > 0 && this.size.Height > 0)
            //{
            //    logic.character.PosX = (double)size.Width / (double)this.size.Width * logic.character.PosX;
            //    logic.character.PosY = (double)size.Height / (double)this.size.Height * logic.character.PosY;

            //}

            this.size = size;
            this.InvalidateVisual();
        }

        public void SetupModel(IGameLogic logic)
        {
            this.logic = logic;
            playerObject = new PlayerObject(logic.Player, "Assets\\Wizard");
            logic.GameStateChanged += this.GameStateChanged;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (logic != null)
            {
                double rectWidth = size.Width / logic.Map.GetLength(1);
                double rectHeight = size.Height / logic.Map.GetLength(0);


                for (int i = 0; i < logic.Map.GetLength(0); i++)
                {
                    for (int j = 0; j < logic.Map.GetLength(1); j++)
                    {
                        drawingContext.DrawRectangle(
                                    Brushes.LightCoral,
                                    new Pen(Brushes.Black, 0),
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth + 1, rectHeight + 1)
                                    );
                    }
                }




                        for (int i = 0; i < logic.Map.GetLength(0); i++)
                {
                    for (int j = 0; j < logic.Map.GetLength(1); j++)
                    {
                        SolidColorBrush brush = new SolidColorBrush();

                        switch (logic.Map[i, j])
                        {
                            //case GameItem.Player:

                            //    brush = Brushes.LightBlue;

                            //    break;

                            case GameItem.Wall:
                                brush = Brushes.Brown;
                                break;


                            case GameItem.Floor:
                                brush = Brushes.LightCoral;

                                break;
                            case GameItem.Door:
                                brush = Brushes.LightPink;
                                
                                break;

                            case GameItem.Spell:
                                brush = Brushes.Red;
                                break;
                            case GameItem.Enemy:
                                brush = Brushes.Black;
                                break;
                            default:

                                break;
                        }

                        if (logic.Map[i, j] == GameItem.Player)
                        {
                            ImageBrush playerBrush;
                            if (playerObject.IsWalking)
                            {
                                playerBrush = new ImageBrush(playerObject.CurrentWalkImage);
                            }
                            else
                            {
                                playerBrush = new ImageBrush(playerObject.CurrentIdleImage);
                            }
                            drawingContext.DrawRectangle(
                                   playerBrush,
                                   new Pen(Brushes.Black, 0),
                                   new Rect(j * rectWidth, i * rectHeight, rectWidth + 1, rectHeight + 1)
                                   );
                        }
                        else
                        {
                            drawingContext.DrawRectangle(
                                    brush,
                                    new Pen(Brushes.Black, 0),
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth + 1, rectHeight + 1)
                                    );
                        }
                        

                    }
                }
            }
        }

        private void GameStateChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }
    }
}
