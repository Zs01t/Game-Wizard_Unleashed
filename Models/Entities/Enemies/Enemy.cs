using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enemies
{
    public abstract class Enemy : Entity
    {
        protected Enemy(Coords position, int health, Direction direction = Direction.Left) : base(position, health, direction) { }


        //public Coords Position { get; set; }
        //// 0-100
        //public int Health { get; set; }
        public abstract void Move();
        //public Direction Direction { get; set; }
    }
}
