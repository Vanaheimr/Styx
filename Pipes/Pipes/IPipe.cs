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

	/// <summary>
	/// The generic interface for any Pipe implementation.
	/// A Pipe takes/consumes objects of type S and returns/emits objects of type E.
	/// S refers to <i>starts</i> and the E refers to <i>ends</i>.
	/// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
	public interface IPipe<S, E> : IEnumerator<E>, IEnumerable<E>
		//where E : IEquatable<E>
	{
	//extends Iterator<E>, Iterable<E> {
	
	    /// <summary>
	    /// Set an iterator of S objects to the head (start) of the pipe.
	    /// </summary> 
	    /// <param name="starts">starts the iterator of incoming objects</param>
	    void SetStarts(IEnumerator<S> starts);
		
		
	    /// <summary>
	    /// Set an iterable of S objects to the head (start) of the pipe.
	    /// </summary> 
	    /// <param name="starts">starts the iterable of incoming objects</param>
	    void SetStarts(IEnumerable<S> starts);


		/// <summary>
	    /// Returns the path traversed to arrive at the current result of the pipe.
	    /// </summary> 
	    /// <returns>A List of all of the objects traversed for the current iterator position of the pipe.</returns>
	    List<E> getPath();
	
	}

}
