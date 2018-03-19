using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdxHelper
{
    public class IdxReader
    {
        private int magicNumber;
        private int[] dimensions;
        private int samples;
        private IdxDataType dataType;
        private byte[] data;

        public IdxReader(string fileName, bool isGzip = false)
        {
            using(FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Stream sourceStream = fs;
                if (isGzip)
                {
                    GZipStream gzip = new GZipStream(fs, CompressionMode.Decompress);
                    sourceStream = gzip;
                }
                byte[] buffer = new byte[4];
                sourceStream.Read(buffer, 0, 4);
                try
                {
                    dataType = (IdxDataType)buffer[2];
                }
                catch
                {
                    throw new NotAValidDataTypeException("Data type byte does not represent a known data type.");
                }
                int dimCount = buffer[3] - 1;
                dimensions = new int[dimCount];
                magicNumber = Utils.IntFromBytesBigEndian(buffer);
                sourceStream.Read(buffer, 0, 4);
                samples = Utils.IntFromBytesBigEndian(buffer);
                for(int i = 0; i < dimCount; ++i)
                {
                    sourceStream.Read(buffer, 0, 4);
                    dimensions[i] = Utils.IntFromBytesBigEndian(buffer);
                }
                //read data
                int sampleValuesCount = 1;
                foreach (int dim in dimensions)
                    sampleValuesCount *= dim;
                int sampleSizeInBytes = sampleValuesCount * Utils.BytesPerDataType[dataType];
                data = new byte[sampleSizeInBytes * samples];
                try
                {
                    sourceStream.Read(data, 0, data.Length);
                }
                catch
                {
                    throw new MismatchingFileSizeException($"Couldn't read data. Number of data bytes expected:{data.Length}");
                }

                sourceStream.Dispose();
            }
        }

        public int MagicNumber => magicNumber;
        public int[] Dimensions => dimensions;
        public int Samples => samples;
        public IdxDataType DataType => dataType;
        public byte[] Data => data;

        public T[][] GetSamples<T>()
        {
            int sampleValuesCount = 1;
            foreach (int dim in dimensions)
                sampleValuesCount *= dim;
            int sampleSizeInBytes = sampleValuesCount * Utils.BytesPerDataType[dataType];
            T[][] result = new T[samples][];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new T[sampleValuesCount];
                Buffer.BlockCopy(data, sampleSizeInBytes * i, result[i], 0, sampleSizeInBytes);
            }
            return result;
        }
    }
}
