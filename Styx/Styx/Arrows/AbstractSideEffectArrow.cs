/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    #region AbstractSideEffectArrow<TIn, TOut, TSideeffect>

    /// <summary>
    /// The AbstractSideEffectArrow provides the same functionality as the 
    /// AbstractArrow, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    /// <typeparam name="TSideEffect">The type of the sideeffect.</typeparam>
    public abstract class AbstractSideEffectArrow<TIn, TOut, TSideEffect> : AbstractArrow<TIn, TOut>,
                                                                            ISideEffectArrow<TIn, TOut, TSideEffect>
    {

        #region Properties

        #region SideEffect

        /// <summary>
        /// The SideEffect produced by this Arrow.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref _SideEffect);
        /// </summary>
        protected TSideEffect _SideEffect;

        /// <summary>
        /// The SideEffect produced by this Arrow.
        /// </summary>
        public TSideEffect SideEffect
        {

            get
            {
                return _SideEffect;
            }

            protected set
            {
                _SideEffect = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new AbstractSideEffectArrow.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public AbstractSideEffectArrow(IArrowSender<TIn> ArrowSender = null)

            : base(ArrowSender)

        { }

        #endregion

    }

    #endregion

    #region AbstractSideEffectArrow<TIn, TOut, T1, T2>

    /// <summary>
    /// The AbstractSideEffectArrow provides the same functionality as the 
    /// AbstractArrow, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    /// <typeparam name="T1">The type of the first sideeffect.</typeparam>
    /// <typeparam name="T2">The type of the second sideeffect.</typeparam>
    public abstract class AbstractSideEffectArrow<TIn, TOut, T1, T2> : AbstractArrow<TIn, TOut>,
                                                                       ISideEffectArrow<TIn, TOut, T1, T2>
    {

        #region Properties

        #region SideEffect1

        /// <summary>
        /// The first SideEffect produced by this Arrow.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref _SideEffect);
        /// </summary>
        protected T1 _SideEffect1;

        /// <summary>
        /// The first SideEffect produced by this Arrow.
        /// </summary>
        public T1 SideEffect1
        {

            get
            {
                return _SideEffect1;
            }

            protected set
            {
                _SideEffect1 = value;
            }

        }

        #endregion

        #region SideEffect2

        /// <summary>
        /// The second SideEffect produced by this Arrow.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref _SideEffect);
        /// </summary>
        protected T2 _SideEffect2;

        /// <summary>
        /// The second SideEffect produced by this Arrow.
        /// </summary>
        public T2 SideEffect2
        {

            get
            {
                return _SideEffect2;
            }

            protected set
            {
                _SideEffect2 = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new AbstractSideEffectArrow.
        /// </summary>
        public AbstractSideEffectArrow(IArrowSender<TIn> ArrowSender = null)

            : base(ArrowSender)

        { }

        #endregion

    }

    #endregion

    #region AbstractSideEffectArrow<TIn, TOut, T1, T2, T3>

    /// <summary>
    /// The AbstractSideEffectArrow provides the same functionality as the 
    /// AbstractArrow, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    /// <typeparam name="T1">The type of the first sideeffect.</typeparam>
    /// <typeparam name="T2">The type of the second sideeffect.</typeparam>
    /// <typeparam name="T3">The type of the third sideeffect.</typeparam>
    public abstract class AbstractSideEffectArrow<TIn, TOut, T1, T2, T3> : AbstractArrow<TIn, TOut>,
                                                                           ISideEffectArrow<TIn, TOut, T1, T2, T3>
    {

        #region Properties

        #region SideEffect1

        /// <summary>
        /// The first SideEffect produced by this Arrow.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref _SideEffect);
        /// </summary>
        protected T1 _SideEffect1;

        /// <summary>
        /// The first SideEffect produced by this Arrow.
        /// </summary>
        public T1 SideEffect1
        {

            get
            {
                return _SideEffect1;
            }

            protected set
            {
                _SideEffect1 = value;
            }

        }

        #endregion

        #region SideEffect2

        /// <summary>
        /// The second SideEffect produced by this Arrow.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref _SideEffect);
        /// </summary>
        protected T2 _SideEffect2;

        /// <summary>
        /// The second SideEffect produced by this Arrow.
        /// </summary>
        public T2 SideEffect2
        {

            get
            {
                return _SideEffect2;
            }

            protected set
            {
                _SideEffect2 = value;
            }

        }

        #endregion

        #region SideEffect3

        /// <summary>
        /// The third SideEffect produced by this Arrow.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref _SideEffect);
        /// </summary>
        protected T3 _SideEffect3;

        /// <summary>
        /// The third SideEffect produced by this Arrow.
        /// </summary>
        public T3 SideEffect3
        {

            get
            {
                return _SideEffect3;
            }

            protected set
            {
                _SideEffect3 = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new AbstractSideEffectArrow.
        /// </summary>
        public AbstractSideEffectArrow(IArrowSender<TIn> ArrowSender = null)

            : base(ArrowSender)

        { }

        #endregion

    }

    #endregion

}
