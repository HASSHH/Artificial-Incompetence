using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    class MnistDataContainer : LabeledDataContainer<byte, byte>
    {
        public MnistDataContainer()
        {
            isDataCompressed = true;
            autoReadDataSize = false;
            inputDataSize = 784;
            outputDataSize = 10;
        }

        protected override double[] NormalizeInputData(byte[] input)
        {
            double[] result = new double[input.Length];
            for (int i = 0; i < input.Length; ++i)
                result[i] = input[i] / 255d;
            return result;
        }

        protected override double[] NormalizeOutputData(byte[] output)
        {
            int digit = output[0];
            double[] result = new double[10];
            for (int i = 0; i < 10; ++i)
                result[i] = i == digit ? 1d : 0d;
            return result;
        }

        public override bool CheckIfOutputIsCorrect(double[] output, double[] correct)
        {
            int oMax, cMax;
            oMax = cMax = 0;
            for (int i = 1; i < output.Length; ++i)
            {
                if (output[i] > output[oMax])
                    oMax = i;
                if (correct[i] > correct[cMax])
                    cMax = i;
            }
            return oMax == cMax;
        }
    }
}
