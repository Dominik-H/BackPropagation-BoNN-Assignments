using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace BackPropagation.BackPropagation
{
    public class Loader
    {
        private string _path;
        private int _trainPerc;
        private int _testPerc;
        private Data _allData;

        public Loader(string path, int trainPercent)
        {
            _path = path;
            _trainPerc = trainPercent;
            _testPerc = 100 - trainPercent;
            Init();
        }

        private void Init()
        {
            _allData = new Data();
            var reader = new StreamReader(File.OpenRead(_path));

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var vals = line.Split(',');
                bool isNum = false;
                Data.Line ln = new Data.Line(vals.Length);

                for(int i = 0; i < vals.Length; ++i)
                {
                    double val;
                    isNum = double.TryParse(vals[i], NumberStyles.Any, CultureInfo.InvariantCulture, out val);
                    if(!isNum)
                    {
                        break;
                    }

                    if(i == (vals.Length - 1))
                    {
                        ln.SetClass(val);
                    } else
                    {
                        ln.AddItem(val);
                    }
                }

                if(!isNum)
                {
                    continue;
                }

                _allData.AddLine(ln);
            }

            _allData.Shuffle();
        }

        public Data GenerateTrainSet()
        {
            Data dat = new Data();
            dat = _allData.GetTrainSet(_trainPerc);
            return dat;
        }

        public Data GenerateTestSet()
        {
            Data dat = new Data();
            dat = _allData.GetTestSet(_testPerc);
            return dat;
        }
    }
}