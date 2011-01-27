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
		
		protected IEnumerator<S> _Starts;
	    protected E              _CurrentItem;
		
		#endregion
		
		#region Constructor(s)
		
		#region AbstractPipe()
		
		public AbstractPipe()
		{ }
		
		#endregion
		
		#endregion


        #region SetStarts(myStartPipe)

        public void SetStarts(IPipe<Object, S> myStartPipe)
        {
            _Starts = myStartPipe;
	    }

        #endregion

        #region SetStarts(myStartsEnumerator)

        public void SetStarts(IEnumerator<S> myStartsEnumerator)
		{
	        if (myStartsEnumerator is IPipe<S, E>)
	            this._Starts = myStartsEnumerator;
	        else
	            this._Starts = new HistoryEnumerator<S>(myStartsEnumerator);
	    }

        #endregion

        #region SetStarts(myStartsEnumerable)

        public void SetStarts(IEnumerable<S> myStartsEnumerable)
		{
	        SetStarts(myStartsEnumerable.GetEnumerator());
	    }

        #endregion


        #region Path

        public virtual List<E> Path
		{
            get
            {

                var _PathElements = PathToHere;
                var _Size         = _PathElements.Count;

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)	        
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(this._CurrentItem))
                    _PathElements.Add(this._CurrentItem);

                return _PathElements;

            }
		}

        #endregion

        #region Current

        public E Current
		{
			get
			{
                return _CurrentItem;
			}
		}
		
		Object System.Collections.IEnumerator.Current
		{	
			get
			{
                return _CurrentItem;
			}
		}

        #endregion

        #region MoveNext()

        public abstract Boolean MoveNext();

        #endregion

        #region Reset()

        public void Reset()
		{
            _Starts.Reset();
		}

        #endregion

        #region Dispose()

        public void Dispose()
		{
            _Starts.Dispose();
		}

        #endregion


        #region GetEnumerator()

        public IEnumerator<E> GetEnumerator()
		{
            return this;
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
            return this;
		}

        #endregion


        #region PathToHere

        private List<E> PathToHere
		{
            get
            {

                if (_Starts is IPipe<S, E>)
                {
                    return ((IPipe<S, E>) _Starts).Path;
                    //	        } else if (this.starts is HistoryIterator<S>) {
                    //	            var list = new List();
                    //	            list.add(((HistoryIterator) starts).getLast());
                    //	            return list;
                }

                else
                    return new List<E>();

            }
		}

        #endregion


        #region ToString()

        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion

    }

}
