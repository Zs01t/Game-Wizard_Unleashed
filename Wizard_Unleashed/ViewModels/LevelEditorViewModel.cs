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
            //a logicnak ezt nem kell átadni, mert ő speciel nem kezd ezzel semmit
            Tiles = new ObservableCollection<Tile>();
            //ez már "szebb" ( =dinamikusabb...)
            //sajnos ennek ellenére új tile felvételekor fel kell venni az enumok közé, ami már nem dinamikus


            string tilesPath = Directory.GetCurrentDirectory().Replace("Wizard_Unleashed\\bin\\Debug\\net5.0-windows", "Wizard_Unleashed\\Assets\\Tiles");
            string[] tilesPngs = Directory.GetFiles(tilesPath, "*.png");
            foreach (var png in tilesPngs)
            {
                string tmpString = png.Replace(".png", "").Replace(tilesPath + "\\", "");
                GameItem tmp = (GameItem) Enum.Parse(typeof(GameItem), tmpString, true);
                Tiles.Add(new Tile(tmp));
            }

            //Tiles.Add(new Tile(GameItem.Wall));
            //Tiles.Add(new Tile(GameItem.Floor));
            //Tiles.Add(new Tile(GameItem.Enemy));
            //Tiles.Add(new Tile(GameItem.Player));
            //Tiles.Add(new Tile(GameItem.Door));
            //Tiles.Add(new Tile(GameItem.IronBar));
            //Tiles.Add(new Tile(GameItem.IronBarTop));
            //Tiles.Add(new Tile(GameItem.Void));

            //Tiles.Add(new Tile(GameItem.UnderLeftCornerWall));
            //Tiles.Add(new Tile(GameItem.UnderWall));
            //Tiles.Add(new Tile(GameItem.UnderRightCornerWall));
            //Tiles.Add(new Tile(GameItem.LeftSideWall));
            //Tiles.Add(new Tile(GameItem.MiddleWall));
            //Tiles.Add(new Tile(GameItem.RightSideWall));
            //Tiles.Add(new Tile(GameItem.UpperCornerLeftWall));
            //Tiles.Add(new Tile(GameItem.UpperWall));
            //Tiles.Add(new Tile(GameItem.UpperCornerRightWall));

            //Tiles.Add(new Tile(GameItem.LeftLowerCornerPiece));
            //Tiles.Add(new Tile(GameItem.LeftUpperCornerPiece));
            //Tiles.Add(new Tile(GameItem.RightLowerCornerPiece));
            //Tiles.Add(new Tile(GameItem.RightUpperCornerPiece));
            //;


            
            
            //Commandok betöltése
            SaveMapCommand = new RelayCommand(
                () => logic.SaveMap()
                );


        }



    }
}
