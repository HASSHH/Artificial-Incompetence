using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// Contains info about the status of the training stage: epochs done in current epoch, etc.
    /// </summary>
    public class TrainingStatus
    {
        public int EpochsDone { get; set; } = -1;

        public TimeSpan ElapsedTime { get; set; } = TimeSpan.MinValue;

        public TimeSpan TimeLeft { get; set; } = TimeSpan.MaxValue;

        public double Error { get; set; } = -1;

        public int Correct { get; set; } = -1;
    }

    public delegate void UpdateTrainingStatusDelegate(TrainingStatus status);
}
