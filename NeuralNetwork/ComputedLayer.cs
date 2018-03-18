using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{

    /// <summary>
    /// A layers whose values need to be calculated from a previous layer.
    /// </summary>
    class ComputedLayer : Layer
    {
        public ComputedLayer(int neuronCount, int prevLayerNeuronCount) : base(neuronCount)
        {
            double variance = 2d / prevLayerNeuronCount;
            Normal normalDist = new Normal(0d, Math.Sqrt(variance));

            //init weights with random values
            Weights = new DenseMatrix(neuronCount, prevLayerNeuronCount);
            for (int i = 0; i < neuronCount; ++i)
                for (int j = 0; j < prevLayerNeuronCount; ++j)
                    do
                    {
                        Weights[i, j] = normalDist.Sample();
                    } while (Weights[i, j] == 0);

            //init biasses with 0 values
            Biasses = new DenseVector(neuronCount);
        }

        public ComputedLayer(int wRows, int wCols, double[] wStorage, double[] b) : base(b.Length)
        {
            //the wStorage should be colwise but it is rowwise
            Weights = (new DenseMatrix(wCols, wRows, wStorage)).Transpose();
            Biasses = new DenseVector(b);
        }

        public Matrix<double> Weights;
        public Vector<double> Biasses;
    }
}
