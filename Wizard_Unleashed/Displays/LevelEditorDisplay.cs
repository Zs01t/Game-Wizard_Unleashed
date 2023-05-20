using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Wizard_Unleashed
{
    public class LevelEditorDisplay : FrameworkElement
    {

        private const int sizeOfMap = 16;

        private Size size;

        //ezeket lehet ki kell majd szervezni egy a LevelDeitorLogic-ba...
        public string[,] StringMap { get; set; }
        public Tile[,] TileMap { get; set; }

        public void SetupSizes(Size size)
        {
            this.size = size;
            StringMap = new string[sizeOfMap, sizeOfMap];
            TileMap = new Tile[sizeOfMap, sizeOfMap];

            for (int i = 0; i < TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < TileMap.GetLength(1); j++)
                {
                    //alapesetben minden Tile floor és ennek megfelelően minden string szóköz
                    TileMap[i, j] = new Tile("floor");
                    StringMap[i, j] = " ";
                }
            }
        }



        public void PlaceItemIntoGrid(double mousePosX, double mousePosY, Tile SelectedTile)
        {
            //átlaakítjuk a tile-t a neki megfelelő stringgé, hogy majd a Save gombra kattintva kimentsük
            string TileInString = ConvertTileToString(SelectedTile);


            //kiszámoljuk az egér pozíciójából, hogy az a StringMap melyik sor melyik oszlopának felel meg
            int arrayPosX = (int)((size.Width - (size.Width - mousePosX)) / (size.Width  / sizeOfMap));
            int arrayPosY = (int)((size.Height -(size.Height -mousePosY)) / (size.Height / sizeOfMap));

            //belehelyezzük az adott stringet és Tile-t a megfelelő helyre
            StringMap[arrayPosY, arrayPosX] = TileInString;
            TileMap[arrayPosY, arrayPosX] = SelectedTile;


            //majd firssítjük a képernyőt
            this.InvalidateVisual();

        }

        private string ConvertTileToString(Tile tile)
        {
            switch (tile.Name)
            {
                case "wall":
                    return "#";
                    

                case "floor":
                    return " ";

                case "enemy":
                    return "E";

                case "wizard":
                    return "P";
       
                default:
                    return " ";

            }
        }

        
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (TileMap != null)
            {
                base.OnRender(drawingContext);
                double rectWidth = size.Width / sizeOfMap;
                double rectHeight = size.Height / sizeOfMap;

                for (int i = 0; i < sizeOfMap; i++)
                {
                    for (int j = 0; j < sizeOfMap; j++)
                    {
                        ImageBrush tileBursh = new ImageBrush(TileMap[i, j].Image);
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
