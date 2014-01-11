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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using eu.Vanaheimr.Illias.Commons;

#endregion

namespace eu.Vanaheimr.Styx
{

    #region AbstractPipe<S, E>

    /// <summary>
    /// An AbstractPipe provides most of the functionality that is repeated
    /// in every instance of a Pipe. Any subclass of AbstractPipe should simply
    /// implement MoveNext().
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public abstract class AbstractPipe<S, E> : IPipe<S, E>
    {

        #region Data

        /// <summary>
        /// The source pipe.
        /// </summary>
        protected IEndPipe<S> SourcePipe;

        /// <summary>
        /// The current element in the pipe.
        /// </summary>
        protected E _CurrentElement;

        #endregion

        #region Constructor(s)

        #region (protected) AbstractPipe()

        /// <summary>
        /// Creates an abstract pipe.
        /// </summary>
        protected AbstractPipe()
        { }

        #endregion

        #region AbstractPipe(SourceElement)

        /// <summary>
        /// Creates an new abstract pipe using the given single value as element source.
        /// </summary>
        /// <param name="SourceElement">A single value as element source.</param>
        public AbstractPipe(S SourceElement)
        {

            SourceElement.CheckNull("SourceElement");

            this.SourcePipe = new IEnumerator2IEndPipe<S>((new List<S>() { SourceElement }).GetEnumerator());

        }

        #endregion

        #region AbstractPipe(SourcePipe)

        /// <summary>
        /// Creates an new abstract pipe using the given pipe as element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public AbstractPipe(IEndPipe<S> SourcePipe)
        {

            SourcePipe.CheckNull("SourcePipe");

            this.SourcePipe = SourcePipe;

        }

        #endregion

        #region AbstractPipe(SourceEnumerator)

        /// <summary>
        /// Creates an new abstract pipe using the given enumerator as element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public AbstractPipe(IEnumerator<S> SourceEnumerator)
        {

            SourceEnumerator.CheckNull("SourceEnumerator");

            this.SourcePipe = new IEnumerator2IEndPipe<S>(SourceEnumerator);

        }

        #endregion

        #region AbstractPipe(SourceEnumerable)

        /// <summary>
        /// Creates an new abstract pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public AbstractPipe(IEnumerable<S> SourceEnumerable)
        {

            SourceEnumerable.CheckNull("SourceEnumerable");

            this.SourcePipe = new IEnumerator2IEndPipe<S>(SourceEnumerable.GetEnumerator());

        }

        #endregion

        #endregion


        #region SetSource(SourceElement)

        /// <summary>
        /// Set the given single value as element source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource(S SourceElement)
        {

            SourceElement.CheckNull("SourceElement");

            this.SourcePipe = new IEnumerator2IEndPipe<S>((new List<S>() { SourceElement }).GetEnumerator());

        }

        #endregion

        #region SetSource(SourcePipe)

        /// <summary>
        /// Set the given pipe as element source.
        /// </summary> 
        /// <param name="SourcePipe">A pipe as element source.</param>
        public virtual void SetSource(IEndPipe<S> SourcePipe)
        {

            SourcePipe.CheckNull("SourcePipe");

            this.SourcePipe = SourcePipe;

        }

        #endregion

        #region SetSource(SourceEnumerator)

        /// <summary>
        /// Set the given enumerator as element source.
        /// </summary> 
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public virtual void SetSource(IEnumerator<S> SourceEnumerator)
        {

            SourceEnumerator.CheckNull("SourceEnumerator");

            this.SourcePipe = new IEnumerator2IEndPipe<S>(SourceEnumerator);

        }

        #endregion

        #region SetSource(SourceEnumerable)

        /// <summary>
        /// Set the given enumerable as element source.
        /// </summary> 
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public virtual void SetSource(IEnumerable<S> SourceEnumerable)
        {

            SourceEnumerable.CheckNull("SourceEnumerable");

            this.SourcePipe = new IEnumerator2IEndPipe<S>(SourceEnumerable.GetEnumerator());

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator that iterates through the pipe.
        /// </summary>
        /// <returns>
        /// A IEnumerator&lt;E&gt; that can be used to iterate through the pipe.
        /// </returns>
        public IEnumerator<E> GetEnumerator()
        {
            return new IEndPipe2IEnumerator<E>(this);
        }

        #endregion

        #region Current

        /// <summary>
        /// Return the current element in the pipe.
        /// </summary>
        public E Current
        {
            get
            {
                return _CurrentElement;
            }
        }

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the pipe.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// pipe.
        /// </returns>
        public abstract Boolean MoveNext();

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the pipe. If the pipe has
        /// no internal state the pipe will just call Reset() on
        /// its source pipe.
        /// </summary>
        public virtual IEndPipe<E> Reset()
        {

            SourcePipe.Reset();

            return this;

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        {
            if (SourcePipe != null)
                SourcePipe.Dispose();
        }

        #endregion


        #region Path

        /// <summary>
        /// Returns the transformation path to arrive at the current object
        /// of the pipe. This is a list of all of the objects traversed for
        /// the current iterator position of the pipe.
        /// </summary>
        public virtual IEnumerable<Object> Path
        {

            get
            {

                var _PathElements = PathToHere.ToList();
                var _Size         = _PathElements.Count();

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(_CurrentElement))
                    _PathElements.Add(_CurrentElement);

                return _PathElements;

            }

        }

        #endregion

        #region PathToHere

        private IEnumerable<Object> PathToHere
        {

            get
            {

                if (SourcePipe is IPipe)
                    return ((IPipe) SourcePipe).Path;

                else if (SourcePipe is IHistoryEnumerator)
                {

                    var _List = new List<Object>();
                    var _Last = ((IHistoryEnumerator) SourcePipe).Last;

                    if (_Last == null)
                    {
                        //if (_InputEnumerator.MoveNext())
                        SourcePipe.MoveNext();
                        _List.Add(SourcePipe.Current);
                    }
                    else
                        _List.Add(_Last);

                    return _List;

                }

                else if (SourcePipe is ISingleEnumerator)
                    return new List<Object>() { ((ISingleEnumerator) SourcePipe).Current };

                else
                    return new List<Object>();

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {

            return (SourcePipe != null)
                        ? this.GetType().Name + "<" + SourcePipe.Current + ">"
                        : this.GetType().Name;

        }

        #endregion

    }

    #endregion

    #region AbstractPipe<S1, S2, E>

    /// <summary>
    /// An AbstractPipe provides most of the functionality that is repeated
    /// in every instance of a Pipe. Any subclass of AbstractPipe should simply
    /// implement MoveNext().
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public abstract class AbstractPipe<S1, S2, E> : IPipe<S1, S2, E>
    {

        #region Data

        /// <summary>
        /// The first source pipe.
        /// </summary>
        protected IEndPipe<S1> SourcePipe1;

        /// <summary>
        /// The second source pipe.
        /// </summary>
        protected IEndPipe<S2> SourcePipe2;

        /// <summary>
        /// The current element in the pipe.
        /// </summary>
        protected E _CurrentElement;

        #endregion

        #region Constructor(s)

        #region (protected) AbstractPipe()

        /// <summary>
        /// Creates a new abstract pipe.
        /// </summary>
        protected AbstractPipe()
        { }

        #endregion

        #region AbstractPipe(SourceElement1, SourceElement2)

        /// <summary>
        /// Creates an new abstract pipe using the given single values as element sources.
        /// </summary>
        /// <param name="SourceElement1">A single value as first element source.</param>
        /// <param name="SourceElement2">A single value as second element source.</param>
        public AbstractPipe(S1  SourceElement1,
                            S2  SourceElement2)
        {

            SourceElement1.CheckNull("SourceElement1");
            SourceElement2.CheckNull("SourceElement2");

            this.SourcePipe1 = new IEnumerator2IEndPipe<S1>((new List<S1>() { SourceElement1 }).GetEnumerator());
            this.SourcePipe2 = new IEnumerator2IEndPipe<S2>((new List<S2>() { SourceElement2 }).GetEnumerator());

        }

        #endregion

        #region AbstractPipe(SourcePipe1, SourcePipe2)

        /// <summary>
        /// Creates an new abstract pipe using the given pipes as element sources.
        /// </summary>
        /// <param name="SourcePipe1">A pipe as first element source.</param>
        /// <param name="SourcePipe2">A pipe as second element source.</param>
        public AbstractPipe(IEndPipe<S1>  SourcePipe1,
                            IEndPipe<S2>  SourcePipe2)
        {

            SourcePipe1.CheckNull("SourcePipe1");
            SourcePipe2.CheckNull("SourcePipe2");

            this.SourcePipe1 = SourcePipe1;
            this.SourcePipe2 = SourcePipe2;

        }

        #endregion

        #region AbstractPipe(SourceEnumerator1, SourceEnumerator2)

        /// <summary>
        /// Creates an new abstract pipe using the given enumerators as element sources.
        /// </summary>
        /// <param name="SourceEnumerator1">An enumerator as first element source.</param>
        /// <param name="SourceEnumerator2">An enumerator as second element source.</param>
        public AbstractPipe(IEnumerator<S1>  SourceEnumerator1,
                            IEnumerator<S2>  SourceEnumerator2)
        {

            SourceEnumerator1.CheckNull("SourceEnumerator1");
            SourceEnumerator2.CheckNull("SourceEnumerator2");

            this.SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerator1);
            this.SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerator2);

        }

        #endregion

        #region AbstractPipe(SourceEnumerable1, SourceEnumerable2)

        /// <summary>
        /// Creates an new abstract pipe using the given enumerables as element sources.
        /// </summary> 
        /// <param name="SourceEnumerable1">An enumerable as first element source.</param>
        /// <param name="SourceEnumerable2">An enumerable as second element source.</param>
        public AbstractPipe(IEnumerable<S1>  SourceEnumerable1,
                            IEnumerable<S2>  SourceEnumerable2)
        {

            SourceEnumerable1.CheckNull("SourceEnumerable1");
            SourceEnumerable2.CheckNull("SourceEnumerable2");

            this.SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerable1.GetEnumerator());
            this.SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerable2.GetEnumerator());

        }

