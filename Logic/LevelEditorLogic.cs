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
            switch (tile.TileType)
            {

                case GameItem.Wall: return "v";
                case GameItem.Floor: return " ";
                case GameItem.Enemy: return "E";
                case GameItem.Player: return "P";
                case GameItem.Door: return "D";
                case GameItem.IronBar: return "B";
                case GameItem.IronBarTop: return "b";
                case GameItem.Void: return "0";



                //numpad alapján logikus
                case GameItem.UnderLeftCornerWall: return "1";
                case GameItem.UnderWall: return "2";
                case GameItem.UnderRightCornerWall: return "3";
                case GameItem.LeftSideWall: return "4";
                case GameItem.MiddleWall: return "5";
                case GameItem.RightSideWall: return "6";
                case GameItem.UpperCornerLeftWall: return "7";
                case GameItem.UpperWall: return "8";
                case GameItem.UpperCornerRightWall: return "9";


                case GameItem.LeftUpperCornerPiece: return "Ő" ;
                case GameItem.RightUpperCornerPiece: return "Ú";
                case GameItem.LeftLowerCornerPiece: return "Á";
                case GameItem.RightLowerCornerPiece: return "Ű";
                default: return " ";



            }
        }

        public void SaveMap()
        {

            //oszlopk száma
            //sorok száma
            //pálya
            string save = StringMap.GetLength(1).ToString() + "\n" + StringMap.GetLength(0).ToString() + "\n";
            for (int i = 0; i < StringMap.GetLength(0); i++)
            {
                for (int j = 0; j < StringMap.GetLength(1); j++)
                {
                    save += StringMap[i, j];
                }
                if (i < StringMap.GetLength(0) - 1)
                    save += "\n";
            }

            // ...\Wizard_Unleashed_Reloaded\Wizard_Unleashed\bin\Debug\net5.0-windows\Levels
            //A fentebb írt mappába menti el a fájlt, nem túl szép de ez van
            //Azért töröltem ki a Level mappát, mert amikor oda mentettem szintet, akkor ott külön manuálisan be kellett volna állítani, hogy Content Always Copy legyen
            //A szinteket olyan néven mentem el, hogy megadom a szint számát az alapján, hogy hány .lvl fájlt talált az adott mappában
            int levelCount = 0;
            if (Directory.Exists("Levels"))
            {
                levelCount = Directory.GetFiles("Levels", "*lvl").Length;
            }
            else
            {
                Directory.CreateDirectory("Levels");
            }
            
            if (levelCount < 10)
            {
                File.WriteAllText(Directory.GetCurrentDirectory()+ @"\Levels" + @"\L0" + levelCount + ".lvl", save);
                
            }
            else
            {
                File.WriteAllText(Directory.GetCurrentDirectory() +  @"\Levels" + @"\L" + levelCount + ".lvl", save);
            }
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
                    TileMap[i, j] = new Tile(GameItem.Floor);
                    StringMap[i, j] = " ";
                }
            }

        }
    }
}
