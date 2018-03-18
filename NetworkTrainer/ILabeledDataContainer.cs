using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    interface ILabeledDataContainer
    {
        int GetInputDataSize();
        int GetOutputDataSize();
        int GetTrainingSetSize();
        int GetTestingSetSize();

        void LoadTrainingData(string dataFile, string labelFile);
        void LoadTestingData(string dataFile, string labelFile);

        LabeledData[] TrainingSetAsLabeledDataArray();
        LabeledData[] TestingSetAsLabeledDataArray();
        
        bool CheckIfOutputIsCorrect(double[] output, double[] correct);
    }
}
