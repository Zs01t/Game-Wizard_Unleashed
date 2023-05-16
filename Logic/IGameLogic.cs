using Models;

namespace Logic
{
    public interface IGameLogic
    {
        GameItem[,] Map { get; }
        Player Player { get; set; }
        void Control(Direction direction);
        void LoadNext(string path);
        void TimeStep();
        void CastSpell(Direction direction);
    }
}