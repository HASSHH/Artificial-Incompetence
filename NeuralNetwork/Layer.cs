using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// Represents a layer of the network. Neurons have double values between 0 and 1.
    /// </summary>
    abstract class Layer
    {
        private int neuronCount;

        internal Layer(int neuronCount)
        {
            this.neuronCount = neuronCount;
        }

        internal int GetNeuronCount() => neuronCount;

        internal Vector<double> Activation;
    }
}
