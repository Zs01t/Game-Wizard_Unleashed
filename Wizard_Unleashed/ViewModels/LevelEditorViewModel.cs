using Logic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Wizard_Unleashed;

namespace Wizard_Unleashed.ViewModels
{
    public class LevelEditorViewModel : ObservableRecipient
    {
        public ObservableCollection<Tile> Tiles { get; set; }

        public ILevelEditorLogic logic { get; set; }

        private Tile selectedTile;
        public Tile SelectedTile 
        {
            get
            {
                return selectedTile;
            }
            set
            { 
               SetProperty(ref selectedTile, value);
            }
        }
        public void SetUpLogic(ILevelEditorLogic logic)
        { 
            this.logic= logic;
        }

        public ICommand SaveMapCommand { get; set; }

        public LevelEditorViewModel()
        {
            Tiles = new ObservableCollection<Tile>();

            Tiles.Add(new Tile("wall"));
            Tiles.Add(new Tile("middleWall"));
            Tiles.Add(new Tile("upperWall"));
            Tiles.Add(new Tile("floor"));
            Tiles.Add(new Tile("wizard"));
            Tiles.Add(new Tile("slime"));



            SaveMapCommand = new RelayCommand(
                () => logic.SaveMap()
                );


        }

    }
}
