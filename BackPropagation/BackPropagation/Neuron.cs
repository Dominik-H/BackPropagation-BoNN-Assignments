using System;
using System.Collections.Generic;

namespace BackPropagation.BackPropagation
{
    public class Neuron
    {
        // Neuron, Weight pairs
        private List<Pair<Neuron, double>> _preSynapticNeurons;
        private List<double> _deltaW;
        private double? _biasWeight;
        // For Changing weights
        private List<Pair<Neuron, bool>> _postSynapticNeurons;

        private double? _value = null;
        private double _in;

        private double? _derAct = null;
        private double? _delta = null;
        private bool _output;

        // Creates Input Neuron
        public Neuron()
        {
            _preSynapticNeurons = null;
            _deltaW = null;
            _postSynapticNeurons = new List<Pair<Neuron, bool>>();
            _output = false;
            _biasWeight = null;
        }

        // Creates Hiden Layer or Output Neuron
        public Neuron(List<Pair<Neuron, double>> previousLayer, bool output = false)
        {
            _preSynapticNeurons = previousLayer;
            _deltaW = new List<double>();
            for(int i =0; i < _preSynapticNeurons.Count; ++i)
            {
                _deltaW.Add(0.0d);
            }
            _output = output;
            _biasWeight = 1.0d;
            _value = null;
            if (!_output)
            {
                _postSynapticNeurons = new List<Pair<Neuron, bool>>();
            }
        }

        private double IN()
        {
            if (_preSynapticNeurons == null) // is Input Layer
            {
                _in = _value.Value;

                return _in;
            }

            foreach (var item in _preSynapticNeurons) // is Regular Layer
            {
                _in += item.Item2 * item.Item1.Count();
            }

            _in -= _biasWeight.Value;

            return _in;
        }

        private double Act()
        {
            double _out = 1.0d / (1.0d + Math.Exp(-IN()));
            DerivateAct();

            return _out;
        }

        private double OUT()
        {
            if(_value == null)
            {
                _value = Act();
            }

            return _value.Value;
        }

        private void DerivateAct()
        {
            double exp = Math.Exp(-_in);
            _derAct = exp / (Math.Pow(1.0d + exp, 2));
        }

        private bool AllDeltasIn()
        {
            if (_postSynapticNeurons != null)
            {
                foreach (var n in _postSynapticNeurons)
                {
                    if (!n.Item2)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Set Value of Input neuron
        public void SetValue(double value)
        {
            if(_preSynapticNeurons == null)
            {
                _value = value;
            } else
            {
                throw new InvalidOperationException();
            }
        }

        public List<Pair<Neuron, bool>> GetPostSynapticNeurons()
        {
            return _postSynapticNeurons;
        }

        public void AddPostSynapse(Neuron neuron)
        {
            if (!_output)
            {
                Pair<Neuron, bool> n = new Pair<Neuron, bool>();
                n.Item1 = neuron;
                n.Item2 = false;
                _postSynapticNeurons.Add(n);
            }
        }

        public double Count()
        {
            return OUT();            
        }

        // Call after each input set is processed on each neuron!!
        public void ResetValue()
        {
            _value = null;
            _in = 0.0d;
            _derAct = null;
            _delta = null;

            if (_postSynapticNeurons != null)
            {
                foreach (var n in _postSynapticNeurons)
                {
                    n.Item2 = false;
                }
            }
        }


        // Call on Each Output Neuron
        public bool CountDeltas(double error, Neuron from)
        {
            if(_preSynapticNeurons == null)
            {
                return true;
            }

            if (from == null && _output)
            {
                _delta = error * _derAct.Value;
            }

            if (!AllDeltasIn())
            {
                if (_delta == null)
                {
                    _delta = 0.0d;
                    _delta += error;
                }
                else
                {
                    _delta += error;
                }

                if (_postSynapticNeurons != null)
                {
                    foreach (var n in _postSynapticNeurons)
                    {
                        if (n.Item1 == from)
                        {
                            n.Item2 = true;
                            break;
                        }
                    }
                }
            }

            if (AllDeltasIn())
            {
                if (!_output)
                {
                    _delta *= _derAct.Value;
                }

                if (_preSynapticNeurons != null)
                {
                    foreach (var neuron in _preSynapticNeurons)
                    {                        
                        neuron.Item1.CountDeltas(_delta.Value * neuron.Item2, this);
                    }
                }
            }            

            return true;
        }

        // Call on Each Neuron except input layer after counting deltas
        public void ChangeWeights(double gamma, double momentum)
        {
            if(_delta != null)
            {
                for(int i = 0; i < _preSynapticNeurons.Count; ++i)
                {
                    double dw = gamma * _delta.Value * _preSynapticNeurons[i].Item1.Count();
                    _preSynapticNeurons[i].Item2 += dw + momentum * _deltaW[i];
                    _deltaW[i] = dw;
                }

                _biasWeight -= gamma * _delta.Value;
            } else
            {
                throw new InvalidOperationException();
            }
        }
        
        public double? GetWeight(Neuron n)
        {
            foreach(var item in _preSynapticNeurons)
            {
                if(item.Item1 == n)
                {
                    return item.Item2;
                }
            }

            return null;
        }

        public List<Pair<Neuron, double>> GetPreSynapses()
        {
            return _preSynapticNeurons;
        }
    }
}