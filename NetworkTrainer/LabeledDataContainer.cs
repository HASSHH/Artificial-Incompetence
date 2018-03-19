using Accord.IO;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    class LabeledDataContainer<T> : ILabeledDataContainer
    {
        protected T[][] trainingSet;
        protected T[][] trainingLabels;
        protected T[][] testingSet;
        protected T[][] testingLabels;

        protected int inputDataSize = 0;
        protected int outputDataSize = 0;
        protected bool autoReadDataSize = true;

        static LabeledDataContainer()
        {
            bytesPerType[IdxDataType.UnsignedByte] = 1;
            bytesPerType[IdxDataType.SignedByte] = 1;
            bytesPerType[IdxDataType.Short] = 2;
            bytesPerType[IdxDataType.Integer] = 4;
            bytesPerType[IdxDataType.Float] = 4;
            bytesPerType[IdxDataType.Double] = 8;
        }

        public T[][] TrainingSet => trainingSet;
        public T[][] TrainingLabels => trainingLabels;
        public T[][] TestingSet => testingSet;
        public T[][] TestingLabels => testingLabels;

        public double MaxOutputDistance { get; set; } = 0.05d;

        public int GetInputDataSize()
        {
            return inputDataSize;
        }
        public int GetOutputDataSize()
        {
            return outputDataSize;
        }

        public int GetTrainingSetSize()
        {
            return trainingSet == null ? 0 : trainingSet.Length;
        }

        public int GetTestingSetSize()
        {
            return testingSet == null ? 0 : testingSet.Length;
        }

        public void LoadTrainingData(string dataFile, string labelFile)
        {
            LoadData(dataFile, labelFile, out trainingSet, out trainingLabels);
            //read input output neuron count
            if (autoReadDataSize)
            {
                IdxReader dataReader = new IdxReader(dataFile);
                IdxReader labelsReader = new IdxReader(labelFile);
                inputDataSize = 1;
                foreach (int dimSize in dataReader.Dimensions)
                    inputDataSize *= dimSize;
                outputDataSize = 1;
                foreach (int dimSize in labelsReader.Dimensions)
                    outputDataSize *= dimSize;
            }
        }

        public void LoadTestingData(string dataFile, string labelFile)
        {
            LoadData(dataFile, labelFile, out testingSet, out testingLabels);
        }

        public LabeledData[] TrainingSetAsLabeledDataArray()
        {
            LabeledData[] resultArray = new LabeledData[trainingSet.Length];
            for (int i = 0; i < resultArray.Length; ++i)
                resultArray[i] = DataAsLabeledData(trainingSet[i], trainingLabels[i]);
            return resultArray;
        }
        public LabeledData[] TestingSetAsLabeledDataArray()
        {
            LabeledData[] resultArray = new LabeledData[testingSet.Length];
            for (int i = 0; i < resultArray.Length; ++i)
                resultArray[i] = DataAsLabeledData(testingSet[i], testingLabels[i]);
            return resultArray;
        }

        public virtual bool CheckIfOutputIsCorrect(double[] output, double[] correct)
        {
            double distanceSum = 0;
            for (int i = 0; i < output.Length; ++i)
                distanceSum += Math.Abs(output[i] - correct[i]);
            double avgDistance = distanceSum / output.Length;
            return avgDistance <= MaxOutputDistance;
        }

        /// <summary>
        /// Override only when overriding the normalization function isn't enough.
        /// </summary>
        protected virtual LabeledData DataAsLabeledData(T[] data, T[] label)
        {
            double[] input = new double[data.Length];
            for (int i = 0; i < input.Length; ++i)
                input[i] = NormalizeData(data[i]);
            double[] output = new double[label.Length];
            for (int i = 0; i < output.Length; ++i)
                output[i] = NormalizeData(label[i]);
            return LabeledData.MakeFromArrays(input, output);
        }

        /// <summary>
        /// Override with the respective function to bring inputs in the [0,1] range.
        /// </summary>
        protected virtual double NormalizeData(T input)
        {
            return Convert.ToDouble(input);
        }

        protected void LoadData(string dataFile, string labelFile, out T[][] data, out T[][] labels)
        {
            data = LoadSingleFile(dataFile);
            labels = LoadSingleFile(labelFile);
        }

        protected T[][] LoadSingleFile(string dataFile)
        {
            IdxReader dataReader = new IdxReader(dataFile);
            int bpt = bytesPerType[dataReader.DataType];
            int sampleValuesCount = 1;
            foreach (int dim in dataReader.Dimensions)
                sampleValuesCount *= dim;
            int sampleSize = sampleValuesCount * bpt;
            byte[][] source = dataReader.ReadToEndAsVectors<byte>();
            byte[] tempData = new byte[source.Length * sampleSize];
            for(int i = 0; i < source.Length; ++i)
                Buffer.BlockCopy(source[i], 0, tempData, sampleSize*i, sampleSize);
            T[][] data = new T[tempData.Length / sampleSize][];
            for (int i = 0; i < data.Length; ++i)
            {
                int offset = sampleSize * i;
                data[i] = new T[sampleValuesCount];
                Buffer.BlockCopy(tempData, offset, data[i], 0, sampleSize);
            }
            return data;
        }

        protected static readonly Dictionary<IdxDataType, int> bytesPerType = new Dictionary<IdxDataType, int>();
    }
}
