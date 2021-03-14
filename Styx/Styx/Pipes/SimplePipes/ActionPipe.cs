/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

    /// <summary>
    /// The ActionPipe is much like the IdentityPipe, but calls
    /// a delegate for every consuming object.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class ActionPipe<S> : AbstractPipe<S, S>
    {

        #region Data

        private Action<S> Delegate;

        #endregion

        #region Constructor(s)

        #region ActionPipe(Delegate)

        /// <summary>
        /// Creates a new ActionPipe using the given Action&lt;S&gt;.
        /// </summary>
        /// <param name="Delegate">An Action&lt;S&gt; to be called on every consuming object.</param>
        public ActionPipe(Action<S> Delegate)
        {
            this.Delegate  = Delegate;
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

            if (SourcePipe == null)
                return false;

            if (SourcePipe.MoveNext())
            {

                _CurrentElement = SourcePipe.Current;

                if (Delegate != null)
                    Delegate(_CurrentElement);

                return true;

            }

            return false;

        }

        #endregion

    }

}
