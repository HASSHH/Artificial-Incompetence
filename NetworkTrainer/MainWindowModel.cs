using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    class MainWindowModel : INotifyPropertyChanged
    {
        private int trainingDataCount;
        private int testingDataCount;
        private string trainerType = "---";
        private string consoleString = string.Empty;
        private string hiddenLayers = "30 15";
        private string learningRate = "3";
        private string epochs = "30";
        private string batchSize = "10";
        private bool useLoadedNN = false;
        private bool trainingActive = false;
        private string loadedNNName = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public int TrainingDataCount { get => trainingDataCount; set { trainingDataCount = value; OnPropertyChanged("TrainingDataCount"); } }
        public int TestingDataCount { get => testingDataCount; set { testingDataCount = value; OnPropertyChanged("TestingDataCount"); } }
        public string ConsoleString { get => consoleString; set { consoleString = value; OnPropertyChanged("ConsoleString"); } }
        public string HiddenLayers { get => hiddenLayers; set { hiddenLayers = value; OnPropertyChanged("HiddenLayers"); } }
        public string LearningRate { get => learningRate; set { learningRate = value; OnPropertyChanged("LearningRate"); } }
        public string Epochs { get => epochs; set { epochs = value; OnPropertyChanged("Epochs"); } }
        public string BatchSize { get => batchSize; set { batchSize = value; OnPropertyChanged("BatchSize"); } }
        public bool UseLoadedNN { get => useLoadedNN; set { useLoadedNN = value; OnPropertyChanged("UseLoadedNN"); } }
        public bool TrainingActive { get => trainingActive; set { trainingActive = value; OnPropertyChanged("TrainingActive"); } }
        public string LoadedNNName { get => loadedNNName; set { loadedNNName = value; OnPropertyChanged("LoadedNNName"); } }
        public string TrainerType { get => trainerType; set { trainerType = value; OnPropertyChanged("TrainerType"); }
}

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
