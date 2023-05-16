﻿using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic
{
    public class GameLogic : IGameLogic
    {
        public GameItem[,] Map { get; private set; }
        public Player Player { get; set; }

        public List<Spell> Spells { get; private set; }

        private Queue<string> levels;

        public GameLogic(Player player)
        {
            this.Player = player;
            Spells = new List<Spell>();
            levels = new Queue<string>();
            var lvlFiles = Directory.GetFiles("Levels",
                    "*.lvl");

            foreach (var item in lvlFiles)
            {
                levels.Enqueue(item);
            }
            LoadNext(levels.Dequeue());
        }

        public void TimeStep()
        {
            SpellAnimation();
        }

        public void LoadNext(string path)
        {
            //a sorok az Y-nak, az oszlopok az X-nek felelnek meg
            string[] lines = File.ReadAllLines(path);
            Map = new GameItem[int.Parse(lines[1]), int.Parse(lines[0])];

            for (int i = 0; i < Map.GetLength(0); i++)
            {
                ;
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
                    if(posY < Map.GetLength(1))
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
            ;

            if (Map[posX, posY] != GameItem.Wall)
            {
                Map[posX, posY] = GameItem.Player;
                Player.Position.X = posX;
                Player.Position.Y = posY;
                Map[oldPosX, oldPosY] = GameItem.Floor;

                //lövéshez (később jó lehet a player rendereléséhez is)
                Player.Direction = direction;
            }
            else
            {
                //erre az ágra nincs is szükség valószínűleg
                Player.Position.X = oldPosX;
                Player.Position.Y = oldPosY;

            }
        }

        //spell lövése (csak ellenőrzi, hogy lőhet e spellt a jelen körülmények között, ha igen, létrehozza)
        public void CastSpell()
        {
            // a player helyéhez képest fogjuk meghatározni a spell helyét
            int posX = Player.Position.X; 
            int posY = Player.Position.Y;

            // ezzel okézzuk le, hogy a switch-ben a játéktér egy valid pontjára kerül a spell
            bool castable = false;

            // player melyik oldalára kerül a spell
            Direction direction = Player.Direction;

            // player iránya alapján megnézi, hogy a játéktéren belül marad e spell,
            // ha igen módosítja a megfelelő koordinátát és leokézza a boolt
            switch (direction)
            {
                case Direction.Left:
                    if (posY > 0)
                    { 
                        posY -= 1;
                        castable = true;
                    }
                    break;

                case Direction.Right:
                    if (posY < Map.GetLength(1))
                    { 
                        posY += 1;
                        castable = true;
                    }
                    break;

                case Direction.Up:
                    if (posX > 0)
                    { 
                        posX -= 1;
                        castable = true;
                    }
                    break;

                case Direction.Down:
                    if (posX < Map.GetLength(0))
                    { 
                        posX += 1;
                        castable = true;
                    }
                    break;
            }
            
            // ha a spell megjelenésének a helye nem fal és a switch is leokézta, akkor létrehozzuk a spell-t
            if (Map[posX, posY] != GameItem.Wall && castable)
            {
                Spells.Add(new Spell(new Coords(posX, posY), direction));
                Map[posX, posY] = GameItem.Spell;
            }
        }

        // spellek mozgatása a játéktéren
        public void SpellAnimation()
        {
            // ha van mozgatandó spell:
            if (Spells.Count > 0)
            {
                // végig megyünk az összesen
                for (int i = 0; i < Spells.Count; i++)
                {
                    Spell spell = Spells[i];

                    // kimentjük a spell régi helyét és létrehozzuk a potenciális új koordinátákat
                    int oldposX = spell.Position.X;
                    int oldposY = spell.Position.Y;
                    int posX = spell.Position.X;
                    int posY = spell.Position.Y;

                    // a spell iránya alapján módosítjuk az új koordinátákat
                    switch (spell.Direction)
                    {
                        case Direction.Left:
                            if (posY > 0)
                                posY -= 1;
                            break;

                        case Direction.Right:
                            if (posY < Map.GetLength(1))
                                posY += 1;
                            break;

                        case Direction.Up:
                            if (posX > 0)
                                posX -= 1;
                            break;

                        case Direction.Down:
                            if (posX < Map.GetLength(0))
                                posX += 1;
                            break;
                    }

                    // a spell régi helyére vissza rakjuk a floor-t
                    Map[oldposX, oldposY] = GameItem.Floor;

                    // ha a spell új koordinátáján nem fal van, odamozgatjuk és a spell koordinátáit is beállítjuk
                    if (Map[posX, posY] != GameItem.Wall)
                    {
                        Map[posX, posY] = GameItem.Spell;
                        spell.Position.X = posX;
                        spell.Position.Y = posY;
                    }
                    // amúgy kiszedjük a listából a spell-t
                    else
                    {
                        Spells.Remove(spell);
                        i--;
                    }
                }
            }
        }
    }
}
