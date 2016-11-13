using System;
using System.Collections.Generic;

namespace BackPropagation.BackPropagation
{
    public class Network
    {
        private Topology _topology;
        private Data _trainSet;
        private Data _testSet;
        private List<Neuron> _inputLayer;
        private List<List<Neuron>> _hiddenLayers;
        private List<Neuron> _outputLayer;
        private bool run = false;

        public Network(Topology top, bool randomWeights)
        {
            run = true;
            _topology = top;
            _inputLayer = new List<Neuron>();
            _hiddenLayers = new List<List<Neuron>>();
            _outputLayer = new List<Neuron>();
            List<Pair<Neuron, double>> prevLayer = new List<Pair<Neuron, double>>();
            Random r = new Random();

            // Create Input Layer
            for (int i = 0; i < _topology.GetInputs(); ++i)
            {
                Neuron n = new Neuron();
                _inputLayer.Add(n);

                if (randomWeights)
                {
                    Pair<Neuron, double> synapse = new Pair<Neuron, double>();
                    synapse.Item1 = n;
                    synapse.Item2 = (r.NextDouble() - 0.5d) * 2.0d;
                    prevLayer.Add(synapse);
                }
                else
                {
                    Pair<Neuron, double> synapse = new Pair<Neuron, double>();
                    synapse.Item1 = n;
                    synapse.Item2 = 0.0d;
                    prevLayer.Add(synapse);
                }
            }

            // Create Hidden Layers
            List<int> _hidden = _topology.GetHidden();

            for(int i = 0; i < _hidden.Count; ++i)
            {
                List<Pair<Neuron, double>> currLayer = new List<Pair<Neuron, double>>();
                List<Neuron> layer = new List<Neuron>();

                for (int j = 0; j < _hidden[i]; ++j)
                {
                    Neuron n = new Neuron(prevLayer, false);
                    layer.Add(n);

                    if (randomWeights)
                    {
                        Pair<Neuron, double> synapse = new Pair<Neuron, double>();
                        synapse.Item1 = n;
                        synapse.Item2 = (r.NextDouble() - 0.5d) * 2.0d;
                        currLayer.Add(synapse);
                    }
                    else
                    {
                        Pair<Neuron, double> synapse = new Pair<Neuron, double>();
                        synapse.Item1 = n;
                        synapse.Item2 = 0.0d;
                        currLayer.Add(synapse);
                    }
                }

                prevLayer = currLayer;
                _hiddenLayers.Add(layer);
            }

            // Create Output Layer
            for(int i = 0; i < _topology.GetOutputs(); ++i)
            {
                Neuron n = new Neuron(prevLayer, true);
                _outputLayer.Add(n);
            }

            // Add PostSynapses
            foreach(var n in _inputLayer) // input
            {
                if (_hiddenLayers.Count > 0)
                {
                    foreach (var ne in _hiddenLayers[0])
                    {
                        n.AddPostSynapse(ne);
                    }
                } else
                {
                    foreach (var ne in _outputLayer)
                    {
                        n.AddPostSynapse(ne);
                    }

                    return;
                }
            }

            for(int i = 0; i < _hiddenLayers.Count - 1; ++i) // each hidden except last
            {
                foreach(var n in _hiddenLayers[i])
                {                    
                    foreach(var ne in _hiddenLayers[i + 1])
                    {
                        n.AddPostSynapse(ne);
                    }
                }
            }

            foreach(var n in _hiddenLayers[_hiddenLayers.Count - 1]) // last hidden
            {
                foreach(var ne in _outputLayer)
                {
                    n.AddPostSynapse(ne);
                }
            }
        }

        public void SetTrain(Data trainSet)
        {
            _trainSet = trainSet;
        }

        public void SetTest(Data testSet)
        {
            _testSet = testSet;
        }

        public double CountErrorOnTest()
        {
            double error = 0.0d;
            for (int i = 0; i < _testSet.GetNumData(); ++i)
            {
                //List<double> output = new List<double>();
                double output = 0.0d;
                List<double> input = _testSet.GetLine(i).GetLine();
                for (int j = 0; j < input.Count; ++j)
                {
                    _inputLayer[j].SetValue(input[j]);
                }

                //foreach (var n in _outputLayer)
                //{
                //    output.Add(n.Count());
                //}

                output = _outputLayer[0].Count();       
                double cl = _testSet.GetLine(i).GetClass();
#region a
                //if (output.Count > 2)
                //{
                //    for (int j = 0; j < output.Count; ++j)
                //    {
                //        if (j == _testSet.GetClassNormNumber(cl))
                //        {
                //            if (Math.Round(output[j]) != 1)
                //            {
                //                error += 1.0d;
                //                break;
                //            }
                //        }
                //        else
                //        {
                //            if (Math.Round(output[j]) != 0)
                //            {
                //                error += 1.0d;
                //                break;
                //            }
                //        }
                //    }
                //} else
                //{
                //error += Math.Pow(_testSet.GetClassNormNumber(cl) - output[0], 2);
                //}
#endregion

                if (Math.Abs(_testSet.GetClassNormNumber(cl) - output) > 0.24d)
                {
                    error += 1.0d;
                }

                ResetAll();
            }

            error /= _testSet.GetNumData();
            Console.WriteLine("Perc Err Test: " + error);

            return error;
        }

