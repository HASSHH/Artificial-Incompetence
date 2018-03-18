using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public delegate bool CheckIfCorrectDelegate(double[] networkOutput, double[] correctOutput);

    public class Trainer
    {
        /// <summary>
        /// Stochastic gradient descent algorithm.
        /// </summary>
        public static void Sgd(DeepNeuralNetwork nn, LabeledData[] trainingData, int batchSize, int epochs, double learningRate, 
            UpdateTrainingStatusDelegate statusUpdate, LabeledData[] testingData = null, CheckIfCorrectDelegate checkCorrect = null)
        {
            if (testingData == null || checkCorrect == null)
                SgdLazy(nn, trainingData.Length, getTrainingData, batchSize, epochs, learningRate, statusUpdate);
            else
                SgdLazy(nn, trainingData.Length, getTrainingData, batchSize, epochs, learningRate, statusUpdate, testingData.Length, getTestingData, checkCorrect);

            LabeledData getTrainingData(int index)
            {
                return trainingData[index];
            }

            LabeledData getTestingData(int index)
            {
                return testingData[index];
            }
        }

        /// <summary>
        /// Stochastic gradient descent algorithm with lazy data loading.
        /// </summary>
        public static void SgdLazy(DeepNeuralNetwork nn, int trainingDataSetSize, GetNextDataDelegate getTrainingData, 
            int batchSize, int epochs, double learningRate, UpdateTrainingStatusDelegate statusUpdate,
            int testDataSetSize = 0, GetNextDataDelegate getTestingData = null, CheckIfCorrectDelegate checkCorrect = null)
        {
            Random rng = new Random();
            DateTime startingTime = DateTime.Now;
            int[] dataSetIndexes = new int[trainingDataSetSize];
            for (int i = 0; i < dataSetIndexes.Length; ++i)
                dataSetIndexes[i] = i;
            int currentBatchOffset;
            int currentBatchSize;
            for (int epoch = 1; epoch <= epochs; ++epoch)
            {
                dataSetIndexes.Shuffle(rng);
                currentBatchOffset = 0;
                int currentBatch = 0;
                for (; currentBatchOffset < trainingDataSetSize; currentBatchOffset += batchSize, ++currentBatch)
                {
                    int remainingDataSize = trainingDataSetSize - currentBatchOffset;
                    currentBatchSize = remainingDataSize < batchSize ? remainingDataSize : batchSize;
                    TrainWithBatchLazy(nn, currentBatchSize, getData, learningRate);
                }
                TrainingStatus status = new TrainingStatus
                {
                    EpochsDone = epoch
                };
                if (testDataSetSize != 0 && getTestingData != null && checkCorrect != null)
                {
                    double errorRate = TestNetwork(nn, testDataSetSize, getTestingData, out int correctCount, checkCorrect);
                    status.Error = errorRate;
                    status.Correct = correctCount;
                }

                //status update
                TimeSpan elapsed = DateTime.Now - startingTime;
                TimeSpan remaining = TimeSpan.FromTicks((long)(elapsed.Ticks * ((epochs - epoch) / (double)epoch)));
                status.ElapsedTime = elapsed;
                status.TimeLeft = remaining;
                statusUpdate(status);
            }

            LabeledData getData(int indexInBatch)
            {
                int actualIndex = dataSetIndexes[currentBatchOffset + indexInBatch];
                return getTrainingData(actualIndex);
            }
        }

        /// <summary>
        /// Train the network using the given training set. The starting weights and biasses are the ones present in the network when the call to this function is made.
        /// </summary>
        public static void TrainWithBatch(DeepNeuralNetwork nn, IEnumerable<LabeledData> batch, int batchSize, double learningRate)
        {
            Matrix<double>[] nablaW;
            Vector<double>[] nablaB;
            (nablaW, nablaB) = GenParamsZero(nn);

            foreach (LabeledData trainingData in batch)
            {
                var delta = BackProp(nn, trainingData);
                for (int i = 0; i < nn.ComputedLayers.Length; ++i)
                {
                    nablaW[i] += delta.nablaW[i];
                    nablaB[i] += delta.nablaB[i];
                }
            }
            double ratio = learningRate / batchSize;
            for (int i = 0; i < nn.ComputedLayers.Length; ++i)
            {
                ComputedLayer layer = nn.ComputedLayers[i];
                layer.Weights = layer.Weights - ratio * nablaW[i];
                layer.Biasses = layer.Biasses - ratio * nablaB[i];
            }
        }

        /// <summary>
        /// Train the network using the given training set. The starting weights and biasses are the ones present in the network when the call to this function is made.
        /// This "lazy" version means that each training value pair will be read (using the function given as a parameter) only when it has to be processed.
        /// Recomended when the size of one training data is very large and loading the whole batch would need too much memory.
        /// Obs.: The lazy loading of the data should be ensured by the function given.
        /// </summary>
        public static void TrainWithBatchLazy(DeepNeuralNetwork nn, int batchSize, GetNextDataDelegate getNextData, double learningRate)
        {
            LazyBatch batch = new LazyBatch(getNextData, batchSize);
            TrainWithBatch(nn, batch, batchSize, learningRate);
        }

        /// <summary>
        /// Tests the network over a given test dataset. Returns the error ( sum(|a - y(x)|^2)/n ). The out param will count the data that was correctly categorized using a given function. 
        /// </summary>
        private static double TestNetwork(DeepNeuralNetwork nn, int testingDataSetSize, GetNextDataDelegate getNextData, out int correctCount, CheckIfCorrectDelegate checkCorrect)
        {
            correctCount = 0;
            Vector<double> error = new DenseVector(nn.OutputLayer.GetNeuronCount());
            for(int i = 0; i < testingDataSetSize; ++i)
            {
                LabeledData labeledData = getNextData(i);
                Vector<double> result = nn.ProcessInput(labeledData.InputValues);
                if (checkCorrect(result.AsArray(), labeledData.OutputValues.AsArray()))
                    ++correctCount;
                Vector<double> diff = labeledData.OutputValues - result;
                error += diff.PointwiseMultiply(diff);
            }
            error = error.Divide(testingDataSetSize);
            return error.Average();
        }

        private static (Matrix<double>[] nablaW, Vector<double>[] nablaB) BackProp(DeepNeuralNetwork nn, LabeledData trainingData)
        {
            Matrix<double>[] nablaW;
            Vector<double>[] nablaB;
            (nablaW, nablaB) = GenParamsZero(nn);

            //activation before applying sigm function
            Vector<double> z;
            List<Vector<double>> zs = new List<Vector<double>>();
            //activation vector
            Vector<double> a = trainingData.InputValues;
            List<Vector<double>> activations = new List<Vector<double>>();
            activations.Add(a);
            //feedforward
            foreach (ComputedLayer layer in nn.ComputedLayers)
            {
                z = layer.Weights * a + layer.Biasses;
                zs.Add(z);
                a = Utils.Sigmoid(z);
                activations.Add(a);
            }
            //backward pass
            Vector<double> delta = CostDerivative(activations.Last(), trainingData.OutputValues).PointwiseMultiply(Utils.SigmoidPrime(zs.Last()));
            nablaB[nn.ComputedLayers.Length - 1] = delta;
            nablaW[nn.ComputedLayers.Length - 1] = delta.ToColumnMatrix() * activations[activations.Count - 2].ToRowMatrix();
            for (int i = nn.ComputedLayers.Length - 2; i >= 0; --i)
            {
                delta = nn.ComputedLayers[i + 1].Weights.TransposeThisAndMultiply(delta).PointwiseMultiply(Utils.SigmoidPrime(zs[i]));
                nablaB[i] = delta;
                nablaW[i] = delta.ToColumnMatrix() * activations[i].ToRowMatrix();
                //note: activations[i] is actualy the activation of the previous layer since it counts the input layer as well
            }
            return (nablaW, nablaB);
        }

        /// <summary>
        /// Creates matrices (for weights) and vectors (for biasses) initialized with 0 values coresponding to every computed layer of the NN.
        /// </summary>
        private static (Matrix<double>[] nablaW, Vector<double>[] nablaB) GenParamsZero(DeepNeuralNetwork nn)
        {
            Matrix<double>[] nablaW = new Matrix<double>[nn.ComputedLayers.Length];
            Vector<double>[] nablaB = new Vector<double>[nn.ComputedLayers.Length];
            for (int i = 0; i < nn.ComputedLayers.Length; ++i)
            {
                ComputedLayer layer = nn.ComputedLayers[i];
                nablaW[i] = new DenseMatrix(layer.Weights.RowCount, layer.Weights.ColumnCount);
                nablaB[i] = new DenseVector(layer.Biasses.Count);
            }
            return (nablaW, nablaB);
        }

        private static Vector<double> CostDerivative(Vector<double> outputValues, Vector<double> correctValues)
        {
            return outputValues - correctValues;
        }
    }
}
