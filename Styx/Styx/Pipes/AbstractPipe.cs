/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

#region Usings

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
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

            this.SourcePipe = EndPipe.CreatePipe(SourceElement);

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

        #region AbstractPipe(SourceEnumeration)

        /// <summary>
        /// Creates an new abstract pipe using the given enumeration as element source.
        /// </summary>
        /// <param name="SourceEnumeration">An enumeration as element source.</param>
        public AbstractPipe(IEnumerable<S> SourceEnumeration)
        {

            SourceEnumeration.CheckNull("SourceEnumerable");

            this.SourcePipe = EndPipe.CreatePipe(SourceEnumeration);

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

            this.SourcePipe = EndPipe.CreatePipe(SourceEnumerator);

        }

        #endregion

        #region AbstractPipe(SourceEnumeration, SourceEnumerator)

        /// <summary>
        /// Creates an new abstract pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumeration as element source.</param>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public AbstractPipe(IEnumerable<S> SourceEnumeration, IEnumerator<S> SourceEnumerator)
        {

            if (SourceEnumeration == null && SourceEnumerator == null)
                throw new ArgumentNullException("The given sources must not both be null!");

            if (SourceEnumeration != null)
                this.SourcePipe = EndPipe.CreatePipe(SourceEnumeration);

            if (SourceEnumerator != null)
                this.SourcePipe = EndPipe.CreatePipe(SourceEnumerator);

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

            this.SourcePipe = new EndPipe<S>((new List<S>() { SourceElement }).GetEnumerator());

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

            this.SourcePipe = new EndPipe<S>(SourceEnumerator);

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

            this.SourcePipe = new EndPipe<S>(SourceEnumerable.GetEnumerator());

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
            return new EndPipeEnumerator<E>(this);
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
        public IEnumerable<Object> Path
        {

            get
            {

                if (SourcePipe is IEndPipe)
                {

                    var List = new List<Object>(SourcePipe.Path);

                    if (!(this is IFilterPipe))
                        List.Add(_CurrentElement);

                    return List;

                }

                return new List<Object>() { _CurrentElement };

            }

        }

        #endregion

        #region (private) PathToHere

        [Obsolete]
        private IEnumerable<Object> PathToHere
        {

            get
            {

                if (SourcePipe is IEndPipe)
                    return SourcePipe.Path;

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


        #region (override) ToString()

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

            this.SourcePipe1 = new EndPipe<S1>((new List<S1>() { SourceElement1 }).GetEnumerator());
            this.SourcePipe2 = new EndPipe<S2>((new List<S2>() { SourceElement2 }).GetEnumerator());

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

            this.SourcePipe1 = new EndPipe<S1>(SourceEnumerator1);
            this.SourcePipe2 = new EndPipe<S2>(SourceEnumerator2);

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

            this.SourcePipe1 = new EndPipe<S1>(SourceEnumerable1.GetEnumerator());
            this.SourcePipe2 = new EndPipe<S2>(SourceEnumerable2.GetEnumerator());

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
            SourcePipe1 = new EndPipe<S1>((new List<S1>() { SourceElement }).GetEnumerator());
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
            SourcePipe2 = new EndPipe<S2>((new List<S2>() { SourceElement }).GetEnumerator());
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
            SourcePipe1 = new EndPipe<S1>(SourceEnumerator);
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
            SourcePipe2 = new EndPipe<S2>(SourceEnumerator);
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
            SourcePipe1 = new EndPipe<S1>(SourceEnumerable.GetEnumerator());
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
            SourcePipe2 = new EndPipe<S2>(SourceEnumerable.GetEnumerator());
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
            return new EndPipeEnumerator<E>(this);
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


        #region (override) ToString()

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

            this.SourcePipe1 = new EndPipe<S1>((new List<S1>() { SourceElement1 }).GetEnumerator());
            this.SourcePipe2 = new EndPipe<S2>((new List<S2>() { SourceElement2 }).GetEnumerator());
            this.SourcePipe3 = new EndPipe<S3>((new List<S3>() { SourceElement3 }).GetEnumerator());

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

            this.SourcePipe1 = new EndPipe<S1>(SourceEnumerator1);
            this.SourcePipe2 = new EndPipe<S2>(SourceEnumerator2);
            this.SourcePipe3 = new EndPipe<S3>(SourceEnumerator3);

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

            this.SourcePipe1 = new EndPipe<S1>(SourceEnumerable1.GetEnumerator());
            this.SourcePipe2 = new EndPipe<S2>(SourceEnumerable2.GetEnumerator());
            this.SourcePipe3 = new EndPipe<S3>(SourceEnumerable3.GetEnumerator());

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
            SourcePipe1 = new EndPipe<S1>((new List<S1>() { SourceElement }).GetEnumerator());
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
            SourcePipe2 = new EndPipe<S2>((new List<S2>() { SourceElement }).GetEnumerator());
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
            SourcePipe3 = new EndPipe<S3>((new List<S3>() { SourceElement }).GetEnumerator());
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
            SourcePipe1 = new EndPipe<S1>(SourceEnumerator);
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
            SourcePipe2 = new EndPipe<S2>(SourceEnumerator);
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
            SourcePipe3 = new EndPipe<S3>(SourceEnumerator);
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
            SourcePipe1 = new EndPipe<S1>(SourceEnumerable.GetEnumerator());
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
            SourcePipe2 = new EndPipe<S2>(SourceEnumerable.GetEnumerator());
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
            SourcePipe3 = new EndPipe<S3>(SourceEnumerable.GetEnumerator());
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
            return new EndPipeEnumerator<E>(this);
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


        #region (override) ToString()

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
