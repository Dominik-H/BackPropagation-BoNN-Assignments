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
        private List<Pair<double, double>> _transformTable; // class number, class normalized number

        public Data()
        {
            _lines = new List<Line>();
            _transformTable = new List<Pair<double, double>>();
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

        public double GetClassNormNumber(double cl)
        {
            for (int i = 0; i < _transformTable.Count; ++i)
            {
                if(_transformTable[i].Item1 == cl)
                {
                    return _transformTable[i].Item2;
                }
            }

            return -1.0d;
        }

        public double GetClassNumber(double clNorm)
        {
            for(int i = 0; i < _transformTable.Count; ++i)
            {
                if(_transformTable[i].Item2 == clNorm)
                {
                    return _transformTable[i].Item1;
                }
            }

            return -1.0d;
        }

        public void Normalize()
        {
            for(int i = 0; i < _lines[0].GetLine().Count; ++i)
            {
                // find max and min
                double min = _lines[0].GetLine()[i];
                double max = _lines[0].GetLine()[i];
                for(int j = 1; j < _lines.Count; ++j)
                {
                    if(_lines[j].GetLine()[i] < min)
                    {
                        min = _lines[j].GetLine()[i];
                    }
                    if (_lines[j].GetLine()[i] > max)
                    {
                        max = _lines[j].GetLine()[i];
                    }
                }

                // normalize
                for(int j = 0; j < _lines.Count; ++j)
                {
                    _lines[j].GetLine()[i] = 2.0d * ((_lines[j].GetLine()[i] - min) / (max - min)) - 1.0d;
                }
            }

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

            if (_transformTable.Count == 0)
            {
                classes.Sort();
                for (int i = 0; i < classes.Count; ++i)
                {
                    Pair<double, double> transformLine = new Pair<double, double>();
                    transformLine.Item1 = classes[i];
                    transformLine.Item2 = (classes[i] - classes.Min()) / (classes.Max() - classes.Min());

                    _transformTable.Add(transformLine);
                }
            }
            
            return classes.Count();
        }
    }
}