        /*private List<double> CountErrorsOnOutputs(double cl, List<double> outs)
        {
            List<double> errs = new List<double>();

            //if (outs.Count > 2)
            //{
            //    for (int i = 0; i < outs.Count; ++i)
            //    {
            //        if ((double)i == _trainSet.GetClassNormNumber(cl))
            //        {
            //            errs.Add(1.0d - outs[i]);
            //        }
            //        else
            //        {
            //            errs.Add(0.0d - outs[i]);
            //        }
            //    }
            //} else
            //{
            errs.Add(_trainSet.GetClassNormNumber(cl) - outs[0]);
            //}

            return errs;
        }*/

        private void ChangeWeightsOnAll(double gamma, double momentum)
        {
            foreach(var l in _hiddenLayers)
            {
                foreach(var n in l)
                {
                    n.ChangeWeights(gamma, momentum);
                }
            }

            foreach(var n in _outputLayer)
            {
                n.ChangeWeights(gamma, momentum);
            }
        }

        private void ResetAll()
        {
            foreach(var n in _inputLayer)
            {
                n.ResetValue();
            }
            foreach (var l in _hiddenLayers)
            {
                foreach (var n in l)
                {
                    n.ResetValue();
                }
            }

            foreach (var n in _outputLayer)
            {
                n.ResetValue();
            }
        }

        public void Train(double gamma, double momentum, double epsilon = 0.02d, int maxIter = 5000)
        {
            int iterations = 0;
            while(CountErrorOnTest() > epsilon && iterations < maxIter)
            {
                iterations++;   
                double error = 0.0d;
                for (int i = 0; i < _trainSet.GetNumData(); ++i)
                {
                    //List<double> output = new List<double>();
                    double output = 0.0d;
                    List<double> input = _trainSet.GetLine(i).GetLine();
                    for (int j = 0; j < input.Count; ++j)
                    {
                        _inputLayer[j].SetValue(input[j]);
                    }

                    double cl = _trainSet.GetLine(i).GetClass();
                    output = _outputLayer[0].Count();
                    double err = _trainSet.GetClassNormNumber(cl) - output;
                    _outputLayer[0].CountDeltas(err, null);

#region old
                    //foreach (var n in _outputLayer)
                    //{
                    //    output.Add(n.Count());
                    //}

                    //List<double> errs = CountErrorsOnOutputs(cl, output);

                    //if (output.Count > 2)
                    //{
                    //    for (int j = 0; j < output.Count; ++j)
                    //    {
                    //        if (j == _trainSet.GetClassNormNumber(cl))
                    //        {
                    //            if (Math.Round(output[j]) != 1)
                    //            {
                    //                error += 1.0d;
                    //                break;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (Math.Round(output[j]) != 0)
                    //            {
                    //                error += 1.0d;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
#endregion
                    if (Math.Abs(output - _trainSet.GetClassNormNumber(cl)) > 0.24d)
                    {
                        error += 1.0d;
                    }
#region tr
                    //error += Math.Pow(_trainSet.GetClassNormNumber(cl) - output[0], 2);
                    //}

                    //for (int j = 0; j < errs.Count; ++j)
                    //{
                    //    _outputLayer[j].CountDeltas(errs[j], null);
                    //}
#endregion
                    ChangeWeightsOnAll(gamma, momentum);
                    //DrawNetwork(_trainSet.GetClassNormNumber(cl));
                    //Console.WriteLine("Err: " + errs[0]);
                    ResetAll();
                }

                error /= _trainSet.GetNumData();
                Console.WriteLine("Perc Err Train: " + error);          
            }
        }

        // TODO: Add methods for returning results!

        private void DrawNetwork(double cl)
        {
            for (int i = 0; i < _inputLayer.Count; ++i)
            {
                Console.Write("   X" + i + "   ");
            }
            Console.Write("\nV: ");

            for (int i = 0; i < _inputLayer.Count; ++i)
            {
                Console.Write(_inputLayer[i].Count() + " ");
            }
            Console.Write("\nW1: ");

            for (int i = 0; i < _hiddenLayers.Count; ++i)
            {
                for (int j = 0; j < _hiddenLayers[i].Count; ++j)
                {
                    List<Pair<Neuron, double>> pre = _hiddenLayers[i][j].GetPreSynapses();

                    for (int k = 0; k < pre.Count; ++k)
                    {
                        Console.Write(pre[k].Item2 + " ");
                    }
                    Console.Write("\nW" + (j + 1) + ": ");
                }
                Console.Write("\nV: ");

                for (int j = 0; j < _hiddenLayers[i].Count; ++j)
                {
                    Console.Write(_hiddenLayers[i][j].Count() + " ");
                }
                Console.Write("\nW1: ");
            }

            for (int j = 0; j < _outputLayer.Count; ++j)
            {
                List<Pair<Neuron, double>> pre = _outputLayer[j].GetPreSynapses();

                for (int k = 0; k < pre.Count; ++k)
                {
                    Console.Write(pre[k].Item2 + " ");
                }
                Console.Write("\nW" + (j + 1) + ": ");
            }
            Console.Write("\nO: ");

            for (int j = 0; j < _outputLayer.Count; ++j)
            {
                Console.Write(_outputLayer[j].Count() + " ");
            }

            Console.Write("\nCl: " + cl + "\n");

            
            //Console.Read();
        }
    }
}