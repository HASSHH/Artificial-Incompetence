using Accord.IO;
using Microsoft.Win32;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkTrainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DeepNeuralNetwork neuralNetwork;
        private DeepNeuralNetwork loadedNetwork;
        private ILabeledDataContainer dataContainer;
        private MainWindowModel model;

        private int _epochs;
        private string _tempCsvFileName = "temp_stats.csv";

        public MainWindow()
        {
            InitializeComponent();
            model = DataContext as MainWindowModel;
        }

        private void LoadMnistDataButton_Click(object sender, RoutedEventArgs e)
        {
            LoadIdxDataWindow ldw = new LoadIdxDataWindow { Title = "Load MNIST data"};
            if (ldw.ShowDialog().GetValueOrDefault())
            {
                LoadIdxDataWindowModel dc = ldw.DataContext as LoadIdxDataWindowModel;
                dataContainer = new MnistDataContainer();
                LoadData(dc.TrainingData, dc.TrainingLabels, dc.TestingData, dc.TestingLabels);
                model.TrainerType = "MNIST trainer";
            }
        }

        private void LoadGenericIdxDataButton_Click(object sender, RoutedEventArgs e)
        {
            LoadIdxDataWindow ldw = new LoadIdxDataWindow { Title = "Load generic IDX data" };
            if (ldw.ShowDialog().GetValueOrDefault())
            {

                LoadIdxDataWindowModel dc = ldw.DataContext as LoadIdxDataWindowModel;
                IdxReader dataReader = new IdxReader(dc.TrainingData);
                switch (dataReader.DataType)
                {
                    case IdxDataType.UnsignedByte:
                        dataContainer = new LabeledDataContainer<byte>();
                        break;
                    case IdxDataType.Short:
                        dataContainer = new LabeledDataContainer<short>();
                        break;
                    case IdxDataType.Integer:
                        dataContainer = new LabeledDataContainer<int>();
                        break;
                    case IdxDataType.Float:
                        dataContainer = new LabeledDataContainer<float>();
                        break;
                    default:
                        dataContainer = new LabeledDataContainer<double>();
                        break;
                }
                LoadData(dc.TrainingData, dc.TrainingLabels, dc.TestingData, dc.TestingLabels);
                model.TrainerType = "Generic trainer";
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (model.TrainingActive)
            {
                MessageBox.Show("Training is already active.");
                return;
            }
            if(dataContainer == null)
            {
                MessageBox.Show("No data loaded.");
                return;
            }
            model.TrainingActive = true;
            //prepare network
            if (model.UseLoadedNN && loadedNetwork != null &&
                loadedNetwork.GetLayerCounts()[0] == dataContainer.GetInputDataSize() && loadedNetwork.GetLayerCounts().Last() == dataContainer.GetOutputDataSize())
                neuralNetwork = loadedNetwork;
            else
            {
                string[] tokens = model.HiddenLayers.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                List<int> hlList = new List<int>();
                foreach (string token in tokens)
                    if (int.TryParse(token, out int hl))
                        hlList.Add(hl);
                neuralNetwork = new DeepNeuralNetwork(dataContainer.GetInputDataSize(), dataContainer.GetOutputDataSize(), hlList.ToArray());
            }
            //hyper-params
            int epochs = 0, batchSize = 0;
            double learningRate = 0;
            bool okToStart = true;
            okToStart = okToStart && int.TryParse(model.Epochs, out epochs);
            okToStart = okToStart && int.TryParse(model.BatchSize, out batchSize);
            okToStart = okToStart && double.TryParse(model.LearningRate, out learningRate);
            if (!okToStart)
                return;
            _epochs = epochs;
            string layerString = string.Empty;
            foreach (int ls in neuralNetwork.GetLayerCounts())
                layerString += ls.ToString() + " ";
            model.ConsoleString += $"{layerString} - epochs = {epochs}, batch size = {batchSize}, learning rate = {learningRate}\n\n\n";
            //clear temp csv file
            File.Delete(_tempCsvFileName);
            //train
            await Task.Run(() =>
            Trainer.Sgd(neuralNetwork, dataContainer.TrainingSetAsLabeledDataArray(), batchSize, epochs, learningRate,
                UpdateTrainingStatus, dataContainer.TestingSetAsLabeledDataArray(), dataContainer.CheckIfOutputIsCorrect));
            MessageBox.Show("Training done.");
            model.TrainingActive = false;
        }

        private void LoadNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.DefaultExt = ".nn";
                ofd.Filter = "Neural Network definition files (.nn)|*.nn";
                if (ofd.ShowDialog().GetValueOrDefault())
                {
                    loadedNetwork = new DeepNeuralNetwork(ofd.FileName);
                    string layerString = string.Empty;
                    foreach (int ls in loadedNetwork.GetLayerCounts())
                        layerString += ls.ToString() + " ";
                    model.LoadedNNName = System.IO.Path.GetFileName(ofd.FileName) + " ( " + layerString + ")";
                }
            }
            catch
            {
                MessageBox.Show("Error loading file.");
            }
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            if (neuralNetwork == null)
            {
                MessageBox.Show("No network to be saved.");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Network";
            sfd.DefaultExt = ".nn";
            sfd.Filter = "Neural Network definition files (.nn)|*.nn";
            if (sfd.ShowDialog().GetValueOrDefault())
            {
                neuralNetwork.SaveToFile(sfd.FileName);
            }
        }

        private void UpdateTrainingStatus(TrainingStatus status)
        {
            string string1 = $"epoch: {status.EpochsDone}/{_epochs} ({status.Correct}/{dataContainer.GetTestingSetSize()} - e: {status.Error})\n";
            string string2 = $"elapsed {status.ElapsedTime.ToString()}, remaining {status.TimeLeft.ToString()}\n";
            //save nn to temp.nn
            string fileName = "temp.nn";
            neuralNetwork.SaveToFile(fileName);
            string string3 = $"Saved nn state to: \"{fileName}\"\n";
            model.ConsoleString += string1 + string2 + string3 + "\n";
            Dispatcher.Invoke(() => ConsoleTextBox.ScrollToEnd());
            //save accuracy and error into a csv file
            using (StreamWriter sw = File.AppendText(_tempCsvFileName))
            {
                sw.WriteLine($"{(double)status.Correct / dataContainer.GetTestingSetSize()},{status.Error}");
            }
        }

        private void LoadData(string trainD, string trainL, string testD, string testL)
        {
            dataContainer.LoadTrainingData(trainD, trainL);
            model.TrainingDataCount = dataContainer.GetTrainingSetSize();
            dataContainer.LoadTestingData(testD, testL);
            model.TestingDataCount = dataContainer.GetTestingSetSize();
        }
    }
}
