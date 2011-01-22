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

	public abstract class AbstractPipe<S,E> : IPipe<S,E>
		where E : IEquatable<E>
	{
		
		#region Data
		
		protected IEnumerator<S> starts;
	    private   E              nextEnd;
	    protected E              currentEnd;
	    private   Boolean        available = false;
		
		#endregion
		
		#region Constructor(s)
		
		#region AbstractPipe()
		
		public AbstractPipe()
		{
		}
		
		#endregion
		
		#endregion
	
//	    public void setStarts(final Pipe<?, S> starts) {
//	        this.starts = starts;
//	    }

	    public void SetStarts(IEnumerator<S> starts)
		{
	        if (starts is IPipe<S,E>)
	            this.starts = starts;
	        else
	            this.starts = new HistoryIterator<S>(starts);
	    }

	    public void SetStarts(IEnumerable<S> starts)
		{
	        SetStarts(starts.GetEnumerator());
	    }

	    public List<E> getPath()
		{
			
	        var pathElements = getPathToHere();
	        var size         = pathElements.Count;
			
			// do not repeat filters as they dup the object
	        // todo: why is size == 0 required (Pangloss?)	        
			if (size == 0 || !pathElements[size - 1].Equals(this.currentEnd))
	            pathElements.Add(this.currentEnd);
			
	        return pathElements;
	    
		}
		
		
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
			
		
		
		
		
		
		
		

	    public void remove() {
	        throw new NotImplementedException();
	    }

	    public E next()
		{
			
	        if (this.available)
			{
	            this.available  = false;
	            this.currentEnd = this.nextEnd;
	            return this.currentEnd;
	        }
			
			else
			{
	            this.currentEnd = this.processNextStart();
	            return this.currentEnd;
	        }
			
	    }

	    public Boolean hasNext()
		{
	        
			if (this.available)
	            return true;
			
	        else
			{
	            try
				{
	                this.nextEnd = this.processNextStart();
	                this.available = true;
	                return true;
	            } catch (Exception) {
	                this.available = false;
	                return false;
	            }
	        }
			
	    }

	    public IEnumerator<E> iterator()
		{
	        return this;
	    }

	    public override String ToString()
		{
	        return this.GetType().Name;
	    }

	    protected abstract E processNextStart();// throws NoSuchElementException;

	    private List<E> getPathToHere()
		{

			if (this.starts is IPipe<S,E>)
			{
	            return ((IPipe<S,E>) this.starts).getPath();
//	        } else if (this.starts is HistoryIterator<S>) {
//	            var list = new List();
//	            list.add(((HistoryIterator) starts).getLast());
//	            return list;
	        }
			
			else
	            return new List<E>();

		}

	}

}
