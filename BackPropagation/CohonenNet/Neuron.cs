using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CohonenNet
{
    class Neuron
    {
        public Pixel position { get; }
        public double weightX { get; set; }
        public double weightY { get; set; }
        public double dist { get; set; }
        private double theta;

        public Neuron(Pixel pos, Pixel w)
        {
            position = pos;
            weightX = w.X;
            weightY = w.Y;
        }

        public double CountDistance(Pixel input)
        {
            dist = Math.Sqrt(Math.Pow(input.X - weightX, 2) + Math.Pow(input.Y - weightY, 2));
            return dist;
        }

        public void UpdateTheta(double d, double radius)
        {
            theta = Math.Exp(-d / (2 * Math.Pow(radius, 2)));
        }

        public void ChangeWeights(Pixel input, double gamma)
        {
            weightX += theta * gamma * (input.X - weightX);
            weightY += theta * gamma * (input.Y - weightY);
        }
    }
}
