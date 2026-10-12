/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

        // The further active currencies of ISO 4217, with their English names; without funds, metals and test codes.
        public static readonly Currency AED = new ("UAE Dirham",                "AED", 784, null, Symbol_Location.NoSymbol);
        public static readonly Currency AFN = new ("Afghani",                   "AFN", 971, null, Symbol_Location.NoSymbol);
        public static readonly Currency ALL = new ("Lek",                       "ALL",   8, null, Symbol_Location.NoSymbol);
        public static readonly Currency AMD = new ("Armenian Dram",             "AMD",  51, null, Symbol_Location.NoSymbol);
        public static readonly Currency AOA = new ("Kwanza",                    "AOA", 973, null, Symbol_Location.NoSymbol);
        public static readonly Currency ARS = new ("Argentine Peso",            "ARS",  32, null, Symbol_Location.NoSymbol);
        public static readonly Currency AUD = new ("Australian Dollar",         "AUD",  36, null, Symbol_Location.NoSymbol);
        public static readonly Currency AWG = new ("Aruban Florin",             "AWG", 533, null, Symbol_Location.NoSymbol);
        public static readonly Currency AZN = new ("Azerbaijan Manat",          "AZN", 944, null, Symbol_Location.NoSymbol);
        public static readonly Currency BAM = new ("Convertible Mark",          "BAM", 977, null, Symbol_Location.NoSymbol);
        public static readonly Currency BBD = new ("Barbados Dollar",           "BBD",  52, null, Symbol_Location.NoSymbol);
        public static readonly Currency BDT = new ("Taka",                      "BDT",  50, null, Symbol_Location.NoSymbol);
        public static readonly Currency BGN = new ("Bulgarian Lev",             "BGN", 975, null, Symbol_Location.NoSymbol);
        public static readonly Currency BHD = new ("Bahraini Dinar",            "BHD",  48, null, Symbol_Location.NoSymbol);
        public static readonly Currency BIF = new ("Burundi Franc",             "BIF", 108, null, Symbol_Location.NoSymbol);
        public static readonly Currency BMD = new ("Bermudian Dollar",          "BMD",  60, null, Symbol_Location.NoSymbol);
        public static readonly Currency BND = new ("Brunei Dollar",             "BND",  96, null, Symbol_Location.NoSymbol);
        public static readonly Currency BOB = new ("Boliviano",                 "BOB",  68, null, Symbol_Location.NoSymbol);
        public static readonly Currency BRL = new ("Brazilian Real",            "BRL", 986, null, Symbol_Location.NoSymbol);
        public static readonly Currency BSD = new ("Bahamian Dollar",           "BSD",  44, null, Symbol_Location.NoSymbol);
        public static readonly Currency BTN = new ("Ngultrum",                  "BTN",  64, null, Symbol_Location.NoSymbol);
        public static readonly Currency BWP = new ("Pula",                      "BWP",  72, null, Symbol_Location.NoSymbol);
        public static readonly Currency BYN = new ("Belarusian Ruble",          "BYN", 933, null, Symbol_Location.NoSymbol);
        public static readonly Currency BZD = new ("Belize Dollar",             "BZD",  84, null, Symbol_Location.NoSymbol);
        public static readonly Currency CAD = new ("Canadian Dollar",           "CAD", 124, null, Symbol_Location.NoSymbol);
        public static readonly Currency CDF = new ("Congolese Franc",           "CDF", 976, null, Symbol_Location.NoSymbol);
        public static readonly Currency CLP = new ("Chilean Peso",              "CLP", 152, null, Symbol_Location.NoSymbol);
        public static readonly Currency CNY = new ("Yuan Renminbi",             "CNY", 156, null, Symbol_Location.NoSymbol);
        public static readonly Currency COP = new ("Colombian Peso",            "COP", 170, null, Symbol_Location.NoSymbol);
        public static readonly Currency CRC = new ("Costa Rican Colon",         "CRC", 188, null, Symbol_Location.NoSymbol);
        public static readonly Currency CUP = new ("Cuban Peso",                "CUP", 192, null, Symbol_Location.NoSymbol);
        public static readonly Currency CVE = new ("Cabo Verde Escudo",         "CVE", 132, null, Symbol_Location.NoSymbol);
        public static readonly Currency DJF = new ("Djibouti Franc",            "DJF", 262, null, Symbol_Location.NoSymbol);
        public static readonly Currency DKK = new ("Danish Krone",              "DKK", 208, null, Symbol_Location.NoSymbol);
        public static readonly Currency DOP = new ("Dominican Peso",            "DOP", 214, null, Symbol_Location.NoSymbol);
        public static readonly Currency DZD = new ("Algerian Dinar",            "DZD",  12, null, Symbol_Location.NoSymbol);
        public static readonly Currency EGP = new ("Egyptian Pound",            "EGP", 818, null, Symbol_Location.NoSymbol);
        public static readonly Currency ERN = new ("Nakfa",                     "ERN", 232, null, Symbol_Location.NoSymbol);
        public static readonly Currency ETB = new ("Ethiopian Birr",            "ETB", 230, null, Symbol_Location.NoSymbol);
        public static readonly Currency FJD = new ("Fiji Dollar",               "FJD", 242, null, Symbol_Location.NoSymbol);
        public static readonly Currency FKP = new ("Falkland Islands Pound",    "FKP", 238, null, Symbol_Location.NoSymbol);
        public static readonly Currency GEL = new ("Lari",                      "GEL", 981, null, Symbol_Location.NoSymbol);
        public static readonly Currency GHS = new ("Ghana Cedi",                "GHS", 936, null, Symbol_Location.NoSymbol);
        public static readonly Currency GIP = new ("Gibraltar Pound",           "GIP", 292, null, Symbol_Location.NoSymbol);
        public static readonly Currency GMD = new ("Dalasi",                    "GMD", 270, null, Symbol_Location.NoSymbol);
        public static readonly Currency GNF = new ("Guinean Franc",             "GNF", 324, null, Symbol_Location.NoSymbol);
        public static readonly Currency GTQ = new ("Quetzal",                   "GTQ", 320, null, Symbol_Location.NoSymbol);
        public static readonly Currency GYD = new ("Guyana Dollar",             "GYD", 328, null, Symbol_Location.NoSymbol);
        public static readonly Currency HKD = new ("Hong Kong Dollar",          "HKD", 344, null, Symbol_Location.NoSymbol);
        public static readonly Currency HNL = new ("Lempira",                   "HNL", 340, null, Symbol_Location.NoSymbol);
        public static readonly Currency HTG = new ("Gourde",                    "HTG", 332, null, Symbol_Location.NoSymbol);
        public static readonly Currency HUF = new ("Forint",                    "HUF", 348, null, Symbol_Location.NoSymbol);
        public static readonly Currency IDR = new ("Rupiah",                    "IDR", 360, null, Symbol_Location.NoSymbol);
        public static readonly Currency ILS = new ("New Israeli Sheqel",        "ILS", 376, null, Symbol_Location.NoSymbol);
        public static readonly Currency INR = new ("Indian Rupee",              "INR", 356, null, Symbol_Location.NoSymbol);
        public static readonly Currency IQD = new ("Iraqi Dinar",               "IQD", 368, null, Symbol_Location.NoSymbol);
        public static readonly Currency IRR = new ("Iranian Rial",              "IRR", 364, null, Symbol_Location.NoSymbol);
        public static readonly Currency ISK = new ("Iceland Krona",             "ISK", 352, null, Symbol_Location.NoSymbol);
        public static readonly Currency JMD = new ("Jamaican Dollar",           "JMD", 388, null, Symbol_Location.NoSymbol);
        public static readonly Currency JOD = new ("Jordanian Dinar",           "JOD", 400, null, Symbol_Location.NoSymbol);
        public static readonly Currency JPY = new ("Yen",                       "JPY", 392, null, Symbol_Location.NoSymbol);
        public static readonly Currency KES = new ("Kenyan Shilling",           "KES", 404, null, Symbol_Location.NoSymbol);
        public static readonly Currency KGS = new ("Som",                       "KGS", 417, null, Symbol_Location.NoSymbol);
        public static readonly Currency KHR = new ("Riel",                      "KHR", 116, null, Symbol_Location.NoSymbol);
        public static readonly Currency KMF = new ("Comorian Franc",            "KMF", 174, null, Symbol_Location.NoSymbol);
        public static readonly Currency KPW = new ("North Korean Won",          "KPW", 408, null, Symbol_Location.NoSymbol);
        public static readonly Currency KRW = new ("Won",                       "KRW", 410, null, Symbol_Location.NoSymbol);
        public static readonly Currency KWD = new ("Kuwaiti Dinar",             "KWD", 414, null, Symbol_Location.NoSymbol);
        public static readonly Currency KYD = new ("Cayman Islands Dollar",     "KYD", 136, null, Symbol_Location.NoSymbol);
        public static readonly Currency KZT = new ("Tenge",                     "KZT", 398, null, Symbol_Location.NoSymbol);
        public static readonly Currency LAK = new ("Lao Kip",                   "LAK", 418, null, Symbol_Location.NoSymbol);
        public static readonly Currency LBP = new ("Lebanese Pound",            "LBP", 422, null, Symbol_Location.NoSymbol);
        public static readonly Currency LKR = new ("Sri Lanka Rupee",           "LKR", 144, null, Symbol_Location.NoSymbol);
        public static readonly Currency LRD = new ("Liberian Dollar",           "LRD", 430, null, Symbol_Location.NoSymbol);
        public static readonly Currency LSL = new ("Loti",                      "LSL", 426, null, Symbol_Location.NoSymbol);
        public static readonly Currency LYD = new ("Libyan Dinar",              "LYD", 434, null, Symbol_Location.NoSymbol);
        public static readonly Currency MAD = new ("Moroccan Dirham",           "MAD", 504, null, Symbol_Location.NoSymbol);
        public static readonly Currency MDL = new ("Moldovan Leu",              "MDL", 498, null, Symbol_Location.NoSymbol);
        public static readonly Currency MGA = new ("Malagasy Ariary",           "MGA", 969, null, Symbol_Location.NoSymbol);
        public static readonly Currency MKD = new ("Denar",                     "MKD", 807, null, Symbol_Location.NoSymbol);
        public static readonly Currency MMK = new ("Kyat",                      "MMK", 104, null, Symbol_Location.NoSymbol);
        public static readonly Currency MNT = new ("Tugrik",                    "MNT", 496, null, Symbol_Location.NoSymbol);
        public static readonly Currency MOP = new ("Pataca",                    "MOP", 446, null, Symbol_Location.NoSymbol);
        public static readonly Currency MRU = new ("Ouguiya",                   "MRU", 929, null, Symbol_Location.NoSymbol);
        public static readonly Currency MUR = new ("Mauritius Rupee",           "MUR", 480, null, Symbol_Location.NoSymbol);
        public static readonly Currency MVR = new ("Rufiyaa",                   "MVR", 462, null, Symbol_Location.NoSymbol);
        public static readonly Currency MWK = new ("Malawi Kwacha",             "MWK", 454, null, Symbol_Location.NoSymbol);
        public static readonly Currency MXN = new ("Mexican Peso",              "MXN", 484, null, Symbol_Location.NoSymbol);
        public static readonly Currency MYR = new ("Malaysian Ringgit",         "MYR", 458, null, Symbol_Location.NoSymbol);
        public static readonly Currency MZN = new ("Mozambique Metical",        "MZN", 943, null, Symbol_Location.NoSymbol);
        public static readonly Currency NAD = new ("Namibia Dollar",            "NAD", 516, null, Symbol_Location.NoSymbol);
        public static readonly Currency NGN = new ("Naira",                     "NGN", 566, null, Symbol_Location.NoSymbol);
        public static readonly Currency NIO = new ("Cordoba Oro",               "NIO", 558, null, Symbol_Location.NoSymbol);
        public static readonly Currency NOK = new ("Norwegian Krone",           "NOK", 578, null, Symbol_Location.NoSymbol);
        public static readonly Currency NPR = new ("Nepalese Rupee",            "NPR", 524, null, Symbol_Location.NoSymbol);
        public static readonly Currency NZD = new ("New Zealand Dollar",        "NZD", 554, null, Symbol_Location.NoSymbol);
        public static readonly Currency OMR = new ("Rial Omani",                "OMR", 512, null, Symbol_Location.NoSymbol);
        public static readonly Currency PAB = new ("Balboa",                    "PAB", 590, null, Symbol_Location.NoSymbol);
        public static readonly Currency PEN = new ("Sol",                       "PEN", 604, null, Symbol_Location.NoSymbol);
        public static readonly Currency PGK = new ("Kina",                      "PGK", 598, null, Symbol_Location.NoSymbol);
        public static readonly Currency PHP = new ("Philippine Peso",           "PHP", 608, null, Symbol_Location.NoSymbol);
        public static readonly Currency PKR = new ("Pakistan Rupee",            "PKR", 586, null, Symbol_Location.NoSymbol);
        public static readonly Currency PYG = new ("Guarani",                   "PYG", 600, null, Symbol_Location.NoSymbol);
        public static readonly Currency QAR = new ("Qatari Rial",               "QAR", 634, null, Symbol_Location.NoSymbol);
        public static readonly Currency RWF = new ("Rwanda Franc",              "RWF", 646, null, Symbol_Location.NoSymbol);
        public static readonly Currency SAR = new ("Saudi Riyal",               "SAR", 682, null, Symbol_Location.NoSymbol);
        public static readonly Currency SBD = new ("Solomon Islands Dollar",    "SBD",  90, null, Symbol_Location.NoSymbol);
        public static readonly Currency SCR = new ("Seychelles Rupee",          "SCR", 690, null, Symbol_Location.NoSymbol);
        public static readonly Currency SDG = new ("Sudanese Pound",            "SDG", 938, null, Symbol_Location.NoSymbol);
        public static readonly Currency SGD = new ("Singapore Dollar",          "SGD", 702, null, Symbol_Location.NoSymbol);
        public static readonly Currency SHP = new ("Saint Helena Pound",        "SHP", 654, null, Symbol_Location.NoSymbol);
        public static readonly Currency SLE = new ("Leone",                     "SLE", 925, null, Symbol_Location.NoSymbol);
        public static readonly Currency SOS = new ("Somali Shilling",           "SOS", 706, null, Symbol_Location.NoSymbol);
        public static readonly Currency SRD = new ("Surinam Dollar",            "SRD", 968, null, Symbol_Location.NoSymbol);
        public static readonly Currency SSP = new ("South Sudanese Pound",      "SSP", 728, null, Symbol_Location.NoSymbol);
        public static readonly Currency STN = new ("Dobra",                     "STN", 930, null, Symbol_Location.NoSymbol);
        public static readonly Currency SVC = new ("El Salvador Colon",         "SVC", 222, null, Symbol_Location.NoSymbol);
        public static readonly Currency SYP = new ("Syrian Pound",              "SYP", 760, null, Symbol_Location.NoSymbol);
        public static readonly Currency SZL = new ("Lilangeni",                 "SZL", 748, null, Symbol_Location.NoSymbol);
        public static readonly Currency THB = new ("Baht",                      "THB", 764, null, Symbol_Location.NoSymbol);
        public static readonly Currency TJS = new ("Somoni",                    "TJS", 972, null, Symbol_Location.NoSymbol);
        public static readonly Currency TMT = new ("Turkmenistan New Manat",    "TMT", 934, null, Symbol_Location.NoSymbol);
        public static readonly Currency TND = new ("Tunisian Dinar",            "TND", 788, null, Symbol_Location.NoSymbol);
        public static readonly Currency TOP = new ("Pa'anga",                   "TOP", 776, null, Symbol_Location.NoSymbol);
        public static readonly Currency TRY = new ("Turkish Lira",              "TRY", 949, null, Symbol_Location.NoSymbol);
        public static readonly Currency TTD = new ("Trinidad and Tobago Dollar","TTD", 780, null, Symbol_Location.NoSymbol);
        public static readonly Currency TWD = new ("New Taiwan Dollar",         "TWD", 901, null, Symbol_Location.NoSymbol);
        public static readonly Currency TZS = new ("Tanzanian Shilling",        "TZS", 834, null, Symbol_Location.NoSymbol);
        public static readonly Currency UAH = new ("Hryvnia",                   "UAH", 980, null, Symbol_Location.NoSymbol);
        public static readonly Currency UGX = new ("Uganda Shilling",           "UGX", 800, null, Symbol_Location.NoSymbol);
        public static readonly Currency UYU = new ("Peso Uruguayo",             "UYU", 858, null, Symbol_Location.NoSymbol);
        public static readonly Currency UZS = new ("Uzbekistan Sum",            "UZS", 860, null, Symbol_Location.NoSymbol);
        public static readonly Currency VED = new ("Bolivar Soberano (digital)","VED", 926, null, Symbol_Location.NoSymbol);
        public static readonly Currency VES = new ("Bolivar Soberano",          "VES", 928, null, Symbol_Location.NoSymbol);
        public static readonly Currency VND = new ("Dong",                      "VND", 704, null, Symbol_Location.NoSymbol);
        public static readonly Currency VUV = new ("Vatu",                      "VUV", 548, null, Symbol_Location.NoSymbol);
        public static readonly Currency WST = new ("Tala",                      "WST", 882, null, Symbol_Location.NoSymbol);
        public static readonly Currency XAF = new ("CFA Franc BEAC",            "XAF", 950, null, Symbol_Location.NoSymbol);
        public static readonly Currency XCD = new ("East Caribbean Dollar",     "XCD", 951, null, Symbol_Location.NoSymbol);
        public static readonly Currency XCG = new ("Caribbean Guilder",         "XCG", 532, null, Symbol_Location.NoSymbol);
        public static readonly Currency XOF = new ("CFA Franc BCEAO",           "XOF", 952, null, Symbol_Location.NoSymbol);
        public static readonly Currency XPF = new ("CFP Franc",                 "XPF", 953, null, Symbol_Location.NoSymbol);
        public static readonly Currency YER = new ("Yemeni Rial",               "YER", 886, null, Symbol_Location.NoSymbol);
        public static readonly Currency ZAR = new ("Rand",                      "ZAR", 710, null, Symbol_Location.NoSymbol);
        public static readonly Currency ZMW = new ("Zambian Kwacha",            "ZMW", 967, null, Symbol_Location.NoSymbol);
        public static readonly Currency ZWG = new ("Zimbabwe Gold",             "ZWG", 924, null, Symbol_Location.NoSymbol);

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
