using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CohonenNet
{
    class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Pixels
    {
        public List<Pixel> set { get; }

        public Pixels()
        {
            set = new List<Pixel>();
        }

        public void Add(Pixel p)
        {
            set.Add(p);
        }

        public void Shuffle()
        {
            int n = set.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                Pixel value = set[k];
                set[k] = set[n];
                set[n] = value;
            }
        }
    }
}
