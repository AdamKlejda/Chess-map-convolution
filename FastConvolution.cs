using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess_map_convolution
{
    class FastConvolution
    {
        private MyImage _image;         
        private float[] _values;        //tutaj przechowujemy wartości na podstawie których liczymy 
        private float[] _Resultvalues;  //tutaj przechowujemy wartości wyniku

        private int _height;
        private int _width;
        private float _c; //wartość maski w środku dla konwolucji
        private float _s; //wartość maski po bokach dla konwolucji


        public FastConvolution(MyImage image,float centerMask,float sideMask)
        {
            this._Resultvalues = new float[image.Values.Length];
            this._values = image.Values;
            this._height = image.Size[0];
            this._width = image.Size[1];
            this._image = image;
            this._c = centerMask;
            this._s = sideMask;


        }

        public  MyImage testConvolution(int nThreads,int nConvulate)
        {
            /*
             * Przyjmuje jako parametry (ilosć watków na ilu mają być wykonywane obliczenia,ilosć powtórzeń konwolucji)
             * Tworzymy tablice dla tasków dla każdego wiersza po czym wchodzimy dla każdego obiegu konwolucji liczymy kolejno:
             * 
             *              ->Góre
             *              ->Granice boczne
             *      ----czekamy na zakończenie tych obliczeń----
             *      
             *              ->Wnętrze obrazu
             *      ----czekamy na zakończenie tych obliczeń----
             *      
             *              ->Dół
             *      ----czekamy na zakończenie tych obliczeń----
             *         
             *  Bez czekania na zakończenie obliczeń, procesor wyszedłby z funkcji za szybko co by skutkowało złym wynikiem konwolucji
             */
            Task[] tasks = new Task[_height];
          

            for(int times = 0; times < nConvulate; times++)
            {
                var taskb = CalculateTop();             //liczenie dołu
                var taskborders = CalculateBorders();   //liczenie krawedzi

                //taskb.Wait();
                //taskborders.Wait();

                for (int i = 1; i < _height - nThreads; i = i + nThreads)
                {

                    for (int j = 0; j < nThreads; j++)
                    {
                        tasks[i + j] = CalculateInside(((i * _width) + (j * 1024)) + 1, ((i * _width + (j * 1024))) + 1022); //przydzielenie watkowi długości jednego wiersza bez krawędzi 
                    }
                    for (int j = 0; j < nThreads; j++) //oczekiwanie aż wątki się zakończą
                    {
                        tasks[i + j].Wait();
                    }
                }
                var taskn = CalculateBottom();
                taskn.Wait();

                _values = _Resultvalues;
            }

            
            _image.Values = _Resultvalues;
            return _image;
        }

        private Task CalculateTop()
        {
            return Task.Run(() => {
            //prawa góra
            _Resultvalues[_width] = (_c * _values[_width - 1])              //srodek
                                    +(_s * _values[_width - 2])             //lewo
                                    +(_s * _values[_width - 1 + _width]);   //dol

            //góra pomiedzy
            for(int i= _width-2; i>0;i--)
            {
                _Resultvalues[i] = (_c * _values[i])                        //srodek 
                                    + (_s * _values[i+1])                   //prawo
                                    + (_s * _values[i-1])                   //lewo
                                    + (_s * _values[i+_width]);             //dol
            }
            //lewa góra
            _Resultvalues[0] = (_c * _values[0]) 
                                +(_s*_values[1])
                                +(_s*_values[_width]);
            });



        }

        private Task CalculateBorders()
        {
            return Task.Run(() => {
                //Lewa strona
                for(int i=1;i<_height-1;i++)
                {
                    _Resultvalues[i*_width] =( _c * _values[_width *i] )                    //srodek
                                            + (_s * _values[i * _width-1024])               //gora
                                            + (_s * _values[i * _width +1])                 //prawo
                                            +(_s*_values[i * _width + 1024]);               //dol
                }

                //Prawa strona
                for (int i = 1; i < _height-1; i++)
                {
                    _Resultvalues[i * _width+1023] = (_c * _values[i * _width + 1023])          //srodek
                                                    + (_s * _values[i * _width + 1022])           //lewo
                                                    + (_s * _values[i * _width + 2047])         //dol
                                                    + (_s * _values[i * _width - 1]);            //gora
                }
              
            });



        }

        private Task CalculateInside(int from, int to)
        {
            return Task.Run(() => {

            for( int i=from; i<to; i++)
                    {
                        _Resultvalues[i] =( (_c * _values[i])                 //srodek
                                            + (_s * _values[i - 1])           //lewo
                                            + (_s * _values[i - 1024])        // gora
                                            + (_s * _values[i + 1])           // prawo 
                                            + (_s * _values[i + 1024]));      //dol

                    }
            });



        }

        private Task CalculateBottom()
        {
            return Task.Run(() => {
                //prawy dolny róg
                _Resultvalues[_width * _height -1] = (_c * _values[_width * _height - 1])//srodek 
                                                    + (_s * _values[_width * _height - 2])//lewo
                                                    + (_s * _values[_width * (_height - 1)-1]);//prawo

                //dół bez rogów
                for (int i = ((_width*_height) - 2); i >( _width * (_height - 1)); i--)
                {
                    _Resultvalues[i] = _c * _values[i]                  //srodek
                                        + _s * _values[i + 1]           //prawo
                                        + _s * _values[i - 1]           //lewo
                                        + _s * _values[i - _width];     //gora
                }

                //lewy dolny róg
                _Resultvalues[_width * (_height - 1)] = (_c * _values[_width * (_height - 1)])      //srodek
                                                    + (_s * _values[_width * (_height - 1) + 1])    //prawo
                                                    + (_s * _values[_width * (_height - 2)]);       //gora
        });
           
        }
    }
}
