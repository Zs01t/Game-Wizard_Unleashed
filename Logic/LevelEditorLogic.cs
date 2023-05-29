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
        public char[,] CharMap { get; set; }

        public Tile[,] TileMap { get; set; }

        private char ConvertTileToString(Tile tile)
        {
            //ezt lehetne dinamikussá tenni, de az enumok nem alkalmasak erre... hatalmas átírásra lenne szükség, csak azért, hogy ha egy új képet rakok a tiles mappába, akkor azt mindenhol jól letudja kezelni
            switch (tile.TileType)
            {
                //átírtam a stringeket char-rá hogy konzisztensebb legyen
                case GameItem.Wall: return 'v';
                case GameItem.Floor: return ' ';
                case GameItem.Enemy: return 'E';
                case GameItem.Player: return 'P';
                case GameItem.Door: return 'D';
                case GameItem.OpenTrapDoor: return 'd';
                case GameItem.IronBar: return 'B';
                case GameItem.IronBarTop: return 'b';
                case GameItem.Void: return '0';



                //numpad alapján logikus
                case GameItem.UnderLeftCornerWall: return '1';
                case GameItem.UnderWall: return '2';
                case GameItem.UnderRightCornerWall: return '3';
                case GameItem.LeftSideWall: return '4';
                case GameItem.MiddleWall: return '5';
                case GameItem.RightSideWall: return '6';
                case GameItem.UpperCornerLeftWall: return '7';
                case GameItem.UpperWall: return '8';
                case GameItem.UpperCornerRightWall: return '9';


                case GameItem.LeftUpperCornerPiece: return 'Ő' ;
                case GameItem.RightUpperCornerPiece: return 'Ú';
                case GameItem.LeftLowerCornerPiece: return 'Á';
                case GameItem.RightLowerCornerPiece: return 'Ű';
                default: return ' ';



            }
        }

        public void SaveMap()
        {

            //oszlopk száma
            //sorok száma
            //pálya
            string save = CharMap.GetLength(1).ToString() + "\n" + CharMap.GetLength(0).ToString() + "\n";
            for (int i = 0; i < CharMap.GetLength(0); i++)
            {
                for (int j = 0; j < CharMap.GetLength(1); j++)
                {
                    save += CharMap[i, j];
                }
                if (i < CharMap.GetLength(0) - 1)
                    save += "\n";
            }

            //ez most már lokáció függetlenül a megfelelő helyre menti a pályákat
            int levelCount = 0;

            string lvlPath = Directory.GetCurrentDirectory().Replace("Wizard_Unleashed\\bin\\Debug\\net5.0-windows", "Logic\\Levels\\");

            if (Directory.Exists(lvlPath))
            {
                levelCount = Directory.GetFiles(lvlPath).Length;
            }
            else
            {
                Directory.CreateDirectory(lvlPath);
            }
            
            if (levelCount < 10)
            {
                File.WriteAllText(lvlPath + @"\L0" + levelCount + ".lvl", save);
                
            }
            else
            {
                File.WriteAllText(lvlPath + @"\L" + levelCount + ".lvl", save);
            }


            //TODO: ennél nem működik a képlétrehozás, egy nagy zöld négyzetet hoz létre, hiába illesztem össze a képeket

            //egy képként elmentjük az egész pályát, így rendernél nem kell minden egyes statikus objektumot újragenerálni
            //BitmapFrame[,] frames = new BitmapFrame[TileMap.GetLength(0), TileMap.GetLength(1)];
            //for (int i = 0; i < TileMap.GetLength(0); i++)
            //{
            //    for (int j = 0; j < TileMap.GetLength(1); j++)
            //    {

            //        Uri uri = FindCorrectImage(TileMap[i, j].Name);
            //        BitmapFrame tmp = BitmapDecoder.Create(uri, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad).Frames.First();
            //        frames[i, j] = tmp ;

            //    }
            //}


            //Uri FindCorrectImage(string name)
            //{
            //    return new Uri("C:\\Users\\bekef\\Source\\Repos\\Zs01t\\Wizard_Unleashed_Reloaded\\Wizard_Unleashed\\Assets\\Tiles\\" + name + ".png", UriKind.RelativeOrAbsolute);
            //}

            ////feltételezzük, hogy mindegyik kép ugyanakkora
            //int imageWidth = frames[0, 0].PixelWidth;
            //int imageHeight = frames[0, 0].PixelHeight;

            ////egy DrawingVisual component-re rárajzoljuk a frameket
            //DrawingVisual drawingVisual = new DrawingVisual();
            //using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            //{
            //    for (int i = 0; i < TileMap.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < TileMap.GetLength(1); j++)
            //        {
            //            drawingContext.DrawImage(frames[i, j], new Rect(j * imageWidth, i * imageHeight, imageWidth, imageHeight));
            //        }
            //    }

            //    //ezt a DrawingVisual-t egy BitmapSource-á alakítjuk

            //    RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * frames.GetLength(1), imageHeight * frames.GetLength(0), 96, 96, PixelFormats.Pbgra32);
            //    bmp.Render(drawingVisual);

            //    PngBitmapEncoder encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create(bmp));

            //    //elmentjük a képet

            //    string mapImageName = lvlPath;
            //    if (levelCount < 10)
            //    {
            //        mapImageName += @"\L0" + levelCount + ".png";

            //    }
            //    else
            //    {
            //        mapImageName += @"\L" + levelCount + ".png";
            //    }



            //    using (FileStream stream = File.Create(mapImageName))

            //        encoder.Save(stream);

            //}

        }

        public void PlaceItemIntoGrid(double mousePosX, double mousePosY, Tile SelectedTile)
        {

            //átlaakítjuk a tile-t a neki megfelelő stringgé, hogy majd a Save gombra kattintva kimentsük
            char TileInString = ConvertTileToString(SelectedTile);


            //kiszámoljuk az egér pozíciójából, hogy az a StringMap melyik sor melyik oszlopának felel meg
            int arrayPosX = (int)((size.Width - (size.Width - mousePosX)) / (size.Width / sizeOfMap));
            int arrayPosY = (int)((size.Height - (size.Height - mousePosY)) / (size.Height / sizeOfMap));

            //belehelyezzük az adott stringet és Tile-t a megfelelő helyre
            CharMap[arrayPosY, arrayPosX] = TileInString;
            TileMap[arrayPosY, arrayPosX] = SelectedTile;


        }

        public void SetupSizes(Size size)
        {
            this.size = size;
            CharMap = new char[sizeOfMap, sizeOfMap];
            TileMap = new Tile[sizeOfMap, sizeOfMap];

            for (int i = 0; i < TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < TileMap.GetLength(1); j++)
                {
                    //alapesetben minden Tile floor és ennek megfelelően minden string szóköz
                    TileMap[i, j] = new Tile(GameItem.Floor);
                    CharMap[i, j] = ' ';
                }
            }

        }
    }
}
