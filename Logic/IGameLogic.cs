using Models;
using Models.Enemies;
using System;
using System.Collections.Generic;

namespace Logic
{
    public interface IGameLogic
    {
        event EventHandler GameStateChanged;
        public event EventHandler NoMoreLevel;
        event EventHandler PlayerDead;
        GameItem[,] Map { get; }
        Player Player { get; set; }

        public List<Enemy> Enemies { get;  set; }

        public List<Spell> Spells { get; set; }
        void Control(Direction direction);
        void LoadNext(string path);
        void TimeStep();
        void CastSpell();
        public bool mapChanged { get; set; }
    }
}