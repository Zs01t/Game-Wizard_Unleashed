using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wizard_Unleashed
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
        public Tile(string name)
        {
            Name = name;
            Image = new BitmapImage(new Uri(Path.Combine(Path.Combine("Assets", "Tiles"), name+".png"), UriKind.RelativeOrAbsolute));
        }
    }
}
