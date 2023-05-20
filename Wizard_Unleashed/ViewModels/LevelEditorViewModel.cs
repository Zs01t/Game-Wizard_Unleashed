using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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

        //public LevelEditorDisplay LvlDisplay { get; set; }



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

        public ICommand SaveMapCommand { get; set; }

        public LevelEditorViewModel()
        {
            //LvlDisplay = new LevelEditorDisplay();
            Tiles = new ObservableCollection<Tile>();
            
            //StringMap = new string[16,16];

            Tiles.Add(new Tile("wall"));
            Tiles.Add(new Tile("floor"));
            Tiles.Add(new Tile("wizard"));
            Tiles.Add(new Tile("slime"));


            //SaveMapCommand = new RelayCommand(
            //    () =>
            //    () =>
            //    );


        }

        

       


    }
}
