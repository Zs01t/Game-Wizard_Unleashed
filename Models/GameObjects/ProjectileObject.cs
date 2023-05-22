using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Models.GameObjects
{
    public class ProjectileObject
    {
        private BitmapImage[,] projectileImages;

        public BitmapImage CurrentProjectileImage;
        public Entity Entity;

        public ProjectileObject(Entity Entity, string path)
        {
            this.Entity = Entity;
            ImportImages(path);
        }

        private void ImportImages(string path)
        {

            //pl. a path= "Assets\\Fireball"
            //Annak megnézem a Down mappájának elemszámát, feltételezem, hogy minden irányban ugyanannyi az elemszám
            int imageCount = Directory.GetFiles(Path.Combine(path, "Down"), "*", SearchOption.TopDirectoryOnly).Length;


            //létrehozom a képeket tároló mátrixokat, 4 sorral, ami a bal, jobb, lent és fent irányú képeket tárolja
            projectileImages = new BitmapImage[imageCount, imageCount];

            //betöltöm az IdleImageket
            for (int i = 0; i < imageCount; i++)
            {
                string leftImageName = "spell_left_0" + (i + 1) + ".png";
                string rightImageName = "spell_right_0" + (i + 1) + ".png";
                string downImageName = "spell_down_0" + (i + 1) + ".png";
                string upImageName = "spell_up_0" + (i + 1) + ".png";

                //úgy határoztam, hogy a 0. sorba kerülnek a bal irányba néző képek az 1. sorba a jobb irányba néző képek
                //a 2. sorba a lelfele néző képek és a 3. sorban a felfele néző képek
                
                projectileImages[0, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Left", leftImageName)), UriKind.RelativeOrAbsolute));
                projectileImages[1, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Right", rightImageName)), UriKind.RelativeOrAbsolute));
                projectileImages[2, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Down", downImageName)), UriKind.RelativeOrAbsolute));
                projectileImages[3, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Up", upImageName)), UriKind.RelativeOrAbsolute));
            }

            //beállíítom az alapképállapotot
            //ChangeCurrentProjectileImage();
        }

        private int leftState = 0;
        private int rightState = 0;
        private int upState = 0;
        private int downState = 0;
        public void ChangeCurrentProjectileImage()
        {
            //A Player jelenlegi iránya alapján meghatározom, hogy melyik WalkImage-t töltse be
            //A GameDisplay osztályban majd ez a függvény úgy lesz meghívva, hogy végigmenjen az adott irányú sétálás képek mindegyikén, tehát jelen esetben 7-szer
            //ehhez persze majd kell egy Dispatcher
            switch (Entity.Direction)
            {
                case Direction.Left:
                    leftState++;
                    if (leftState > projectileImages.GetLength(1) - 1)
                    {
                        leftState = 0;
                    }
                    CurrentProjectileImage = projectileImages[0, leftState];
                    break;

                case Direction.Right:
                    rightState++;
                    if (rightState > projectileImages.GetLength(1) - 1)
                    {
                        rightState = 0;
                    }
                    CurrentProjectileImage = projectileImages[1, rightState];
                    break;

                case Direction.Up:
                    upState++;
                    if (upState > projectileImages.GetLength(1) - 1)
                    {
                        upState = 0;
                    }
                    CurrentProjectileImage = projectileImages[2, upState];
                    break;

                case Direction.Down:
                    downState++;
                    if (downState > projectileImages.GetLength(1) - 1)
                    {
                        downState = 0;
                    }
                    CurrentProjectileImage = projectileImages[3, downState];
                    break;
            }

        }
    }
}
