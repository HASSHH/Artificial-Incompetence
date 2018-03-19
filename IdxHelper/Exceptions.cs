using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdxHelper
{
    public class NotAValidDataTypeException : Exception
    {
        public NotAValidDataTypeException(string message) : base(message)
        {
            //
        }
    }

    public class MismatchingFileSizeException : Exception
    {
        public MismatchingFileSizeException(string message) : base(message)
        {
            //
        }
    }
}
