/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// An AbstractPipe provides most of the functionality that is repeated
    /// in every instance of a Pipe. Any subclass of AbstractPipe should simply
    /// implement MoveNext().
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public abstract class AbstractPipe<S, E> : IPipe<S, E>
	{
		
		#region Data
		
        /// <summary>
        /// The internal enumerator of the collection.
        /// </summary>
		protected IEnumerator<S> _InternalEnumerator;

        /// <summary>
        /// The internal current element in the collection.
        /// </summary>
	    protected E              _CurrentElement;
		
		#endregion
		
		#region Constructor(s)
		
		#region AbstractPipe()
		
        /// <summary>
        /// Creates a new abstract pipe.
        /// </summary>
		public AbstractPipe()
		{ }
		
		#endregion

        #region AbstractPipe(myIPipe)

        /// <summary>
        /// Creates a new abstract pipe using on the given IPipe.
        /// </summary>
        public AbstractPipe(IPipe myIPipe)
        {
            SetIPipe(myIPipe);
        }

        #endregion

        #region AbstractPipe(myIEnumerator)

        /// <summary>
        /// Creates a new abstract pipe using on the given IEnumerator.
        /// </summary>
        public AbstractPipe(IEnumerator<S> myIEnumerator)
        {
            SetIEnumerator(myIEnumerator);
        }

        #endregion

        #region AbstractPipe(myIEnumerable)

        /// <summary>
        /// Creates a new abstract pipe using on the given IEnumerable.
        /// </summary>
        public AbstractPipe(IEnumerable<S> myIEnumerable)
        {   
            SetIEnumerable(myIEnumerable);
        }

        #endregion
		
		#endregion


        #region SetIPipe(myIPipe)

        //ToDo: Is this really needed?!
        //public void SetIPipe(IPipe<Object, S> myIPipe)
        //{
        //    _Starts = myIPipe;
        //}

        public virtual void SetIPipe(IPipe myIPipe)
        {
            
            var _IEnumerator = myIPipe as IEnumerator<S>;

            if (_IEnumerator != null)
                _InternalEnumerator = _IEnumerator;

            else
                throw new ArgumentNullException("myPipe must not be null and implement the IPipe<S, E> interface!");

        }

        #endregion

        #region SetIEnumerator(myIEnumerator)

        public virtual void SetIEnumerator(IEnumerator<S> myIEnumerator)
		{

            if (myIEnumerator == null)
                throw new ArgumentNullException("myIEnumerator must not be null!");

	        if (myIEnumerator is IPipe)
	            _InternalEnumerator = myIEnumerator;
	        else
	            _InternalEnumerator = new HistoryEnumerator<S>(myIEnumerator);

	    }

        #endregion

        #region SetIEnumerable(myIEnumerable)

        public virtual void SetIEnumerable(IEnumerable<S> myIEnumerable)
		{

            if (myIEnumerable == null)
                throw new ArgumentNullException("myIEnumerator must not be null!");

	        SetIEnumerator(myIEnumerable.GetEnumerator());

	    }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator&lt;E&gt; that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<E> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region Current

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public E Current
		{
			get
			{
                return _CurrentElement;
			}
		}

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
		Object System.Collections.IEnumerator.Current
		{	
			get
			{
                return _CurrentElement;
			}
		}

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public abstract Boolean MoveNext();

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public void Reset()
		{
            _InternalEnumerator.Reset();
		}

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public void Dispose()
		{
            _InternalEnumerator.Dispose();
		}

        #endregion




        #region Path

        public virtual List<Object> Path
        {
            get
            {

                var _PathElements = PathToHere;
                var _Size         = _PathElements.Count;

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)	        
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(_CurrentElement))
                    _PathElements.Add(_CurrentElement);

                return _PathElements;

            }
        }

        #endregion

        #region PathToHere

        private List<Object> PathToHere
		{
            get
            {

                if (_InternalEnumerator is IPipe)
                    return ((IPipe) _InternalEnumerator).Path;

                else if (_InternalEnumerator is IHistoryEnumerator)
                {
                    var _List = new List<Object>();
                    _InternalEnumerator.MoveNext();
                    _List.Add(((IHistoryEnumerator)_InternalEnumerator).Last);
                    return _List;
                }

                else
                    return new List<Object>();

            }
		}

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion

    }

}
