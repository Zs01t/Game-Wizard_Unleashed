using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Player
    {
        public int Health;

        public Coords Position { get; set; }

        public Player()
        {
            Position = new Coords(0, 0);
        }
    }
}
