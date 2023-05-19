using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Wizard_Unleashed;

namespace Wizard_Unleashed.ViewModels
{
    public class LevelEditorViewModel
    {
        public ObservableCollection<Tile> Tiles { get; set; }

        public LevelEditorViewModel()
        {
            Tiles = new ObservableCollection<Tile>();
            Tiles.Add(new Tile("wall"));
            Tiles.Add(new Tile("floor"));
            Tiles.Add(new Tile("wizard"));
            Tiles.Add(new Tile("slime"));
        }

    }
}
