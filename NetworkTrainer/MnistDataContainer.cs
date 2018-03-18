using Accord.IO;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    class MnistDataContainer : LabeledDataContainer<byte>
    {
        public MnistDataContainer()
        {
            autoReadDataSize = false;
            inputDataSize = 784;
            outputDataSize = 10;
        }

        protected override LabeledData DataAsLabeledData(byte[] data, byte[] label)
        {
            double[] input = new double[data.Length];
            for (int i = 0; i < input.Length; ++i)
                input[i] = NormalizeData(data[i]);
            int digit = label[0];
            double[] output = new double[10];
            for (int i = 0; i < 10; ++i)
                output[i] = i == digit ? 1d : 0d;
            return LabeledData.MakeFromArrays(input, output);
        }

        protected override double NormalizeData(byte input)
        {
            return Convert.ToDouble(input) / 255d;
        }

        private LabeledData DataAsLabeledData(byte[] data, byte label)
        {
            double[] input = new double[data.Length];
            //normalization
            for (int i = 0; i < input.Length; ++i)
                input[i] = Convert.ToDouble(data[i]) / 255d;
            int digit = Convert.ToInt32(label);
            double[] output = new double[10];
            for (int i = 0; i < 10; ++i)
                output[i] = i == digit ? 1d : 0d;
            return LabeledData.MakeFromArrays(input, output);
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

        private void LoadData(string dataFile, string labelFile, out byte[][] data, out byte[] labels)
        {
            IdxReader dataReader = new IdxReader(dataFile);
            data = dataReader.ReadToEndAsVectors<byte>();
            IdxReader labelsReader = new IdxReader(labelFile);
            labels = labelsReader.ReadToEndAsValues<byte>();
        }
    }
}
