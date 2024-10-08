﻿using Models;
using Models.Enemies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Logic
{
    public class GameLogic : IGameLogic
    {
        public event EventHandler GameStateChanged;
        public event EventHandler NoMoreLevel;
        private static Random rnd = new Random();
        public GameItem[,] Map { get; private set; }
        public Player Player { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<Spell> Spells { get; set; }

        private Queue<string> levels;

        public bool mapChanged { get; set; }

        public GameLogic(Player player)
        {
            this.Player = player;           
            Spells = new List<Spell>();
            Enemies = new List<Enemy>();
            levels = new Queue<string>();
            mapChanged = false;

            string lvlPath = Directory.GetCurrentDirectory().Replace("Wizard_Unleashed\\bin\\Debug\\net5.0-windows", "Logic\\Levels\\");




            var lvlFiles = Directory.GetFiles(lvlPath,
                    "*.lvl");

            foreach (var item in lvlFiles)
            {
                levels.Enqueue(item);
            }
            LoadNext(levels.Dequeue());
        }

        // a timesteppel óvatosan kell bánni, nem mindegy, hogy melyik metódus melyik után fut le, úgyhogy ha beillesztetek 
        public void TimeStep()
        {
            for (int i = 0; i < Enemies.Count(); i++)
            {
                i = EnemyHit(Enemies[i], i);
            }
            SpellAnimation();
            EnemyStepDistributor();
            //for (int i = 0; i < Enemies.Count(); i++)
            //{
            //    i = EnemyHit(Enemies[i], i);
            //}
            IsPlayerDead(); //halálellenőrzés
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
                    else if (Map[i, j] == GameItem.Enemy)
                    {
                        // ebben az if-ben csak setup-olni akarom a különböző pályákon a különböző enemy generálásokat aszerint,
                        // hogy hány pálya van még a queue-ban (ha még 4 pálya van, azaz az első pályán vagyunk akkor csak BasicEnemy-ket rak, későbbiekben pedig egyre durvábbakat)
                        //
                        // de egyébként olyasmi is lehet, hogy randomizáljuk,
                        // hogy hány enemy legyen egy pályán, milyen enemy-k legyenek (nyílván nehézséghez igazítva) és azt is hogy hol,
                        // nem lenne annyira nehéz megoldani, és egy beállítható nehézségi fok is a fejemben van, de azt tényleg csak ha az idő engedi
                        // -G
                        switch (levels.Count())
                        {
                            case 4:
                                Enemies.Add(new NormalEnemy(new Coords(i, j), 20));
                                break;
                            case 3:
                                Enemies.Add(new NormalEnemy(new Coords(i, j), 20));
                                break;
                            case 2:
                                Enemies.Add(new NormalEnemy(new Coords(i, j), 20));
                                break;
                            case 1:
                                Enemies.Add(new NormalEnemy(new Coords(i, j), 20));
                                break;
                            default:
                                Enemies.Add(new NormalEnemy(new Coords(i, j), 20));
                                break;
                        }
                    }
                }
            }
        }

        private GameItem ConvertToEnum(char v)
        {
            //ezt lehetne dinamikussá tenni
            switch (v)
            {


                case 'v': return GameItem.Wall;
                case ' ': return GameItem.Floor;

                case 'E': return GameItem.Enemy;
                case 'P': return GameItem.Player;
                case 'S': return GameItem.Spell;

                case 'D': return GameItem.Door;
                case 'd': return GameItem.OpenTrapDoor;
                case 'B': return GameItem.IronBar;
                case 'b': return GameItem.IronBarTop;
                case '0': return GameItem.Void;



                //numpad alapján logikus...
                case '1': return GameItem.UnderLeftCornerWall;
                case '2': return GameItem.UnderWall;
                case '3': return GameItem.UnderRightCornerWall;
                case '4': return GameItem.LeftSideWall;
                case '5': return GameItem.MiddleWall;
                case '6': return GameItem.RightSideWall;
                case '7': return GameItem.UpperCornerLeftWall;
                case '8': return GameItem.UpperWall;
                case '9': return GameItem.UpperCornerRightWall;


                case 'Ő': return GameItem.LeftUpperCornerPiece;
                case 'Ú': return GameItem.RightUpperCornerPiece;
                case 'Á': return GameItem.LeftLowerCornerPiece;
                case 'Ű': return GameItem.RightLowerCornerPiece;
                default: return GameItem.Floor;
            }
        }

        // MUST: ne lehessen enemy-re lépni
        public void Control(Direction direction)
        {
            int oldPosX = Player.Position.X;
            int oldPosY = Player.Position.Y;
            int posX = Player.Position.X;
            int posY = Player.Position.Y;

            switch (direction)
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

            //itt majd meg kell írni hogy mely falakkal nem szabad collidolni -Zs
            if (!CollidedWithObject(posX, posY))
            {
                if (Map[posX, posY] == GameItem.OpenTrapDoor)
                {

                    mapChanged = true;
                    this.Enemies.RemoveAll(t => true);
                    if (levels.Count == 0)
                    {
                        NoMoreLevel?.Invoke(this, null);
                    }
                    else
                    {
                        LoadNext(levels.Dequeue());
                    }
                    
                }
                else
                {
                    Map[posX, posY] = GameItem.Player;
                    Player.Position.X = posX;
                    Player.Position.Y = posY;
                    Map[oldPosX, oldPosY] = GameItem.Floor;
                    //lövéshez (később jó lehet a player rendereléséhez is)
                    Player.Direction = direction;
                }



            }


            this.GameStateChanged.Invoke(this, null);
        }

        public event EventHandler PlayerDead;
        public void IsPlayerDead()
        {
            if(Player.Health <= 0)
            {
                this.PlayerDead.Invoke(this, null);
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

            // ha a spell megjelenésének a helye nem fal és a switch is leokézta és a player cooldown-ja lement akkor létrehozzuk a spell-t
            if (!CollidedWithObject(posX, posY) && castable && Player.canCast()) //1mp a cooldown
            {
                Spells.Add(new Spell(new Coords(posX, posY), direction));
                Map[posX, posY] = GameItem.Spell;
            }
        }

        //falnak ütközés metódus, a walls elnevezés nem jó már, mert nem csak falaknak ütközhet neki, viszont a kód egyes részeiben ez a lista lett használva
        private List<GameItem> walls = new List<GameItem>() { GameItem.Wall, GameItem.LeftSideWall, GameItem.UpperWall, GameItem.RightSideWall,
                                                              GameItem.MiddleWall, GameItem.IronBar, GameItem.IronBarTop, GameItem.UnderLeftCornerWall,
                                                              GameItem.UnderRightCornerWall, GameItem.UpperCornerLeftWall, GameItem.UpperCornerRightWall,
                                                              GameItem.Void, GameItem.UnderWall, GameItem.Door, GameItem.Enemy, GameItem.Spell, GameItem.LeftLowerCornerPiece, GameItem.LeftUpperCornerPiece, GameItem.RightLowerCornerPiece, GameItem.RightUpperCornerPiece
                                                            };
        private bool CollidedWithObject(int posX, int PosY)
        {
            foreach (var wall in walls)
            {
                if (Map[posX, PosY] == wall)
                {
                    return true;
                }
            }
            return false;

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
                    if (!CollidedWithObject(posX, posY))
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

            this.GameStateChanged.Invoke(this, null);
        }

        private void EnemyStepDistributor()
        {
            if (Enemies.Count() > 0)
            {
                for (int i = 0; i < Enemies.Count(); i++)
                {
                    Enemy enemy = Enemies[i];

                    if (enemy is BasicEnemy)
                    {
                        //i = EnemyHit(enemy, i);
                        BasicEnemyStep(enemy);
                        //i = EnemyHit(enemy, i);
                    }
                    else if (enemy is NormalEnemy)
                    {
                        NormalEnemyStep(enemy as NormalEnemy);
                    }
                }
            }
            else
            {
                //itt végzek egy ellenőrzést, hogy a pályán az enemyCount 0, akkor nyíljon ki a Door, ergó cseréljük ki a Door-t OpenTrapDoorrá
                for (int i = 0; i < Map.GetLength(0); i++)
                {
                    for (int j = 0; j < Map.GetLength(1); j++)
                    {
                        if (Map[i, j] == GameItem.Door)
                        {
                            Map[i, j] = GameItem.OpenTrapDoor;
                            break;
                        }
                    }
                }
            }
        }

        // eltüntetni is el kell!!!! (de már mukszik?)
        private int EnemyHit(Enemy enemy, int k)
        {
   

            for (int i = 0; i < Spells.Count(); i++)
            {
                Spell spell = Spells[i];

                if (spell.Position.X == enemy.Position.X && spell.Position.Y == enemy.Position.Y)
                {
                    enemy.Health -= 10;
                    //Map[spell.Position.X, spell.Position.Y] = GameItem.Floor;
                    this.Spells.Remove(spell);
                    //this.Enemies.Remove(enemy);
                    i--;
                }
                else
                {
                    int new_xa = spell.Position.X + 1;
                    int new_ya = spell.Position.Y + 1;
                    int new_xs = spell.Position.X - 1;
                    int new_ys = spell.Position.Y - 1;

                    switch (spell.Direction)
                    {
                        case Direction.Left:
                            if (spell.Position.X == enemy.Position.X && new_ys == enemy.Position.Y)
                            {
                                enemy.Health -= 10;
                                Map[spell.Position.X, spell.Position.Y] = GameItem.Floor;
                                Spells.Remove(spell);
                                //i--;
                                //this.Enemies.Remove(enemy);
                                i--;
                            }
                            break;
                        case Direction.Right:
                            if (spell.Position.X == enemy.Position.X && new_ya == enemy.Position.Y)
                            {
                                enemy.Health -= 10;
                                Map[spell.Position.X, spell.Position.Y] = GameItem.Floor;
                                Spells.Remove(spell);
                                //i--;
                                //this.Enemies.Remove(enemy);
                                i--;
                            }
                            break;
                        case Direction.Up:
                            if (new_xs == enemy.Position.X && spell.Position.Y == enemy.Position.Y)
                            {
                                enemy.Health -= 10;
                                Map[spell.Position.X, spell.Position.Y] = GameItem.Floor;
                                Spells.Remove(spell);
                                //i--;
                                //this.Enemies.Remove(enemy);
                                i--;
                            }
                            break;
                        case Direction.Down:
                            if (new_xa == enemy.Position.X && spell.Position.Y == enemy.Position.Y)
                            {
                                enemy.Health -= 10;
                                Map[spell.Position.X, spell.Position.Y] = GameItem.Floor;
                                Spells.Remove(spell);
                                //i--;
                                //this.Enemies.Remove(enemy);
                                i--;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (enemy.Health <= 0)
            {
                Map[enemy.Position.X, enemy.Position.Y] = GameItem.Floor;
                Enemies.Remove(enemy);
                this.GameStateChanged.Invoke(this, null);
                return k--;
            }
            else
            {
                this.GameStateChanged.Invoke(this, null);
                return k;
            }
        }

        private void BasicEnemyStep(Enemy enemy)
        {
            int oldposX = enemy.Position.X;
            int oldposY = enemy.Position.Y;
            int posX = enemy.Position.X;
            int posY = enemy.Position.Y;

            if (enemy.Position.X == this.Player.Position.X)
            {
                if (enemy.Position.Y == this.Player.Position.Y)
                {
                    // rajta áll
                }
                else if (enemy.Position.Y < this.Player.Position.Y)
                {
                    posY++;
                    enemy.Direction = Direction.Right;
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posY--;
                    enemy.Direction = Direction.Left;
                }
            }
            else if (enemy.Position.X < this.Player.Position.X)
            {
                if (enemy.Position.Y == this.Player.Position.Y)
                {
                    posX++;
                }
                else if (enemy.Position.Y < this.Player.Position.Y)
                {
                    posX++;
                    posY++;
                    enemy.Direction = Direction.Right;
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX++;
                    posY--;
                    enemy.Direction = Direction.Left;
                }
            }
            else if (enemy.Position.X > this.Player.Position.X)
            {
                if (enemy.Position.Y == this.Player.Position.Y)
                {
                    posX--;
                }
                else if (enemy.Position.Y < this.Player.Position.Y)
                {
                    posX--;
                    posY++;
                    enemy.Direction = Direction.Right;
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX--;
                    posY--;
                    enemy.Direction = Direction.Left;
                }
            }

            if (!CollidedWithObject(posX, posY) && Map[posX, posY] != GameItem.Enemy && Map[posX, posY] != GameItem.Player)
            {
                Map[oldposX, oldposY] = GameItem.Floor;

                enemy.Position.X = posX;
                enemy.Position.Y = posY;

                Map[posX, posY] = GameItem.Enemy;
            }
            else if (Map[posX,posY] == GameItem.Player)
            {
                Player.Injure(10);
            }
            else if (CollidedWithObject(posX, posY) && Map[posX, posY] != GameItem.Enemy && Map[posX, posY] != GameItem.Player)
            {
                Coords forBypass = EnemyBypassWall(new Coords(oldposX, oldposY));

                Map[oldposX, oldposY] = GameItem.Floor;

                enemy.Position.X = forBypass.X;
                enemy.Position.Y = forBypass.Y;

                Map[forBypass.X, forBypass.Y] = GameItem.Enemy;

            }
        }

        //ez jobb lenne a basicenemynek, mert a falat ölelve szeretne ezzel végigmenni
        private Coords EnemyBypassWall(Coords PosE)
        {
            //enemy coords
            int EX = PosE.X;
            int EY = PosE.Y;

            //player coords
            int PX = Player.Position.X;
            int PY = Player.Position.Y;

            //bypass jóságot számoló változók
            int BnX = 0; //bypass negative X (felfelé)
            int BpX = 0; //bypass positive X (lefelé)
            int BnY = 0; //bypass negative Y (balra)
            int BpY = 0; //bypass Positive Y (jobbra)

            //egy oszlopban vannak
            if (EY == PY)
            {
                //enemy player fölött van, enemynek EX++ irányba kéne mennie, lefelé
                if (EX < PX)
                {
                    //potenciális új coord, elsőnek jobbra nézzük meg
                    int potY1 = EY;
                    while (walls.Contains(Map[EX + 1, potY1]) && potY1 < Map.GetLength(1) - 1)
                    {
                        potY1++;
                        BpY++;
                    }

                    //megnézzük balra is
                    int potY2 = EY;
                    while (walls.Contains(Map[EX + 1, potY2]) && potY2 > 0)
                    {
                        potY2--;
                        BnY++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán és nem is falba vezetné az enemy-t,
                    //azt adjuk vissza, egyébként random adjuk vissza az egyiket (olyat ami nem falba vezeti az enemy-t)
                    if (BpY < BnY && potY1 < Map.GetLength(1) - 1 && !walls.Contains(Map[EX, EY + 1]))
                    {
                        return new Coords(EX, EY + 1);
                    }
                    else if (BnY < BpY && potY2 > 0 && !walls.Contains(Map[EX, EY - 1]))
                    {
                        return new Coords(EX, EY - 1);
                    }
                    else if (BnY == BpY)
                    {
                        if (!walls.Contains(Map[EX, EY + 1]) && !walls.Contains(Map[EX, EY - 1]))
                        {
                            int randomret = rnd.Next(0, 2);
                            if (randomret == 0 && potY1 < Map.GetLength(1) - 1)
                            {
                                return new Coords(EX, EY + 1);
                            }
                            else if (potY2 > 0)
                            {
                                return new Coords(EX, EY - 1);
                            }
                        }
                    }
                    //erre az ágra akkor futunk rá, ha az egyik falba ütközne
                    else if (!walls.Contains(Map[EX, EY + 1]) && potY1 < Map.GetLength(1) - 1)
                    {
                        return new Coords(EX, EY + 1);
                    }
                    else if (!walls.Contains(Map[EX, EY - 1]) && potY2 > 0)
                    {
                        return new Coords(EX, EY - 1);
                    }

                }
                //enemy player alatt van, enemynek EX-- irányba kéne mennie, felfelé
                else if (EX > PX)
                {
                    //potenciális új coord, elsőnek jobbra nézzük meg
                    int potY1 = EY;
                    while (walls.Contains(Map[EX - 1, potY1]) && potY1 < Map.GetLength(1) - 1)
                    {
                        potY1++;
                        BpY++;
                    }

                    //megnézzük balra is
                    int potY2 = EY;
                    while (walls.Contains(Map[EX - 1, potY2]) && potY2 > 0)
                    {
                        potY2--;
                        BnY++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán és falba sem viszi az enemy-t, azt adjuk vissza, egyébként random adjuk vissza az egyiket
                    if (BpY < BnY && potY1 < Map.GetLength(1) - 1 && !walls.Contains(Map[EX, EY + 1]))
                    {
                        return new Coords(EX, EY + 1);
                    }
                    else if (BnY < BpY && potY2 > 0 && !walls.Contains(Map[EX, EY - 1]))
                    {
                        return new Coords(EX, EY - 1);
                    }
                    else if (BnY == BpY)
                    {
                        if (!walls.Contains(Map[EX, EY + 1]) && !walls.Contains(Map[EX, EY - 1]))
                        {
                            int randomret = rnd.Next(0, 2);
                            if (randomret == 0 && potY1 < Map.GetLength(1) - 1)
                            {
                                return new Coords(EX, EY + 1);
                            }
                            else if (potY2 > 0)
                            {
                                return new Coords(EX, EY - 1);
                            }
                        }
                    }
                    //ha az egyik falba vezetné az enemy-t
                    else if (!walls.Contains(Map[EX, EY + 1]) && potY1 < Map.GetLength(1) - 1)
                    {
                        return new Coords(EX, EY + 1);
                    }
                    else if (!walls.Contains(Map[EX, EY - 1]) && potY2 > 0)
                    {
                        return new Coords(EX, EY - 1);
                    }

                }
            }
            //egy oszlopban ellenőrzés vége

            //egy sorban vannak
            if (EX == PX)
            {
                //enemy playertől balra van, enemynek EY++ irányba kéne mennie, jobbra
                if (EY < PY)
                {
                    //potenciális új coord, elsőnek lefelé nézzük meg
                    int potX1 = EX;
                    while (walls.Contains(Map[potX1, EY + 1]) && potX1 < Map.GetLength(0) - 1)
                    {
                        potX1++;
                        BpX++;
                    }

                    //megnézzük felfelé is
                    int potX2 = EX;
                    while (walls.Contains(Map[potX2, EY + 1]) && potX2 > 0)
                    {
                        potX2--;
                        BnX++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán és falba sem vezet azt adjuk vissza, egyébként random adjuk vissza az egyiket
                    if (BpX < BnX && potX1 < Map.GetLength(0) - 1 && !walls.Contains(Map[EX + 1, EY]))
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (BnX < BpX && potX2 > 0 && !walls.Contains(Map[EX - 1, EY]))
                    {
                        return new Coords(EX - 1, EY);
                    }
                    else if (BnX == BpX)
                    {
                        if (!walls.Contains(Map[EX + 1, EY]) && !walls.Contains(Map[EX - 1, EY]))
                        {
                            int randomret = rnd.Next(0, 2);
                            if (randomret == 0 && potX1 < Map.GetLength(0) - 1)
                            {
                                return new Coords(EX + 1, EY);
                            }
                            else if (potX2 > 0)
                            {
                                return new Coords(EX - 1, EY);
                            }
                        }
                    }
                    //ha az egyik falba vezetné
                    else if (!walls.Contains(Map[EX + 1, EY]) && potX1 < Map.GetLength(0) - 1)
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (!walls.Contains(Map[EX - 1, EY]) && potX2 > 0)
                    {
                        return new Coords(EX - 1, EY);
                    }

                }
                //enemy playertől jobbra van, enemynek EY-- irányba kéne mennie, balra
                else if (EY > PY)
                {
                    //potenciális új coord, elsőnek lefelé nézzük meg
                    int potX1 = EX;
                    while (walls.Contains(Map[potX1, EY - 1]) && potX1 < Map.GetLength(1) - 1)
                    {
                        potX1++;
                        BpX++;
                    }

                    //megnézzük felfelé is
                    int potX2 = EX;
                    while (walls.Contains(Map[potX2, EY - 1]) && potX2 > 0)
                    {
                        potX2--;
                        BnX++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán és falba se vezeti azt adjuk vissza, egyébként random adjuk vissza az egyiket
                    if (BpX < BnX && potX1 < Map.GetLength(0) - 1 && !walls.Contains(Map[EX + 1, EY]))
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (BnX < BpX && potX2 > 0 && !walls.Contains(Map[EX - 1, EY]))
                    {
                        return new Coords(EX - 1, EY);
                    }
                    else if (BnX == BpX)
                    {
                        if (!walls.Contains(Map[EX + 1, EY]) && !walls.Contains(Map[EX - 1, EY]))
                        {
                            int randomret = rnd.Next(0, 2);
                            if (randomret == 0 && potX1 < Map.GetLength(0) - 1)
                            {
                                return new Coords(EX + 1, EY);
                            }
                            else if (potX2 > 0)
                            {
                                return new Coords(EX - 1, EY);
                            }
                        }
                    }
                    //ha az egyik falba vezeti
                    else if (!walls.Contains(Map[EX + 1, EY]) && potX1 < Map.GetLength(0) - 1)
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (!walls.Contains(Map[EX - 1, EY]) && potX2 > 0)
                    {
                        return new Coords(EX - 1, EY);
                    }

                }
            }
            //egy sorban ellenőrzés vége

            //ÁTLÓSAN FALBA ÜTKÖZÉS:
            //enemy playertől balra és (...) van
            if (EY < PY)
            {
                //enemy playertől balra és felfelé van, EX++ és EY++ irányba kéne mennie, jobbra lefelé
                if (EX < PX)
                {
                    //potenciális új coord, elsőnek lefelé nézzük meg
                    int potX1 = EX;
                    while (walls.Contains(Map[potX1, EY + 1]) && potX1 < Map.GetLength(0) - 1)
                    {
                        potX1++;
                        BpX++;
                    }

                    //megnézzük jobbra is
                    int potY2 = EY;
                    while (walls.Contains(Map[EX + 1, potY2]) && potY2 < Map.GetLength(1) - 1)
                    {
                        potY2++;
                        BpY++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán és falba se vezeti azt adjuk vissza, egyébként random adjuk vissza az egyiket
                    if (BpX < BpY && potX1 < Map.GetLength(0) - 1 && !walls.Contains(Map[EX + 1, EY]))
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (BpY < BpX && potY2 < Map.GetLength(1) - 1 && !walls.Contains(Map[EX, EY + 1]))
                    {
                        return new Coords(EX, EY + 1);
                    }
                    else if (BpX == BpY)
                    {
                        if (!walls.Contains(Map[EX + 1, EY]) && !walls.Contains(Map[EX, EY + 1]))
                        {
                            int randomret = rnd.Next(0, 2);
                            if (randomret == 0 && potX1 < Map.GetLength(0) - 1)
                            {
                                return new Coords(EX + 1, EY);
                            }
                            else if (potY2 < Map.GetLength(1) - 1)
                            {
                                return new Coords(EX, EY + 1);
                            }
                        }
                    }
                    //ha az egyik falba vezeti
                    else if (!walls.Contains(Map[EX + 1, EY]) && potX1 < Map.GetLength(0) - 1)
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (!walls.Contains(Map[EX, EY + 1]) && potY2 < Map.GetLength(1) - 1)
                    {
                        return new Coords(EX, EY + 1);
                    }
                    //ha mindkettő falba vezetné és mozdulatlan maradna
                    /*
                    else if (!walls.Contains(Map[EX - 1,EY]) && (EX - 1 > 0) && walls.Contains(Map[EX, EY - 1]))
                    {
                        return new Coords(EX - 1, EY);
                    }
                    
                    else if (!walls.Contains(Map[EX, EY - 1]) && (EY - 1 > 0) && walls.Contains(Map[EX - 1, EY]))
                    {
                        return new Coords(EX, EY - 1);
                    }
                    else if(!walls.Contains(Map[EX - 1, EY + 1]) && walls.Contains(Map[EX + 1, EY - 1]))
                    {
                        return new Coords(EX - 1,EY + 1);
                    }
                    else if (walls.Contains(Map[EX - 1, EY + 1]) && !walls.Contains(Map[EX + 1, EY - 1]))
                    {
                        return new Coords(EX + 1, EY - 1);
                    }
                    */


                }
                //enemy playertől balra és lefelé van, EX-- és EY++ irányba kéne mennie, jobbra felfelé
                else if (EX > PX)
                {
                    //potenciális új coord, elsőnek jobbra nézzük meg
                    int potY1 = EY;
                    while (walls.Contains(Map[EX - 1, potY1]) && potY1 < Map.GetLength(1) - 1)
                    {
                        potY1++;
                        BpY++;
                    }

                    //megnézzük felfelé is
                    int potX2 = EX;
                    while (walls.Contains(Map[potX2, EY + 1]) && potX2 > 0)
                    {
                        potX2--;
                        BnX++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán és falba se vezeti azt adjuk vissza, egyébként random adjuk vissza az egyiket
                    if (BpY < BnX && potY1 < Map.GetLength(1) - 1 && !walls.Contains(Map[EX, EY + 1]))
                    {
                        return new Coords(EX, EY + 1);
                    }
                    else if (BnX < BpY && potX2 > 0 && !walls.Contains(Map[EX - 1, EY]))
                    {
                        return new Coords(EX - 1, EY);
                    }
                    else if (BpY == BnX)
                    {
                        if (!walls.Contains(Map[EX, EY + 1]) && !walls.Contains(Map[EX - 1, EY]))
                        {
                            int randomret = rnd.Next(0, 2);
                            if (randomret == 0 && potY1 < Map.GetLength(1) - 1)
                            {
                                return new Coords(EX, EY + 1);
                            }
                            else if (potX2 > 0)
                            {
                                return new Coords(EX - 1, EY);
                            }
                        }
                    }
                    //ha az egyik falba vezeti
                    else if (!walls.Contains(Map[EX, EY + 1]) && potY1 < Map.GetLength(1) - 1)
                    {
                        return new Coords(EX, EY + 1);
                    }
                    else if (!walls.Contains(Map[EX - 1, EY]) && potX2 > 0)
                    {
                        return new Coords(EX - 1, EY);
                    }
                }
            }

            //enemy playertől jobbra és (...) van
            if (EY > PY)
            {
                //enemy playertől jobbra és felfelé van tehát balra és lefelé fogjuk ellenőrizni
                if (EX < PX)
                {
                    //potenciális új coord, elsőnek lefelé nézzük meg
                    int potX1 = EX;
                    while (walls.Contains(Map[potX1, EY - 1]) && potX1 < Map.GetLength(0) - 1)
                    {
                        potX1++;
                        BpX++;
                    }

                    //megnézzük balra is
                    int potY2 = EY;
                    while (walls.Contains(Map[EX + 1, potY2]) && potY2 > 0)
                    {
                        potY2--;
                        BpY++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán és falba se vezeti azt adjuk vissza, egyébként random adjuk vissza az egyiket
                    if (BpX < BpY && potX1 < Map.GetLength(0) - 1 && !walls.Contains(Map[EX + 1, EY]))
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (BpY < BpX && potY2 > 0 && !walls.Contains(Map[EX, EY - 1]))
                    {
                        return new Coords(EX, EY - 1);
                    }
                    else if (BpX == BpY)
                    {
                        if (!walls.Contains(Map[EX + 1, EY]) && !walls.Contains(Map[EX, EY - 1]))
                        {
                            int randomret = rnd.Next(0, 2);
                            if (randomret == 0 && potX1 < Map.GetLength(0) - 1)
                            {
                                return new Coords(EX + 1, EY);
                            }
                            else if (potY2 > 0)
                            {
                                return new Coords(EX, EY - 1);
                            }
                        }
                    }
                    //ha az egyik falba vezeti
                    else if (!walls.Contains(Map[EX + 1, EY]) && potX1 < Map.GetLength(0) - 1)
                    {
                        return new Coords(EX + 1, EY);
                    }
                    else if (!walls.Contains(Map[EX, EY - 1]) && potY2 > 0)
                    {
                        return new Coords(EX, EY - 1);
                    }
                    //Ha meg kéne kerülnie a falat
                    /*
                    else if (!walls.Contains(Map[EX,EY+1]) && EY < Map.GetLength(1) - 1)
                    {
                        return new Coords(EX, EY + 1);
                    }*/

                }
                //enemy playertől jobbra és lefelé van, EX-- és EY-- irányba kéne mennie, balra felfelé
                else if (EX > PX)
                {
                    //potenciális új coord, elsőnek balra nézzük meg
                    int potY1 = EY;
                    while (walls.Contains(Map[EX - 1, potY1]) && potY1 > 0)
                    {
                        potY1--;
                        BnY++;
                    }

                    //megnézzük felfelé is
                    int potX2 = EX;
                    while (walls.Contains(Map[potX2, EY - 1]) && potX2 > 0)
                    {
                        potX2--;
                        BnX++;
                    }

                    //amelyik jóságszámláló kisebb a másiknál és bent is van a pályán, azt adjuk vissza, egyébként random adjuk vissza az egyiket
                    if (BnY < BnX && potY1 > 0 && !walls.Contains(Map[EX, EY - 1]))
                    {
                        return new Coords(EX, EY - 1);
                    }
                    else if (BnX < BnY && potX2 > 0 && !walls.Contains(Map[EX - 1, EY]))
                    {
                        return new Coords(EX - 1, EY);
                    }
                    else if (BnY == BnX)
                    {
                        int randomret = rnd.Next(0, 2);
                        if (randomret == 0 && potY1 > 0)
                        {
                            //RETURN JOBBRA MENŐT
                            return new Coords(EX, EY - 1);
                        }
                        else if (potX2 > 0)
                        {
                            //RETURN BALRA MENŐT
                            return new Coords(EX - 1, EY);
                        }
                    }
                    else if (!walls.Contains(Map[EX, EY - 1]) && potY1 > 0)
                    {
                        return new Coords(EX, EY - 1);
                    }
                    else if (!walls.Contains(Map[EX - 1, EY]) && potX2 > 0)
                    {
                        return new Coords(EX - 1, EY);
                    }

                }
            }

            //ha egy konkáv alakú falba ütközik, hogy ne ragadjon be, próbáljon meg hátrálni

            return new Coords(EX, EY);
        }

        // nem fordul elő, hogy éppenhogy belemozog amior átlósan akarna menni???
        private void NormalEnemyStep(NormalEnemy enemy)
        {
            int oldposX = enemy.Position.X;
            int oldposY = enemy.Position.Y;
            int posX = enemy.Position.X;
            int posY = enemy.Position.Y;

            if (enemy.Position.X == this.Player.Position.X)
            {
                if (enemy.Position.Y == this.Player.Position.Y)
                {
                    // rajta áll
                }
                else if (enemy.Position.Y < this.Player.Position.Y)
                {
                    posY++;
                    enemy.Direction = Direction.Right;
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posY--;
                    enemy.Direction = Direction.Left;
                }
            }
            else if (enemy.Position.X < this.Player.Position.X)
            {
                if (enemy.Position.Y == this.Player.Position.Y)
                {
                    posX++;

                }
                else if (enemy.Position.Y < this.Player.Position.Y)
                {
                    posX++;
                    posY++;
                    enemy.Direction = Direction.Right;
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX++;
                    posY--;
                    enemy.Direction = Direction.Left;
                }
            }
            else if (enemy.Position.X > this.Player.Position.X)
            {
                if (enemy.Position.Y == this.Player.Position.Y)
                {
                    posX--;
                }
                else if (enemy.Position.Y < this.Player.Position.Y)
                {
                    posX--;
                    posY++;
                    enemy.Direction = Direction.Right;
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX--;
                    posY--;
                    enemy.Direction = Direction.Left;
                }
            }

            //test function
            //enemy.Health--;
            //if(enemy.Health <= 15) 
            //{
            //    ;
            //}

            Direction direction = Direction.Left;
            bool dodgeNeeded = false;

            foreach (Spell spell in Spells)
            {
                if ((spell.Position.X == posX && spell.Position.Y == posY) ||
                   (spell.Position.X == posX + 1 && spell.Position.Y == posY) ||
                   (spell.Position.X == posX - 1 && spell.Position.Y == posY) ||
                   (spell.Position.X == posX && spell.Position.Y == posY + 1) ||
                   (spell.Position.X == posX && spell.Position.Y == posY - 1))
                {
                    direction = spell.Direction;
                    dodgeNeeded = true;
                    break;
                }
            }

            if (dodgeNeeded && enemy.canDodge()) //5 másodpercenként dodgeolhat
            {
                switch (direction)
                {
                    case Direction.Left:
                        if (!CollidedWithObject(oldposX + 1, oldposY) &&
                            Map[oldposX + 1, oldposY] != GameItem.Enemy &&
                            Map[oldposX + 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX + 1;
                            posY = oldposY;
                        }
                        else if (!CollidedWithObject(oldposX - 1, oldposY) &&
                                 Map[oldposX - 1, oldposY] != GameItem.Enemy &&
                                 Map[oldposX - 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX - 1;
                            posY = oldposY;
                        }
                        break;
                    case Direction.Right:
                        if (!CollidedWithObject(oldposX + 1, oldposY) &&
                            Map[oldposX + 1, oldposY] != GameItem.Enemy &&
                            Map[oldposX + 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX + 1;
                            posY = oldposY;
                        }
                        else if (!CollidedWithObject(oldposX - 1, oldposY) &&
                                 Map[oldposX - 1, oldposY] != GameItem.Enemy &&
                                 Map[oldposX - 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX - 1;
                            posY = oldposY;
                        }
                        break;
                    case Direction.Up:
                        if (!CollidedWithObject(oldposX, oldposY + 1) &&
                            Map[oldposX, oldposY + 1] != GameItem.Enemy &&
                            Map[oldposX, oldposY + 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY + 1;
                        }
                        else if (!CollidedWithObject(oldposX, oldposY - 1) &&
                                 Map[oldposX, oldposY - 1] != GameItem.Enemy &&
                                 Map[oldposX, oldposY - 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY - 1;
                        }
                        break;
                    case Direction.Down:
                        if (!CollidedWithObject(oldposX, oldposY + 1) &&
                            Map[oldposX, oldposY + 1] != GameItem.Enemy &&
                            Map[oldposX, oldposY + 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY + 1;
                        }
                        else if (!CollidedWithObject(oldposX, oldposY - 1) &&
                                 Map[oldposX, oldposY - 1] != GameItem.Enemy &&
                                 Map[oldposX, oldposY - 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY - 1;
                        }
                        break;
                    default:
                        break;
                }
            }

            #region not totally working
            /*
            if (Map[posX,posY] == GameItem.Spell || Map[oldposX, oldposY] == GameItem.Spell)
            {
                ;
                Direction direction = Direction.Left;

                foreach (Spell spell in Spells)
                {
                    if((spell.Position.X == posX && spell.Position.Y == posY) ||
                       (spell.Position.X == oldposX && spell.Position.Y == oldposY))
                    {
                        direction = spell.Direction;
                        break;
                    }
                }

                switch (direction)
                {
                    case Direction.Left:
                        if (Map[oldposX + 1, oldposY] != GameItem.Wall &&
                            Map[oldposX + 1, oldposY] != GameItem.Enemy &&
                            Map[oldposX + 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX + 1;
                            posY = oldposY;
                        }
                        else if (Map[oldposX - 1, oldposY] != GameItem.Wall &&
                                 Map[oldposX - 1, oldposY] != GameItem.Enemy &&
                                 Map[oldposX - 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX - 1;
                            posY = oldposY;
                        }
                        break;
                    case Direction.Right:
                        if (Map[oldposX + 1, oldposY] != GameItem.Wall &&
                            Map[oldposX + 1, oldposY] != GameItem.Enemy &&
                            Map[oldposX + 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX + 1;
                            posY = oldposY;
                        }
                        else if (Map[oldposX - 1, oldposY] != GameItem.Wall &&
                                 Map[oldposX - 1, oldposY] != GameItem.Enemy &&
                                 Map[oldposX - 1, oldposY] != GameItem.Player)
                        {
                            posX = oldposX - 1;
                            posY = oldposY;
                        }
                        break;
                    case Direction.Up:
                        if (Map[oldposX, oldposY + 1] != GameItem.Wall &&
                            Map[oldposX, oldposY + 1] != GameItem.Enemy &&
                            Map[oldposX, oldposY + 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY + 1;
                        }
                        else if (Map[oldposX, oldposY - 1] != GameItem.Wall &&
                                 Map[oldposX, oldposY - 1] != GameItem.Enemy &&
                                 Map[oldposX, oldposY - 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY - 1;
                        }
                        break;
                    case Direction.Down:
                        if (Map[oldposX, oldposY + 1] != GameItem.Wall &&
                            Map[oldposX, oldposY + 1] != GameItem.Enemy &&
                            Map[oldposX, oldposY + 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY + 1;
                        }
                        else if (Map[oldposX, oldposY - 1] != GameItem.Wall &&
                                 Map[oldposX, oldposY - 1] != GameItem.Enemy &&
                                 Map[oldposX, oldposY - 1] != GameItem.Player)
                        {
                            posX = oldposX;
                            posY = oldposY - 1;
                        }
                        break;
                    default:
                        break;
                }
            }
            */
            #endregion

            if (!CollidedWithObject(posX, posY) && Map[posX, posY] != GameItem.Enemy && Map[posX, posY] != GameItem.Player)
            {
                Map[oldposX, oldposY] = GameItem.Floor;

                enemy.Position.X = posX;
                enemy.Position.Y = posY;

                Map[posX, posY] = GameItem.Enemy;
            }
            else if (Map[posX,posY] == GameItem.Player)
            {
                Player.Injure(20);
            }
            else if (CollidedWithObject(posX, posY) && Map[posX, posY] != GameItem.Enemy && Map[posX, posY] != GameItem.Player)
            {

                Coords forBypass = EnemyBypassWall(new Coords(oldposX, oldposY));

                Map[oldposX, oldposY] = GameItem.Floor;

                enemy.Position.X = forBypass.X;
                enemy.Position.Y = forBypass.Y;

                Map[forBypass.X, forBypass.Y] = GameItem.Enemy;

            }
        }
    }
}
