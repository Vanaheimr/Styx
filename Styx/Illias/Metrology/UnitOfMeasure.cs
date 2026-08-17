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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A unit of measure with a stable numeric identification for
    /// compact (CBOR) wire formats, a unit symbol and optional aliases.
    /// The numeric identifications 0..32767 are reserved for well-known
    /// units defined by this registry; 32768 and above are available
    /// for user-registered units.
    /// Note: The prefixable base unit of mass is the Gram, therefore
    /// a kilogram is expressed as (Gram, SIPrefix.Kilo).
    /// </summary>
    public class UnitOfMeasure : IEquatable <UnitOfMeasure>,
                                 IComparable<UnitOfMeasure>,
                                 IComparable
    {

        #region Data

        private static readonly ConcurrentDictionary<UInt16, UnitOfMeasure>  byNumeric   = [];
        private static readonly ConcurrentDictionary<String, UnitOfMeasure>  bySymbol    = new (StringComparer.Ordinal);
        private static readonly ConcurrentDictionary<String, UnitOfMeasure>  byName      = new (StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Properties

        /// <summary>
        /// The stable numeric identification of this unit of measure.
        /// </summary>
        public UInt16                 Numeric    { get; }

        /// <summary>
        /// The symbol of this unit of measure, e.g. "A" for the Ampere.
        /// </summary>
        public String                 Symbol     { get; }

        /// <summary>
        /// The name of this unit of measure, e.g. "Ampere".
        /// </summary>
        public String                 Name       { get; }

        /// <summary>
        /// Optional aliases of this unit of measure,
        /// e.g. "Ohm" for "Ω" or the SenML "Cel" for "°C".
        /// </summary>
        public IReadOnlyList<String>  Aliases    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unit of measure.
        /// </summary>
        private UnitOfMeasure(String    Name,
                              String    Symbol,
                              UInt16    Numeric,
                              String[]  Aliases)
        {

            this.Name     = Name;
            this.Symbol   = Symbol;
            this.Numeric  = Numeric;
            this.Aliases  = Aliases;

        }

        #endregion


        #region (static) Register   (Name, Symbol, Numeric, params Aliases)

        /// <summary>
        /// Register a new unit of measure in a thread-safe way.
        /// Conflicting numeric identifications, symbols, aliases
        /// or names throw an ArgumentException.
        /// </summary>
        /// <param name="Name">The name of the unit of measure, e.g. "Ampere".</param>
        /// <param name="Symbol">The symbol of the unit of measure, e.g. "A".</param>
        /// <param name="Numeric">The stable numeric identification of the unit of measure.</param>
        /// <param name="Aliases">Optional aliases of the unit of measure.</param>
        public static UnitOfMeasure Register(String           Name,
                                             String           Symbol,
                                             UInt16           Numeric,
                                             params String[]  Aliases)
        {

            if (TryRegister(Name, Symbol, Numeric, out var unitOfMeasure, Aliases))
                return unitOfMeasure;

            throw new ArgumentException($"The unit of measure '{Name}' ('{Symbol}', {Numeric}) conflicts with an already registered unit of measure!");

        }

        #endregion

        #region (static) TryRegister(Name, Symbol, Numeric, out UnitOfMeasure, params Aliases)

        /// <summary>
        /// Try to register a new unit of measure in a thread-safe way.
        /// </summary>
        /// <param name="Name">The name of the unit of measure, e.g. "Ampere".</param>
        /// <param name="Symbol">The symbol of the unit of measure, e.g. "A".</param>
        /// <param name="Numeric">The stable numeric identification of the unit of measure.</param>
        /// <param name="UnitOfMeasure">The registered unit of measure.</param>
        /// <param name="Aliases">Optional aliases of the unit of measure.</param>
        public static Boolean TryRegister(String                                    Name,
                                          String                                    Symbol,
                                          UInt16                                    Numeric,
                                          [NotNullWhen(true)] out UnitOfMeasure?    UnitOfMeasure,
                                          params String[]                           Aliases)
        {

            UnitOfMeasure = null;

            if (Name.  IsNullOrEmpty() ||
                Symbol.IsNullOrEmpty())
            {
                return false;
            }

            var newUnit = new UnitOfMeasure(Name.  Trim(),
                                            Symbol.Trim(),
                                            Numeric,
                                            [.. Aliases.Select(alias => alias.Trim())]);

            // The numeric identification is the primary key...
            if (!byNumeric.TryAdd(Numeric, newUnit))
                return false;

            var registeredTexts = new List<String>();

            try
            {

                if (!bySymbol.TryAdd(newUnit.Symbol, newUnit))
                    throw new ArgumentException();

                registeredTexts.Add(newUnit.Symbol);

                foreach (var alias in newUnit.Aliases)
                {

                    if (!bySymbol.TryAdd(alias, newUnit))
                        throw new ArgumentException();

                    registeredTexts.Add(alias);

                }

                if (!byName.TryAdd(newUnit.Name, newUnit))
                    throw new ArgumentException();

                UnitOfMeasure = newUnit;
                return true;

            }
            catch (ArgumentException)
            {

                // Roll back the partial registration...
                foreach (var text in registeredTexts)
                    bySymbol.TryRemove(text, out _);

                byNumeric.TryRemove(Numeric, out _);

                return false;

            }

        }

        #endregion


        #region Static defaults

        /// <summary>
        /// The number one (1), the coherent SI unit of every dimensionless
        /// quantity: ratios, efficiencies, counts, refractive indices.
        /// It holds the first identification because it is the neutral element
        /// of unit multiplication - and because its symbol is "1".
        /// SenML spells it "/".
        /// </summary>
        public static readonly UnitOfMeasure  One                     = Register("One",                    "1",      1, "one", "/");

        // NOTE: The single-byte identifications 1..23 are allocated by how often a
        // unit actually occurs in e-mobility traffic, NOT taxonomically - CBOR
        // encodes 1..23 in one byte and everything above in two. Taxonomy resumes
        // at 24. See Styx/Illias/CBOR/tag-44252.md, section 4.

        // Single-byte range, by frequency (1..23); the prefixable base of mass is the Gram!

        /// <summary>
        /// The second (s, 8), the SI base unit of time.
        /// It sits outside the base unit block because identification 1 was
        /// given to the dimensionless one; see the specification.
        /// </summary>
        public static readonly UnitOfMeasure  Second                  = Register("Second",                 "s",      8);

        /// <summary>
        /// The meter (m, 15), the SI base unit of length.
        /// </summary>
        public static readonly UnitOfMeasure  Meter                   = Register("Meter",                  "m",      15, "Metre");

        /// <summary>
        /// The gram (g, 16), the prefixable base unit of mass: A kilogram is (Gram, Kilo).
        /// </summary>
        public static readonly UnitOfMeasure  Gram                    = Register("Gram",                   "g",      16);

        /// <summary>
        /// The ampere (A, 4), the SI base unit of electric current.
        /// </summary>
        public static readonly UnitOfMeasure  Ampere                  = Register("Ampere",                 "A",      4);

        /// <summary>
        /// The kelvin (K, 17), the SI base unit of thermodynamic temperature.
        /// </summary>
        public static readonly UnitOfMeasure  Kelvin                  = Register("Kelvin",                 "K",      17);

        /// <summary>
        /// The mole (mol, 24), the SI base unit of the amount of substance.
        /// </summary>
        public static readonly UnitOfMeasure  Mole                    = Register("Mole",                   "mol",    24);

        /// <summary>
        /// The candela (cd, 25), the SI base unit of luminous intensity.
        /// </summary>
        public static readonly UnitOfMeasure  Candela                 = Register("Candela",                "cd",     25);


        // Remaining SI base and named derived units (24..39); 40..59 reserved

        /// <summary>
        /// The hertz (Hz, 9), frequency.
        /// </summary>
        public static readonly UnitOfMeasure  Hertz                   = Register("Hertz",                  "Hz",     9);

        /// <summary>
        /// The newton (N, 26), force.
        /// </summary>
        public static readonly UnitOfMeasure  Newton                  = Register("Newton",                 "N",      26);

        /// <summary>
        /// The pascal (Pa, 21), pressure.
        /// </summary>
        public static readonly UnitOfMeasure  Pascal                  = Register("Pascal",                 "Pa",    21);

        /// <summary>
        /// The joule (J, 20), energy.
        /// </summary>
        public static readonly UnitOfMeasure  Joule                   = Register("Joule",                  "J",     20);

        /// <summary>
        /// The watt (W, 3), power.
        /// </summary>
        public static readonly UnitOfMeasure  Watt                    = Register("Watt",                   "W",      3);

        /// <summary>
        /// The coulomb (C, 27), electric charge.
        /// </summary>
        public static readonly UnitOfMeasure  Coulomb                 = Register("Coulomb",                "C",     27);

        /// <summary>
        /// The volt (V, 5), electric potential.
        /// </summary>
        public static readonly UnitOfMeasure  Volt                    = Register("Volt",                   "V",      5);

        /// <summary>
        /// The farad (F, 28), capacitance.
        /// </summary>
        public static readonly UnitOfMeasure  Farad                   = Register("Farad",                  "F",     28);

        /// <summary>
        /// The ohm (Ω, 14), electric resistance.
        /// Accepts the Greek capital letter omega (U+03A9, canonical) as well as
        /// the visually identical, Unicode-deprecated ohm sign (U+2126), which
        /// older sources - including the Ohm struct of this namespace - still use.
        /// </summary>
        public static readonly UnitOfMeasure  Ohm                     = Register("Ohm",                    "Ω",     14, "Ohm", "\u2126");   // the Unicode-deprecated OHM SIGN, kept parseable on purpose

        /// <summary>
        /// The siemens (S, 23), electric conductance.
        /// </summary>
        public static readonly UnitOfMeasure  Siemens                 = Register("Siemens",                "S",     23);

        /// <summary>
        /// The weber (Wb, 29), magnetic flux.
        /// </summary>
        public static readonly UnitOfMeasure  Weber                   = Register("Weber",                  "Wb",    29);

        /// <summary>
        /// The tesla (T, 30), magnetic flux density.
        /// </summary>
        public static readonly UnitOfMeasure  Tesla                   = Register("Tesla",                  "T",     30);

        /// <summary>
        /// The henry (H, 31), inductance.
        /// </summary>
        public static readonly UnitOfMeasure  Henry                   = Register("Henry",                  "H",     31);

        /// <summary>
        /// The degree Celsius (°C, 7), temperature. SenML alias: "Cel".
        /// </summary>
        public static readonly UnitOfMeasure  Celsius                 = Register("Celsius",                "°C",     7, "Cel");

        /// <summary>
        /// The lumen (lm, 32), luminous flux.
        /// </summary>
        public static readonly UnitOfMeasure  Lumen                   = Register("Lumen",                  "lm",    32);

        /// <summary>
        /// The lux (lx, 33), illuminance.
        /// </summary>
        public static readonly UnitOfMeasure  Lux                     = Register("Lux",                    "lx",    33);

        /// <summary>
        /// The becquerel (Bq, 34), radioactivity.
        /// </summary>
        public static readonly UnitOfMeasure  Becquerel               = Register("Becquerel",              "Bq",    34);

        /// <summary>
        /// The gray (Gy, 35), absorbed dose.
        /// </summary>
        public static readonly UnitOfMeasure  Gray                    = Register("Gray",                   "Gy",    35);

        /// <summary>
        /// The sievert (Sv, 36), equivalent dose.
        /// </summary>
        public static readonly UnitOfMeasure  Sievert                 = Register("Sievert",                "Sv",    36);

        /// <summary>
        /// The katal (kat, 37), catalytic activity.
        /// </summary>
        public static readonly UnitOfMeasure  Katal                   = Register("Katal",                  "kat",   37);

        /// <summary>
        /// The radian (rad, 38), plane angle.
        /// </summary>
        public static readonly UnitOfMeasure  Radian                  = Register("Radian",                 "rad",   38);

        /// <summary>
        /// The steradian (sr, 39), solid angle.
        /// </summary>
        public static readonly UnitOfMeasure  Steradian               = Register("Steradian",              "sr",    39);


        // Remaining accepted non-SI units (60..65); 66..99 reserved

        /// <summary>
        /// The minute (min, 19), time.
        /// </summary>
        public static readonly UnitOfMeasure  Minute                  = Register("Minute",                 "min",   19);

        /// <summary>
        /// The hour (h, 18), time.
        /// </summary>
        public static readonly UnitOfMeasure  Hour                    = Register("Hour",                   "h",     18);

        /// <summary>
        /// The day (d, 60), time.
        /// </summary>
        public static readonly UnitOfMeasure  Day                     = Register("Day",                    "d",     60);

        /// <summary>
        /// The degree (°, 61), plane angle.
        /// </summary>
        public static readonly UnitOfMeasure  Degree                  = Register("Degree",                 "°",     61, "deg");

        /// <summary>
        /// The litre (l, 62), volume.
        /// </summary>
        public static readonly UnitOfMeasure  Litre                   = Register("Litre",                  "l",     62, "L", "Liter");

        /// <summary>
        /// The tonne (t, 63), mass.
        /// </summary>
        public static readonly UnitOfMeasure  Tonne                   = Register("Tonne",                  "t",     63);

        /// <summary>
        /// The percent (%, 6), dimensionless ratio.
        /// </summary>
        public static readonly UnitOfMeasure  Percent                 = Register("Percent",                "%",      6);

        /// <summary>
        /// The permille (‰, 64), dimensionless ratio.
        /// </summary>
        public static readonly UnitOfMeasure  Permille                = Register("Permille",               "‰",     64);

        /// <summary>
        /// Parts per million (ppm, 65), dimensionless ratio.
        /// </summary>
        public static readonly UnitOfMeasure  PartsPerMillion         = Register("PartsPerMillion",        "ppm",   65);

        // Further electrotechnical and energy units: 100..119 reserved

        /// <summary>
        /// The watt-hour (Wh, 2), energy.
        /// </summary>
        public static readonly UnitOfMeasure  WattHour                = Register("WattHour",               "Wh",     2);

        /// <summary>
        /// The volt-ampere (VA, 11), apparent power.
        /// </summary>
        public static readonly UnitOfMeasure  VoltAmpere              = Register("VoltAmpere",             "VA",    11);

        /// <summary>
        /// The volt-ampere reactive (var, 10), reactive power.
        /// </summary>
        public static readonly UnitOfMeasure  VoltAmpereReactive      = Register("VoltAmpereReactive",     "var",   10);

        /// <summary>
        /// The volt-ampere-reactive hour (varh, 13), reactive energy.
        /// </summary>
        public static readonly UnitOfMeasure  VoltAmpereReactiveHour  = Register("VoltAmpereReactiveHour", "varh",  13);

        /// <summary>
        /// The ampere-hour (Ah, 12), electric charge.
        /// </summary>
        public static readonly UnitOfMeasure  AmpereHour              = Register("AmpereHour",             "Ah",    12);


        // Data units (120..122); 123..139 reserved

        /// <summary>
        /// The bit (bit, 120), information.
        /// </summary>
        public static readonly UnitOfMeasure  Bit                     = Register("Bit",                    "bit",   120);

        /// <summary>
        /// The byte (B, 121), information.
        /// </summary>
        public static readonly UnitOfMeasure  Byte                    = Register("Byte",                   "B",     121);

        /// <summary>
        /// Bit per second (bit/s, 22), data rate.
        /// </summary>
        public static readonly UnitOfMeasure  BitPerSecond            = Register("BitPerSecond",           "bit/s", 22, "bps");

        /// <summary>
        /// Byte per second (B/s, 122), data rate.
        /// </summary>
        public static readonly UnitOfMeasure  BytePerSecond           = Register("BytePerSecond",          "B/s",   122);


        // Geometric units (140..141)

        /// <summary>
        /// The square meter (m², 140), area.
        /// </summary>
        public static readonly UnitOfMeasure  SquareMeter             = Register("SquareMeter",            "m²",    140, "m2");

        /// <summary>
        /// The cubic meter (m³, 141), volume.
        /// </summary>
        public static readonly UnitOfMeasure  CubicMeter              = Register("CubicMeter",             "m³",    141, "m3");


        /// <summary>
        /// All currently registered units of measure,
        /// ordered by their numeric identification.
        /// </summary>
        public static IEnumerable<UnitOfMeasure>  All

            => byNumeric.Values.OrderBy(unit => unit.Numeric);

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a unit symbol (case-sensitive),
        /// alias or unit name (case-insensitive).
        /// </summary>
        /// <param name="Text">A text representation of a unit of measure.</param>
        public static UnitOfMeasure Parse(String Text)
        {

            if (TryParse(Text, out var unitOfMeasure))
                return unitOfMeasure;

            throw new ArgumentException($"Invalid text representation of a unit of measure: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text,    out UnitOfMeasure)

        /// <summary>
        /// Try to parse the given text as a unit symbol (case-sensitive),
        /// alias or unit name (case-insensitive).
        /// </summary>
        /// <param name="Text">A text representation of a unit of measure.</param>
        /// <param name="UnitOfMeasure">The parsed unit of measure.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?         Text,
                                       [NotNullWhen(true)] out UnitOfMeasure?  UnitOfMeasure)
        {

            UnitOfMeasure = null;

            if (Text is null)
                return false;

            Text = Text.Trim();

            if (Text.Length == 0)
                return false;

            if (bySymbol.TryGetValue(Text, out UnitOfMeasure))
                return true;

            return byName.TryGetValue(Text, out UnitOfMeasure);

        }

        #endregion

        #region (static) TryParse(Numeric, out UnitOfMeasure)

        /// <summary>
        /// Try to return the unit of measure of the given
        /// numeric identification.
        /// </summary>
        /// <param name="Numeric">The numeric identification of a unit of measure.</param>
        /// <param name="UnitOfMeasure">The unit of measure.</param>
        public static Boolean TryParse(UInt16                                  Numeric,
                                       [NotNullWhen(true)] out UnitOfMeasure?  UnitOfMeasure)

            => byNumeric.TryGetValue(Numeric, out UnitOfMeasure);

        #endregion


        #region Operator overloading

        #region Operator == (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UnitOfMeasure? UnitOfMeasure1,
                                           UnitOfMeasure? UnitOfMeasure2)
        {

            if (Object.ReferenceEquals(UnitOfMeasure1, UnitOfMeasure2))
                return true;

            if (UnitOfMeasure1 is null || UnitOfMeasure2 is null)
                return false;

            return UnitOfMeasure1.Equals(UnitOfMeasure2);

        }

        #endregion

        #region Operator != (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UnitOfMeasure? UnitOfMeasure1,
                                           UnitOfMeasure? UnitOfMeasure2)

            => !(UnitOfMeasure1 == UnitOfMeasure2);

        #endregion

        #endregion

        #region IComparable<UnitOfMeasure> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two units of measure.
        /// </summary>
        /// <param name="Object">A unit of measure to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null                          => 1,
                   UnitOfMeasure unitOfMeasure   => CompareTo(unitOfMeasure),
                   _                             => throw new ArgumentException("The given object is not a unit of measure!", nameof(Object))
               };

        #endregion

        #region CompareTo(UnitOfMeasure)

        /// <summary>
        /// Compares two units of measure.
        /// </summary>
        /// <param name="UnitOfMeasure">A unit of measure to compare with.</param>
        public Int32 CompareTo(UnitOfMeasure? UnitOfMeasure)

            => UnitOfMeasure is null
                   ? 1
                   : Numeric.CompareTo(UnitOfMeasure.Numeric);

        #endregion

        #endregion

        #region IEquatable<UnitOfMeasure> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two units of measure for equality.
        /// </summary>
        /// <param name="Object">A unit of measure to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnitOfMeasure unitOfMeasure &&
                   Equals(unitOfMeasure);

        #endregion

        #region Equals(UnitOfMeasure)

        /// <summary>
        /// Compares two units of measure for equality.
        /// </summary>
        /// <param name="UnitOfMeasure">A unit of measure to compare with.</param>
        public Boolean Equals(UnitOfMeasure? UnitOfMeasure)

            => UnitOfMeasure is not null &&
               Numeric == UnitOfMeasure.Numeric;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Numeric.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Symbol;

        #endregion

    }

}
