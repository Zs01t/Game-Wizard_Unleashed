using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Models
{
    public class Tile : ObservableObject
    {
        private string name;
        public string Name 
        {
            get
            { 
                return name;
            }
            set
            { 
                SetProperty(ref name, value);
            }
        
        }

        private BitmapImage image;
        public BitmapImage Image 
        {
            get 
            { 
                return image;
            }
            set
            { 
                SetProperty(ref image, value);
            }
        }

        private GameItem tileType;
        public GameItem TileType
        {
            get
            {
                return tileType;
            }
            set
            { 
                SetProperty(ref tileType, value);
            }
        
        }

        public Tile(GameItem tileType)
        {
            Name = tileType.ToString();
            Name[0].ToString().ToLower();
            Image = new BitmapImage(new Uri(Path.Combine(Path.Combine("Assets", "Tiles"), name+".png"), UriKind.RelativeOrAbsolute));
            this.tileType = tileType;
        }
    }
}
