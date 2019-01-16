using System;
using System.Collections.Generic;

namespace Chess_map_convolution
{
    class MyImage
    {
        private float[] values; //zawiera wartości pixeli z naszego obrazu
        private int[] size;     //zawiera dane o rozdzielczości [0]-wysokość [1]-szerokość


        public MyImage(int width, int height)
        {
            this.Size = new int[2];
            Size[0] = height;
            Size[1] = width;
            values = new float[height * width];
            for (int i = 0; i < height * width; i++)
            {
                values[i] = 0;
            }
        }

        public void CreateCheckerboard(int l)
        {
            /*
             * Tworzy na obrazie szachownice o l ilosci kwadratów w wierszu
             */
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    int index = i * size[1] + j;
                    int liczba_pikseli_na_pole_w = size[1] / l;
                    int liczba_pikseli_na_pole_h = size[0] / l;
                    if (((i / liczba_pikseli_na_pole_h) + (j / liczba_pikseli_na_pole_w)) % 2 == 0)
                        values[index] = 0;
                    else
                        values[index] = 1;
                }
            }
        }
        public void printConsole()
        {
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    int index = i * size[1] + j;
                    Console.Write("{0}\t", values[index]);
                }
                Console.WriteLine();
            }
        }
        public float[] Values { get => values; set => values = value; }
        public int[] Size { get => size; set => size = value; }
    }
   
}