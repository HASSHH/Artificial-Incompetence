using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    class LoadIdxDataWindowModel : INotifyPropertyChanged
    {
        private string trainingData;
        private string trainingLabels;
        private string testingData;
        private string testingLabels;

        public event PropertyChangedEventHandler PropertyChanged;

        public string TrainingData { get => trainingData; set { trainingData = value; OnPropertyChanged("TrainingData"); } }
        public string TrainingLabels { get => trainingLabels; set { trainingLabels = value; OnPropertyChanged("TrainingLabels"); } }
        public string TestingData { get => testingData; set { testingData = value; OnPropertyChanged("TestingData"); } }
        public string TestingLabels { get => testingLabels; set { testingLabels = value; OnPropertyChanged("TestingLabels"); } }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
