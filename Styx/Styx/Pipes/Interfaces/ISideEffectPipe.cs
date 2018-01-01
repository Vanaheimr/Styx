/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    #region ISideEffectPipe

    /// <summary>
    /// A side effect pipe will produce one or more side effects
    /// which can be retrieved by the SideEffect properties.
    /// </summary>
    public interface ISideEffectPipe : IPipe, IDisposable
    { }

    #endregion

    #region ISideEffectPipe<S, E, out T>

    /// <summary>
    /// This side effect pipe produces a controlled side effect
    /// which can be retrieved by the SideEffect property.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T">The type of the side effect.</typeparam>
    public interface ISideEffectPipe<S, E, out T> : ISideEffectPipe, IPipe<S, E>
    {

        /// <summary>
        /// The side effect produced by this pipe.
        /// </summary>
        T SideEffect { get; }

    }

    #endregion

    #region ISideEffectPipe<S1, S2, E, out T>

    /// <summary>
    /// This side effect pipe produces a controlled side effect
    /// which can be retrieved by the SideEffect property.
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T">The type of the side effect.</typeparam>
    public interface ISideEffectPipe<S1, S2, E, out T> : ISideEffectPipe, IPipe<S1, S2, E>
    {

        /// <summary>
        /// The side effect produced by this pipe.
        /// </summary>
        T SideEffect { get; }

    }

    #endregion

    #region ISideEffectPipe<S1, S2, S3, E, out T>

    /// <summary>
    /// This side effect pipe produces a controlled side effect
    /// which can be retrieved by the SideEffect property.
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the third consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T">The type of the side effect.</typeparam>
    public interface ISideEffectPipe<S1, S2, S3, E, out T> : ISideEffectPipe, IPipe<S1, S2, S3, E>
    {

        /// <summary>
        /// The side effect produced by this pipe.
        /// </summary>
        T SideEffect { get; }

    }

    #endregion


    #region ITwoSideEffectsPipe<S, E, out T1, out T2>

    /// <summary>
    /// This side effect pipe produces two controlled side effects
    /// which can be retrieved by the SideEffect properties.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T1">The type of the first side effect.</typeparam>
    /// <typeparam name="T2">The type of the second side effect.</typeparam>
    public interface ITwoSideEffectsPipe<S, E, out T1, out T2> : ISideEffectPipe, IPipe<S, E>
    {

        /// <summary>
        /// The first side effect produced by this pipe.
        /// </summary>
        T1 SideEffect1 { get; }

        /// <summary>
        /// The second side effect produced by this pipe.
        /// </summary>
        T2 SideEffect2 { get; }

    }

    #endregion

    #region ISideEffectPipe<S, E, out T1, out T2, out T3>

    /// <summary>
    /// This side effect pipe produces three controlled side effects
    /// which can be retrieved by the SideEffect properties.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T1">The type of the first side effect.</typeparam>
    /// <typeparam name="T2">The type of the second side effect.</typeparam>
    /// <typeparam name="T3">The type of the third side effect.</typeparam>
    public interface IThreeSideEffectsPipe<S, E, out T1, out T2, out T3> : ISideEffectPipe, IPipe<S, E>
    {

        /// <summary>
        /// The first side effect produced by this pipe.
        /// </summary>
        T1 SideEffect1 { get; }

        /// <summary>
        /// The second side effect produced by this pipe.
        /// </summary>
        T2 SideEffect2 { get; }

        /// <summary>
        /// The third side effect produced by this pipe.
        /// </summary>
        T3 SideEffect3 { get; }

    }

    #endregion

}
