using Logic;
using Models;
using Models.Enemies;
using Models.GameObjects;
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
        public List<ProjectileObject> spellObjects;

        public void SetupSizes(Size size)
        {

            this.size = size;
            this.InvalidateVisual();
        }

        public void SetupModel(IGameLogic logic)
        {
            this.logic = logic;
            playerObject = new EntityObject(logic.Player, @"Assets\Wizard");


            enemyObjects = new List<EntityObject>();
            foreach (var enemy in logic.Enemies)
            {
                enemyObjects.Add(new EntityObject(enemy, @"Assets\Slime"));
            }

            spellObjects = new List<ProjectileObject>();
            
            logic.GameStateChanged += this.GameStateChanged;
        }

        public void AddSpell()
        {
            if (logic!= null)
            {
                foreach (var spell in logic.Spells)
                {
                    if (!spell.IsImageAssigned)
                    {
                        spellObjects.Add(new ProjectileObject(spell, @"Assets\Fireball"));
                        spell.IsImageAssigned = true;
                    }

                }
            }
            
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

            this.AddSpell();
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
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth , rectHeight )
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

                            case GameItem.Wall:brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\wall.png", UriKind.RelativeOrAbsolute)));break;
                            case GameItem.Floor: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\floor.png", UriKind.RelativeOrAbsolute))); break;

                            case GameItem.Door: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\door.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.IronBar: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\ironBar.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.IronBarTop: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\ironBarTop.png", UriKind.RelativeOrAbsolute))); break;

                            case GameItem.UnderLeftCornerWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\underLeftCornerWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.UnderWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\underWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.UnderRightCornerWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\underRightCornerWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.LeftSideWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\leftSideWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.MiddleWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\middleWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.RightSideWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\rightSideWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.UpperCornerLeftWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\upperCornerLeftWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.UpperWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\upperWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.UpperCornerRightWall: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\upperCornerRightWall.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.LeftUpperCornerPiece: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\LeftUpperCornerPiece.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.LeftLowerCornerPiece: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\LeftLowerCornerPiece.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.RightLowerCornerPiece: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\RightLowerCornerPiece.png", UriKind.RelativeOrAbsolute))); break;
                            case GameItem.RightUpperCornerPiece: brush = new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\RightUpperCornerPiece.png", UriKind.RelativeOrAbsolute))); break;


                            default:break;
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
                                   new Rect(j * rectWidth, i * rectHeight, rectWidth , rectHeight)
                                   );
                        }

                        else
                        {
                            drawingContext.DrawRectangle(
                                    brush,
                                    new Pen(Brushes.Black, 0),
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth , rectHeight )
                                    );
                        }
                        

                    }
                    if (logic.mapChanged)
                    {
                        EnemyListAfterMapChanged();
                        logic.mapChanged = false;
                    }
                   
                    for (int k = 0; k < enemyObjects.Count; k++)
                    {
                        if (enemyObjects[k].Entity.Health > 0)
                        {
                            ImageBrush enemyBrush = new ImageBrush(enemyObjects[k].CurrentWalkImage);
                            drawingContext.DrawRectangle(
                                        enemyBrush,
                                        new Pen(Brushes.Black, 0),
                            new Rect(enemyObjects[k].Entity.Position.Y * rectWidth, enemyObjects[k].Entity.Position.X * rectHeight, rectWidth, rectHeight)
                                        );
                        }
                        else
                        {
                            enemyObjects.Remove(enemyObjects[k]);
                        }
                    }



                    foreach (var spell in spellObjects)
                    {
                        if (logic.Spells.Contains(spell.Entity))
                        {
                            ImageBrush spellBrush = new ImageBrush(spell.CurrentProjectileImage);
                            drawingContext.DrawRectangle(
                                        spellBrush,
                                        new Pen(Brushes.Black, 0),
                                        new Rect(spell.Entity.Position.Y * rectWidth, spell.Entity.Position.X * rectHeight, rectWidth, rectHeight)
                                        );
                        }

                    }
                }
            }
        }

        public void EnemyListAfterMapChanged()
        {
            enemyObjects = new List<EntityObject>();
            foreach (var enemy in logic.Enemies)
            {
                enemyObjects.Add(new EntityObject(enemy, @"Assets\Slime"));
            }
        }

        private void GameStateChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }
    }
}
