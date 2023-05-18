using Models;
using Models.Enemies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic
{
    public class GameLogic : IGameLogic
    {
        public event EventHandler GameStateChanged;

        private static Random rnd = new Random();
        public GameItem[,] Map { get; private set; }
        public Player Player { get; set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Spell> Spells { get; private set; }

        private Queue<string> levels;

        public GameLogic(Player player)
        {
            this.Player = player;
            Spells = new List<Spell>();
            Enemies = new List<Enemy>();
            levels = new Queue<string>();

            var lvlFiles = Directory.GetFiles("Levels",
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

            this.GameStateChanged.Invoke(this, null);
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
                        NormalEnemyStep(enemy);
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
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posY--;
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
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX++;
                    posY--;
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
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX--;
                    posY--;
                }
            }

            if (Map[posX, posY] != GameItem.Wall && Map[posX, posY] != GameItem.Enemy && Map[posX, posY] != GameItem.Player)
            {
                Map[oldposX, oldposY] = GameItem.Floor;

                enemy.Position.X = posX;
                enemy.Position.Y = posY;

                Map[posX, posY] = GameItem.Enemy;
            }
        }

        // nem fordul elő, hogy éppenhogy belemozog amior átlósan akarna menni???
        private void NormalEnemyStep(Enemy enemy)
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
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posY--;
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
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX++;
                    posY--;
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
                }
                else if (enemy.Position.Y > this.Player.Position.Y)
                {
                    posX--;
                    posY--;
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

            if(dodgeNeeded)
            {
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

            if (Map[posX, posY] != GameItem.Wall && Map[posX, posY] != GameItem.Enemy && Map[posX, posY] != GameItem.Player)
            {
                Map[oldposX, oldposY] = GameItem.Floor;

                enemy.Position.X = posX;
                enemy.Position.Y = posY;

                Map[posX, posY] = GameItem.Enemy;
            }
        }
    }
}
