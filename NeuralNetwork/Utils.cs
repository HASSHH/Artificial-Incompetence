using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class Utils
    {
        public static Vector<double> Sigmoid(Vector<double> v)
        {
            return v.Map((old) => 1d / (1d + Math.Exp(-old)), Zeros.Include);
        }

        /// <summary>
        /// Derivative of the sigmoid function
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector<double> SigmoidPrime(Vector<double> v)
        {
            Vector<double> sigmV = Sigmoid(v);
            return sigmV.Map((old) => old * (1d - old), Zeros.Include);
        }
    }
}
