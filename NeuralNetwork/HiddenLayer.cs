using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// A hidden layer of the network.
    /// </summary>
    class HiddenLayer : ComputedLayer
    {
        public HiddenLayer(int neuronCount, int prevLayerNeuronCount) : base(neuronCount, prevLayerNeuronCount)
        {
            //
        }

        public HiddenLayer(int wRows, int wCols, double[] wStorage, double[] b) : base(wRows, wCols, wStorage, b)
        {
            //
        }
    }
}
