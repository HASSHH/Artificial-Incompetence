using IdxHelper;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    class LabeledDataContainer<T, L> : ILabeledDataContainer
    {
        protected T[][] trainingSet;
        protected L[][] trainingLabels;
        protected T[][] testingSet;
        protected L[][] testingLabels;

        protected int inputDataSize = 0;
        protected int outputDataSize = 0;
        protected bool autoReadDataSize = true;
        protected bool isDataCompressed = false;

        public T[][] TrainingSet => trainingSet;
        public L[][] TrainingLabels => trainingLabels;
        public T[][] TestingSet => testingSet;
        public L[][] TestingLabels => testingLabels;

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
        /// Override only when overriding the normalization functions isn't enough.
        /// </summary>
        protected virtual LabeledData DataAsLabeledData(T[] data, L[] label)
        {
            double[] input = NormalizeInputData(data);
            double[] output = NormalizeOutputData(label);
            return LabeledData.MakeFromArrays(input, output);
        }

        /// <summary>
        /// Override with the respective function to bring inputs in the [0,1] range.
        /// </summary>
        protected virtual double[] NormalizeInputData(T[] input)
        {
            double[] result = new double[input.Length];
            for (int i = 0; i < result.Length; ++i)
                result[i] = Convert.ToDouble(input[i]);
            return result;
        }

        /// <summary>
        /// Override with the respective function to bring output in the [0,1] range.
        /// </summary>
        protected virtual double[] NormalizeOutputData(L[] output)
        {
            double[] result = new double[output.Length];
            for (int i = 0; i < result.Length; ++i)
                result[i] = Convert.ToDouble(output[i]);
            return result;
        }

        protected void LoadData(string dataFile, string labelFile, out T[][] data, out L[][] labels)
        {
            data = LoadSingleFile<T>(dataFile);
            labels = LoadSingleFile<L>(labelFile);
        }

        protected V[][] LoadSingleFile<V>(string dataFile)
        {
            IdxReader dataReader = new IdxReader(dataFile, isDataCompressed);
            return dataReader.GetSamples<V>();
        }
    }
}
