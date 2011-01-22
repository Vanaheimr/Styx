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
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{
	
	public class HistoryIterator<T> : IEnumerator<T>
	{
		
		#region Data
		
	    private readonly IEnumerator<T> iterator;
	    private          T              last;
	
		#endregion
		
		#region Constructor(s)
		
	    public HistoryIterator(IEnumerator<T> iterator) {
	        this.iterator = iterator;
	    }
		
		#endregion
		
		
		public T Current
		{
			get
			{
				return iterator.Current;
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
			this.last = Current;
			return iterator.MoveNext();
		}
		
		public void Reset()
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}		
		

	
	    public T getLast()
		{
	        return this.last;
	    }
	
	    public void Remove()
		{
	        throw new InvalidOperationException();// UnsupportedOperationException();
	    }
	
	}

}
