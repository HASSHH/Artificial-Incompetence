using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTrainer
{
    class AudioRpmDataContainer : LabeledDataContainer<double, short>
    {
        private int maxRpm = 20000;
        private int minRpm = 0;
        private int acceptedRpmError = 1000;

        protected override double[] NormalizeOutputData(short[] output)
        {
            //using sigmoid on the normalized -1 1 data
            //normalize first
            double rawRpm = output[1];
            double normalizedRpm = (rawRpm - minRpm) * 2d / (maxRpm - minRpm) - 1d;
            //apply sigmoid
            double finalRpm = 1d / (1d + Math.Exp(-normalizedRpm));
            return new double[] { output[0], finalRpm };
        }
        
        public override bool CheckIfOutputIsCorrect(double[] output, double[] correct)
        {
            if (correct[0] < 0.5d && output[0] < 0.5)
                return true;
            if (correct[0] < 0.5d && output[0] >= 0.5d || correct[0] >= 0.5d && output[0] < 0.5d)
                return false;
            double outputRpm = ExtractFromSigmoid(output[1]);
            double correctRpm = ExtractFromSigmoid(correct[1]);
            //denormalize
            outputRpm = (outputRpm + 1d) / 2d * (maxRpm - minRpm) + minRpm;
            correctRpm = (correctRpm + 1d) / 2d * (maxRpm - minRpm) + minRpm;
            return (Math.Abs(outputRpm - correctRpm) <= acceptedRpmError);
        }

        private double Sigmoid(double value)
        {
            return 1d / (1d + Math.Exp(-value));
        }

        private double ExtractFromSigmoid(double sig)
        {
            return -Math.Log(1d / sig - 1);
        }
    }
}
