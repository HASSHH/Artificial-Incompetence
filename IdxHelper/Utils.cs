using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdxHelper
{
    public class Utils
    {
        public static readonly Dictionary<IdxDataType, int> BytesPerDataType = new Dictionary<IdxDataType, int>();

        static Utils()
        {
            BytesPerDataType[IdxDataType.UByte] = 1;
            BytesPerDataType[IdxDataType.SByte] = 1;
            BytesPerDataType[IdxDataType.Short] = 2;
            BytesPerDataType[IdxDataType.Int] = 4;
            BytesPerDataType[IdxDataType.Float] = 4;
            BytesPerDataType[IdxDataType.Double] = 8;
        }

        public static int ChangeEndian(int x)
        {
            byte[] data = BitConverter.GetBytes(x);
            return IntFromBytesBigEndian(data);
        }

        public static int IntFromBytesBigEndian(byte[] data)
        {
            int y = (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3];
            return y;
        }

    }
}
