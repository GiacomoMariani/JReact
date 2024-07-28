using System;

namespace JReact.Utilities
{
    //an interface to implement heaps
    public interface iHeapElement<T> : IComparable<T>
    {
        int indexInHeap { get; set; }
    }

    /// <summary>
    /// this class implements a heap, a collection of elements in a given order
    /// </summary>
    /// <typeparam name="T">the elements we want to store, needs to be comparable</typeparam>
    public class JHeap<T> where T : iHeapElement<T>
    {
        // --------------- VALUES AND PROPERTIES --------------- //
        //the collection of items in the current heap
        private T[] _collection;
        //the count of items in the current heap
        private int _heapSize;
        public int Count => _heapSize;

        /// <summary>
        /// used to check if the heap contains a given element
        /// </summary>
        /// <param name="element">the element to be checked</param>
        /// <returns>returns true if the element is inside the heap</returns>
        public bool Contains(T element) => Equals(_collection[element.indexInHeap], element);

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// the constructor creates the base array, given an amount of nodes
        /// </summary>
        /// <param name="maxHeapSize">this is the maximum number of given nodes</param>
        public JHeap(int maxHeapSize) => _collection = new T[maxHeapSize];

        /// <summary>
        /// used to add an item into the heap
        /// </summary>
        /// <param name="element">the element to be added</param>
        public void Push(T element)
        {
            //set the index on the element to be added
            element.indexInHeap = _heapSize;
            //add the element to the collection
            _collection[_heapSize] = element;
            //sort the element
            HeapSortFromTop(element);
            //add one to the index counting
            _heapSize++;
        }

        /// <summary>
        /// this method return the first element on the heap
        /// </summary>
        /// <returns>returns the element with minimal value</returns>
        public T Pop()
        {
            //get the first item of the collection
            T firstItem = _collection[0];
            //update the count
            _heapSize--;
            //updates the collection with the new element
            _collection[0]             = _collection[_heapSize];
            _collection[0].indexInHeap = 0;
            //sort all the other elements down
            Heapify(_collection[0]);
            return firstItem;
        }

        /// <summary>
        /// updates one element inside the heap
        /// </summary>
        /// <param name="element">the element we want to update</param>
        public void UpdateItem(T element) { HeapSortFromTop(element); }

        // --------------- ORDERING --------------- //
        // this method is used to sort down one element
        private void Heapify(T element)
        {
            //find the index of the borders
            int leftIndex  = element.indexInHeap * 2 + 1;
            int rightIndex = element.indexInHeap * 2 + 2;
            //stop if the index on the left is above the max index
            if (leftIndex >= _heapSize) return;

            //used to store the index for swapping
            int largest = leftIndex;

            //check if we can change the current index with the one on the left
            if (rightIndex < _heapSize)
                if (_collection[leftIndex].CompareTo(_collection[rightIndex]) < 0)
                    largest = rightIndex;

            //stop if we run out of elements
            if (element.CompareTo(_collection[largest]) >= 0) return;

            //swap the element
            Swap(element, _collection[largest]);
            Heapify(element);
        }

        // this method is used to sort the heap on the right
        private void HeapSortFromTop(T element)
        {
            //get the parent index
            int parentIndex = (element.indexInHeap - 1) / 2;
            //get the parent item
            T parentElement = _collection[parentIndex];
            //check if the element to be sorted is higher than the parent item, if not we reached the end of the loop
            if (element.CompareTo(parentElement) <= 0) return;
            //otherwise swap the item
            Swap(element, parentElement);
            //keep checking until we run out of elements
            HeapSortFromTop(element);
        }

        //this method is used to swap 2 elements, and update their index
        private void Swap(T elementA, T elementB)
        {
            //update the heap collection
            _collection[elementA.indexInHeap] = elementB;
            _collection[elementB.indexInHeap] = elementA;
            //update the indexes of the elements in the heap
            (elementA.indexInHeap, elementB.indexInHeap) = (elementB.indexInHeap, elementA.indexInHeap);
        }
    }
}
