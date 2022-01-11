/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// An object sending notifications when its value changed.
    /// </summary>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class ArrowObject<TOut> : AbstractArrowSender<TOut>
    {

        #region Properties

        #region Value

        private TOut _Value;

        /// <summary>
        /// The value of the ArrowObject.
        /// </summary>
        public TOut Value
        {

            get
            {
                return _Value;
            }

            set
            {

                if (_Value.Equals(value))
                    return;

                _Value = value;

                base.NotifyRecipients(this, value);

            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new object sending notifications when its value changed.
        /// </summary>
        /// <param name="Value">The value of the object.</param>
        public ArrowObject(TOut Value)
        {
            this._Value = Value;
        }

        #endregion


        #region Implicit conversions

        /// <summary>
        /// Implicit conversion from ArrowObject to TOut.
        /// </summary>
        /// <param name="ArrowObject">An ArrowObject.</param>
        /// <returns>The value of the ArrowObject.</returns>
        public static implicit operator TOut(ArrowObject<TOut> ArrowObject)
        {
            return ArrowObject.Value;
        }

        /// <summary>
        /// Implicit conversion from TOut to ArrowObject.
        /// </summary>
        /// <param name="Value">A value.</param>
        /// <returns>A new ArrowObject.</returns>
        public static implicit operator ArrowObject<TOut>(TOut Value)
        {
            return new ArrowObject<TOut>(Value);
        }

        #endregion

    }

}