        #endregion

        #endregion


        #region SetSource1(SourceElement)

        /// <summary>
        /// Set the given single value as first element source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource1(S1 SourceElement)
        {
            SourceElement.CheckNull("SourceElement");
            SourcePipe1 = new IEnumerator2IEndPipe<S1>((new List<S1>() { SourceElement }).GetEnumerator());
        }

        #endregion

        #region SetSource2(SourceElement)

        /// <summary>
        /// Set the given single value as second element source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource2(S2 SourceElement)
        {
            SourceElement.CheckNull("SourceElement");
            SourcePipe2 = new IEnumerator2IEndPipe<S2>((new List<S2>() { SourceElement }).GetEnumerator());
        }

        #endregion


        #region SetSource1(SourcePipe)

        /// <summary>
        /// Set the given pipe as first element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public virtual void SetSource1(IEndPipe<S1> SourcePipe)
        {
            SourcePipe.CheckNull("SourcePipe");
            SourcePipe1 = SourcePipe;
        }

        #endregion

        #region SetSource2(SourcePipe)

        /// <summary>
        /// Set the given pipe as second element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public virtual void SetSource2(IEndPipe<S2> SourcePipe)
        {
            SourcePipe.CheckNull("SourcePipe");
            SourcePipe2 = SourcePipe;
        }

        #endregion


        #region SetSource1(SourceEnumerator)

