using System.Collections.Generic;

namespace BackPropagation.BackPropagation
{
    public class Topology
    {
        private int _numInputs;
        private int _numOutputs;
        private List<int> _numHidden;

        public Topology()
        {
            _numHidden = new List<int>();
        }

        public void SetInputs(int num)
        {
            _numInputs = num;
        }

        public int GetInputs()
        {
            return _numInputs;
        }

        public void SetOutputs(int num)
        {
            _numOutputs = num;
        }

        public int GetOutputs()
        {
            return _numOutputs;
        }

        public void AddHidden(int num)
        {
            _numHidden.Add(num);
        }

        public List<int> GetHidden()
        {
            return _numHidden;
        }
    }
}