using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CohonenNet
{
    class BitmapLoader
    {
        public Pixels data { get; }
        public Pixel mid { get; }

        public BitmapLoader(Bitmap img)
        {
            data = new Pixels();
            Color pColor;
            mid = new Pixel(img.Width / 2, img.Height / 2);

            for(int i = 0; i < img.Width; ++i)
            {
                for(int j = 0; j < img.Height; ++j)
                {
                    pColor = img.GetPixel(i, j);

                    if(pColor.R < 21 && pColor.G < 21 && pColor.B < 21)
                    {
                        data.Add(new Pixel(i, j));
                    }
                }
            }

            data.Shuffle();
        }
    }
}
