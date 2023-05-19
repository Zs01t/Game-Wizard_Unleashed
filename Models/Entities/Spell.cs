using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Spell
    {
        public Spell(Coords position, Direction direction)
        {
            Position = position;
            Direction = direction;
        }

        public Coords Position { get; set; }

        public Direction Direction { get; set; }
    }
}
