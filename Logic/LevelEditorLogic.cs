using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Logic
{
    public class LevelEditorLogic : ILevelEditorLogic
    {

        public int sizeOfMap { get; set; }

        public Size size { get; set; }
        public string[,] StringMap { get; set; }

        public Tile[,] TileMap { get; set; }

        private string ConvertTileToString(Tile tile)
        {
            switch (tile.Name)
            {
                case "upperWall":
                    return "W";

                case "middleWall":
                    return "V";

                case "wall":
                    return "v";


                case "floor":
                    return " ";

                case "slime":
                    return "E";

                case "wizard":
                    return "P";

                default:
                    return " ";

            }
        }
        private static int levelCount = 5;
        public void SaveMap()
        { 
            //oszlopk száma
            //sorok száma
            //pálya
            string save = StringMap.GetLength(1).ToString()  + "\n" + StringMap.GetLength(0).ToString() + "\n";
            for (int i = 0; i < StringMap.GetLength(0); i++)
            {
                for (int j = 0; j < StringMap.GetLength(1); j++)
                {
                    save += StringMap[i, j];
                }
                if(i < StringMap.GetLength(0)-1)
                save += "\n";
            }

            if (levelCount < 10)
            {
                File.WriteAllText("C:\\Users\\bekef\\Source\\Repos\\Zs01t\\Wizard_Unleashed_Reloaded\\Logic\\Levels\\" + "0" + levelCount + ".lvl", save);
            }
            else
            {
                File.WriteAllText("C:\\Users\\bekef\\Source\\Repos\\Zs01t\\Wizard_Unleashed_Reloaded\\Logic\\Levels\\" + levelCount + ".lvl", save);
            }
            levelCount++;

        }

        public void PlaceItemIntoGrid(double mousePosX, double mousePosY, Tile SelectedTile)
        {

            //átlaakítjuk a tile-t a neki megfelelő stringgé, hogy majd a Save gombra kattintva kimentsük
            string TileInString = ConvertTileToString(SelectedTile);


            //kiszámoljuk az egér pozíciójából, hogy az a StringMap melyik sor melyik oszlopának felel meg
            int arrayPosX = (int)((size.Width - (size.Width - mousePosX)) / (size.Width / sizeOfMap));
            int arrayPosY = (int)((size.Height - (size.Height - mousePosY)) / (size.Height / sizeOfMap));

            //belehelyezzük az adott stringet és Tile-t a megfelelő helyre
            StringMap[arrayPosY, arrayPosX] = TileInString;
            TileMap[arrayPosY, arrayPosX] = SelectedTile;


        }

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
    }
}
