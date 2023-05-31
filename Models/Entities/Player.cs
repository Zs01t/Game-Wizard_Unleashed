using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Player : Entity
    {

        public Player(Coords Position , int Health, Direction direction) : base(Position, Health, direction)
        {
            timeSinceLastSpell = 1;

        }
        public Player()
        {
            timeSinceLastSpell = 1;
            this.Health = 100; //!!!
        }

        public void Injure(int dmg)
        {
            //kell bele > 0 ellenőrzés?
            Health -= dmg;
        }

        public double timeSinceLastSpell { get; set; }

        public bool canCast()
        {
            if(timeSinceLastSpell > 1)
            {
                timeSinceLastSpell = 0;
                return true;
            }
            return false;
        }
    }
}
