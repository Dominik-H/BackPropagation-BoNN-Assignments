using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CohonenNet
{
    class Network
    {
        private double gamma;
        private double radius;
        private double timeConst;
        private List<List<Neuron>> net;
        private int maxIter;
        private Pixels data;

        public Network(double g0, double r0, Pixel mid, int numX, int numY, int maxIter, Pixels dat)
        {
            data = dat;
            gamma = g0;
            radius = r0;
            this.maxIter = maxIter;
            net = new List<List<Neuron>>();
            timeConst = maxIter / Math.Log(mid.X > mid.Y ? mid.X : mid.Y);

            for(int i = 1; i <= numY; ++i)
            {
                var line = new List<Neuron>();
                for(int j = 1; j <= numX; ++j)
                {
                    var n = new Neuron(new Pixel(j, i), mid);
                    line.Add(n);
                }
                net.Add(line);
            }
        }

        public List<List<Neuron>> GetNet()
        {
            return net;
        }

        public Neuron GetNeuron(int x, int y)
        {
            foreach(var l in net)
            {
                foreach(var n in l)
                {
                    if(n.position.X == x && n.position.Y == y)
                    {
                        return n;
                    }
                }
            }

            return null;
        }

        public void Train()
        {
            for(int i = 0; i < maxIter; ++i)
            {
                for(int j = 0; j < data.set.Count; ++j)
                {
                    Neuron win = FindWinner(data.set[j]);
                    Update(win, data.set[j]);
                }

                radius *= Math.Exp(-i / timeConst);
                gamma *= Math.Exp(-i / timeConst);
            }
        }

        private Neuron FindWinner(Pixel p)
        {
            Neuron winn = net[0][0];
            foreach (var l in net)
            {
                foreach (var n in l)
                {
                    double dist = n.CountDistance(p);
                    if (dist < winn.dist)
                    {
                        winn = n;
                    }
                }
            }

            return winn;
        }

        private void Update(Neuron win, Pixel input)
        {
            foreach (var l in net)
            {
                foreach (var n in l)
                {
                    double d = Math.Pow(win.position.X - n.position.X, 2) + Math.Pow(win.position.Y - n.position.Y, 2);
                    if(d < Math.Pow(radius, 2))
                    {
                        n.UpdateTheta(d, radius);
                        n.ChangeWeights(input, gamma);
                    }
                }
            }
        }       
    }
}
