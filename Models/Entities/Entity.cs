using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public abstract class Entity
    {
        public Coords Position { get; set; }
        public Direction Direction { get; set; }
        public int Health { get; set; }

        public Entity(Coords position,  int health)
        {
            Position = position;
            Health = health;
        }

        public Entity()
        {
            Position = new Coords(0, 0);
        }
    }
}
