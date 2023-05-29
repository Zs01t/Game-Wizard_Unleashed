
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Models
{
    public class EntityObject
    {

        private BitmapImage[,] idleImages;
        private BitmapImage[,] walkImages;
        private BitmapImage[] deathImages;


        public BitmapImage CurrentIdleImage;
        public BitmapImage CurrentWalkImage;
        public BitmapImage CurrentDeathImage;
        public Entity Entity;
        public bool IsWalking;
        public bool IsDead;
        public EntityObject(Entity Entity, string path)
        {
            this.Entity = Entity;
            IsWalking = false;
            IsDead= false;
            ImportImages(path);
        }

        private void ImportImages(string path)
        {

            //megnézem, hogy az Idle és Walk mappában mennyi a képek száma, igaziból megadhattam volna hogy 8 illetve 7
            int idleImageCount = Directory.GetFiles(Path.Combine(Path.Combine(path, "Idle"), "Left"), "*", SearchOption.TopDirectoryOnly).Length;
            int walkImageCount = Directory.GetFiles(Path.Combine(Path.Combine(path, "Walk"), "Left"), "*", SearchOption.TopDirectoryOnly).Length;
            int deathImageCount = Directory.GetFiles(Path.Combine(path, "Death"), "*", SearchOption.TopDirectoryOnly).Length;





            //int idleImageCount = 8;
            //int walkImageCount = 7;

            //létrehozom a képeket tároló mátrixokat, 2 sorral, ami a bal és jobb irányú képeket tárolja
            idleImages = new BitmapImage[2, idleImageCount];
            walkImages = new BitmapImage[2, walkImageCount];
            deathImages = new BitmapImage[deathImageCount];
            //betöltöm az IdleImageket
            for (int i = 0; i < idleImageCount; i++)
            {
                string leftImageName = "idle_left_0" + (i+1) + ".png";
                string rightImageName = "idle_right_0" + (i+1) + ".png";

                //úgy határoztam, hogy a 0. sorba kerülnek a bal irányba néző képek az 1. sorba pedig a jobb irányba néző képek
                idleImages[0, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Idle", Path.Combine("Left", leftImageName))), UriKind.RelativeOrAbsolute));
                idleImages[1, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Idle", Path.Combine("Right", rightImageName))), UriKind.RelativeOrAbsolute));
            }

            //betöltöm a WalkImageket
            for (int i = 0; i < walkImageCount; i++)
            {
                string leftImageName = "walk_left_0" + (i + 1) + ".png";
                string rightImageName = "walk_right_0" + (i + 1) + ".png";

                //úgy határoztam, hogy a 0. sorba kerülnek a bal irányba néző képek az 1. sorba pedig a jobb irányba néző képek
                walkImages[0, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Walk", Path.Combine("Left", leftImageName))), UriKind.RelativeOrAbsolute));
                walkImages[1, i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Walk", Path.Combine("Right", rightImageName))), UriKind.RelativeOrAbsolute));
            }

            for (int i = 0; i < deathImageCount; i++)
            {
                string deathImageName = "death0" + (i + 1) + ".png";
                deathImages[i] = new BitmapImage(new Uri(Path.Combine(path, Path.Combine("Death",deathImageName)), UriKind.RelativeOrAbsolute));
            }




            //beállíítom az alapképállapotot
            CurrentIdleImage = idleImages[0, 0];
            CurrentWalkImage = walkImages[0, 0];
            CurrentDeathImage = deathImages[0];
        }


        private int deathState = 0;
        public void ChangeCurrentDeathImage()
        {
            deathState++;
            if(deathState > deathImages.Length-1) 
            {
                deathState = 0;
            }
            CurrentDeathImage = deathImages[deathState];
        }

        //private int walkUpState = 0;
        //private int walkDownState = 0;
        private int walkLeftState = 0;
        private int walkRightState = 0;
        public void ChangeCurrentWalkImage()
        {
            //A Player jelenlegi iránya alapján meghatározom, hogy melyik WalkImage-t töltse be
            //A GameDisplay osztályban majd ez a függvény úgy lesz meghívva, hogy végigmenjen az adott irányú sétálás képek mindegyikén, tehát jelen esetben 7-szer
            //ehhez persze majd kell egy Dispatcher
            switch (Entity.Direction)
            {
                case Direction.Left:
                    walkLeftState++;
                    if (walkLeftState > walkImages.GetLength(1)-1)
                    {
                        walkLeftState = 0;
                    }
                    CurrentWalkImage = walkImages[0, walkLeftState];
                    break;

                case Direction.Right:
                    walkRightState++;
                    if (walkRightState > walkImages.GetLength(1)-1)
                    {
                        walkRightState = 0;
                    }
                    CurrentWalkImage = walkImages[1, walkRightState];
                    break;

                //jelen esetben mivel nincsen felfele meg lefele sétáló image frame, ezért default esetben amikor fel vagy le megy, akkor a jobbra néző sétáló képeket fogjuk látni
                default:
                    walkRightState++;
                    if (walkRightState > walkImages.GetLength(1) - 1)
                    {
                        walkRightState = 0;
                    }
                    CurrentWalkImage = walkImages[1, walkRightState];
                    break;
            }

        }


        private int idleLeftState = 0;
        private int idleRightState = 0;
        public void ChangeCurrentIdleImage()
        {
            //ugyanaz a mechanika, mint a ChangeCurrentWalkImage esetén
            switch (Entity.Direction)
            {

                case Direction.Left:
                    idleLeftState++;
                    if (idleLeftState > idleImages.GetLength(1)-1)
                    {
                        idleLeftState = 0;
                    }
                    CurrentIdleImage = idleImages[0, idleLeftState];
                    break;

                case Direction.Right:
                    idleRightState++;
                    if (idleRightState > idleImages.GetLength(1)-1)
                    {
                        idleRightState = 0;
                    }
                    CurrentIdleImage = idleImages[1, idleRightState];
                    break;

                default:
                    idleRightState++;
                    if (idleRightState > idleImages.GetLength(1) - 1)
                    {
                        idleRightState = 0;
                    }
                    CurrentIdleImage = idleImages[1, idleRightState];
                    break;

            }
        }
    }
}
