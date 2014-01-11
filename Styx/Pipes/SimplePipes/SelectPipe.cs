/*
 * Copyright (c) 2010-2014, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

namespace eu.Vanaheimr.Styx
{

    public static class SelectPipeExtensions
    {

        public static SelectPipe<S, E> Select<S, E>(this IEndPipe<S>  Pipe,
                                                    Func<S, E>        Mapper)
        {
            return new SelectPipe<S, E>(Pipe, Mapper);
        }

        /// <summary>
        /// Starts with 1!
        /// </summary>
        public static SelectPipe<S, E> Select<S, E>(this IEndPipe<S>    Pipe,
                                                    Func<S, UInt64, E>  CountedMapper)
        {
            return new SelectPipe<S, E>(Pipe, CountedMapper);
        }

    }


    #region SelectPipe<S, E>

    /// <summary>
    /// Converts the consuming objects to emitting objects
    /// by calling a Func&lt;S, E&gt;.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class SelectPipe<S, E> : AbstractPipe<S, E>
    {

        #region Data

        private readonly Func<Boolean>       MapperDelegate;
        private readonly Func<S, E>          Mapper;
        private readonly Func<S, UInt64, E>  CountedMapper;
        private          UInt64              Counter;

        #endregion

        #region Constructor(s)

        #region SelectPipe(SourceValue, Mapper)

        public SelectPipe(S            SourceValue,
                          Func<S, E>   Mapper)

            : base(SourceValue)

        {

            this.Mapper = Mapper;

            if (SourcePipe == null)
                this.MapperDelegate = () => false;

            if (Mapper == null)
                this.MapperDelegate = () => false;

            this.MapperDelegate = () => {

                if (SourcePipe.MoveNext())
                {
                    _CurrentElement = Mapper(SourcePipe.Current);
                    return true;
                }

                return false;

            };

        }

        #endregion

        #region SelectPipe(SourcePipe, Mapper)

        public SelectPipe(IEndPipe<S>  SourcePipe,
                          Func<S, E>   Mapper)

            : base(SourcePipe)

        {

            this.Mapper = Mapper;

            if (SourcePipe == null)
                this.MapperDelegate = () => false;

            if (Mapper == null)
                this.MapperDelegate = () => false;

            this.MapperDelegate = () => {

                if (SourcePipe.MoveNext())
                {
                    _CurrentElement = Mapper(SourcePipe.Current);
                    return true;
                }

                return false;

            };

        }

        #endregion

        #region SelectPipe(SourceEnumerator, Mapper)

        public SelectPipe(IEnumerator<S>  SourceEnumerator,
                          Func<S, E>      Mapper)

            : this(new IEnumerator2IEndPipe<S>(SourceEnumerator), Mapper)

        { }

        #endregion

        #region SelectPipe(SourceEnumerable, Mapper)

        /// <summary>
        /// Creates a new AbstractPipe using the elements emitted
        /// by the given IEnumerable as input.
        /// </summary>
        /// <param name="SourceEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        public SelectPipe(IEnumerable<S> SourceEnumerable, Func<S, E> Mapper)
            : this(new IEnumerator2IEndPipe<S>(SourceEnumerable.GetEnumerator()), Mapper)
        { }

        #endregion


        #region SelectPipe(SourceValue, CountedMapper)

        public SelectPipe(S                   SourceValue,
                          Func<S, UInt64, E>  CountedMapper)

            : base(SourceValue)

        {

            this.CountedMapper  = CountedMapper;
            this.Counter        = 1UL;

            if (SourcePipe == null)
                this.MapperDelegate = () => false;

            if (Mapper == null)
                this.MapperDelegate = () => false;

            this.MapperDelegate = () =>
            {

                if (SourcePipe.MoveNext())
                {
                    _CurrentElement = CountedMapper(SourcePipe.Current, Counter++);
                    return true;
                }

                return false;

            };

        }

        #endregion

        #region SelectPipe(SourcePipe, CountedMapper)

        /// <summary>
        /// Creates a new FuncPipe using the given Func&lt;S, E&gt;.
        /// </summary>
        public SelectPipe(IEndPipe<S>         SourcePipe,
                          Func<S, UInt64, E>  CountedMapper)

            : base(SourcePipe)

        {

            this.CountedMapper  = CountedMapper;
            this.Counter        = 1UL;

            if (SourcePipe == null)
                this.MapperDelegate = () => false;

            if (Mapper == null)
                this.MapperDelegate = () => false;


            this.MapperDelegate = () =>
            {

                if (SourcePipe.MoveNext())
                {
                    _CurrentElement = CountedMapper(SourcePipe.Current, Counter++);
                    return true;
                }

                return false;

            };

        }

        #endregion

        #region SelectPipe(SourceEnumerator, CountedMapper)

        public SelectPipe(IEnumerator<S>      SourceEnumerator,
                          Func<S, UInt64, E>  CountedMapper)

            : this(new IEnumerator2IEndPipe<S>(SourceEnumerator), CountedMapper)

        { }

        #endregion

        #region SelectPipe(SourceEnumerable, CountedMapper)

        /// <summary>
        /// Creates a new AbstractPipe using the elements emitted
        /// by the given IEnumerable as input.
        /// </summary>
        /// <param name="SourceEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        public SelectPipe(IEnumerable<S>      SourceEnumerable,
                          Func<S, UInt64, E>  CountedMapper)

            : this(new IEnumerator2IEndPipe<S>(SourceEnumerable.GetEnumerator()), CountedMapper)
        { }

        #endregion

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            return MapperDelegate();

            //if (_SourceEnumerator == null)
            //    return false;

            //if (Mapper == null)
            //    return false;

            //if (_SourceEnumerator.MoveNext())
            //{
            //    _CurrentElement = Mapper(_SourceEnumerator.Current);
            //    return true;
            //}

            //return false;

        }

        #endregion

    }

    #endregion

    #region FuncPipe<S1, S2, E>

    /// <summary>
    /// Converts the consuming objects to emitting objects
    /// by calling a Func&lt;S1, S2, E&gt;.
    /// </summary>
    /// <typeparam name="S1">The type of the consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class FuncPipe<S1, S2, E> : AbstractPipe<S1, S2, E>
    {

        #region Data

        private readonly Func<S1, S2, E> Func;

        #endregion

        #region Constructor(s)

        #region FuncPipe(Func)

        /// <summary>
        /// Creates a new FuncPipe using the elements emitted
        /// by the given IEnumerables as input.
        /// </summary>
        /// <param name="Func">A Func&lt;S1, S2, E&gt; converting the consuming objects into emitting objects.</param>
        /// <param name="IEnumerator1">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator2">An optional enumerator of directories as element source.</param>
        public FuncPipe(Func<S1, S2, E> Func,
                        IEnumerator<S1> IEnumerator1 = null,
                        IEnumerator<S2> IEnumerator2 = null)
        {

            if (Func == null)
                throw new ArgumentNullException("The given Func must not be null!");

            this.Func = Func;

            if (IEnumerator1 != null)
                SetSource1(IEnumerator1);

            if (IEnumerator2 != null)
                SetSource2(IEnumerator2);

        }

        #endregion

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (SourcePipe1 == null)
                return false;

            if (SourcePipe2 == null)
                return false;

            if (SourcePipe1.MoveNext())
                if (SourcePipe2.MoveNext())
                {
                    _CurrentElement = Func(SourcePipe1.Current, SourcePipe2.Current);
                    return true;
                }

            return false;

        }

        #endregion

    }

    #endregion

    #region FuncPipe<S1, S2, S3, E>

    /// <summary>
    /// Converts the consuming objects to emitting objects
    /// by calling a Func&lt;S1, S2, S3, E&gt;.
    /// </summary>
    /// <typeparam name="S1">The type of the consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class FuncPipe<S1, S2, S3, E> : AbstractPipe<S1, S2, S3, E>
    {

        #region Data

        private Func<S1, S2, S3, E> Func;

        #endregion

        #region Constructor(s)

        #region FuncPipe(Func)

        /// <summary>
        /// Creates a new FuncPipe using the elements emitted
        /// by the given IEnumerables as input.
        /// </summary>
        /// <param name="Func">A Func&lt;S1, S2, S3, E&gt; converting the consuming objects into emitting objects.</param>
        /// <param name="IEnumerator1">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator2">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator3">An optional enumerator of directories as element source.</param>
        public FuncPipe(Func<S1, S2, S3, E> Func,
                        IEnumerator<S1> IEnumerator1 = null,
                        IEnumerator<S2> IEnumerator2 = null,
                        IEnumerator<S3> IEnumerator3 = null)
        {

            if (Func == null)
                throw new ArgumentNullException("The given Func must not be null!");

            this.Func = Func;

        }

        #endregion

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (SourcePipe1 == null)
                return false;

            if (SourcePipe2 == null)
                return false;

            if (SourcePipe3 == null)
                return false;

            if (SourcePipe1.MoveNext())
                if (SourcePipe2.MoveNext())
                    if (SourcePipe3.MoveNext())
                    {
                        _CurrentElement = Func(SourcePipe1.Current, SourcePipe2.Current, SourcePipe3.Current);
                        return true;
                    }

            return false;

        }

        #endregion

    }

    #endregion

    #region FuncPipe<S1, S2, S3, S4, E>

    /// <summary>
    /// Converts the consuming objects to emitting objects
    /// by calling a Func&lt;S1, S2, S3, S4, E&gt;.
    /// </summary>
    /// <typeparam name="S1">The type of the consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the consuming objects.</typeparam>
    /// <typeparam name="S4">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class FuncPipe<S1, S2, S3, S4, E> : AbstractPipe<S1, S2, S3, S4, E>
    {

        #region Data

        private Func<S1, S2, S3, S4, E> Func;

        #endregion

        #region Constructor(s)

        #region FuncPipe(Func)

        /// <summary>
        /// Creates a new FuncPipe using the elements emitted
        /// by the given IEnumerables as input.
        /// </summary>
        /// <param name="Func">A Func&lt;S1, S2, S3, S4, E&gt; converting the consuming objects into emitting objects.</param>
        /// <param name="IEnumerator1">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator2">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator3">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator4">An optional enumerator of directories as element source.</param>
        public FuncPipe(Func<S1, S2, S3, S4, E> Func,
                        IEnumerator<S1> IEnumerator1 = null,
                        IEnumerator<S2> IEnumerator2 = null,
                        IEnumerator<S3> IEnumerator3 = null,
                        IEnumerator<S4> IEnumerator4 = null)
        {

            if (Func == null)
                throw new ArgumentNullException("The given Func must not be null!");

            this.Func = Func;

        }

        #endregion

        #endregion


        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (_InternalEnumerator1 == null)
                return false;

            if (_InternalEnumerator2 == null)
                return false;

            if (_InternalEnumerator3 == null)
                return false;

            if (_InternalEnumerator4 == null)
                return false;

            if (_InternalEnumerator1.MoveNext())
                if (_InternalEnumerator2.MoveNext())
                    if (_InternalEnumerator3.MoveNext())
                        if (_InternalEnumerator4.MoveNext())
                        {
                            _CurrentElement = Func(_InternalEnumerator1.Current, _InternalEnumerator2.Current, _InternalEnumerator3.Current, _InternalEnumerator4.Current);
                            return true;
                        }

            return false;

        }

        #endregion

    }

    #endregion

    #region FuncPipe<S1, S2, S3, S4, S5, E>

    /// <summary>
    /// Converts the consuming objects to emitting objects
    /// by calling a Func&lt;S1, S2, S3, S4, E&gt;.
    /// </summary>
    /// <typeparam name="S1">The type of the consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the consuming objects.</typeparam>
    /// <typeparam name="S4">The type of the consuming objects.</typeparam>
    /// <typeparam name="S5">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class FuncPipe<S1, S2, S3, S4, S5, E> : AbstractPipe<S1, S2, S3, S4, S5, E>
    {

        #region Data

        private Func<S1, S2, S3, S4, E> Func;

        #endregion

        #region Constructor(s)

        #region FuncPipe(Func)

        /// <summary>
        /// Creates a new FuncPipe using the elements emitted
        /// by the given IEnumerables as input.
        /// </summary>
        /// <param name="Func">A Func&lt;S1, S2, S3, S4, E&gt; converting the consuming objects into emitting objects.</param>
        /// <param name="IEnumerator1">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator2">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator3">An optional enumerator of directories as element source.</param>
        /// <param name="IEnumerator4">An optional enumerator of directories as element source.</param>
        public FuncPipe(Func<S1, S2, S3, S4, E> Func,
                        IEnumerator<S1> IEnumerator1 = null,
                        IEnumerator<S2> IEnumerator2 = null,
                        IEnumerator<S3> IEnumerator3 = null,
                        IEnumerator<S4> IEnumerator4 = null)
        {

            if (Func == null)
                throw new ArgumentNullException("The given Func must not be null!");

            this.Func = Func;

        }

        #endregion

        #endregion


        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (_InternalEnumerator1 == null)
                return false;

            if (_InternalEnumerator2 == null)
                return false;

            if (_InternalEnumerator3 == null)
                return false;

            if (_InternalEnumerator4 == null)
                return false;

            if (_InternalEnumerator1.MoveNext())
                if (_InternalEnumerator2.MoveNext())
                    if (_InternalEnumerator3.MoveNext())
                        if (_InternalEnumerator4.MoveNext())
                        {
                            _CurrentElement = Func(_InternalEnumerator1.Current, _InternalEnumerator2.Current, _InternalEnumerator3.Current, _InternalEnumerator4.Current);
                            return true;
                        }

            return false;

        }

        #endregion

    }

    #endregion

}
