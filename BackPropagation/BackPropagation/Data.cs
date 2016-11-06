using System;
using System.Collections.Generic;
using System.Linq;

namespace BackPropagation.BackPropagation
{
    public class Data
    {
        public class Line
        {
            private int _numEl;
            private List<double> _line;
            private double _class;

            public Line(int numElements)
            {
                _numEl = numElements;
                _line = new List<double>(numElements);
            }

            public void AddItem(double item)
            {
                _line.Add(item);
            }

            public void SetClass(double cl)
            {
                _class = cl;
            }

            public List<double> GetLine()
            {
                return _line;
            }

            public double GetClass()
            {
                return _class;
            }
        }

        private List<Line> _lines;

        public Data()
        {
            _lines = new List<Line>();
        }

        public void AddLine(Line ln)
        {
            _lines.Add(ln);
        }

        public void Shuffle()
        {
            int n = _lines.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                Line value = _lines[k];
                _lines[k] = _lines[n];
                _lines[n] = value;
            }
        }

        public int GetNumData()
        {
            return _lines.Count;
        }

        public Data GetTrainSet(int percentTrain)
        {
            Data o = new Data();
            double perc = percentTrain / 100.0d;
            int numEl = Convert.ToInt32(_lines.Count * perc);

            for(int i = 0; i < numEl; ++i)
            {
                o.AddLine(_lines[i]);
            }

            return o;
        }

        public Data GetTestSet(int percentTest)
        {
            Data o = new Data();
            double perc = percentTest / 100.0d;
            int numEl = Convert.ToInt32(_lines.Count * perc);

            for (int i = 1; i <= numEl; ++i)
            {
                o.AddLine(_lines[_lines.Count - i]);
            }

            return o;
        }

        public Line GetLine(int index)
        {
            return _lines[index];
        }

        public int GetNumFeatures()
        {
            return _lines[0].GetLine().Count;
        }

        public int GetNumClasses()
        {
            List<double> classes = new List<double>();

            for(int i = 0; i < _lines.Count; ++i)
            {
                double cl = _lines[i].GetClass();
                bool unique = true;
                for(int j = 0; j < classes.Count(); ++j)
                {
                    if(classes[j] == cl)
                    {
                        unique = false;
                        break;
                    }
                }

                if(unique)
                {
                    classes.Add(cl);
                }
            }
            
            return classes.Count();
        }
    }
}