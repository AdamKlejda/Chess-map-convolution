using System;
using System.Drawing;

namespace Chess_map_convolution
{
    class Program
    {
        static void Main(string[] args)
        {
            MyImage img = new MyImage(1024, 1024);
            img.CreateCheckerboard(16);
            ImageManager.saveImage("image.pgm", img);
            MyImage img2 = new MyImage(1024, 1024);
            MyImage img3 = img;
            FastConvolution fc = new FastConvolution(img,0.6f,0.1f);
            double timeTemp = 0; //zmienna pomagająca w obliczeniu czasu średniego

            // Wyliczanie Synchroniczne
            DateTime start = new DateTime();

            for (int z = 0; z < 1; z++)
            {
                start = DateTime.Now;
                for (int i = 0; i < 200; i++)
                {

                   // img2.Values = ImageManager.naiveConvolution(img3);
                    img3.Values = img2.Values;

                }
                Console.Write("\nS" +z+":" +(DateTime.Now - start));
                timeTemp += (DateTime.Now - start).TotalMilliseconds;
            }
            Console.Write("\nSynchronicznie: " + TimeSpan.FromMilliseconds(timeTemp/20));

            ImageManager.saveImage("imageAfterSynchronicznie.pgm", img3);

            // Wyliczanie Asynchroniczne
            timeTemp = 0;            

             for (int i = 0; i <20; i++)
             {
                start = DateTime.Now;
                img3 =fc.testConvolution(12,200);// testConvolution przyjmuje za parametru (ilosc watkow, ilosc powtórzeń)
                Console.Write("\nA" + i + ":" + (DateTime.Now - start));
                timeTemp += (DateTime.Now - start).TotalMilliseconds;            }


            Console.Write("\nAsynchronicznie: " + TimeSpan.FromMilliseconds(timeTemp / 20) + "\n");
            ImageManager.saveImage("imageAfterAsynchronicznie.pgm", img3);

            
        }
    }
}