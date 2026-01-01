/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Globalization;
using System.Text.RegularExpressions;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Money: A decimal value with an ISO 4217 currency.
    /// </summary>
    public readonly struct Money : IEquatable<Money>,
                                   IComparable<Money>,
                                   IComparable
    {

        #region Data

        private const          String  MoneyRegExprString  = "([^0-9]*)([\\s]*)([0-9]+[\\.\\,]?[0-9]*)([\\s]*)([^0-9]*)";

        public static readonly Regex   MoneyRegExpr        = new Regex(MoneyRegExprString);

        #endregion

        #region Properties

        /// <summary>
        /// The value of the money.
        /// </summary>
        public Decimal   Value       { get; }

        /// <summary>
        /// The currency of the money.
        /// </summary>
        public Currency  Currency    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new money.
        /// </summary>
        /// <param name="Value">The value of the money.</param>
        /// <param name="Currency">The currency of the money.</param>
        private Money(Decimal   Value,
                      Currency  Currency)
        {
            this.Value     = Value;
            this.Currency  = Currency;
        }

        #endregion


        #region (static) Create(Value, Currency)

        /// <summary>
        /// Create new money.
        /// </summary>
        /// <param name="Value">The value of the money.</param>
        /// <param name="Currency">The currency of the money.</param>
        public static Money Create(Decimal   Value,
                                   Currency  Currency)

            => new Money(Value,
                         Currency);

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Return the appropriate money for the given text representation of money.
        /// </summary>
        /// <param name="Text">The value of the money and the ISO code or name of a currency.</param>
        public static Money Parse(String Text)
        {

            if (!TryParse(Text, out Money money))
                throw new ArgumentException("The given text '" + Text + "' is not a valid text representation of money!", nameof(Text));

            return money;

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Return the appropriate money for the given string.
        /// </summary>
        /// <param name="Text">The text representation of the money value and ISO code or name of a currency.</param>
        public static Money? TryParse(String Text)
        {

            if (TryParse(Text, out Money mmmoney))
                return mmmoney;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Money)

        /// <summary>
        /// Return the appropriate money for the given string.
        /// </summary>
        /// <param name="Text">The text representation of the money value and ISO code or name of a currency.</param>
        /// <param name="Money">The parsed Money</param>
        public static Boolean TryParse(String Text, out Money Money)
        {

            Money = default;

            var Match = MoneyRegExpr.Match(Text);

            if (!Match.Success)
                return false;

            if (!Decimal.TryParse(Match.Groups[3].Value.Replace(',', '.'),
                                   NumberStyles.Number,
                                   CultureInfo.InvariantCulture,
                                   out Decimal value))
            {
                return false;
            }

            if (!Currency.TryParse(Match.Groups[1].Value.IsNotNullOrEmpty()
                                       ? Match.Groups[1].Value
                                       : Match.Groups[5].Value,
                                   out var currency))
            {
                return false;
            }

            Money = new Money(
                        value,
                        currency
                    );

            return true;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Money1, Money2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Money1">A money object.</param>
        /// <param name="Money2">Another money object.</param>
        public static Boolean operator == (Money Money1, Money Money2)
            => Money1.Equals(Money2);

        #endregion

        #region Operator != (Money1, Money2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Money1">A money object.</param>
        /// <param name="Money2">Another money object.</param>
        public static Boolean operator != (Money Money1, Money Money2)
            => !Money1.Equals(Money2);

        #endregion

        #region Operator <  (Money1, Money2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Money1">A money object.</param>
        /// <param name="Money2">Another money object.</param>
        public static Boolean operator <  (Money Money1, Money Money2)
            => Money1.CompareTo(Money2) < 0;

        #endregion

        #region Operator <= (Money1, Money2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Money1">A money object.</param>
        /// <param name="Money2">Another money object.</param>
        public static Boolean operator <= (Money Money1, Money Money2)
            => Money1.CompareTo(Money2) <= 0;

        #endregion

        #region Operator >  (Money1, Money2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Money1">A money object.</param>
        /// <param name="Money2">Another money object.</param>
        public static Boolean operator >  (Money Money1, Money Money2)
            => Money1.CompareTo(Money2) > 0;

        #endregion

        #region Operator >= (Money1, Money2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Money1">A money object.</param>
        /// <param name="Money2">Another money object.</param>
        public static Boolean operator >= (Money Money1, Money Money2)
            => Money1.CompareTo(Money2) >= 0;

        #endregion

        #endregion

        #region IComparable<Money> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is Money))
                throw new ArgumentException("The given object is not a money object!",
                                            nameof(Object));

            return CompareTo((Money) Object);

        }

        #endregion

        #region CompareTo(Money)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Money">An object to compare with.</param>
        public Int32 CompareTo(Money Money)
        {

            var c = Decimal.Compare(Value, Money.Value);

            if (c != 0)
                return c;

            return Currency.CompareTo(Money.Currency);

        }

        #endregion

        #endregion

        #region IEquatable<Money> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is Money))
                return false;

            return Equals((Money) Object);

        }

        #endregion

        #region Equals(Money)

        /// <summary>
        /// Compares two money objects for equality.
        /// </summary>
        /// <param name="Money">A money object to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Money Money)

            => Value.   Equals(Money.Value) &&
               Currency.Equals(Money.Currency);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value.   GetHashCode() ^
               Currency.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Currency.SymbolLocation switch {
                Currency.Symbol_Location.before       => Currency.Symbol +       Value,
                Currency.Symbol_Location.beforeSpace  => Currency.Symbol + " " + Value,
                Currency.Symbol_Location.behind       => Value           +       Currency.Symbol,
                Currency.Symbol_Location.behindSpace  => Value           + " " + Currency.Symbol,
                _                                     => Value.ToString(),
            };

        #endregion

    }

}
