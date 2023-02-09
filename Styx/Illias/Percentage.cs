/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    public readonly struct Percentage<TValue>

        where TValue : IEquatable<TValue>

    {

        #region Properties

        /// <summary>
        /// The value.
        /// </summary>
        public TValue  Value      { get; }

        /// <summary>
        /// The percentage.
        /// </summary>
        public Single  Percent    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new percentage.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Percent">The percentage.</param>
        public Percentage(TValue Value, Single Percent)
        {
            this.Value    = Value;
            this.Percent  = Percent;
        }

        #endregion


        #region Deconstruct(out Value, out Percent)

        public void Deconstruct(out TValue Value, out Single Percent)
        {
            Value    = this.Value;
            Percent  = this.Percent;
        }

        #endregion


        #region Operator overloading

        #region Operator == (Left, Right)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="Left">A percentage.</param>
        /// <param name="Right">Another percentage.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Percentage<TValue> Left,
                                           Percentage<TValue> Right)

            => Left.Equals(Right);

        #endregion

        #region Operator != (Left, Right)

        /// <summary>
        /// Compares two percentages for inequality.
        /// </summary>
        /// <param name="Left">A percentage.</param>
        /// <param name="Right">Another percentage.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Percentage<TValue> Left,
                                           Percentage<TValue> Right)

            => Left.Equals(Right);

        #endregion

        #endregion

        #region IEquatable<Triple> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="Object">A percentage to compare with.</param>
        public override Boolean Equals(Object? Triple)

            => Triple is Percentage<TValue> percentage &&
               Equals(percentage);

        #endregion

        #region Equals(Triple)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="Triple">A percentage to compare with.</param>
        public Boolean Equals(Percentage<TValue> Triple)

            => Value.  Equals(Triple.Value) &&
               Percent.Equals(Triple.Percent);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Value.  GetHashCode() * 3 ^
                       Percent.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Value, ": ", Percent.ToString("0.00"), "%");

        #endregion

    }

}
