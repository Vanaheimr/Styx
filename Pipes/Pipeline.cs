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
	/// A Pipeline is a linear composite of Pipes.
	/// Pipeline takes a List of Pipes and joins them according to their order as specified by their location in the List.
	/// It is important to ensure that the provided ordered Pipes can connect together.
	/// That is, that the output of the n-1 Pipe is the same as the input to n Pipe.
	/// Once all provided Pipes are composed, a Pipeline can be treated like any other Pipe.
	/// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="E"></typeparam>
	public class Pipeline<S, E> : IPipe<S, E>
	{
	
		#region Data
		
	    private IPipe<S, E> _StartPipe;
	    private IPipe<S, E> _EndPipe;
	    private String     _PipelineString;
	
		#endregion
		
		#region Constructor(s)
		
		#region Pipeline()
		
	    public Pipeline()
		{
			_PipelineString = null;
		}
		
		#endregion
		
		#region Pipeline(myPipes)

		/// <summary>
		/// Constructs a pipeline from the provided pipes.
		/// The ordered list determines how the pipes will be chained together.
		/// When the pipes are chained together, the start of pipe n is the end of pipe n-1.
		/// </summary>
		/// <param name="myPipes">The ordered list of pipes to chain together into a pipeline</param>
	    public Pipeline(IEnumerable<IPipe<S, E>> myPipes)
			: this()
		{
	        SetPipes(myPipes);
	    }
		
		#endregion
	
		#region Pipeline(myPipes)

		/// <summary>
		/// Constructs a pipeline from the provided pipes.
		/// The ordered array determines how the pipes will be chained together.
		/// When the pipes are chained together, the start of pipe n is the end of pipe n-1.
		/// </summary>
		/// <param name="myPipes">the ordered array of pipes to chain together into a pipeline</param>
	    public Pipeline(IPipe<S, E>[] myPipes)
			: this()
		{
	        SetPipes(myPipes);
	    }
		
		#endregion
		
		#endregion


		#region SetPipes(myPipes)
	
	    /// <summary>
	    /// Use when extending Pipeline and setting the pipeline chain without making use of the constructor.
	    /// </summary>
	    /// <param name="myPipes">the ordered list of pipes to chain together into a pipeline.</param>
	    protected void SetPipes(IEnumerable<IPipe<S, E>> myPipes)
		{
			SetPipes(myPipes.ToArray());
		}
		
		#endregion

		#region SetPipes(myPipes)
		
	    /// <summary>
	    /// Use when extending Pipeline and setting the pipeline chain without making use of the constructor.
	    /// </summary>
	    /// <param name="myPipes">the ordered array of pipes to chain together into a pipeline.</param>
	    protected void SetPipes(IPipe<S, E>[] myPipes)
		{

			var _PipeNames = new List<String>();
			var _Length    = myPipes.Length;
	        
			_StartPipe = myPipes[0];
	        _PipeNames.Add(_StartPipe.ToString());
			
	        _EndPipe   = myPipes[_Length-1];
		
	        for (var i=1; i < _Length; i++)
			{
	            myPipes[i].SetStarts((IEnumerator<S>) myPipes[i-1]); //(Iterator)
	            _PipeNames.Add(myPipes[i].ToString());
	        }
			
	        _PipelineString = _PipeNames.ToString();
		
		}
		
		#endregion
		
	
		#region SetStartPipe(myStartPipe)
		
		/// <summary>
		/// Only use if the intermediate pipes of the pipeline have been chained together manually.
		/// </summary>
		/// <param name="myStartPipe">the start of the pipeline</param>
		public void SetStartPipe(IPipe<S, E> myStartPipe)
		{
	        _StartPipe = myStartPipe;
	    }
		
		#endregion
		
		#region SetEndPipe(myEndPipe)

		/// <summary>
		/// Only use if the intermediate pipes of the pipeline have been chained together manually.
		/// </summary>
		/// <param name="myEndPipe">the end of the pipeline</param>
	    public void SetEndPipe(IPipe<S, E> myEndPipe)
		{
	        _EndPipe = myEndPipe;
	    }
		
		#endregion
	
		#region SetStarts(myStarts)
		
	    public void SetStarts(IEnumerator<S> myStarts)
		{
	        _StartPipe.SetStarts(myStarts);
	    }
		
		#endregion
		
		#region SetStarts(myStarts)
	
	    public void SetStarts(IEnumerable<S> myStarts)
		{
	        this.SetStarts(myStarts.GetEnumerator());
	    }
		
		#endregion
		
		
		
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
		

		
		
		
		
		
		public IEnumerator<E> GetEnumerator()
		{
			throw new NotImplementedException();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
			
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
	    /**
	     * An unsupported operation that throws an UnsupportedOperationException.
	     */
	    public void remove()
		{
	        throw new NotImplementedException();// UnsupportedOperationException();
	    }
	
	    /**
	     * Determines if there is another object that can be emitted from the pipeline.
	     *
	     * @return true if an object can be next()'d out of the pipeline
	     */
	    public Boolean hasNext()
		{
			throw new NotImplementedException();
	        //return endPipe.hasNext();
	    }
	
	    /**
	     * Get the next object emitted from the pipeline.
	     * If no such object exists, then a NoSuchElementException is thrown.
	     *
	     * @return the next emitted object
	     */
	    public E next()
		{
			throw new NotImplementedException();
//	        return endPipe.next();
	    }
	
	    public List<E> getPath()
		{
	        return _EndPipe.getPath();
	    }
	
	    /**
	     * Simply returns this as as a pipeline (more specifically, pipe) implements Iterator.
	     *
	     * @return returns the iterator representation of this pipeline
	     */
	    public IEnumerator<E> iterator()
		{
	        return this;
	    }
		

		#region Dispose()
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		#endregion
	
		#region ToString()
		
	    public override String ToString()
		{
	        return _PipelineString;
	    }
		
		#endregion

	}

}