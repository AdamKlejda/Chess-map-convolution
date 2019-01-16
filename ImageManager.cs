using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Chess_map_convolution
{
    class ImageManager
    {

        public static void saveImage(string path, MyImage image)
        {
            /*
             *Zapisywanie w odpowiednim formacie PGM
             */
            var stream = File.Open(path, FileMode.OpenOrCreate);

            StringBuilder sb = new StringBuilder();
            sb = sb.Append("P2\n");
            sb = sb.Append(image.Size[0]);
            sb = sb.Append(" ");
            sb = sb.Append(image.Size[1]);
            sb = sb.Append("\n");
            sb = sb.Append("255\n");
            for (int i = 0; i < image.Size[0] * image.Size[1]; i++)
            {
                sb = sb.Append(Math.Ceiling(image.Values[i] * 255));
                sb = sb.Append(" ");

            }
            stream.Write(Encoding.ASCII.GetBytes(sb.ToString()), 0, sb.Length);
            stream.Close();

        }


        public static float[] naiveConvolution(MyImage image)
        {
            /*
             *Przy każdym pixelu sprawdzamy które pixele należy w jego przypadku obliczyć
             */
            float[] output = new float[image.Values.Length];

            for (int i = 1; i < image.Values.Length; i++)
            {
                float up = 0;
                float down = 0;
                float left = 0;
                float right = 0;
                float center = 0;

                if (((i - 1023) % image.Size[0]) > 0)
                {
                    up =( image.Values[i - 1024] * 0.1f);
                }
                   

                if ((i + 1024) <( image.Size[0] * image.Size[1]))
                {
                    down = (image.Values[i + 1024] * 0.1f);

                }

                if (((i) % 1024>0))
                {
                    left = (image.Values[i - 1] * 0.1f);

                }

                if (((i + 1) % image.Size[0]) < 1022 && ((i + 1 )< (image.Size[0] * image.Size[1])))
                {
                    right = (image.Values[i + 1] * 0.1f);

                }

                center = image.Values[i] * 0.6f;

                output.SetValue(up + down + left + right + center,i);
            }

            return output;
        }

       
    }
}
