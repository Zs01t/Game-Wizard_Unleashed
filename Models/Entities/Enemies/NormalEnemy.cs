using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enemies
{
    public class NormalEnemy : Enemy
    {
        public NormalEnemy(Coords position, int health, Direction direction = Direction.Left) : base(position, health, direction)
        {
            timeSinceLastDodge = 0;
        }

        public override void Move()
        {
            throw new NotImplementedException();
        }

        public double timeSinceLastDodge { get; set; }

        public bool canDodge()
        {
            if(timeSinceLastDodge > 5) //5 másodpercenként dodgeolhat
            {
                timeSinceLastDodge = 0;
                return true;
            }
            return false;
        }
    }
}
