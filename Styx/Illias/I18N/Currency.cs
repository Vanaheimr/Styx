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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// ISO 4217 currencies.
    /// </summary>
    /// <seealso cref="https://de.wikipedia.org/wiki/ISO_4217" />
    public class Currency : IComparable,
                            IComparable<Currency>,
                            IEquatable<Currency>
    {

        #region (enum) Symbol_Location

        /// <summary>
        /// The location of the currency symbol in relation to its numeric value.
        /// </summary>
        public enum Symbol_Location
        {

            /// <summary>
            /// No symbol defined.
            /// </summary>
            NoSymbol,

            /// <summary>
            /// e.g. "15€"
            /// </summary>
            behind,

            /// <summary>
            /// e.g. "15 €"
            /// </summary>
            behindSpace,

            /// <summary>
            /// e.g. "$15"
            /// </summary>
            before,

            /// <summary>
            /// e.g. "$ 15"
            /// </summary>
            beforeSpace

        }

        #endregion


        #region Properties

        /// <summary>
        /// The ISO code of the currency.
        /// </summary>
        public String                ISOCode           { get; }

        /// <summary>
        /// The numeric code of the currency.
        /// </summary>
        public UInt16                Numeric           { get; }

        /// <summary>
        /// The symbol of the currency, e.g. '€' or '$'.
        /// </summary>
        public String?               Symbol            { get; }

        /// <summary>
        /// The location of the currency symbol in relation to its numeric value.
        /// </summary>
        public Symbol_Location       SymbolLocation    { get; }

        /// <summary>
        /// The name of the currency.
        /// </summary>
        public String                Name              { get; }

        /// <summary>
        /// Countries using this currency.
        /// </summary>
        public IEnumerable<Country>  Countries         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// ISO 4217 currencies.
        /// </summary>
        /// <param name="ISOCode">The ISO code of the currency.</param>
        /// <param name="Numeric">The numeric code of the currency.</param>
        /// <param name="Symbol">The symbol of the currency, e.g. '€' or '$'.</param>
        /// <param name="SymbolLocation">The location of the currency symbol in relation to its numeric value.</param>
        /// <param name="Name">The name of the currency.</param>
        /// <param name="Countries">Countries using this currency.</param>
        public Currency(String            Name,
                        String            ISOCode,
                        UInt16            Numeric,
                        String?           Symbol,
                        Symbol_Location   SymbolLocation,
                        params Country[]  Countries)
        {

            if (Name.   IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name),     "The given name of the currency must not be null or empty!");

            if (ISOCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ISOCode),  "The given ISO code of the currency must not be null or empty!");

            this.Name            = Name.   Trim();
            this.ISOCode         = ISOCode.Trim();
            this.Numeric         = Numeric;
            this.Symbol          = Symbol?.Trim();
            this.SymbolLocation  = this.Symbol is not null ? SymbolLocation : Symbol_Location.NoSymbol;
            this.Countries       = Countries;

        }

        #endregion


        #region (static) Parse       (Text)

        /// <summary>
        /// Return the appropriate currency for the given string.
        /// </summary>
        /// <param name="Text">The ISO code or name of a currency.</param>
        public static Currency Parse(String Text)
        {

            if (TryParse(Text,
                         out var currency))
            {
                return currency!;
            }

            throw new ArgumentException("The given JSON representation of a currency is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse    (Text, out Currency)

        /// <summary>
        /// Return the appropriate currency for the given string.
        /// </summary>
        /// <param name="Text">The ISO code or name of a currency.</param>
        /// <param name="Currency">The parsed Currency</param>
        public static Boolean TryParse(String                             Text,
                                       [NotNullWhen(true)] out Currency?  Currency)
        {

            Currency = default;

            if (Text.IsNullOrEmpty())
                return false;

            Text = Text.Trim();

            Currency = (from   fieldInfo in typeof(Currency).GetFields()
                        let    currency = fieldInfo.GetValue(null) as Currency
                        where  currency is not null
                        where  currency.ISOCode == Text || currency.Symbol == Text || currency.Name == Text
                        select currency).FirstOrDefault();

            return Currency is not null;

        }

        #endregion

        #region (static) TryParseISO (Text, out Currency)

        /// <summary>
        /// Return the appropriate currency for the given string.
        /// </summary>
        /// <param name="Text">The ISO code or name of a currency.</param>
        /// <param name="Currency">The parsed Currency</param>
        public static Boolean TryParseISO(String Text, [NotNullWhen(true)] out Currency? Currency)
        {

            Currency = default;

            if (Text.IsNullOrEmpty())
                return false;

            Text = Text.Trim();

            Currency = (from   fieldInfo in typeof(Currency).GetFields()
                        let    currency = fieldInfo.GetValue(null) as Currency
                        where  currency is not null
                        where  currency.ISOCode == Text
                        select currency).FirstOrDefault();

            return Currency is not null;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this currency.
        /// </summary>
        public Currency Clone()

            => new (
                   Name.     CloneString(),
                   ISOCode.  CloneString(),
                   Numeric,
                   Symbol?.  CloneString(),
                   SymbolLocation,
                   [.. Countries]
               );

        #endregion


        #region Static defaults

        /// <summary>
        /// Euro, €
        /// </summary>
        public static readonly Currency EUR = new ("Euro",              "EUR", 978, "€",   Symbol_Location.behind,   Country.Belgium, Country.Germany,   Country.Estonia,  Country.Finland,  Country.Luxembourg,
                                                                                                                     Country.Greece,  Country.Ireland,   Country.Italy,    Country.Latvia,   Country.Montenegro,
                                                                                                                     Country.Malta,   Country.Austria,   Country.Portugal, Country.Slovakia, Country.VaticanState,
                                                                                                                     Country.Spain,   Country.Cyprus,    Country.Andorra,  Country.Monaco,   Country.Netherlands,
                                                                                                                     Country.France,  Country.SanMarino, Country.Slovenia, Country.Lithuania);

        /// <summary>
        /// Schweizer Franken
        /// </summary>
        public static readonly Currency CHF = new ("Schweizer Franken", "CHF", 756,  null, Symbol_Location.NoSymbol, Country.Switzerland);

        /// <summary>
        /// US Dollar, $
        /// </summary>
        public static readonly Currency USD = new ("US Dollar",         "USD", 840, "$",   Symbol_Location.before,   Country.UnitedStatesOfAmerica);

        public static readonly Currency CZK = new ("Kronen",            "CZK", 203,  null, Symbol_Location.NoSymbol, Country.CzechRepublic);
        public static readonly Currency GBP = new ("Pfund",             "GBP", 826,  null, Symbol_Location.NoSymbol, Country.UnitedKingdom);
        public static readonly Currency HRK = new ("Kuna",              "HRK", 191,  null, Symbol_Location.NoSymbol, Country.Croatia);
        public static readonly Currency PLN = new ("Zloty",             "PLN", 985,  null, Symbol_Location.NoSymbol, Country.Poland);
        public static readonly Currency RON = new ("Leu",               "RON", 946,  null, Symbol_Location.NoSymbol, Country.Romania);
        public static readonly Currency RSD = new ("Dinar",             "RSD", 941,  null, Symbol_Location.NoSymbol, Country.Serbia);
        public static readonly Currency RUB = new ("Rubel",             "RUB", 643,  null, Symbol_Location.NoSymbol, Country.RussianFederation);
        public static readonly Currency SEK = new ("Krone",             "SEK", 752,  null, Symbol_Location.NoSymbol, Country.Sweden);

        #endregion


        #region Operator overloading

        #region Operator == (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object for equality.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        public static Boolean operator == (Currency Currency1,
                                           Currency Currency2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Currency1, Currency2))
                return true;

            // If one is null, but not both, return false.
            if (Currency1 is null || Currency2 is null)
                return false;

            return Currency1.Equals(Currency2);

        }

        #endregion

        #region Operator != (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object for inequality.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        public static Boolean operator != (Currency Currency1,
                                           Currency Currency2)

            => !(Currency1 == Currency2);

        #endregion

        #endregion

        #region IComparable<Currency> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two currencies.
        /// </summary>
        /// <param name="Object">A currency to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Currency currency
                   ? CompareTo(currency)
                   : throw new ArgumentException("The given object is not a currency!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Currency)

        /// <summary>
        /// Compares two currencies.
        /// </summary>
        /// <param name="Currency">A currency to compare with.</param>
        public Int32 CompareTo(Currency? Currency)
        {

            if (Currency is null)
                throw new ArgumentNullException(nameof(Currency), "The given currency must not be null!");

            var c = ISOCode.CompareTo(Currency.ISOCode);

            if (c == 0)
                c = Numeric.CompareTo(Currency.Numeric);

            if (c == 0)
                c = Name.   CompareTo(Currency.Name);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Currency> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two currencies for equality.
        /// </summary>
        /// <param name="Object">A currency to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Currency currency &&
                  Equals(currency);

        #endregion

        #region Equals(Currency)

        /// <summary>
        /// Compares two currencies for equality.
        /// </summary>
        /// <param name="Currency">A currency to compare with.</param>
        public Boolean Equals(Currency? Currency)

            => Currency is not null &&

               ISOCode == Currency.ISOCode &&
               Numeric == Currency.Numeric &&
               Name    == Currency.Name;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return ISOCode.GetHashCode() * 5 ^
                       Numeric.GetHashCode() * 3 ^
                       Name.   GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Name,
                             " [", ISOCode, " / ", Numeric, "]");

        #endregion

    }

}
