using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// A feedforward neural network.
    /// </summary>
    public class DeepNeuralNetwork
    {
        internal InputLayer InputLayer;
        internal OutputLayer OutputLayer;
        internal HiddenLayer[] HiddenLayers;

        /// <summary>
        /// A short handle list containing the hidden layers plus the output layer.
        /// </summary>
        internal ComputedLayer[] ComputedLayers;

        /// <summary>
        /// Constructs the structure of the NN with randomized weights and biasses.
        /// </summary>
        /// <param name="inputCount">number of input neurons</param>
        /// <param name="outputCount">number of output neurons</param>
        /// <param name="hiddenCount">an array of neurons count for the hidden layers</param>
        public DeepNeuralNetwork(int inputCount, int outputCount, int[] hiddenCount)
        {
            InputLayer = new InputLayer(inputCount);
            int prevLayerNeuronCount = inputCount;
            HiddenLayers = new HiddenLayer[hiddenCount.Length];
            for (int i = 0; i < hiddenCount.Length; ++i)
            {
                HiddenLayers[i] = new HiddenLayer(hiddenCount[i], prevLayerNeuronCount);
                prevLayerNeuronCount = hiddenCount[i];
            }
            OutputLayer = new OutputLayer(outputCount, prevLayerNeuronCount);
            List<ComputedLayer> compLayers = new List<ComputedLayer>();
            compLayers.AddRange(HiddenLayers);
            compLayers.Add(OutputLayer);
            ComputedLayers = compLayers.ToArray();
        }

        /// <summary>
        /// Constructs the NN from a given file.
        /// </summary>
        /// <param name="fileName"></param>
        public DeepNeuralNetwork(string fileName)
        {
            try
            {
                using(StreamReader reader = new StreamReader(fileName))
                {
                    string[] layerTokens = reader.ReadLine().Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    int[] layerCount = new int[layerTokens.Length];
                    for (int i = 0; i < layerTokens.Length; ++i)
                        layerCount[i] = int.Parse(layerTokens[i]);
                    InputLayer = new InputLayer(layerCount[0]);
                    //hidden layers
                    HiddenLayers = new HiddenLayer[layerCount.Length - 2];
                    for(int i = 1; i < layerCount.Length - 1; ++i)
                    {
                        var storage = ReadLayerFromFile(reader, layerCount[i]);
                        HiddenLayers[i - 1] = new HiddenLayer(layerCount[i], layerCount[i - 1], storage.w, storage.b);
                    }
                    //output layer
                    var stor = ReadLayerFromFile(reader, layerCount[layerCount.Length - 1]);
                    OutputLayer = new OutputLayer(layerCount[layerCount.Length - 1], layerCount[layerCount.Length - 2], stor.w, stor.b);

                    List<ComputedLayer> compLayers = new List<ComputedLayer>();
                    compLayers.AddRange(HiddenLayers);
                    compLayers.Add(OutputLayer);
                    ComputedLayers = compLayers.ToArray();
                }
            }
            catch
            {
                throw new BadFileFormatException("Bad file format.");
            }
        }

        /// <summary>
        /// Processes the input data and returns te output values.
        /// </summary>
        /// <param name="inputData">the input values (length must match input layer neuron count)</param>
        /// <returns></returns>
        public Vector<double> ProcessInput(Vector<double> inputData)
        {
            if (inputData.Count != InputLayer.GetNeuronCount())
                throw new MismatchingInputValuesCountException($"Expected {InputLayer.GetNeuronCount()} input values, got {inputData.Count}.");
            //set neuron values for input layer
            InputLayer.Activation = inputData;
            //keep track of the last processed layer
            Layer lastLayerDone = InputLayer;
            //hidden layers and the output layer will be processed the same
            foreach(ComputedLayer layer in ComputedLayers)
            {
                //a = sig(aprev*w + b)
                layer.Activation = Utils.Sigmoid(layer.Weights * lastLayerDone.Activation + layer.Biasses);
                lastLayerDone = layer;
            }
            //return output values
            return OutputLayer.Activation;
        }

        public double[] ProcessInput(double[] inputData)
        {
            return ProcessInput(new DenseVector(inputData)).AsArray();
        }

        public int[] GetLayerCounts()
        {
            int[] ls = new int[HiddenLayers.Length + 2];
            ls[0] = InputLayer.GetNeuronCount();
            ls[ls.Length - 1] = OutputLayer.GetNeuronCount();
            for (int i = 0; i < HiddenLayers.Length; ++i)
                ls[i + 1] = HiddenLayers[i].GetNeuronCount();
            return ls;
        }

        public void SaveToFile(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            string layers = InputLayer.GetNeuronCount().ToString() + " ";
            foreach (HiddenLayer hl in HiddenLayers)
                layers += hl.GetNeuronCount().ToString() + " ";
            layers += OutputLayer.GetNeuronCount().ToString();
            sb.AppendLine(layers);
            foreach (ComputedLayer cl in ComputedLayers)
            {
                sb.Append(cl.Weights.ToMatrixString(int.MaxValue, 0, int.MaxValue, 0, "", "", "", " ", Environment.NewLine, x => x.ToString()));
                sb.AppendLine(cl.Biasses.ToVectorString(int.MaxValue, int.MaxValue, "", " ",  " ", x => x.ToString()));
            }
            string temp = sb.ToString();
            File.WriteAllText(fileName, sb.ToString());
        }

        private (double[] w, double[] b) ReadLayerFromFile(StreamReader reader, int nc)
        {
            List<double> ww = new List<double>();
            for(int i =0; i < nc; ++i)
            {
                string[] wTokens = reader.ReadLine().Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                foreach (string token in wTokens)
                    ww.Add(double.Parse(token));
            }
            List<double> bb = new List<double>();
            string[] bTokens = reader.ReadLine().Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            foreach (string token in bTokens)
                bb.Add(double.Parse(token));
            return (ww.ToArray(), bb.ToArray());
        }
    }
}
