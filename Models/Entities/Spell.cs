using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Spell : Entity
    {

        public bool IsImageAssigned { get; set; }
        public Spell(Coords position, Direction direction) : base(position, 0, direction)
        {
            IsImageAssigned = false;
        }

    }
}
