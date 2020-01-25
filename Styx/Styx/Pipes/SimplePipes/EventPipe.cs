/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

    public delegate void PipeEvent<S>(S Value);

    /// <summary>
    /// The EventPipe is much like the IdentityPipe, but calls
    /// an evnet for every consuming object.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class EventPipe<S> : AbstractPipe<S, S>
    {

        #region Data

        public event PipeEvent<S> Event;

        #endregion

        #region Constructor(s)

        #region EventPipe(Delegate)

        /// <summary>
        /// Creates a new ActionPipe using the given Action&lt;S&gt;.
        /// </summary>
        /// <param name="Delegate">An Action&lt;S&gt; to be called on every consuming object.</param>
        public EventPipe()
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

            if (SourcePipe == null)
                return false;

            if (SourcePipe.MoveNext())
            {

                _CurrentElement = SourcePipe.Current;

                var EventLocal = Event;
                if (EventLocal != null)
                    EventLocal(_CurrentElement);

                return true;

            }

            return false;

        }

        #endregion

    }

}
