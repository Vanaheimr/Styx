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
    /// Specifies one exceptional period for opening or access hours.
    /// </summary>
    public readonly struct ExceptionalPeriod : IEquatable<ExceptionalPeriod>
    {

        #region Properties

        /// <summary>
        /// Begin of the opening or access hours exception.
        /// </summary>
        public DateTime Begin   { get; }

        /// <summary>
        /// End of the opening or access hours exception.
        /// </summary>
        public DateTime End     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new exceptional period for opening or access hours.
        /// </summary>
        /// <param name="Begin">Begin of the opening or access hours exception.</param>
        /// <param name="End">End of the opening or access hours exception.</param>
        public ExceptionalPeriod(DateTime  Begin,
                                 DateTime  End)
        {

            this.Begin  = Begin;
            this.End    = End;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two exceptional periods for equality.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => ExceptionalPeriod1.Equals(ExceptionalPeriod2);

        #endregion

        #region Operator != (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two exceptional periods for inequality.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => !ExceptionalPeriod1.Equals(ExceptionalPeriod2);

        #endregion

        #endregion

        #region IEquatable<ExceptionalPeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two exceptional periods for equality.
        /// </summary>
        /// <param name="Object">An exceptional period to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ExceptionalPeriod exceptionalPeriod &&
                   Equals(exceptionalPeriod);

        #endregion

        #region Equals(ExceptionalPeriod)

        /// <summary>
        /// Compares two exceptional periods for equality.
        /// </summary>
        /// <param name="ExceptionalPeriod">An exceptional period to compare with.</param>
        public Boolean Equals(ExceptionalPeriod ExceptionalPeriod)

            => Begin.Equals(ExceptionalPeriod.Begin) &&
               End.  Equals(ExceptionalPeriod.End);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Begin.GetHashCode() * 3 ^
                       End.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Begin.ToIso8601()} -> {End.ToIso8601()}";

        #endregion

    }

}
