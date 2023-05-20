using Models;
using System.Windows;

namespace Logic
{
    public interface ILevelEditorLogic
    {
        string[,] StringMap { get; set; }
        Tile[,] TileMap { get; set; }

        void PlaceItemIntoGrid(double mousePosX, double mousePosY, Tile SelectedTile);
        void SetupSizes(Size size);

        int sizeOfMap { get; set; }

        Size size { get; set; }

        void SaveMap();
    }
}