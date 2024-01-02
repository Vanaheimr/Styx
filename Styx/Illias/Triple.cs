/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

    /// <summary>
    /// A triple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    public readonly struct Triple<T1, T2, T3> : IEquatable<Triple<T1, T2, T3>>

        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
        where T3 : IEquatable<T3>

    {

        #region Properties

        /// <summary>
        /// The first value.
        /// </summary>
        public T1 V1 { get; }

        /// <summary>
        /// The second value.
        /// </summary>
        public T2 V2 { get; }

        /// <summary>
        /// The third value.
        /// </summary>
        public T3 V3 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new triple.
        /// </summary>
        /// <param name="V1">The first value.</param>
        /// <param name="V2">The second value.</param>
        /// <param name="V3">The third value.</param>
        public Triple(T1 V1, T2 V2, T3 V3)
        {
            this.V1 = V1;
            this.V2 = V2;
            this.V3 = V3;
        }

        #endregion


        #region Deconstruct(out V1, out V2, out V3)

        public void Deconstruct(out T1 V1, out T2 V2, out T3 V3)
        {
            V1 = this.V1;
            V2 = this.V2;
            V3 = this.V3;
        }

        #endregion


        #region Operator overloading

        #region Operator == (Left, Right)

        /// <summary>
        /// Compares two triples for equality.
        /// </summary>
        /// <param name="Left">A triple.</param>
        /// <param name="Right">Another triple.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Triple<T1, T2, T3> Left,
                                           Triple<T1, T2, T3> Right)

            => Left.Equals(Right);

        #endregion

        #region Operator != (Left, Right)

        /// <summary>
        /// Compares two triples for inequality.
        /// </summary>
        /// <param name="Left">A triple.</param>
        /// <param name="Right">Another triple.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Triple<T1, T2, T3> Left,
                                           Triple<T1, T2, T3> Right)

            => Left.Equals(Right);

        #endregion

        #endregion

        #region IEquatable<Triple> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two triples for equality.
        /// </summary>
        /// <param name="Object">A triple to compare with.</param>
        public override Boolean Equals(Object? Triple)

            => Triple is Triple<T1, T2, T3> triple &&
               Equals(triple);

        #endregion

        #region Equals(Triple)

        /// <summary>
        /// Compares two triples for equality.
        /// </summary>
        /// <param name="Triple">A triple to compare with.</param>
        public Boolean Equals(Triple<T1, T2, T3> Triple)

            => V1.Equals(Triple.V1) &&
               V2.Equals(Triple.V2) &&
               V3.Equals(Triple.V3);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return V3.GetHashCode() * 5 ^
                       V2.GetHashCode() * 3 ^
                       V1.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(V1, ", ", V2, ", ", V3);

        #endregion

    }

}
