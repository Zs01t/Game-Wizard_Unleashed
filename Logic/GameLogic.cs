using Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Logic
{
    public class GameLogic : IGameLogic
    {
        public GameItem[,] Map { get; private set; }
        public Player Player { get; set; }

        private Queue<string> levels;

        public GameLogic(Player player)
        {
            this.Player = player;
            levels = new Queue<string>();
            var lvlFiles = Directory.GetFiles("Levels",
                    "*.lvl");

            foreach (var item in lvlFiles)
            {
                levels.Enqueue(item);
            }
            LoadNext(levels.Dequeue());
        }
        public void LoadNext(string path)
        {
            //a sorok az Y-nak, az oszlopok az X-nek felelnek meg
            string[] lines = File.ReadAllLines(path);
            Map = new GameItem[int.Parse(lines[1]), int.Parse(lines[0])];

            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    Map[i, j] = ConvertToEnum(lines[i + 2][j]);
                    if (Map[i, j] == GameItem.Player)
                    {
                        //megadjuk a koordinátáit a játékosnak, ami a mozgáshoz kell neki
                        Player.Position.X = i;
                        Player.Position.Y = j;
                    }
                }
            }
        }

        private GameItem ConvertToEnum(char v)
        {
            switch (v)
            {
                case '#': return GameItem.Wall;
                case 'P': return GameItem.Player;
                case 'F': return GameItem.Floor;
                case 'E': return GameItem.Enemy;
                case 'S': return GameItem.Spell;
                case 'D': return GameItem.Door;
                default: return GameItem.Floor;
            }
        }
        GameItem TileYouAreCurrentlyOn;
        GameItem TileYouWerePreviouslyOn;
        public void Control(Direction direction)
        {
            int oldPosX = Player.Position.X;
            int oldPosY = Player.Position.Y;
            int posX = Player.Position.X;
            int posY = Player.Position.Y;
            switch (direction)
            {
                case Direction.Left:
                    if(posY > 0)
                        posY -= 1;

                    break;

                case Direction.Right:
                    if(posY < Map.GetLength(0))
                        posY += 1;
                    break;

                case Direction.Up:
                    if(posX > 0)
                        posX -= 1;

                    break;

                case Direction.Down:
                    if(posX < Map.GetLength(0))
                        posX += 1;

                    break;
            }
            if (Map[posX, posY] != GameItem.Wall)
            {
                
                TileYouAreCurrentlyOn = Map[posX, posY];
                if (TileYouAreCurrentlyOn == GameItem.Player)
                {
                    TileYouAreCurrentlyOn = GameItem.Floor;
                }
                Map[posX, posY] = GameItem.Player;
                Player.Position.X = posX;
                Player.Position.Y = posY;
                Map[oldPosX, oldPosY] = TileYouWerePreviouslyOn;
                TileYouWerePreviouslyOn = TileYouAreCurrentlyOn;
            }
            else
            {
                //erre az ágra nincs is szükség valószínűleg
                Player.Position.X = oldPosX;
                Player.Position.Y = oldPosY;

            }
        }

    }
}
