using Models;
using System;

namespace Logic
{
    public interface IGameLogic
    {
        event EventHandler GameStateChanged;
        GameItem[,] Map { get; }
        Player Player { get; set; }
        void Control(Direction direction);
        void LoadNext(string path);
        void TimeStep();
        void CastSpell();
    }
}