        /// <summary>
        /// Set the given enumerator as first element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public virtual void SetSource1(IEnumerator<S1> SourceEnumerator)
        {
            SourceEnumerator.CheckNull("SourceEnumerator");
            SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerator);
        }

        #endregion

        #region SetSource2(SourceEnumerator)

        /// <summary>
        /// Set the given enumerator as second element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public virtual void SetSource2(IEnumerator<S2> SourceEnumerator)
        {
            SourceEnumerator.CheckNull("SourceEnumerator");
            SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerator);
        }

        #endregion


        #region SetSource1(SourceEnumerable)

        /// <summary>
        /// Set the given enumerable as first element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public virtual void SetSource1(IEnumerable<S1> SourceEnumerable)
        {
            SourceEnumerable.CheckNull("SourceEnumerable");
            SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerable.GetEnumerator());
        }

        #endregion

        #region SetSource2(SourceEnumerable)

        /// <summary>
        /// Set the given enumerable as second element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public virtual void SetSource2(IEnumerable<S2> SourceEnumerable)
        {
            SourceEnumerable.CheckNull("SourceEnumerable");
            SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerable.GetEnumerator());
        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator&lt;E&gt; that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<E> GetEnumerator()
        {
            return new IEndPipe2IEnumerator<E>(this);
        }

        #endregion

        #region Current

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public E Current
        {
            get
            {
                return _CurrentElement;
            }
        }

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
        public abstract Boolean MoveNext();

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerators to their initial positions, which
        /// is before the first element in the collections.
        /// </summary>
        public virtual IEndPipe<E> Reset()
        {

            SourcePipe1.Reset();
            SourcePipe2.Reset();

            return this;

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        {
            SourcePipe1.Dispose();
            SourcePipe2.Dispose();
        }

        #endregion


        #region Path

        /// <summary>
        /// Returns the transformation path to arrive at the current object
        /// of the pipe. This is a list of all of the objects traversed for
        /// the current iterator position of the pipe.
        /// </summary>
        public virtual IEnumerable<Object> Path
        {

            get
            {

                var _PathElements = PathToHere.ToList();
                var _Size         = _PathElements.Count;

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)            
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(_CurrentElement))
                    _PathElements.Add(_CurrentElement);

                return _PathElements;

            }

        }

        #endregion

        #region PathToHere

        private IEnumerable<Object> PathToHere
        {

            get
            {

                throw new NotImplementedException();

                //if (_InternalEnumerator is IPipe)
                //    return ((IPipe) _InternalEnumerator).Path;

                //else if (_InternalEnumerator is IHistoryEnumerator)
                //{

                //    var _List = new List<Object>();
                //    var _Last = ((IHistoryEnumerator) _InternalEnumerator).Last;

                //    if (_Last == null)
                //        _List.Add(_InternalEnumerator.Current);
                //    else
                //        _List.Add(_Last);

                //    return _List;

                //}

                //else if (_InternalEnumerator is ISingleEnumerator)
                //    return new List<Object>() { ((ISingleEnumerator) _InternalEnumerator).Current };

                //else
                //    return new List<Object>();

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion

    }

    #endregion

    #region AbstractPipe<S1, S2, S3, E>

    /// <summary>
    /// An AbstractPipe provides most of the functionality that is repeated
    /// in every instance of a Pipe. Any subclass of AbstractPipe should simply
    /// implement MoveNext().
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the third consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public abstract class AbstractPipe<S1, S2, S3, E> : IPipe<S1, S2, S3, E>
    {

        #region Data

        /// <summary>
        /// The first source pipe.
        /// </summary>
        protected IEndPipe<S1> SourcePipe1;

        /// <summary>
        /// The second source pipe.
        /// </summary>
        protected IEndPipe<S2> SourcePipe2;

        /// <summary>
        /// The third source pipe.
        /// </summary>
        protected IEndPipe<S3> SourcePipe3;

        /// <summary>
        /// The current element in the pipe.
        /// </summary>
        protected E _CurrentElement;

        #endregion

        #region Constructor(s)

        #region (protected) AbstractPipe()

        /// <summary>
        /// Creates a new abstract pipe.
        /// </summary>
        protected AbstractPipe()
        { }

        #endregion

        #region AbstractPipe(SourceElement1, SourceElement2, SourceElement3)

        /// <summary>
        /// Creates an new abstract pipe using the given single values as element sources.
        /// </summary>
        /// <param name="SourceElement1">A single value as first element source.</param>
        /// <param name="SourceElement2">A single value as second element source.</param>
        /// <param name="SourceElement3">A single value as third element source.</param>
        public AbstractPipe(S1  SourceElement1,
                            S2  SourceElement2,
                            S3  SourceElement3)
        {

            SourceElement1.CheckNull("SourceElement1");
            SourceElement2.CheckNull("SourceElement2");
            SourceElement3.CheckNull("SourceElement3");

            this.SourcePipe1 = new IEnumerator2IEndPipe<S1>((new List<S1>() { SourceElement1 }).GetEnumerator());
            this.SourcePipe2 = new IEnumerator2IEndPipe<S2>((new List<S2>() { SourceElement2 }).GetEnumerator());
            this.SourcePipe3 = new IEnumerator2IEndPipe<S3>((new List<S3>() { SourceElement3 }).GetEnumerator());

        }

        #endregion

        #region AbstractPipe(SourcePipe1, SourcePipe2, SourcePipe3)

        /// <summary>
        /// Creates an new abstract pipe using the given pipes as element sources.
        /// </summary>
        /// <param name="SourcePipe1">A pipe as first element source.</param>
        /// <param name="SourcePipe2">A pipe as second element source.</param>
        /// <param name="SourcePipe3">A pipe as third element source.</param>
        public AbstractPipe(IEndPipe<S1>  SourcePipe1,
                            IEndPipe<S2>  SourcePipe2,
                            IEndPipe<S3>  SourcePipe3)
        {

            SourcePipe1.CheckNull("SourcePipe1");
            SourcePipe2.CheckNull("SourcePipe2");
            SourcePipe3.CheckNull("SourcePipe3");

            this.SourcePipe1 = SourcePipe1;
            this.SourcePipe2 = SourcePipe2;
            this.SourcePipe3 = SourcePipe3;

        }

        #endregion

        #region AbstractPipe(SourceEnumerator1, SourceEnumerator2, SourceEnumerator3)

        /// <summary>
        /// Creates an new abstract pipe using the given enumerators as element sources.
        /// </summary>
        /// <param name="SourceEnumerator1">An enumerator as first element source.</param>
        /// <param name="SourceEnumerator2">An enumerator as second element source.</param>
        /// <param name="SourceEnumerator3">An enumerator as third element source.</param>
        public AbstractPipe(IEnumerator<S1>  SourceEnumerator1,
                            IEnumerator<S2>  SourceEnumerator2,
                            IEnumerator<S3>  SourceEnumerator3)
        {

            SourceEnumerator1.CheckNull("SourceEnumerator1");
            SourceEnumerator2.CheckNull("SourceEnumerator2");
            SourceEnumerator3.CheckNull("SourceEnumerator3");

            this.SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerator1);
            this.SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerator2);
            this.SourcePipe3 = new IEnumerator2IEndPipe<S3>(SourceEnumerator3);

        }

        #endregion

        #region AbstractPipe(SourceEnumerable1, SourceEnumerable2, SourceEnumerable3)

        /// <summary>
        /// Creates an new abstract pipe using the given enumerables as element sources.
        /// </summary> 
        /// <param name="SourceEnumerable1">An enumerable as first element source.</param>
        /// <param name="SourceEnumerable2">An enumerable as second element source.</param>
        /// <param name="SourceEnumerable3">An enumerable as third element source.</param>
        public AbstractPipe(IEnumerable<S1>  SourceEnumerable1,
                            IEnumerable<S2>  SourceEnumerable2,
                            IEnumerable<S3>  SourceEnumerable3)
        {

            SourceEnumerable1.CheckNull("SourceEnumerable1");
            SourceEnumerable2.CheckNull("SourceEnumerable2");
            SourceEnumerable3.CheckNull("SourceEnumerable3");

            this.SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerable1.GetEnumerator());
            this.SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerable2.GetEnumerator());
            this.SourcePipe3 = new IEnumerator2IEndPipe<S3>(SourceEnumerable3.GetEnumerator());

        }

        #endregion

        #endregion


        #region SetSource1(SourceElement)

        /// <summary>
        /// Set the given single value as first element source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource1(S1 SourceElement)
        {
            SourceElement.CheckNull("SourceElement");
            SourcePipe1 = new IEnumerator2IEndPipe<S1>((new List<S1>() { SourceElement }).GetEnumerator());
        }

        #endregion

        #region SetSource2(SourceElement)

        /// <summary>
        /// Set the given single value as second element source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource2(S2 SourceElement)
        {
            SourceElement.CheckNull("SourceElement");
            SourcePipe2 = new IEnumerator2IEndPipe<S2>((new List<S2>() { SourceElement }).GetEnumerator());
        }

        #endregion

        #region SetSource3(SourceElement)

        /// <summary>
        /// Set the given single value as third element source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource3(S3 SourceElement)
        {
            SourceElement.CheckNull("SourceElement");
            SourcePipe3 = new IEnumerator2IEndPipe<S3>((new List<S3>() { SourceElement }).GetEnumerator());
        }

        #endregion


        #region SetSource1(SourcePipe)

        /// <summary>
        /// Set the given pipe as first element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public virtual void SetSource1(IEndPipe<S1> SourcePipe)
        {
            SourcePipe.CheckNull("SourcePipe");
            SourcePipe1 = SourcePipe;
        }

        #endregion

        #region SetSource2(SourcePipe)

        /// <summary>
        /// Set the given pipe as second element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public virtual void SetSource2(IEndPipe<S2> SourcePipe)
        {
            SourcePipe.CheckNull("SourcePipe");
            SourcePipe2 = SourcePipe;
        }

        #endregion

        #region SetSource3(SourcePipe)

        /// <summary>
        /// Set the given pipe as third element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public virtual void SetSource3(IEndPipe<S3> SourcePipe)
        {
            SourcePipe.CheckNull("SourcePipe");
            SourcePipe3 = SourcePipe;
        }

        #endregion


        #region SetSource1(SourceEnumerator)

        /// <summary>
        /// Set the given enumerator as first element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public virtual void SetSource1(IEnumerator<S1> SourceEnumerator)
        {
            SourceEnumerator.CheckNull("SourceEnumerator");
            SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerator);
        }

        #endregion

        #region SetSource2(SourceEnumerator)

        /// <summary>
        /// Set the given enumerator as second element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public virtual void SetSource2(IEnumerator<S2> SourceEnumerator)
        {
            SourceEnumerator.CheckNull("SourceEnumerator");
            SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerator);
        }

        #endregion

        #region SetSource3(SourceEnumerator)

        /// <summary>
        /// Set the given enumerator as second element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public virtual void SetSource3(IEnumerator<S3> SourceEnumerator)
        {
            SourceEnumerator.CheckNull("SourceEnumerator");
            SourcePipe3 = new IEnumerator2IEndPipe<S3>(SourceEnumerator);
        }

        #endregion


        #region SetSource1(SourceEnumerable)

        /// <summary>
        /// Set the given enumerable as first element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public virtual void SetSource1(IEnumerable<S1> SourceEnumerable)
        {
            SourceEnumerable.CheckNull("SourceEnumerable");
            SourcePipe1 = new IEnumerator2IEndPipe<S1>(SourceEnumerable.GetEnumerator());
        }

        #endregion

        #region SetSource2(SourceEnumerable)

        /// <summary>
        /// Set the given enumerable as second element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public virtual void SetSource2(IEnumerable<S2> SourceEnumerable)
        {
            SourceEnumerable.CheckNull("SourceEnumerable");
            SourcePipe2 = new IEnumerator2IEndPipe<S2>(SourceEnumerable.GetEnumerator());
        }

        #endregion

        #region SetSource3(SourceEnumerable)

        /// <summary>
        /// Set the given enumerable as second element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public virtual void SetSource3(IEnumerable<S3> SourceEnumerable)
        {
            SourceEnumerable.CheckNull("SourceEnumerable");
            SourcePipe3 = new IEnumerator2IEndPipe<S3>(SourceEnumerable.GetEnumerator());
        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator&lt;E&gt; that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<E> GetEnumerator()
        {
            return new IEndPipe2IEnumerator<E>(this);
        }

        #endregion

        #region Current

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public E Current
        {
            get
            {
                return _CurrentElement;
            }
        }

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
        public abstract Boolean MoveNext();

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerators to their initial positions, which
        /// is before the first element in the collections.
        /// </summary>
        public virtual IEndPipe<E> Reset()
        {

            SourcePipe1.Reset();
            SourcePipe2.Reset();
            SourcePipe3.Reset();

            return this;

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        {
            SourcePipe1.Dispose();
            SourcePipe2.Dispose();
            SourcePipe3.Dispose();
        }

        #endregion


        #region Path

        /// <summary>
        /// Returns the transformation path to arrive at the current object
        /// of the pipe. This is a list of all of the objects traversed for
        /// the current iterator position of the pipe.
        /// </summary>
        public virtual IEnumerable<Object> Path
        {

            get
            {

                var _PathElements = PathToHere.ToList();
                var _Size         = _PathElements.Count;

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)            
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(_CurrentElement))
                    _PathElements.Add(_CurrentElement);

                return _PathElements;

            }

        }

        #endregion

        #region PathToHere

        private IEnumerable<Object> PathToHere
        {

            get
            {

                throw new NotImplementedException();

                //if (_InternalEnumerator is IPipe)
                //    return ((IPipe) _InternalEnumerator).Path;

                //else if (_InternalEnumerator is IHistoryEnumerator)
                //{

                //    var _List = new List<Object>();
                //    var _Last = ((IHistoryEnumerator) _InternalEnumerator).Last;

                //    if (_Last == null)
                //        _List.Add(_InternalEnumerator.Current);
                //    else
                //        _List.Add(_Last);

                //    return _List;

                //}

                //else if (_InternalEnumerator is ISingleEnumerator)
                //    return new List<Object>() { ((ISingleEnumerator) _InternalEnumerator).Current };

                //else
                //    return new List<Object>();

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion

    }

    #endregion

    #region AbstractPipe<S1, S2, S3, S4, E>

    /// <summary>
    /// An AbstractPipe provides most of the functionality that is repeated
    /// in every instance of a Pipe. Any subclass of AbstractPipe should simply
    /// implement MoveNext().
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the third consuming objects.</typeparam>
    /// <typeparam name="S4">The type of the fourth consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public abstract class AbstractPipe<S1, S2, S3, S4, E> : IPipe<S1, S2, S3, S4, E>
    {

        #region Data

        /// <summary>
        /// The internal enumerator of the first collection.
        /// </summary>
        protected IEnumerator<S1> _InternalEnumerator1;


        /// <summary>
        /// The internal enumerator of the second collection.
        /// </summary>
        protected IEnumerator<S2> _InternalEnumerator2;


        /// <summary>
        /// The internal enumerator of the third collection.
        /// </summary>
        protected IEnumerator<S3> _InternalEnumerator3;


        /// <summary>
        /// The internal enumerator of the third collection.
        /// </summary>
        protected IEnumerator<S4> _InternalEnumerator4;


        /// <summary>
        /// The internal current element in the collection.
        /// </summary>
        protected E _CurrentElement;

        #endregion

        #region Constructor(s)

        #region AbstractPipe()
        
        /// <summary>
        /// Creates a new abstract pipe.
        /// </summary>
        public AbstractPipe()
        { }
        
        #endregion

        #region AbstractPipe(IEnumerator1, IEnumerator2, IEnumerator3, IEnumerator4)

        /// <summary>
        /// Creates a new abstract pipe using the elements emitted
        /// by the given IEnumerators as input.
        /// </summary>
        /// <param name="IEnumerator1">An IEnumerator&lt;S1&gt; as element source.</param>
        /// <param name="IEnumerator2">An IEnumerator&lt;S2&gt; as element source.</param>
        /// <param name="IEnumerator3">An IEnumerator&lt;S3&gt; as element source.</param>
        /// <param name="IEnumerator4">An IEnumerator&lt;S4&gt; as element source.</param>
        public AbstractPipe(IEnumerator<S1> IEnumerator1, IEnumerator<S2> IEnumerator2, IEnumerator<S3> IEnumerator3, IEnumerator<S4> IEnumerator4)
        {
            SetSource1(IEnumerator1);
            SetSource2(IEnumerator2);
            SetSource3(IEnumerator3);
            SetSource4(IEnumerator4);
        }

        #endregion

        #region AbstractPipe(IEnumerable1, IEnumerable2, IEnumerable3, IEnumerable4)

        /// <summary>
        /// Creates a new abstract pipe using the elements emitted
        /// by the given IEnumerables as input.
        /// </summary>
        /// <param name="IEnumerable1">An IEnumerable&lt;S1&gt; as element source.</param>
        /// <param name="IEnumerable2">An IEnumerable&lt;S2&gt; as element source.</param>
        /// <param name="IEnumerable3">An IEnumerable&lt;S3&gt; as element source.</param>
        /// <param name="IEnumerable4">An IEnumerable&lt;S4&gt; as element source.</param>
        public AbstractPipe(IEnumerable<S1> IEnumerable1, IEnumerable<S2> IEnumerable2, IEnumerable<S3> IEnumerable3, IEnumerable<S4> IEnumerable4)
        {
            SetSourceCollection1(IEnumerable1);
            SetSourceCollection2(IEnumerable2);
            SetSourceCollection3(IEnumerable3);
            SetSourceCollection4(IEnumerable4);
        }

        #endregion

        #endregion


        #region SetSource(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource(Object SourceElement)
        {
            SetSource1((S1) SourceElement);
        }

        #endregion

        #region SetSource1(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource1(S1 SourceElement)
        {
            _InternalEnumerator1 = new HistoryEnumerator<S1>(new List<S1>() { SourceElement }.GetEnumerator());
        }

        #endregion

        #region SetSource2(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource2(S2 SourceElement)
        {
            _InternalEnumerator2 = new HistoryEnumerator<S2>(new List<S2>() { SourceElement }.GetEnumerator());
        }

        #endregion

        #region SetSource3(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource3(S3 SourceElement)
        {
            _InternalEnumerator3 = new HistoryEnumerator<S3>(new List<S3>() { SourceElement }.GetEnumerator());
        }

        #endregion

        #region SetSource4(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource4(S4 SourceElement)
        {
            _InternalEnumerator4 = new HistoryEnumerator<S4>(new List<S4>() { SourceElement }.GetEnumerator());
        }

        #endregion


        #region SetSource(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator as element source.</param>
        public virtual void SetSource(IEnumerator IEnumerator)
        {
            SetSource1((IEnumerator<S1>) IEnumerator);
        }

        #endregion

        #region SetSource1(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S1&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S1&gt; as element source.</param>
        public virtual void SetSource1(IEnumerator<S1> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S1>)
                _InternalEnumerator1 = IEnumerator;
            else
                _InternalEnumerator1 = new HistoryEnumerator<S1>(IEnumerator);

        }

        #endregion

        #region SetSource2(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S2&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S2&gt; as element source.</param>
        public virtual void SetSource2(IEnumerator<S2> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S2>)
                _InternalEnumerator2 = IEnumerator;
            else
                _InternalEnumerator2 = new HistoryEnumerator<S2>(IEnumerator);

        }

        #endregion

        #region SetSource3(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S3&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S3&gt; as element source.</param>
        public virtual void SetSource3(IEnumerator<S3> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S3>)
                _InternalEnumerator3 = IEnumerator;
            else
                _InternalEnumerator3 = new HistoryEnumerator<S3>(IEnumerator);

        }

        #endregion

        #region SetSource4(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S4&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S4&gt; as element source.</param>
        public virtual void SetSource4(IEnumerator<S4> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S4>)
                _InternalEnumerator4 = IEnumerator;
            else
                _InternalEnumerator4 = new HistoryEnumerator<S4>(IEnumerator);

        }

        #endregion


        #region SetSourceCollection(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable as element source.</param>
        public virtual void SetSourceCollection(IEnumerable IEnumerable)
        {
            SetSourceCollection1((IEnumerable<S1>) IEnumerable);
        }

        #endregion

        #region SetSourceCollection1(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S1&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S1&gt; as element source.</param>
        public virtual void SetSourceCollection1(IEnumerable<S1> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource1(IEnumerable.GetEnumerator());

        }

        #endregion

        #region SetSourceCollection2(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S2&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S2&gt; as element source.</param>
        public virtual void SetSourceCollection2(IEnumerable<S2> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource2(IEnumerable.GetEnumerator());

        }

        #endregion

        #region SetSourceCollection3(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S3&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S3&gt; as element source.</param>
        public virtual void SetSourceCollection3(IEnumerable<S3> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource3(IEnumerable.GetEnumerator());

        }

        #endregion

        #region SetSourceCollection4(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S4&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S4&gt; as element source.</param>
        public virtual void SetSourceCollection4(IEnumerable<S4> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource4(IEnumerable.GetEnumerator());

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator&lt;E&gt; that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<E> GetEnumerator()
        {
            return new IEndPipe2IEnumerator<E>(this);
        }

        #endregion

        #region Current

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public E Current
        {
            get
            {
                return _CurrentElement;
            }
        }

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
        public abstract Boolean MoveNext();

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerators to their initial positions, which
        /// is before the first element in the collections.
        /// </summary>
        public virtual IEndPipe<E> Reset()
        {

            _InternalEnumerator1.Reset();
            _InternalEnumerator2.Reset();
            _InternalEnumerator3.Reset();
            _InternalEnumerator4.Reset();

            return this;

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        {
            _InternalEnumerator1.Dispose();
            _InternalEnumerator2.Dispose();
            _InternalEnumerator3.Dispose();
            _InternalEnumerator4.Dispose();
        }

        #endregion


        #region Path

        /// <summary>
        /// Returns the transformation path to arrive at the current object
        /// of the pipe. This is a list of all of the objects traversed for
        /// the current iterator position of the pipe.
        /// </summary>
        public virtual IEnumerable<Object> Path
        {

            get
            {

                var _PathElements = PathToHere.ToList();
                var _Size         = _PathElements.Count;

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)            
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(_CurrentElement))
                    _PathElements.Add(_CurrentElement);

                return _PathElements;

            }

        }

        #endregion

        #region PathToHere

        private IEnumerable<Object> PathToHere
        {

            get
            {

                throw new NotImplementedException();

                //if (_InternalEnumerator is IPipe)
                //    return ((IPipe) _InternalEnumerator).Path;

                //else if (_InternalEnumerator is IHistoryEnumerator)
                //{

                //    var _List = new List<Object>();
                //    var _Last = ((IHistoryEnumerator) _InternalEnumerator).Last;

                //    if (_Last == null)
                //        _List.Add(_InternalEnumerator.Current);
                //    else
                //        _List.Add(_Last);

                //    return _List;

                //}

                //else if (_InternalEnumerator is ISingleEnumerator)
                //    return new List<Object>() { ((ISingleEnumerator) _InternalEnumerator).Current };

                //else
                //    return new List<Object>();

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion


    }

    #endregion

    #region AbstractPipe<S1, S2, S3, S4, S5, E>

    /// <summary>
    /// An AbstractPipe provides most of the functionality that is repeated
    /// in every instance of a Pipe. Any subclass of AbstractPipe should simply
    /// implement MoveNext().
    /// </summary>
    /// <typeparam name="S1">The type of the first consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the second consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the third consuming objects.</typeparam>
    /// <typeparam name="S4">The type of the fourth consuming objects.</typeparam>
    /// <typeparam name="S5">The type of the fifth consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public abstract class AbstractPipe<S1, S2, S3, S4, S5, E> : IPipe<S1, S2, S3, S4, S5, E>
    {

        #region Data

        /// <summary>
        /// The internal enumerator of the first collection.
        /// </summary>
        protected IEnumerator<S1> _InternalEnumerator1;

        /// <summary>
        /// The internal enumerator of the second collection.
        /// </summary>
        protected IEnumerator<S2> _InternalEnumerator2;

        /// <summary>
        /// The internal enumerator of the third collection.
        /// </summary>
        protected IEnumerator<S3> _InternalEnumerator3;

        /// <summary>
        /// The internal enumerator of the third collection.
        /// </summary>
        protected IEnumerator<S4> _InternalEnumerator4;

        /// <summary>
        /// The internal enumerator of the third collection.
        /// </summary>
        protected IEnumerator<S5> _InternalEnumerator5;


        /// <summary>
        /// The internal current element in the collection.
        /// </summary>
        protected E _CurrentElement;

        #endregion

        #region Constructor(s)

        #region AbstractPipe()

        /// <summary>
        /// Creates a new abstract pipe.
        /// </summary>
        public AbstractPipe()
        { }

        #endregion

        #region AbstractPipe(IEnumerator1, IEnumerator2, IEnumerator3, IEnumerator4, IEnumerator5)

        /// <summary>
        /// Creates a new abstract pipe using the elements emitted
        /// by the given IEnumerators as input.
        /// </summary>
        /// <param name="IEnumerator1">An IEnumerator&lt;S1&gt; as element source.</param>
        /// <param name="IEnumerator2">An IEnumerator&lt;S2&gt; as element source.</param>
        /// <param name="IEnumerator3">An IEnumerator&lt;S3&gt; as element source.</param>
        /// <param name="IEnumerator4">An IEnumerator&lt;S4&gt; as element source.</param>
        /// <param name="IEnumerator5">An IEnumerator&lt;S5&gt; as element source.</param>
        public AbstractPipe(IEnumerator<S1> IEnumerator1,
                            IEnumerator<S2> IEnumerator2,
                            IEnumerator<S3> IEnumerator3,
                            IEnumerator<S4> IEnumerator4,
                            IEnumerator<S5> IEnumerator5)
        {
            SetSource1(IEnumerator1);
            SetSource2(IEnumerator2);
            SetSource3(IEnumerator3);
            SetSource4(IEnumerator4);
            SetSource5(IEnumerator5);
        }

        #endregion

        #region AbstractPipe(IEnumerable1, IEnumerable2, IEnumerable3, IEnumerable4, IEnumerable5)

        /// <summary>
        /// Creates a new abstract pipe using the elements emitted
        /// by the given IEnumerables as input.
        /// </summary>
        /// <param name="IEnumerable1">An IEnumerable&lt;S1&gt; as element source.</param>
        /// <param name="IEnumerable2">An IEnumerable&lt;S2&gt; as element source.</param>
        /// <param name="IEnumerable3">An IEnumerable&lt;S3&gt; as element source.</param>
        /// <param name="IEnumerable4">An IEnumerable&lt;S4&gt; as element source.</param>
        /// <param name="IEnumerable5">An IEnumerable&lt;S5&gt; as element source.</param>
        public AbstractPipe(IEnumerable<S1> IEnumerable1,
                            IEnumerable<S2> IEnumerable2,
                            IEnumerable<S3> IEnumerable3,
                            IEnumerable<S4> IEnumerable4,
                            IEnumerable<S5> IEnumerable5)
        {
            SetSourceCollection1(IEnumerable1);
            SetSourceCollection2(IEnumerable2);
            SetSourceCollection3(IEnumerable3);
            SetSourceCollection4(IEnumerable4);
            SetSourceCollection5(IEnumerable5);
        }

        #endregion

        #endregion


        #region SetSource(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource(Object SourceElement)
        {
            SetSource1((S1)SourceElement);
        }

        #endregion

        #region SetSource1(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource1(S1 SourceElement)
        {
            _InternalEnumerator1 = new HistoryEnumerator<S1>(new List<S1>() { SourceElement }.GetEnumerator());
        }

        #endregion

        #region SetSource2(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource2(S2 SourceElement)
        {
            _InternalEnumerator2 = new HistoryEnumerator<S2>(new List<S2>() { SourceElement }.GetEnumerator());
        }

        #endregion

        #region SetSource3(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource3(S3 SourceElement)
        {
            _InternalEnumerator3 = new HistoryEnumerator<S3>(new List<S3>() { SourceElement }.GetEnumerator());
        }

        #endregion

        #region SetSource4(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource4(S4 SourceElement)
        {
            _InternalEnumerator4 = new HistoryEnumerator<S4>(new List<S4>() { SourceElement }.GetEnumerator());
        }

        #endregion

        #region SetSource5(SourceElement)

        /// <summary>
        /// Set the given element as source.
        /// </summary>
        /// <param name="SourceElement">A single source element.</param>
        public virtual void SetSource5(S5 SourceElement)
        {
            _InternalEnumerator5 = new HistoryEnumerator<S5>(new List<S5>() { SourceElement }.GetEnumerator());
        }

        #endregion


        #region SetSource(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator as element source.</param>
        public virtual void SetSource(IEnumerator IEnumerator)
        {
            SetSource1((IEnumerator<S1>)IEnumerator);
        }

        #endregion

        #region SetSource1(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S1&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S1&gt; as element source.</param>
        public virtual void SetSource1(IEnumerator<S1> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S1>)
                _InternalEnumerator1 = IEnumerator;
            else
                _InternalEnumerator1 = new HistoryEnumerator<S1>(IEnumerator);

        }

        #endregion

        #region SetSource2(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S2&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S2&gt; as element source.</param>
        public virtual void SetSource2(IEnumerator<S2> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S2>)
                _InternalEnumerator2 = IEnumerator;
            else
                _InternalEnumerator2 = new HistoryEnumerator<S2>(IEnumerator);

        }

        #endregion

        #region SetSource3(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S3&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S3&gt; as element source.</param>
        public virtual void SetSource3(IEnumerator<S3> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S3>)
                _InternalEnumerator3 = IEnumerator;
            else
                _InternalEnumerator3 = new HistoryEnumerator<S3>(IEnumerator);

        }

        #endregion

        #region SetSource4(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S4&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S4&gt; as element source.</param>
        public virtual void SetSource4(IEnumerator<S4> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S4>)
                _InternalEnumerator4 = IEnumerator;
            else
                _InternalEnumerator4 = new HistoryEnumerator<S4>(IEnumerator);

        }

        #endregion

        #region SetSource5(IEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S5&gt; as input.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S5&gt; as element source.</param>
        public virtual void SetSource5(IEnumerator<S5> IEnumerator)
        {

            if (IEnumerator == null)
                throw new ArgumentNullException("IEnumerator must not be null!");

            if (IEnumerator is IEndPipe<S5>)
                _InternalEnumerator5 = IEnumerator;
            else
                _InternalEnumerator5 = new HistoryEnumerator<S5>(IEnumerator);

        }

        #endregion


        #region SetSourceCollection(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable as element source.</param>
        public virtual void SetSourceCollection(IEnumerable IEnumerable)
        {
            SetSourceCollection1((IEnumerable<S1>)IEnumerable);
        }

        #endregion

        #region SetSourceCollection1(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S1&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S1&gt; as element source.</param>
        public virtual void SetSourceCollection1(IEnumerable<S1> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource1(IEnumerable.GetEnumerator());

        }

        #endregion

        #region SetSourceCollection2(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S2&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S2&gt; as element source.</param>
        public virtual void SetSourceCollection2(IEnumerable<S2> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource2(IEnumerable.GetEnumerator());

        }

        #endregion

        #region SetSourceCollection3(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S3&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S3&gt; as element source.</param>
        public virtual void SetSourceCollection3(IEnumerable<S3> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource3(IEnumerable.GetEnumerator());

        }

        #endregion

        #region SetSourceCollection4(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S4&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S4&gt; as element source.</param>
        public virtual void SetSourceCollection4(IEnumerable<S4> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource4(IEnumerable.GetEnumerator());

        }

        #endregion

        #region SetSourceCollection5(IEnumerable)

        /// <summary>
        /// Set the elements emitted from the given IEnumerable&lt;S5&gt; as input.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S5&gt; as element source.</param>
        public virtual void SetSourceCollection5(IEnumerable<S5> IEnumerable)
        {

            if (IEnumerable == null)
                throw new ArgumentNullException("IEnumerable must not be null!");

            SetSource5(IEnumerable.GetEnumerator());

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator&lt;E&gt; that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<E> GetEnumerator()
        {
            return new IEndPipe2IEnumerator<E>(this);
        }

        #endregion

        #region Current

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public E Current
        {
            get
            {
                return _CurrentElement;
            }
        }

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
        public abstract Boolean MoveNext();

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerators to their initial positions, which
        /// is before the first element in the collections.
        /// </summary>
        public virtual IEndPipe<E> Reset()
        {

            _InternalEnumerator1.Reset();
            _InternalEnumerator2.Reset();
            _InternalEnumerator3.Reset();
            _InternalEnumerator4.Reset();
            _InternalEnumerator5.Reset();

            return this;

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        {
            _InternalEnumerator1.Dispose();
            _InternalEnumerator2.Dispose();
            _InternalEnumerator3.Dispose();
            _InternalEnumerator4.Dispose();
            _InternalEnumerator5.Dispose();
        }

        #endregion


        #region Path

        /// <summary>
        /// Returns the transformation path to arrive at the current object
        /// of the pipe. This is a list of all of the objects traversed for
        /// the current iterator position of the pipe.
        /// </summary>
        public virtual IEnumerable<Object> Path
        {

            get
            {

                var _PathElements = PathToHere.ToList();
                var _Size         = _PathElements.Count;

                // do not repeat filters as they dup the object
                // todo: why is size == 0 required (Pangloss?)            
                if (_Size == 0 || !_PathElements[_Size - 1].Equals(_CurrentElement))
                    _PathElements.Add(_CurrentElement);

                return _PathElements;

            }

        }

        #endregion

        #region PathToHere

        private IEnumerable<Object> PathToHere
        {

            get
            {

                throw new NotImplementedException();

                //if (_InternalEnumerator is IPipe)
                //    return ((IPipe) _InternalEnumerator).Path;

                //else if (_InternalEnumerator is IHistoryEnumerator)
                //{

                //    var _List = new List<Object>();
                //    var _Last = ((IHistoryEnumerator) _InternalEnumerator).Last;

                //    if (_Last == null)
                //        _List.Add(_InternalEnumerator.Current);
                //    else
                //        _List.Add(_Last);

                //    return _List;

                //}

                //else if (_InternalEnumerator is ISingleEnumerator)
                //    return new List<Object>() { ((ISingleEnumerator) _InternalEnumerator).Current };

                //else
                //    return new List<Object>();

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion

    }

    #endregion

}
