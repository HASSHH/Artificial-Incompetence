using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class MismatchingInputValuesCountException : Exception
    {
        public MismatchingInputValuesCountException(string message) : base(message)
        {
            //
        }
    }

    public class BadFileFormatException : Exception
    {
        public BadFileFormatException(string message) : base(message)
        {
            //
        }
    }
}
