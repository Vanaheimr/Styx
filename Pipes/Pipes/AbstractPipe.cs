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
	        if (myStartsEnumerator is IPipe)
	            _Starts = myStartsEnumerator;
	        else
	            _Starts = new HistoryEnumerator<S>(myStartsEnumerator);
	    }

        #endregion

        #region SetStarts(myStartsEnumerable)

        public void SetStarts(IEnumerable<S> myStartsEnumerable)
		{
	        SetStarts(myStartsEnumerable.GetEnumerator());
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


        #region Path

        public virtual List<Object> Path
        {
            get
            {

                var _PathElements = PathToHere;
                var _Size         = _PathElements.Count;

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)	        
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(_CurrentItem))
                    _PathElements.Add(_CurrentItem);

                return _PathElements;

            }
        }

        #endregion

        #region PathToHere

        private List<Object> PathToHere
		{
            get
            {

                if (_Starts is IPipe)
                    return ((IPipe) _Starts).Path;

                else if (_Starts is IHistoryEnumerator)
                {
                    var _List = new List<Object>();
                    _Starts.MoveNext();
                    _List.Add(((IHistoryEnumerator)_Starts).Last);
                    return _List;
                }

                else
                    return new List<Object>();

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
