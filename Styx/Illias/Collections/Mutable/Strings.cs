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

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public class MultiString : IEquatable<MultiString>, IComparable<MultiString>, IComparable
    {

        private readonly HashedSet<String> _MultiString;


        public MultiString(IEnumerable<String> MultiString)
        {
            this._MultiString = new HashedSet<String>(MultiString);
        }


        //#region Operator overloading

        //#region Operator == (MultiString1, MultiString2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="MultiString1">A MultiString.</param>
        ///// <param name="MultiString2">Another MultiString.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator == (MultiString MultiString1, MultiString MultiString2)
        //{

        //    // If both are null, or both are same instance, return true.
        //    if (Object.ReferenceEquals(MultiString1, MultiString2))
        //        return true;

        //    // If one is null, but not both, return false.
        //    if (((Object) MultiString1 == null) || ((Object) MultiString2 == null))
        //        return false;

        //    return MultiString1.Equals(MultiString2);

        //}

        //#endregion

        //#region Operator != (MultiString1, MultiString2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="MultiString1">A MultiString.</param>
        ///// <param name="MultiString2">Another MultiString.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator != (MultiString MultiString1, MultiString MultiString2)
        //{
        //    return !(MultiString1 == MultiString2);
        //}

        //#endregion

        //#region Operator <  (MultiString1, MultiString2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="MultiString1">A MultiString.</param>
        ///// <param name="MultiString2">Another MultiString.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator < (MultiString MultiString1, MultiString MultiString2)
        //{

        //    if ((Object) MultiString1 == null)
        //        throw new ArgumentNullException("The given MultiString1 must not be null!");

        //    return MultiString1.CompareTo(MultiString2) < 0;

        //}

        //#endregion

        //#region Operator <= (MultiString1, MultiString2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="MultiString1">A MultiString.</param>
        ///// <param name="MultiString2">Another MultiString.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator <= (MultiString MultiString1, MultiString MultiString2)
        //{
        //    return !(MultiString1 > MultiString2);
        //}

        //#endregion

        //#region Operator >  (MultiString1, MultiString2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="MultiString1">A MultiString.</param>
        ///// <param name="MultiString2">Another MultiString.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator > (MultiString MultiString1, MultiString MultiString2)
        //{

        //    if ((Object) MultiString1 == null)
        //        throw new ArgumentNullException("The given MultiString1 must not be null!");

        //    return MultiString1.CompareTo(MultiString2) > 0;

        //}

        //#endregion

        //#region Operator >= (MultiString1, MultiString2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="MultiString1">A MultiString.</param>
        ///// <param name="MultiString2">Another MultiString.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator >= (MultiString MultiString1, MultiString MultiString2)
        //{
        //    return !(MultiString1 < MultiString2);
        //}

        //#endregion

        //#endregion

        #region IComparable<MultiString> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an MultiString.
            var MultiString = Object as MultiString;
            if ((Object) MultiString == null)
                throw new ArgumentException("The given object is not a MultiString!");

            return CompareTo(MultiString);

        }

        #endregion

        #region CompareTo(MultiString)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OtherMultiString">An object to compare with.</param>
        public Int32 CompareTo(MultiString OtherMultiString)
        {

            if ((Object) OtherMultiString == null)
                throw new ArgumentNullException("The given MultiString must not be null!");

            // Compare the length of the MultiStrings
            var _Result = this._MultiString.Count.CompareTo(OtherMultiString._MultiString.Count);

            // If equal: Compare each string
            var a =             this._MultiString.OrderBy(v => v).ToArray();
            var b = OtherMultiString._MultiString.OrderBy(v => v).ToArray();

            for (var i=0; i<a.Length; i++)
                if (a[i].CompareTo(b[i]) != 0)
                    return a[i].CompareTo(b[i]);

            return 0;

        }

        #endregion

        #endregion

        #region IEquatable<MultiString> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an MultiString.
            var MultiString = Object as MultiString;
            if ((Object) MultiString == null)
                return false;

            return this.Equals(MultiString);

        }

        #endregion

        #region Equals(OtherMultiString)

        /// <summary>
        /// Compares two MultiStrings for equality.
        /// </summary>
        /// <param name="MultiString">A MultiString to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(MultiString OtherMultiString)
        {

            if ((Object) OtherMultiString == null)
                return false;

            // Compare the length of the MultiStrings
            if (this._MultiString.Count.CompareTo(OtherMultiString._MultiString.Count) != 0)
                return false;

            // If equal: Compare each string
            var a =             this._MultiString.OrderBy(v => v).ToArray();
            var b = OtherMultiString._MultiString.OrderBy(v => v).ToArray();

            for (var i=0; i<a.Length; i++)
                if (a[i].CompareTo(b[i]) != 0)
                    return false;

            return true;

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {

            return this._MultiString.
                        Select(v => v.GetHashCode()).
                        Aggregate((a, b) => a ^ b);

        }

        #endregion

    }

}
