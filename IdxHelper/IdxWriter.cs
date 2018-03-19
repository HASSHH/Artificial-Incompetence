using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdxHelper
{
    public class IdxWriter
    {
        public static void Write(string fileName, byte[] data, int samples, IdxDataType dataType, int dimensionsCount, int[] dimensions)
        {
            using (FileStream writer = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                WriteHead(writer, samples, dataType, dimensionsCount, dimensions);
                writer.Write(data, 0, data.Length);
            }
        }
        public static void Write(string fileName, List<byte[]> data, int samples, IdxDataType dataType, int dimensionsCount, int[] dimensions)
        {
            using (FileStream writer = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                WriteHead(writer, samples, dataType, dimensionsCount, dimensions);
                foreach(byte[] sample in data)
                    writer.Write(sample, 0, sample.Length);
            }
        }

        private static void WriteHead(FileStream writer, int samples, IdxDataType dataType, int dimensionsCount, int[] dimensions)
        {
            writer.Write(BitConverter.GetBytes(GenMagicNumber(dataType, dimensionsCount)), 0, 4);
            writer.Write(BitConverter.GetBytes(Utils.ChangeEndian(samples)), 0, 4);
            foreach (int dimension in dimensions)
                writer.Write(BitConverter.GetBytes(Utils.ChangeEndian(dimension)), 0, 4);
        }

        private static int GenMagicNumber(IdxDataType dataType, int dimensionsCount)
        {
            int magicNumber = (dimensionsCount << 24) | ((byte)dataType << 16);
            return magicNumber;
        }
    }
}
