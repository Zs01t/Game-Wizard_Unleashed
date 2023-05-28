using Logic;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wizard_Unleashed
{
    public class LevelEditorDisplay : FrameworkElement
    {

        public ILevelEditorLogic editorLogic { get; set; }

        public void SetUpLogic(ILevelEditorLogic logic)
        {
            editorLogic = logic;
            editorLogic.sizeOfMap = 16;
            
        }

        public void SetupSizes(Size size)
        {
            editorLogic.SetupSizes(size);
        }

        public void PlaceItemIntoGrid(double mousePosX, double mousePosY, Tile SelectedTile)
        { 
            editorLogic.PlaceItemIntoGrid(mousePosX, mousePosY, SelectedTile);
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (editorLogic != null)
            {




                base.OnRender(drawingContext);
                double rectWidth = editorLogic.size.Width / editorLogic.sizeOfMap;
                double rectHeight = editorLogic.size.Height / editorLogic.sizeOfMap;

                for (int i = 0; i < editorLogic.sizeOfMap; i++)
                {
                    for (int j = 0; j < editorLogic.sizeOfMap; j++)
                    {
                        
                        drawingContext.DrawRectangle(
                                    new ImageBrush(new BitmapImage(new Uri(@"Assets\Tiles\floor.png", UriKind.RelativeOrAbsolute))),
                                    new Pen(Brushes.Black, 1),
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth, rectHeight)
                                    );
                        ImageBrush tileBursh = new ImageBrush(editorLogic.TileMap[i, j].Image);
                        drawingContext.DrawRectangle(
                                    tileBursh,
                                    new Pen(Brushes.Black, 1),
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth, rectHeight)
                                    );
                    }
                }
            }
        }
    }
}
