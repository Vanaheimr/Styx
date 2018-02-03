using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace de.ahzf.Illias.Commons.Collections
{

    /// <summary>
    /// A thread-safe, lock-free queue.
    /// </summary>
    /// <typeparam name="T">The type of the values stored within the queue.</typeparam>
    public class TSQueue<T> : IEnumerable<T>
    {

        /// <summary>
        /// An element within a queue.
        /// </summary>
        public class QueueElement
        {

            #region Properties

            /// <summary>
            /// Return the next element within the queue.
            /// </summary>
            public QueueElement Next  { get; set; }

            /// <summary>
            /// Return the value stored within the element.
            /// </summary>
            public T            Value { get; private set; }

            #endregion

            #region Constructor(s)

            #region QueueElement(Value)

            /// <summary>
            /// Create a single queue element.
            /// </summary>
            /// <param name="Value">The value stored within the node.</param>
            public QueueElement(T Value)
            {
                this.Value = Value;
                this.Next  = null;
            }

            #endregion

            #endregion

        }

        #region Properties

        #region First

        private QueueElement _First;

        /// <summary>
        /// The first element of the queue.
        /// </summary>
        public QueueElement First
        {
            get
            {
                return _First;
            }
        }

        #endregion

        #region MaxNumberOfElements

        private UInt64 _MaxNumberOfElements;

        /// <summary>
        /// The maximal number of values within the queue.
        /// </summary>
        public UInt64 MaxNumberOfElements
        {

            get
            {
                return _MaxNumberOfElements;
            }

            set
            {
                if (value < Int32.MaxValue)
                    _MaxNumberOfElements = value;
            }

        }

        #endregion

        #region Count

        private Int32 _Count;

        /// <summary>
        /// The current number of elements within the queue.
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this._Count;
            }
        }

        #endregion

        #endregion


        public QueueElement Push(T Value)
        {
            
            var NewElement = new QueueElement(Value);

            QueueElement OldFirst;

            do {
                OldFirst        = First;
                NewElement.Next = OldFirst;
            } while (Interlocked.CompareExchange<QueueElement>(ref _First, NewElement, OldFirst) != OldFirst);

            Interlocked.Increment(ref _Count);

            while ((UInt64) _Count > _MaxNumberOfElements)
                RemoveOldest();

            return NewElement;

        }

        public void RemoveOldest()
        {

            QueueElement Oldest    = _First;
            QueueElement PreOldest = null;

            while (Oldest.Next != null)
            {
                PreOldest = Oldest;
                Oldest    = Oldest.Next;
            }

            if (PreOldest != null)
            {
                PreOldest.Next = null;
                Interlocked.Decrement(ref _Count);
            }

        }


        public IEnumerator<T> GetEnumerator()
        {

            var node = _First;

            if (node != null)
            {
                do
                {
                    yield return node.Value;
                } while ((node = node.Next) != null);
            }

        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }

}
