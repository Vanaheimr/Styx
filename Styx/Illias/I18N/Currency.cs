/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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
using System.Collections.Generic;
using System.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// ISO 4217 currencies.
    /// </summary>
    /// <seealso cref="https://de.wikipedia.org/wiki/ISO_4217" />
    public class Currency : IComparable, IComparable<Currency>, IEquatable<Currency>
    {

        #region Properties

        #region ISOCode

        private readonly String _ISOCode;

        /// <summary>
        /// The ISO code of the currency.
        /// </summary>
        public String ISOCode
        {
            get
            {
                return _ISOCode;
            }
        }

        #endregion

        #region Numeric

        private readonly UInt16 _Numeric;

        /// <summary>
        /// The numeric code of the currency.
        /// </summary>
        public UInt16 Numeric
        {
            get
            {
                return _Numeric;
            }
        }

        #endregion

        #region Name

        private readonly String _Name;

        /// <summary>
        /// The name of the currency.
        /// </summary>
        public String Name
        {
            get
            {
                return _Name;
            }
        }

        #endregion

        #region Countries

        private readonly IEnumerable<Country> _Countries;

        /// <summary>
        /// Countries using this currency.
        /// </summary>
        public IEnumerable<Country> Countries
        {
            get
            {
                return _Countries;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// ISO 4217 currencies.
        /// </summary>
        /// <param name="ISOCode">The ISO code of the currency.</param>
        /// <param name="Numeric">The numeric code of the currency.</param>
        /// <param name="Name">The name of the currency.</param>
        /// <param name="Countries">Countries using this currency.</param>
        public Currency(String            ISOCode,
                        UInt16            Numeric,
                        String            Name,
                        params Country[]  Countries)
        {

            this._ISOCode    = ISOCode;
            this._Numeric    = Numeric;
            this._Name       = Name;
            this._Countries  = Countries;

        }

        #endregion


        #region List of currencies...

        public static readonly Currency CHF = new Currency("CHF", 756, "Schweizer Franken", Country.Switzerland);
        public static readonly Currency EUR = new Currency("EUR", 978, "Euro",              Country.Belgium, Country.Germany,   Country.Estonia,  Country.Finland,  Country.Luxembourg,
                                                                                            Country.Greece,  Country.Ireland,   Country.Italy,    Country.Latvia,   Country.Montenegro,
                                                                                            Country.Malta,   Country.Austria,   Country.Portugal, Country.Slovakia, Country.VaticanState,
                                                                                            Country.Spain,   Country.Cyprus,    Country.Andorra,  Country.Monaco,   Country.Netherlands,
                                                                                            Country.France,  Country.SanMarino, Country.Slovenia, Country.Lithuania);
        public static readonly Currency CZK = new Currency("CZK", 203, "Kronen",            Country.CzechRepublic);
        public static readonly Currency GBP = new Currency("GBP", 826, "Pfund",             Country.UnitedKingdom);
        public static readonly Currency HRK = new Currency("HRK", 191, "Kuna",              Country.Croatia);
        public static readonly Currency PLN = new Currency("PLN", 985, "Zloty",             Country.Poland);
        public static readonly Currency RON = new Currency("RON", 946, "Leu",               Country.Romania);
        public static readonly Currency RSD = new Currency("RSD", 941, "Dinar",             Country.Serbia);
        public static readonly Currency RUB = new Currency("RUB", 643, "Rubel",             Country.RussianFederation);
        public static readonly Currency SEK = new Currency("SEK", 752, "Krone",             Country.Sweden);

        #endregion


        #region (static) ParseString(Text)

        /// <summary>
        /// Return the appropriate currency for the given string.
        /// </summary>
        /// <param name="Text">The ISO code or name of a currency.</param>
        public static Currency ParseString(String Text)
        {

            return (from   _FieldInfo in typeof(Currency).GetFields()
                    let    __Currency = _FieldInfo.GetValue(null) as Currency
                    where  __Currency != null
                    where (__Currency.ISOCode == Text || __Currency.Name == Text)
                    select __Currency).FirstOrDefault();

        }

        #endregion

        #region (static) TryParseEnum(myCode, out Currency)

        /// <summary>
        /// Return the appropriate currency for the given string.
        /// </summary>
        /// <param name="Text">The ISO code or name of a currency.</param>
        /// <param name="Currency">The parsed Currency</param>
        /// <returns>true or false</returns>
        public static Boolean TryParseEnum(String Text, out Currency Currency)
        {

            Currency = (from   _FieldInfo in typeof(Currency).GetFields()
                        let    __Currency = _FieldInfo.GetValue(null) as Currency
                        where  __Currency != null
                        where (__Currency.ISOCode == Text || __Currency.Name == Text)
                        select __Currency).FirstOrDefault();

            return (Currency != null) ? true : false;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Currency1, Currency2)

        public static Boolean operator ==(Currency Currency1, Currency Currency2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Currency1, Currency2))
                return true;

            // If one is null, but not both, return false.
            if (((Object)Currency1 == null) || ((Object)Currency2 == null))
                return false;

            return Currency1.Equals(Currency2);

        }

        #endregion

        #region Operator != (Currency1, Currency2)

        public static Boolean operator !=(Currency Currency1, Currency Currency2)
        {
            return !(Currency1 == Currency2);
        }

        #endregion

        #endregion

        #region IComparable<Currency> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an Currency.
            var Currency = Object as Currency;
            if ((Object) Currency == null)
                throw new ArgumentException("The given object is not a Currency!");

            return CompareTo(Currency);

        }

        #endregion

        #region CompareTo(Currency)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Currency">An object to compare with.</param>
        public Int32 CompareTo(Currency Currency)
        {

            if ((Object) Currency == null)
                throw new ArgumentNullException("The given Currency must not be null!");

            var Result = _ISOCode.CompareTo(Currency._ISOCode);

            if (Result == 0)
                Result = _Numeric.CompareTo(Currency._Numeric);

            if (Result == 0)
                Result = _Name.   CompareTo(Currency._Name);

            return Result;

        }

        #endregion

        #endregion

        #region IEquatable<Currency> Members

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

            // Check if the given object is an Currency.
            var Currency = Object as Currency;
            if ((Object) Currency == null)
                return false;

            return this.Equals(Currency);

        }

        #endregion

        #region Equals(Currency)

        /// <summary>
        /// Compares two Currency for equality.
        /// </summary>
        /// <param name="Currency">An Currency to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Currency Currency)
        {

            if ((Object) Currency == null)
                return false;

            if (_ISOCode != Currency._ISOCode)
                return false;

            if (_Numeric != Currency._Numeric)
                return false;

            if (_Name    != Currency._Name)
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
            unchecked
            {
                return _ISOCode.GetHashCode() * 17 ^ _Numeric.GetHashCode() * 23 ^ _Name.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Concat(_Name, " [", _ISOCode, " / ", _Numeric, "]");
        }

        #endregion

    }

}
