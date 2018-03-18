using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class LabeledData
    {
        public Vector<double> InputValues;
        public Vector<double> OutputValues;

        public void FromArrays(double[] inputValues, double[] outputValues)
        {
            InputValues = new DenseVector(inputValues);
            OutputValues = new DenseVector(outputValues);
        }

        public static LabeledData MakeFromArrays(double[] inputValues, double[] outputValues)
        {
            LabeledData ld = new LabeledData();
            ld.FromArrays(inputValues, outputValues);
            return ld;
        }
    }
}
