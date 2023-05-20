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
using System.Windows.Shapes;

namespace Wizard_Unleashed
{
    public class GameDisplay : FrameworkElement
    {
        private Size size;
        public IGameLogic logic;
        public EntityObject playerObject;
        public List<EntityObject> enemyObjects;
        public List<EntityObject> spellObjects;

        public void SetupSizes(Size size)
        {

            this.size = size;
            this.InvalidateVisual();
        }

        public void SetupModel(IGameLogic logic)
        {
            this.logic = logic;
            playerObject = new EntityObject(logic.Player, "Assets\\Wizard");


            enemyObjects = new List<EntityObject>();
            foreach (var enemy in logic.Enemies)
            {
                enemyObjects.Add(new EntityObject(enemy, "Assets\\Slime"));
            }

            //spellObjects = new List<EntityObject>();
            //foreach (var spell in logic.Spells)
            //{
            //    enemyObjects.Add(new EntityObject(spell, "Assets\\Spell"));
            //}



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
                                    new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\floor.png", UriKind.RelativeOrAbsolute))),
                                    new Pen(Brushes.Black, 0),
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth + 1, rectHeight + 1)
                                    );
                    }
                }




                for (int i = 0; i < logic.Map.GetLength(0); i++)
                {
                    for (int j = 0; j < logic.Map.GetLength(1); j++)
                    {
                        ImageBrush brush = new ImageBrush();

                        switch (logic.Map[i, j])
                        {

                            case GameItem.Wall:
                                brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\wall.png", UriKind.RelativeOrAbsolute)));
                                break;

                            case GameItem.MiddleWall:
                                brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\middleWall.png", UriKind.RelativeOrAbsolute)));
                                break;
                            case GameItem.UpperWall:
                                brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\upperWall.png", UriKind.RelativeOrAbsolute))); ;
                                break;

                            case GameItem.Floor:
                                brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\floor.png", UriKind.RelativeOrAbsolute))); 

                                break;
                            case GameItem.Door:
                                brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\door.png", UriKind.RelativeOrAbsolute)));

                                break;

                            case GameItem.Spell:
                                brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\spell.png", UriKind.RelativeOrAbsolute)));
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

                    foreach (var enemy in enemyObjects)
                    {
                        ImageBrush enemyBrush = new ImageBrush(enemy.CurrentWalkImage);
                        drawingContext.DrawRectangle(
                                    enemyBrush,
                                    new Pen(Brushes.Black, 0),
                                    new Rect(enemy.Entity.Position.Y * rectWidth, enemy.Entity.Position.X * rectHeight, rectWidth, rectHeight)
                                    );
                    }

                    //foreach (var spell in spellObjects)
                    //{
                    //    ImageBrush enemyBrush = new ImageBrush(spell.CurrentWalkImage);
                    //    drawingContext.DrawRectangle(
                    //                enemyBrush,
                    //                new Pen(Brushes.Black, 0),
                    //                new Rect(spell.Entity.Position.Y * rectWidth, spell.Entity.Position.X * rectHeight, rectWidth, rectHeight)
                    //                );
                    //}

                }
            }
        }

        private void GameStateChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }
    }
}
