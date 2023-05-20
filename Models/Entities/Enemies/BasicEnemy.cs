using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enemies
{
    public class BasicEnemy : Enemy
    {
        public BasicEnemy(Coords position, int health, Direction direction = Direction.Left) : base(position, health, direction)
        {
        }

        public override void Move()
        {
            throw new NotImplementedException();
        }
    }
}
