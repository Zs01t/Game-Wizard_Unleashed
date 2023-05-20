using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Player : Entity
    {

        public Player(Coords Position , int Health, Direction direction) : base(Position, Health, direction) 
        { 
        
        
        }
        public Player()
        {

        }


    }
}
