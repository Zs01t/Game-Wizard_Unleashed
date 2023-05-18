using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enemies
{
    public abstract class Enemy : Entity
    {
        protected Enemy(Coords position, int health) : base(position, health) { }
        //{
        //    Position = position;
        //    Health = health;
        //}

        //public Coords Position { get; set; }
        //// 0-100
        //public int Health { get; set; }
        public abstract void Move();
        //public Direction Direction { get; set; }
    }
}
