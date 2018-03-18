using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// The output layer of the network.
    /// </summary>
    class OutputLayer : ComputedLayer
    {
        public OutputLayer(int neuronCount, int prevLayerNeuronCount) : base(neuronCount, prevLayerNeuronCount)
        {
            //
        }

        public OutputLayer(int wRows, int wCols, double[] wStorage, double[] b) : base(wRows, wCols, wStorage, b)
        {
            //
        }
    }
}
