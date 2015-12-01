/*
 * Copyright (c) 2010-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    #region AbstractSideEffectPipe<S, E, T>

    /// <summary>
    /// An AbstractSideEffectPipe provides the same functionality as the 
    /// AbstractPipe, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T">The type of the sideeffect.</typeparam>
    public abstract class AbstractSideEffectPipe<S, E, T> : AbstractPipe<S, E>, ISideEffectPipe<S, E, T>
    {

        #region Properties

        #region SideEffect

        /// <summary>
        /// The internal side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect);
        /// </summary>
        protected T InternalSideEffect;

        /// <summary>
        /// The side effect produced by this pipe.
        /// </summary>
        public T SideEffect
        {

            get
            {
                return InternalSideEffect;
            }

            protected set
            {
                InternalSideEffect = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        #region (protected) AbstractSideEffectPipe()

        /// <summary>
        /// Creates an new abstract side effect pipe.
        /// </summary>
        protected AbstractSideEffectPipe()
        { }

        #endregion

        #region AbstractSideEffectPipe(SourceElement, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given single value as element source.
        /// </summary>
        /// <param name="SourceElement">A single value as element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(S  SourceElement,
                                      T  SideEffect)

            : base(SourceElement)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #region AbstractSideEffectPipe(SourcePipe, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given pipe as element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEndPipe<S>  SourcePipe,
                                      T            SideEffect)

            : base(SourcePipe)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #region AbstractSideEffectPipe(SourceEnumerator, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerator as element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEnumerator<S>  SourceEnumerator,
                                      T               SideEffect)

            : base(SourceEnumerator)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #region AbstractSideEffectPipe(SourceEnumerable, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEnumerable<S>  SourceEnumerable,
                                      T               SideEffect)

            : base(SourceEnumerable)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #endregion

    }

    #endregion

    #region AbstractSideEffectPipe<S1, S2, E, T>

    /// <summary>
    /// An AbstractSideEffectPipe provides the same functionality as the 
    /// AbstractPipe, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T">The type of the sideeffect.</typeparam>
    public abstract class AbstractSideEffectPipe<S1, S2, E, T> : AbstractPipe<S1, S2, E>, ISideEffectPipe<S1, S2, E, T>
    {

        #region Properties

        #region SideEffect

        /// <summary>
        /// The internal side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect);
        /// </summary>
        protected T InternalSideEffect;

        /// <summary>
        /// The side effect produced by this pipe.
        /// </summary>
        public T SideEffect
        {

            get
            {
                return InternalSideEffect;
            }

            protected set
            {
                InternalSideEffect = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        #region (protected) AbstractSideEffectPipe()

        /// <summary>
        /// Creates an new abstract side effect pipe.
        /// </summary>
        protected AbstractSideEffectPipe()
        { }

        #endregion

        #region AbstractSideEffectPipe(SourceElement1, SourceElement2, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given single values as element sources.
        /// </summary>
        /// <param name="SourceElement1">A single value as first element source.</param>
        /// <param name="SourceElement2">A single value as second element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(S1  SourceElement1,
                                      S2  SourceElement2,
                                      T   SideEffect)

            : base(SourceElement1, SourceElement2)

        {

            this.SideEffect = SideEffect;

        }

        #endregion

        #region AbstractSideEffectPipe(SourcePipe1, SourcePipe2, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given pipes as element sources.
        /// </summary>
        /// <param name="SourcePipe1">A pipe as first element source.</param>
        /// <param name="SourcePipe2">A pipe as second element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEndPipe<S1>  SourcePipe1,
                                      IEndPipe<S2>  SourcePipe2,
                                      T             SideEffect)

            : base(SourcePipe1, SourcePipe2)

        {

            this.SideEffect = SideEffect;

        }

        #endregion

        #region AbstractSideEffectPipe(SourceEnumerator1, SourceEnumerator2, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerators as element sources.
        /// </summary>
        /// <param name="SourceEnumerator1">An enumerator as first element source.</param>
        /// <param name="SourceEnumerator2">An enumerator as second element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEnumerator<S1>  SourceEnumerator1,
                                      IEnumerator<S2>  SourceEnumerator2,
                                      T                SideEffect)

            : base(SourceEnumerator1, SourceEnumerator2)

        {

            this.SideEffect = SideEffect;

        }

        #endregion

        #region AbstractSideEffectPipe(SourceEnumerable1, SourceEnumerable2, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerables as element sources.
        /// </summary>
        /// <param name="SourceEnumerable1">An enumerable as element source.</param>
        /// <param name="SourceEnumerable2">An enumerable as element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEnumerable<S1>  SourceEnumerable1,
                                      IEnumerable<S2>  SourceEnumerable2,
                                      T                SideEffect)

            : base(SourceEnumerable1, SourceEnumerable2)

        {

            this.SideEffect = SideEffect;

        }

        #endregion

        #endregion

    }

    #endregion

    #region AbstractSideEffectPipe<S1, S2, S3, E, T>

    /// <summary>
    /// An AbstractSideEffectPipe provides the same functionality as the 
    /// AbstractPipe, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the second consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T">The type of the sideeffect.</typeparam>
    public abstract class AbstractSideEffectPipe<S1, S2, S3, E, T> : AbstractPipe<S1, S2, S3, E>, ISideEffectPipe<S1, S2, S3, E, T>
    {

        #region Properties

        #region SideEffect

        /// <summary>
        /// The internal side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect);
        /// </summary>
        protected T InternalSideEffect;

        /// <summary>
        /// The side effect produced by this pipe.
        /// </summary>
        public T SideEffect
        {

            get
            {
                return InternalSideEffect;
            }

            protected set
            {
                InternalSideEffect = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        #region (protected) AbstractSideEffectPipe()

        /// <summary>
        /// Creates an new abstract side effect pipe.
        /// </summary>
        protected AbstractSideEffectPipe()
        { }

        #endregion

        #region AbstractSideEffectPipe(SourceElement1, SourceElement2, SourceElement3, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given single values as element sources.
        /// </summary>
        /// <param name="SourceElement1">A single value as first element source.</param>
        /// <param name="SourceElement2">A single value as second element source.</param>
        /// <param name="SourceElement3">A single value as third element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(S1  SourceElement1,
                                      S2  SourceElement2,
                                      S3  SourceElement3,
                                      T   SideEffect)

            : base(SourceElement1, SourceElement2, SourceElement3)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #region AbstractSideEffectPipe(SourcePipe1, SourcePipe2, SourcePipe3, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given pipes as element sources.
        /// </summary>
        /// <param name="SourcePipe1">A pipe as first element source.</param>
        /// <param name="SourcePipe2">A pipe as second element source.</param>
        /// <param name="SourcePipe3">A pipe as third element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEndPipe<S1>  SourcePipe1,
                                      IEndPipe<S2>  SourcePipe2,
                                      IEndPipe<S3>  SourcePipe3,
                                      T             SideEffect)

            : base(SourcePipe1, SourcePipe2, SourcePipe3)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #region AbstractSideEffectPipe(SourceEnumerator1, SourceEnumerator2, SourceEnumerator3, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerators as element sources.
        /// </summary>
        /// <param name="SourceEnumerator1">An enumerator as first element source.</param>
        /// <param name="SourceEnumerator2">An enumerator as second element source.</param>
        /// <param name="SourceEnumerator3">An enumerator as third element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEnumerator<S1>  SourceEnumerator1,
                                      IEnumerator<S2>  SourceEnumerator2,
                                      IEnumerator<S3>  SourceEnumerator3,
                                      T                SideEffect)

            : base(SourceEnumerator1, SourceEnumerator2, SourceEnumerator3)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #region AbstractSideEffectPipe(SourceEnumerable1, SourceEnumerable2, SourceEnumerable3, SideEffect)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerables as element sources.
        /// </summary>
        /// <param name="SourceEnumerable1">An enumerable as first element source.</param>
        /// <param name="SourceEnumerable2">An enumerable as second element source.</param>
        /// <param name="SourceEnumerable3">An enumerable as third element source.</param>
        /// <param name="SideEffect">The initial value of the side effect.</param>
        public AbstractSideEffectPipe(IEnumerable<S1>  SourceEnumerable1,
                                      IEnumerable<S2>  SourceEnumerable2,
                                      IEnumerable<S3>  SourceEnumerable3,
                                      T                SideEffect)

            : base(SourceEnumerable1, SourceEnumerable2, SourceEnumerable3)

        {
            this.SideEffect = SideEffect;
        }

        #endregion

        #endregion

    }

    #endregion


    #region AbstractTwoSideEffectsPipe<S, E, T1, T2>

    /// <summary>
    /// An AbstractSideEffectPipe provides the same functionality as the 
    /// AbstractPipe, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T1">The type of the first sideeffect.</typeparam>
    /// <typeparam name="T2">The type of the second sideeffect.</typeparam>
    public abstract class AbstractTwoSideEffectsPipe<S, E, T1, T2> : AbstractPipe<S, E>, ITwoSideEffectsPipe<S, E, T1, T2>
    {

        #region Properties

        #region SideEffect1

        /// <summary>
        /// The first side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect1);
        /// </summary>
        protected T1 InternalSideEffect1;

        /// <summary>
        /// The first side effect produced by this pipe.
        /// </summary>
        public T1 SideEffect1
        {

            get
            {
                return InternalSideEffect1;
            }

            protected set
            {
                InternalSideEffect1 = value;
            }

        }

        #endregion

        #region SideEffect2

        /// <summary>
        /// The second side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect2);
        /// </summary>
        protected T2 InternalSideEffect2;

        /// <summary>
        /// The second side effect produced by this pipe.
        /// </summary>
        public T2 SideEffect2
        {

            get
            {
                return InternalSideEffect2;
            }

            protected set
            {
                InternalSideEffect2 = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        #region (protected) AbstractTwoSideEffectsPipe()

        /// <summary>
        /// Creates an new abstract side effect pipe.
        /// </summary>
        protected AbstractTwoSideEffectsPipe()
        { }

        #endregion

        #region AbstractTwoSideEffectsPipe(SourceElement, SideEffect1, SideEffect2)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given single value as element source.
        /// </summary>
        /// <param name="SourceElement">A single value as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        public AbstractTwoSideEffectsPipe(S   SourceElement,
                                          T1  SideEffect1,
                                          T2  SideEffect2)

            : base(SourceElement)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;

        }

        #endregion

        #region AbstractTwoSideEffectsPipe(SourcePipe, SideEffect1, SideEffect2)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given pipe as element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        public AbstractTwoSideEffectsPipe(IEndPipe<S>  SourcePipe,
                                          T1           SideEffect1,
                                          T2           SideEffect2)

            : base(SourcePipe)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;

        }


        #endregion

        #region AbstractTwoSideEffectsPipe(SourceEnumerator, SideEffect1, SideEffect2)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerator as element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        public AbstractTwoSideEffectsPipe(IEnumerator<S>  SourceEnumerator,
                                          T1              SideEffect1,
                                          T2              SideEffect2)

            : base(SourceEnumerator)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;

        }


        #endregion

        #region AbstractTwoSideEffectsPipe(SourceEnumerable, SideEffect1, SideEffect2)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        public AbstractTwoSideEffectsPipe(IEnumerable<S>  SourceEnumerable,
                                          T1              SideEffect1,
                                          T2              SideEffect2)

            : base(SourceEnumerable)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;

        }

        #endregion

        #endregion

    }

    #endregion

    #region AbstractThreeSideEffectsPipe<S, E, T1, T2, T3>

    /// <summary>
    /// An AbstractSideEffectPipe provides the same functionality as the 
    /// AbstractPipe, but produces a side effect which can be retrieved
    /// by the SideEffect property.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T1">The type of the first sideeffect.</typeparam>
    /// <typeparam name="T2">The type of the second sideeffect.</typeparam>
    /// <typeparam name="T3">The type of the third sideeffect.</typeparam>
    public abstract class AbstractThreeSideEffectsPipe<S, E, T1, T2, T3> : AbstractPipe<S, E>, IThreeSideEffectsPipe<S, E, T1, T2, T3>
    {

        #region Properties

        #region SideEffect1

        /// <summary>
        /// The first side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect1);
        /// </summary>
        protected T1 InternalSideEffect1;

        /// <summary>
        /// The first side effect produced by this pipe.
        /// </summary>
        public T1 SideEffect1
        {

            get
            {
                return InternalSideEffect1;
            }

            protected set
            {
                InternalSideEffect1 = value;
            }

        }

        #endregion

        #region SideEffect2

        /// <summary>
        /// The second side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect2);
        /// </summary>
        protected T2 InternalSideEffect2;

        /// <summary>
        /// The second side effect produced by this pipe.
        /// </summary>
        public T2 SideEffect2
        {

            get
            {
                return InternalSideEffect2;
            }

            protected set
            {
                InternalSideEffect2 = value;
            }

        }

        #endregion

        #region SideEffect3

        /// <summary>
        /// The third side effect produced by this pipe.
        /// Use this reference for operations like:
        /// Interlocked.Increment(ref InternalSideEffect3);
        /// </summary>
        protected T3 InternalSideEffect3;

        /// <summary>
        /// The third side effect produced by this pipe.
        /// </summary>
        public T3 SideEffect3
        {

            get
            {
                return InternalSideEffect3;
            }

            protected set
            {
                InternalSideEffect3 = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        #region (protected) AbstractThreeSideEffectsPipe()

        /// <summary>
        /// Creates an new abstract side effect pipe.
        /// </summary>
        protected AbstractThreeSideEffectsPipe()
        { }

        #endregion

        #region AbstractThreeSideEffectsPipe(SourceElement, SideEffect1, SideEffect2, SideEffect3)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given single value as element source.
        /// </summary>
        /// <param name="SourceElement">A single value as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        /// <param name="SideEffect3">The initial value of the third side effect.</param>
        public AbstractThreeSideEffectsPipe(S   SourceElement,
                                            T1  SideEffect1,
                                            T2  SideEffect2,
                                            T3  SideEffect3)

            : base(SourceElement)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;
            this.SideEffect3 = SideEffect3;

        }

        #endregion

        #region AbstractThreeSideEffectsPipe(SourcePipe, SideEffect1, SideEffect2, SideEffect3)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given pipe as element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        /// <param name="SideEffect3">The initial value of the third side effect.</param>
        public AbstractThreeSideEffectsPipe(IEndPipe<S>  SourcePipe,
                                            T1           SideEffect1,
                                            T2           SideEffect2,
                                            T3           SideEffect3)

            : base(SourcePipe)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;
            this.SideEffect3 = SideEffect3;

        }


        #endregion

        #region AbstractThreeSideEffectsPipe(SourceEnumerator, SideEffect1, SideEffect2, SideEffect3)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerator as element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        /// <param name="SideEffect3">The initial value of the third side effect.</param>
        public AbstractThreeSideEffectsPipe(IEnumerator<S>  SourceEnumerator,
                                            T1              SideEffect1,
                                            T2              SideEffect2,
                                            T3              SideEffect3)

            : base(SourceEnumerator)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;
            this.SideEffect3 = SideEffect3;

        }


        #endregion

        #region AbstractThreeSideEffectsPipe(SourceEnumerable, SideEffect1, SideEffect2, SideEffect3)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        /// <param name="SideEffect1">The initial value of the first side effect.</param>
        /// <param name="SideEffect2">The initial value of the second side effect.</param>
        /// <param name="SideEffect3">The initial value of the third side effect.</param>
        public AbstractThreeSideEffectsPipe(IEnumerable<S>  SourceEnumerable,
                                            T1              SideEffect1,
                                            T2              SideEffect2,
                                            T3              SideEffect3)

            : base(SourceEnumerable)

        {

            this.SideEffect1 = SideEffect1;
            this.SideEffect2 = SideEffect2;
            this.SideEffect3 = SideEffect3;

        }

        #endregion

        #endregion

    }

    #endregion

}
