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
using System.Collections;
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
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
	public class Pipeline<S, E> : IPipe<S, E>
	{

		#region Data

        private IPipe[]        _Pipes;
        private IStartPipe<S>  _StartPipe;
        private IEndPipe<E>    _EndPipe;
        private String         _PipelineString;

        private IEnumerator<S> _InternalEnumerator;
        private E              _CurrentElement;
	
		#endregion
		
		#region Constructor(s)
		
		#region Pipeline()

        /// <summary>
        /// Constructs a pipeline from the provided pipes.
        /// </summary>
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
        public Pipeline(IEnumerable<IPipe> myPipes)
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
        public Pipeline(params IPipe[] myPipes)
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
        protected void SetPipes(IEnumerable<IPipe> myPipes)
        {
            SetPipes(myPipes.ToArray());
        }

        #endregion

        #region SetPipes(myPipes)
		
        /// <summary>
        /// Use when extending Pipeline and setting the pipeline chain without making use of the constructor.
        /// </summary>
        /// <param name="myPipes">the ordered array of pipes to chain together into a pipeline.</param>
        protected void SetPipes(params IPipe[] myPipes)
        {

            _Pipes = myPipes;
            var _PipeNames = new List<String>();
            var _Length    = myPipes.Length;

            _StartPipe = myPipes[0] as IStartPipe<S>;

            if (_StartPipe == null)
                throw new ArgumentException("The first Pipe must implement 'IStartPipe<" + typeof(S) + ">', but '" + myPipes[0].GetType() + "' was provided!");

            _EndPipe = myPipes[_Length - 1] as IEndPipe<E>;

            if (_EndPipe == null)
                throw new ArgumentException("The last Pipe must implement 'IEndPipe<" + typeof(E) + ">', but '" + myPipes[_Length - 1].GetType() + "' was provided!");
			
            _PipeNames.Add(_StartPipe.ToString());

            Type[] _GenericArguments = null;
            Type   _Consumes;
            Type   _Emitts;

            Type _GenericIPipeInterface = _StartPipe.GetType().GetInterface("IPipe`2");
            if (_GenericIPipeInterface == null)
                throw new ArgumentException("IPipe<?,?> expected!");

            _Emitts = _GenericIPipeInterface.GetGenericArguments()[1];

            for (var i = 1; i < _Length; i++)
            {

                _GenericArguments = myPipes[i].GetType().GetInterface("IPipe`2").GetGenericArguments();
                _Consumes = _GenericArguments[0];

                if (_Consumes != _Emitts)
                    throw new ArgumentException(myPipes[i - 1].GetType() + " emitts other objects than " + myPipes[i].GetType() + " consumes!");

                _Emitts = _GenericArguments[1];

                myPipes[i].SetSource(myPipes[i - 1]);

                _PipeNames.Add(myPipes[i].ToString());

            }

            if (_InternalEnumerator != null)
                myPipes[0].SetSource(_InternalEnumerator);
			
            _PipelineString = _PipeNames.ToString();
		
        }
		
        #endregion


        #region SetSource(myIEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S&gt; as input.
        /// </summary>
        /// <param name="myIEnumerator">An IEnumerator&lt;S&gt; as element source.</param>
        public virtual void SetSource(IEnumerator<S> myIEnumerator)
        {

            if (myIEnumerator == null)
                throw new ArgumentNullException("myIEnumerator must not be null!");

            _InternalEnumerator = myIEnumerator;

            if (_Pipes != null && _Pipes.Length > 0)
                _Pipes[0].SetSource(_InternalEnumerator);

        }

        /// <summary>
        /// Set the elements emitted by the given IEnumerator as input.
        /// </summary>
        /// <param name="myIEnumerator">An IEnumerator as element source.</param>
        void IStartPipe.SetSource(IEnumerator myIEnumerator)
        {

            if (myIEnumerator == null)
                throw new ArgumentNullException("myIEnumerator must not be null!");

            _InternalEnumerator = myIEnumerator as IEnumerator<S>;

            if (_InternalEnumerator == null)
                throw new ArgumentNullException("myIEnumerator must implement 'IEnumerator<" + typeof(S) + ">'!");

            if (_Pipes != null && _Pipes.Length > 0)
                _Pipes[0].SetSource(_InternalEnumerator);


        }

        #endregion

        #region SetSourceCollection(myIEnumerable)

        /// <summary>
        /// Set the elements emitted by the given IEnumerable&lt;S&gt; as input.
        /// </summary>
        /// <param name="myIEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        public virtual void SetSourceCollection(IEnumerable<S> myIEnumerable)
        {

            if (myIEnumerable == null)
                throw new ArgumentNullException("myIEnumerator must not be null!");

            _InternalEnumerator = myIEnumerable.GetEnumerator();

            if (_Pipes != null && _Pipes.Length > 0)
                _Pipes[0].SetSource(_InternalEnumerator);


        }

        /// <summary>
        /// Set the elements emitted from the given IEnumerable as input.
        /// </summary>
        /// <param name="myIEnumerable">An IEnumerable as element source.</param>
        void IStartPipe.SetSourceCollection(IEnumerable myIEnumerable)
        {

            if (myIEnumerable == null)
                throw new ArgumentNullException("myIEnumerable must not be null!");

            _InternalEnumerator = myIEnumerable.GetEnumerator() as IEnumerator<S>;

            if (_InternalEnumerator == null)
                throw new ArgumentNullException("myIEnumerable must implement 'IEnumerable<" + typeof(S) + ">'!");

            if (_Pipes != null && _Pipes.Length > 0)
                _Pipes[0].SetSource(_InternalEnumerator);

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
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
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

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        Object System.Collections.IEnumerator.Current
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
        public Boolean MoveNext()
        {

            if (_InternalEnumerator == null)
                return false;

            if (_EndPipe == null)
                return false;

            if (_EndPipe.MoveNext())
            {
                _CurrentElement = _EndPipe.Current;
                return true;
            }

            return false;

        }

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            foreach (var _Pipe in _Pipes)
                _Pipe.Reset();
        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipeline.
        /// </summary>
        public void Dispose()
        {
            foreach (var _Pipe in _Pipes)
                _Pipe.Dispose();
        }

        #endregion


        #region Path

        /// <summary>
        /// Returns the transformation path to arrive at the current object
        /// of the pipe. This is a list of all of the objects traversed for
        /// the current iterator position of the pipe.
        /// </summary>
	    public List<Object> Path
		{
            get
            {
                return _EndPipe.Path;
            }
	    }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return _PipelineString;
        }

        #endregion

	}

}