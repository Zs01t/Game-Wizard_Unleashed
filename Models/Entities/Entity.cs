using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public Coords Position { get; set; }
        public Direction Direction { get; set; }
        
        private int health;

        public Entity(Coords position,  int health, Direction direction = Direction.Left)
        {
            Position = position;
            this.Health = health;
            Direction = direction;
        }

        public Entity()
        {
            Position = new Coords(0, 0);
        }

        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public int Health
        {
            get
            {
                return this.health;
            }

            set
            {
                this.health = value;
                this.OnPropertyChanged(nameof(this.health));
            }
        }
    }
}
