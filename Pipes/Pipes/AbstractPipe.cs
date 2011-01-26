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
    /// implement processNextStart(). The standard model is
    /// <pre>
    /// protected E processNextStart() throws NoSuchElementException {
    ///     S s = this.starts.next();
    ///     E e = // do something with the S to yield an E
    ///     return e;
    /// }
    /// </pre>
    /// If the current incoming S is not to be emitted and there are no other S objects
    /// to process and emit, then throw a NoSuchElementException.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public abstract class AbstractPipe<S, E> : IPipe<S, E>
	{
		
		#region Data
		
		protected IEnumerator<S> _Starts;
	    private   E              _NextEnd;
	    protected E              _CurrentEnd;
	    private   Boolean        _Available;
		
		#endregion
		
		#region Constructor(s)
		
		#region AbstractPipe()
		
		public AbstractPipe()
		{
            _Available = false;
		}
		
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
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(this._CurrentEnd))
                    _PathElements.Add(this._CurrentEnd);

                return _PathElements;

            }
		}

        #endregion

        #region Current

        public E Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		Object System.Collections.IEnumerator.Current
		{	
			get
			{
				throw new NotImplementedException();
			}
		}

        #endregion

        public Boolean MoveNext()
		{
			throw new NotImplementedException();
		}
		
		public void Reset()
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}	
		
		
		
		
		
		public IEnumerator<E> GetEnumerator()
		{
			throw new NotImplementedException();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
			
		
		
		
		
		
		
		


	    public E next()
		{
			
	        if (_Available)
			{
	            _Available  = false;
	            _CurrentEnd = _NextEnd;
	            return _CurrentEnd;
	        }
			
			else
			{
	            _CurrentEnd = ProcessNextStart();
	            return _CurrentEnd;
	        }
			
	    }

	    public Boolean hasNext()
		{
	        
			if (_Available)
	            return true;
			
	        else
			{

	            try
				{
	                _NextEnd   = ProcessNextStart();
	                _Available = true;
	                return true;

	            }
                
                catch (Exception)
                {
	                _Available = false;
	                return false;
	            }

	        }
			
	    }


        #region ProcessNextStart()
        
        protected abstract E ProcessNextStart();

        #endregion

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


        #region ToString()

        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion

    }

}
