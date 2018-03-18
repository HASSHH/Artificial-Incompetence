using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public delegate LabeledData GetNextDataDelegate(int indexInBatch);

    public class LazyBatch : IEnumerable<LabeledData>
    {
        private GetNextDataDelegate getNext;
        private int size;

        public LazyBatch(GetNextDataDelegate getNext, int size)
        {
            this.getNext = getNext;
            this.size = size;
        }

        public IEnumerator<LabeledData> GetEnumerator()
        {
            return new LazyBatchEnumerator(getNext, size); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class LazyBatchEnumerator : IEnumerator<LabeledData>
    {
        private LabeledData current;
        private GetNextDataDelegate getNext;
        private int size;
        private int currentIndex;

        public LazyBatchEnumerator(GetNextDataDelegate getNext, int size)
        {
            this.getNext = getNext;
            this.size = size;
            currentIndex = 0;
        }

        public LabeledData Current => current;

        object IEnumerator.Current => current;

        public void Dispose()
        {
            //
        }

        public bool MoveNext()
        {
            if(currentIndex < size)
            {
                current = getNext(currentIndex);
                ++currentIndex;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            currentIndex = 0;
        }
    }
}
