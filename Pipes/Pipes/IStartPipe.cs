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
using System.Collections;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    #region IStartPipe

    /// <summary>
    /// A helper interface for the IPipe pipe interface
    /// defining a general pipe consuming elements.
    /// </summary>
    public interface IStartPipe
    {

        /// <summary>
        /// Set the elements emitted by the given IEnumerator as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator as element source.</param>
        void SetSource(IEnumerator myIEnumerator);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable as element source.</param>
        void SetSourceCollection(IEnumerable myIEnumerable);

    }

    #endregion

    #region IStartPipe<in S>

    /// <summary>
    /// A helper interface for the IPipe&lt;S, E&gt; pipe interface
    /// defining a general pipe consuming elements of type S.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public interface IStartPipe<in S> : IStartPipe
    {

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerator">An IEnumerator&lt;S&gt; as element source.</param>
        void SetSource(IEnumerator<S> myIEnumerator);


        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S&gt; as input.
        /// </summary> 
        /// <param name="myIEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        void SetSourceCollection(IEnumerable<S> myIEnumerable);

    }

    #endregion

}
