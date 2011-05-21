/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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
using System.Collections;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    #region IPipe

    /// <summary>
    /// A helper interface for the IPipe&lt;S, E&gt; pipe interface
    /// defining a general pipe.
    /// </summary>
    public interface IPipe : IStartPipe, IEndPipe, IDisposable
    { }

    #endregion

    #region IPipe<in S, out E>

    /// <summary>
    /// The generic interface for any Pipe implementation.
    /// A Pipe takes/consumes objects of type S and returns/emits objects of type E.
    /// S refers to <i>starts</i> and the E refers to <i>ends</i>.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public interface IPipe<in S, out E> : IStartPipe<S>, IEndPipe<E>, IPipe
	{

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S&gt; as element source.</param>
        IPipe<S, E> SetSource(IEnumerator<S> myIEnumerator);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        IPipe<S, E> SetSourceCollection(IEnumerable<S> myIEnumerable);
    
    }

    #endregion

    #region IPipe<in S1, in S2, out E>

    /// <summary>
    /// The generic interface for any Pipe implementation.
    /// A Pipe takes/consumes objects of type S1 and S2 and returns/emits objects of type E.
    /// S1 and S2 refers to <i>starts</i> and the E refers to <i>ends</i>.
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public interface IPipe<in S1, in S2, out E> : IEndPipe<E>
    {

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S1&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S1&gt; as element source.</param>
        void SetSource1(IEnumerator<S1> myIEnumerator);


        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S2&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S2&gt; as element source.</param>
        void SetSource2(IEnumerator<S2> myIEnumerator);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S1&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S1&gt; as element source.</param>
        void SetSourceCollection1(IEnumerable<S1> myIEnumerable);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S2&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S2&gt; as element source.</param>
        void SetSourceCollection2(IEnumerable<S2> myIEnumerable);

    }

    #endregion

    #region IPipe<in S1, in S2, in S3, out E>

    /// <summary>
    /// The generic interface for any Pipe implementation.
    /// A Pipe takes/consumes objects of type S1, S2 and S3 and returns/emits objects of type E.
    /// S1, S2 and S3 refers to <i>starts</i> and the E refers to <i>ends</i>.
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the third consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public interface IPipe<in S1, in S2, in S3, out E> : IEndPipe<E>
    {

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S1&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S1&gt; as element source.</param>
        void SetSource1(IEnumerator<S1> myIEnumerator);


        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S2&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S2&gt; as element source.</param>
        void SetSource2(IEnumerator<S2> myIEnumerator);


        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S3&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S3&gt; as element source.</param>
        void SetSource3(IEnumerator<S3> myIEnumerator);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S1&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S1&gt; as element source.</param>
        void SetSourceCollection1(IEnumerable<S1> myIEnumerable);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S2&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S2&gt; as element source.</param>
        void SetSourceCollection2(IEnumerable<S2> myIEnumerable);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S3&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S3&gt; as element source.</param>
        void SetSourceCollection3(IEnumerable<S3> myIEnumerable);

    }

    #endregion

    #region IPipe<in S1, in S2, in S3, in S4, out E>

    /// <summary>
    /// The generic interface for any Pipe implementation.
    /// A Pipe takes/consumes objects of type S1, S2 and S3 and returns/emits objects of type E.
    /// S1, S2 and S3 refers to <i>starts</i> and the E refers to <i>ends</i>.
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the third consuming objects.</typeparam>
    /// <typeparam name="S4">The type of the fourth consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public interface IPipe<in S1, in S2, in S3, in S4, out E> : IEndPipe<E>
    {

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S1&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S1&gt; as element source.</param>
        void SetSource1(IEnumerator<S1> myIEnumerator);


        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S2&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S2&gt; as element source.</param>
        void SetSource2(IEnumerator<S2> myIEnumerator);


        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S3&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S3&gt; as element source.</param>
        void SetSource3(IEnumerator<S3> myIEnumerator);


        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S4&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S4&gt; as element source.</param>
        void SetSource4(IEnumerator<S4> myIEnumerator);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S1&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S1&gt; as element source.</param>
        void SetSourceCollection1(IEnumerable<S1> myIEnumerable);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S2&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S2&gt; as element source.</param>
        void SetSourceCollection2(IEnumerable<S2> myIEnumerable);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S3&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S3&gt; as element source.</param>
        void SetSourceCollection3(IEnumerable<S3> myIEnumerable);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S4&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S4&gt; as element source.</param>
        void SetSourceCollection4(IEnumerable<S4> myIEnumerable);

    }

    #endregion

}
