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

using System.Globalization;
using System.Reflection;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Cross-checks over the entire metrology family.
    ///
    /// The unit structs of Styx/Illias/Metrology are copy-paste derived from a
    /// single template, between 900 and 1.800 lines each and around 95 % alike.
    /// A defect found in one of them is therefore almost never alone, and
    /// reading them does not scale. These tests state one expectation per
    /// member in a table and check the whole family against it in a loop.
    ///
    /// The exponent column of each table was cross-checked against the XML
    /// documentation of the members it describes before it was frozen here:
    /// the code says "+ 3", the documentation says "KiloWatts", and those are
    /// two independent statements of the same fact. From here on the tables
    /// are the specification - a factory that quietly changes its exponent
    /// fails a test instead of scaling every value by a thousand.
    ///
    /// A member added to the family but not to a table fails
    /// The_tables_describe_every_member_of_the_family().
    ///
    /// SI prefixes are case-sensitive: 'm' is milli, 'M' is mega. Note that
    /// the very same capital 'M' means mega in Watt.FromMW and milli in
    /// Henry.FromMH - the name alone does not decide, which is why the
    /// exponents are tabulated rather than derived.
    /// </summary>
    [TestFixture]
    public class MetrologyFamilyTests
    {

        #region (private record) PrefixedUnit (TypeName, NameSuffix, Exponent)

        /// <summary>
        /// One unit at one SI prefix, named after the suffix that the four
        /// members producing it share: From{X}, TryFrom{X}, Parse{X}, TryParse{X}.
        /// </summary>
        private readonly record struct PrefixedUnit(String  TypeName,
                                                    String  NameSuffix,
                                                    Int32   Exponent);

        #endregion

        #region (private record) Conversion   (TypeName, Property, Exponent)

        /// <summary>
        /// A property that renders the stored value at one SI prefix.
        /// </summary>
        private readonly record struct Conversion(String  TypeName,
                                                  String  Property,
                                                  Int32   Exponent);

        #endregion

        #region (private record) TextSuffix   (TypeName, Suffix, Exponent)

        /// <summary>
        /// A unit suffix accepted by the text parser of a type.
        /// </summary>
        private readonly record struct TextSuffix(String  TypeName,
                                                  String  Suffix,
                                                  Int32   Exponent);

        #endregion

        #region (private record) FormatSpec   (TypeName, Specifier, Exponent)

        /// <summary>
        /// A format specifier accepted by TryFormat/ToString of a type.
        /// </summary>
        private readonly record struct FormatSpec(String  TypeName,
                                                  String  Specifier,
                                                  Int32   Exponent);

        #endregion


        #region Data: prefixedUnits

        /// <summary>
        /// Every prefixed unit of the family. Covers 4 members each:
        /// From{X}, TryFrom{X} (both overloads), Parse{X} and TryParse{X}.
        /// </summary>
        private static readonly PrefixedUnit[] prefixedUnits = [

            new ("Ampere",             "A",               0),
            new ("Ampere",             "KA",              3),

            new ("BitPerSecond",       "BPS",             0),
            new ("BitPerSecond",       "KBPS",            3),
            new ("BitPerSecond",       "MBPS",            6),
            new ("BitPerSecond",       "GBPS",            9),
            new ("BitPerSecond",       "TBPS",           12),

            new ("BytePerSecond",      "BPS",             0),
            new ("BytePerSecond",      "KBPS",            3),
            new ("BytePerSecond",      "MBPS",            6),
            new ("BytePerSecond",      "GBPS",            9),
            new ("BytePerSecond",      "TBPS",           12),

            new ("Celsius",            "C",               0),

            new ("Farad",              "F",               0),
            new ("Farad",              "µF",             -6),
            new ("Farad",              "NF",             -9),
            new ("Farad",              "PF",            -12),

            new ("Henry",              "H",               0),
            new ("Henry",              "KH",              3),
            new ("Henry",              "MH",             -3),   // milli, not mega!
            new ("Henry",              "µH",             -6),
            new ("Henry",              "NH",             -9),
            new ("Henry",              "PH",            -12),

            new ("Hertz",              "Hz",              0),
            new ("Hertz",              "KHz",             3),
            new ("Hertz",              "MHz",             6),
            new ("Hertz",              "GHz",             9),

            new ("Kelvin",             "K",               0),

            new ("Kilogram",           "KG",              0),   // the base is the kilogram,
            new ("Kilogram",           "G",              -3),   // so the gram is the prefixed one

            new ("Meter",              "_m",              0),
            new ("Meter",              "_mm",            -3),
            new ("Meter",              "_cm",            -2),
            new ("Meter",              "_dm",            -1),
            new ("Meter",              "_km",             3),

            new ("Ohm",                "_Ω",              0),
            new ("Ohm",                "_µΩ",            -6),
            new ("Ohm",                "_mΩ",            -3),
            new ("Ohm",                "_KΩ",             3),
            new ("Ohm",                "_MΩ",             6),

            new ("Siemens",            "S",               0),
            new ("Siemens",            "KS",              3),

            new ("Tonne",              "T",               0),
            new ("Tonne",              "KT",              3),

            new ("Volt",               "V",               0),
            new ("Volt",               "KV",              3),

            new ("VoltAmpere",         "VA",              0),
            new ("VoltAmpere",         "KVA",             3),

            new ("VoltAmpereReactive", "VAr",             0),
            new ("VoltAmpereReactive", "KVAr",            3),

            new ("Watt",               "W",               0),
            new ("Watt",               "KW",              3),
            new ("Watt",               "MW",              6),   // mega, not milli!
            new ("Watt",               "GW",              9),

            new ("WattHour",           "Wh",              0),
            new ("WattHour",           "KWh",             3),
            new ("WattHour",           "MWh",             6),
            new ("WattHour",           "GWh",             9)

        ];

        #endregion

        #region Data: conversions

        /// <summary>
        /// Every property that renders the stored value at another prefix.
        /// </summary>
        private static readonly Conversion[] conversions = [

            new ("Ampere",             "kA",              3),

            new ("BitPerSecond",       "kbps",            3),
            new ("BitPerSecond",       "Mbps",            6),
            new ("BitPerSecond",       "Gbps",            9),
            new ("BitPerSecond",       "Tbps",           12),

            new ("BytePerSecond",      "kBps",            3),
            new ("BytePerSecond",      "MBps",            6),
            new ("BytePerSecond",      "GBps",            9),
            new ("BytePerSecond",      "TBps",           12),

            new ("Farad",              "µF",             -6),
            new ("Farad",              "nF",             -9),
            new ("Farad",              "pF",            -12),

            new ("Henry",              "kH",              3),
            new ("Henry",              "mH",             -3),
            new ("Henry",              "µH",             -6),
            new ("Henry",              "nH",             -9),
            new ("Henry",              "pH",            -12),

            new ("Hertz",              "kHz",             3),
            new ("Hertz",              "MHz",             6),
            new ("Hertz",              "GHz",             9),

            new ("Kilogram",           "g",              -3),

            new ("Meter",              "mm",             -3),
            new ("Meter",              "cm",             -2),
            new ("Meter",              "dm",             -1),
            new ("Meter",              "km",              3),

            new ("Ohm",                "µΩ",             -6),
            new ("Ohm",                "mΩ",             -3),
            new ("Ohm",                "kΩ",              3),
            new ("Ohm",                "MΩ",              6),

            new ("Siemens",            "kS",              3),

            new ("Tonne",              "kT",              3),

            new ("Volt",               "kV",              3),

            new ("VoltAmpere",         "kVA",             3),

            new ("VoltAmpereReactive", "kVAr",            3),

            new ("Watt",               "kW",              3),
            new ("Watt",               "MW",              6),
            new ("Watt",               "GW",              9),

            new ("WattHour",           "kWh",             3),
            new ("WattHour",           "MWh",             6),
            new ("WattHour",           "GWh",             9)

        ];

        #endregion

        #region Data: textSuffixes

        /// <summary>
        /// Every unit suffix the text parsers accept. The strip length must
        /// match the suffix length - "kOhm" is four characters, and cutting
        /// three of them leaves an 'O' that no number parser will swallow.
        /// </summary>
        private static readonly TextSuffix[] textSuffixes = [

            new ("Ampere",             "kA",              3),
            new ("Ampere",             "A",               0),

            new ("BitPerSecond",       "kbit/s",          3),
            new ("BitPerSecond",       "kb/s",            3),
            new ("BitPerSecond",       "Mbit/s",          6),
            new ("BitPerSecond",       "Mb/s",            6),
            new ("BitPerSecond",       "Gbit/s",          9),
            new ("BitPerSecond",       "Gb/s",            9),
            new ("BitPerSecond",       "Tbit/s",         12),
            new ("BitPerSecond",       "Tb/s",           12),
            new ("BitPerSecond",       "bit/s",           0),
            new ("BitPerSecond",       "b/s",             0),

            new ("BytePerSecond",      "kByte/s",         3),
            new ("BytePerSecond",      "kB/s",            3),
            new ("BytePerSecond",      "MByte/s",         6),
            new ("BytePerSecond",      "MB/s",            6),
            new ("BytePerSecond",      "GByte/s",         9),
            new ("BytePerSecond",      "GB/s",            9),
            new ("BytePerSecond",      "TByte/s",        12),
            new ("BytePerSecond",      "TB/s",           12),
            new ("BytePerSecond",      "Byte/s",          0),

            new ("Celsius",            "°C",              0),

            new ("Farad",              "µF",             -6),
            new ("Farad",              "nF",             -9),
            new ("Farad",              "pF",            -12),
            new ("Farad",              "F",               0),

            new ("Henry",              "kH",              3),
            new ("Henry",              "mH",             -3),
            new ("Henry",              "µH",             -6),
            new ("Henry",              "nH",             -9),
            new ("Henry",              "pH",            -12),
            new ("Henry",              "H",               0),

            new ("Hertz",              "kHz",             3),
            new ("Hertz",              "MHz",             6),
            new ("Hertz",              "GHz",             9),
            new ("Hertz",              "Hz",              0),

            new ("Kelvin",             "K",               0),

            new ("Kilogram",           "kg",              0),
            new ("Kilogram",           "g",              -3),

            new ("Meter",              "mm",             -3),
            new ("Meter",              "cm",             -2),
            new ("Meter",              "dm",             -1),
            new ("Meter",              "km",              3),
            new ("Meter",              "m",               0),

            new ("Ohm",                "µOhm",           -6),
            new ("Ohm",                "mOhm",           -3),
            new ("Ohm",                "kOhm",            3),
            new ("Ohm",                "MOhm",            6),
            new ("Ohm",                "GOhm",            9),
            new ("Ohm",                "Ohm",             0),

            new ("Siemens",            "kS",              3),
            new ("Siemens",            "S",               0),

            new ("Tonne",              "kt",              3),
            new ("Tonne",              "t",               0),

            new ("Volt",               "kV",              3),
            new ("Volt",               "V",               0),

            new ("VoltAmpere",         "kVA",             3),
            new ("VoltAmpere",         "VA",              0),

            new ("VoltAmpereReactive", "kVAr",            3),
            new ("VoltAmpereReactive", "VAr",             0),

            new ("Watt",               "kW",              3),
            new ("Watt",               "MW",              6),
            new ("Watt",               "GW",              9),
            new ("Watt",               "W",               0),

            new ("WattHour",           "kWh",             3),
            new ("WattHour",           "MWh",             6),
            new ("WattHour",           "GWh",             9),
            new ("WattHour",           "Wh",              0)

        ];

        #endregion

        #region Data: formatSpecs

        /// <summary>
        /// Every format specifier of the family, with the prefix it selects.
        /// The general specifier "G" is compared case-insensitively by most
        /// types, so a unit symbol that differs from it only in case can
        /// never be reached.
        /// </summary>
        private static readonly FormatSpec[] formatSpecs = [

            new ("Ampere",             "A",               0),
            new ("Ampere",             "kA",              3),

            new ("BitPerSecond",       "bit/s",           0),
            new ("BitPerSecond",       "b/s",             0),
            new ("BitPerSecond",       "bps",             0),
            new ("BitPerSecond",       "kbit/s",          3),
            new ("BitPerSecond",       "kb/s",            3),
            new ("BitPerSecond",       "kbps",            3),
            new ("BitPerSecond",       "Mbit/s",          6),
            new ("BitPerSecond",       "Mb/s",            6),
            new ("BitPerSecond",       "Mbps",            6),
            new ("BitPerSecond",       "Gbit/s",          9),
            new ("BitPerSecond",       "Gb/s",            9),
            new ("BitPerSecond",       "Gbps",            9),
            new ("BitPerSecond",       "Tbit/s",         12),
            new ("BitPerSecond",       "Tb/s",           12),
            new ("BitPerSecond",       "Tbps",           12),

            new ("BytePerSecond",      "Byte/s",          0),
            new ("BytePerSecond",      "B/s",             0),
            new ("BytePerSecond",      "Bps",             0),
            new ("BytePerSecond",      "kByte/s",         3),
            new ("BytePerSecond",      "kB/s",            3),
            new ("BytePerSecond",      "kBps",            3),
            new ("BytePerSecond",      "MByte/s",         6),
            new ("BytePerSecond",      "MB/s",            6),
            new ("BytePerSecond",      "MBps",            6),
            new ("BytePerSecond",      "GByte/s",         9),
            new ("BytePerSecond",      "GB/s",            9),
            new ("BytePerSecond",      "GBps",            9),
            new ("BytePerSecond",      "TByte/s",        12),
            new ("BytePerSecond",      "TB/s",           12),
            new ("BytePerSecond",      "TBps",           12),

            new ("Celsius",            "°C",              0),

            new ("Farad",              "F",               0),
            new ("Farad",              "µF",             -6),
            new ("Farad",              "nF",             -9),
            new ("Farad",              "pF",            -12),

            new ("Henry",              "H",               0),
            new ("Henry",              "kH",              3),
            new ("Henry",              "mH",             -3),
            new ("Henry",              "µH",             -6),
            new ("Henry",              "nH",             -9),
            new ("Henry",              "pH",            -12),

            new ("Hertz",              "Hz",              0),
            new ("Hertz",              "kHz",             3),
            new ("Hertz",              "MHz",             6),
            new ("Hertz",              "GHz",             9),

            new ("Kelvin",             "K",               0),

            new ("Kilogram",           "kg",              0),
            new ("Kilogram",           "g",              -3),

            new ("Meter",              "m",               0),
            new ("Meter",              "mm",             -3),
            new ("Meter",              "cm",             -2),
            new ("Meter",              "dm",             -1),
            new ("Meter",              "km",              3),

            new ("Ohm",                "Ω",               0),
            new ("Ohm",                "µΩ",             -6),
            new ("Ohm",                "mΩ",             -3),
            new ("Ohm",                "kΩ",              3),
            new ("Ohm",                "MΩ",              6),

            new ("Siemens",            "S",               0),
            new ("Siemens",            "kS",              3),

            new ("Tonne",              "t",               0),
            new ("Tonne",              "kt",              3),
            new ("Tonne",              "kT",              3),   // both spellings accepted on purpose

            new ("Volt",               "V",               0),
            new ("Volt",               "kV",              3),

            new ("VoltAmpere",         "VA",              0),
            new ("VoltAmpere",         "kVA",             3),

            new ("VoltAmpereReactive", "VAr",             0),
            new ("VoltAmpereReactive", "kVAr",            3),

            new ("Watt",               "W",               0),
            new ("Watt",               "kW",              3),
            new ("Watt",               "MW",              6),
            new ("Watt",               "GW",              9),

            new ("WattHour",           "Wh",              0),
            new ("WattHour",           "kWh",             3),
            new ("WattHour",           "MWh",             6),
            new ("WattHour",           "GWh",             9)

        ];

        #endregion


        #region (private static) TypeOf     (TypeName)

        /// <summary>
        /// Resolve a metrology struct by its unqualified name.
        /// </summary>
        /// <param name="TypeName">The name of a metrology struct.</param>
        private static Type TypeOf(String TypeName)

            => typeof(Watt).Assembly.GetType($"org.GraphDefined.Vanaheimr.Illias.{TypeName}")
                   ?? throw new InvalidOperationException($"There is no metrology type named '{TypeName}'!");

        #endregion

        #region (private static) ValueOf    (Instance)

        /// <summary>
        /// Read the stored value of a metrology struct. Meter is the one type
        /// of the family that calls its backing property 'm' instead of 'Value'.
        /// </summary>
        /// <param name="Instance">An instance of a metrology struct.</param>
        private static Decimal ValueOf(Object Instance)
        {

            var type      = Instance.GetType();

            var property  = type.GetProperty("Value") ??
                            type.GetProperty("m")     ??
                            throw new InvalidOperationException($"{type.Name} has neither a 'Value' nor an 'm' property!");

            return (Decimal) property.GetValue(Instance)!;

        }

        #endregion

        #region (private static) Pow10      (Exponent)

        /// <summary>
        /// Ten to the power of a non-negative exponent, computed on Decimal so
        /// that this test does not depend on the production helper it verifies.
        /// </summary>
        /// <param name="Exponent">A non-negative exponent.</param>
        private static Decimal Pow10(Int32 Exponent)
        {

            var result = 1m;

            for (var i = 0; i < Exponent; i++)
                result *= 10m;

            return result;

        }

        #endregion

        #region (private static) Scale      (Number, Exponent)

        /// <summary>
        /// Scale a number by ten to the power of the given exponent.
        /// </summary>
        /// <param name="Number">A number.</param>
        /// <param name="Exponent">An exponent, positive or negative.</param>
        private static Decimal Scale(Decimal  Number,
                                     Int32    Exponent)

            => Exponent >= 0
                   ? Number * Pow10( Exponent)
                   : Number / Pow10(-Exponent);

        #endregion

        #region (private static) MethodOf   (Type, Name, ParameterCount)

        /// <summary>
        /// Find a public static method by name and parameter count, closed over
        /// Decimal when it is generic.
        /// </summary>
        /// <param name="Type">A metrology struct.</param>
        /// <param name="Name">The name of the method.</param>
        /// <param name="ParameterCount">The number of parameters.</param>
        private static MethodInfo? MethodOf(Type    Type,
                                            String  Name,
                                            Int32   ParameterCount)
        {

            var method = Type.GetMethods(BindingFlags.Public | BindingFlags.Static).
                              FirstOrDefault(m => m.Name           == Name &&
                                                  m.GetParameters().Length == ParameterCount);

            return method is null
                       ? null
                       : method.IsGenericMethodDefinition
                             ? method.MakeGenericMethod(typeof(Decimal))
                             : method;

        }

        #endregion

        #region (private static) BaseUnitOf (TypeName)

        /// <summary>
        /// The prefixed unit of a type whose exponent is zero, which is the
        /// entry point used to build a value at any other prefix.
        /// </summary>
        /// <param name="TypeName">The name of a metrology struct.</param>
        private static PrefixedUnit BaseUnitOf(String TypeName)

            => prefixedUnits.First(unit => unit.TypeName == TypeName &&
                                           unit.Exponent == 0);

        #endregion

        #region (private static) BaseValue  (TypeName, Number)

        /// <summary>
        /// Build an instance of the given type through its unprefixed factory.
        /// </summary>
        /// <param name="TypeName">The name of a metrology struct.</param>
        /// <param name="Number">The value in the base unit.</param>
        private static Object BaseValue(String   TypeName,
                                        Decimal  Number)
        {

            var type     = TypeOf(TypeName);
            var factory  = MethodOf(type, $"From{BaseUnitOf(TypeName).NameSuffix}", 2)
                               ?? throw new InvalidOperationException($"{TypeName} has no unprefixed factory!");

            return factory.Invoke(null, [Number, null])!;

        }

        #endregion

        #region (private static) TypeNames

        /// <summary>
        /// The names of all metrology structs covered by the tables.
        /// </summary>
        private static IEnumerable<String> TypeNames

            => prefixedUnits.Select(unit => unit.TypeName).Distinct();

        #endregion


        #region Every_factory_applies_the_exponent_its_name_promises()

        /// <summary>
        /// FromKW must multiply by a thousand, FromMH must divide by one - the
        /// classic defect of this family is a factory that carries the name of
        /// one prefix and the arithmetic of another, which goes unnoticed
        /// because nothing about the result looks wrong.
        /// </summary>
        [Test]
        public void Every_factory_applies_the_exponent_its_name_promises()
        {

            foreach (var unit in prefixedUnits)
            {

                var name      = $"From{unit.NameSuffix}";
                var factory   = MethodOf(TypeOf(unit.TypeName), name, 2);

                Assert.That(factory,        Is.Not.Null,
                            $"{unit.TypeName}.{name} is missing!");

                var result    = factory.Invoke(null, [5m, null])!;

                Assert.That(ValueOf(result), Is.EqualTo(Scale(5m, unit.Exponent)),
                            $"{unit.TypeName}.{name}(5) must be 5e{unit.Exponent} in the base unit!");


                // The optional exponent parameter must add to the one of the name...
                var shifted   = factory.Invoke(null, [5m, 2])!;

                Assert.That(ValueOf(shifted), Is.EqualTo(Scale(5m, unit.Exponent + 2)),
                            $"{unit.TypeName}.{name}(5, 2) must add its exponent to the one of the name!");

            }

        }

        #endregion

        #region Every_optional_factory_agrees_with_its_mandatory_twin()

        /// <summary>
        /// TryFromKW exists twice, once returning a nullable and once with an
        /// out parameter, and both must land on the same value as FromKW. A
        /// Try method that reports success without producing a result is a
        /// defect this family has had before.
        /// </summary>
        [Test]
        public void Every_optional_factory_agrees_with_its_mandatory_twin()
        {

            foreach (var unit in prefixedUnits)
            {

                var type       = TypeOf(unit.TypeName);
                var expected   = Scale(5m, unit.Exponent);

                var nullable   = MethodOf(type, $"TryFrom{unit.NameSuffix}", 2);
                Assert.That(nullable, Is.Not.Null,
                            $"{unit.TypeName}.TryFrom{unit.NameSuffix}(Number, Exponent) is missing!");

                var returned   = nullable.Invoke(null, [5m, null]);
                Assert.That(returned, Is.Not.Null,
                            $"{unit.TypeName}.TryFrom{unit.NameSuffix}(5) must not return null!");
                Assert.That(ValueOf(returned), Is.EqualTo(expected),
                            $"{unit.TypeName}.TryFrom{unit.NameSuffix}(5) must agree with From{unit.NameSuffix}(5)!");


                var withOut    = MethodOf(type, $"TryFrom{unit.NameSuffix}", 3);
                Assert.That(withOut, Is.Not.Null,
                            $"{unit.TypeName}.TryFrom{unit.NameSuffix}(Number, out {unit.TypeName}, Exponent) is missing!");

                var arguments  = new Object?[] { 5m, null, null };
                Assert.That(withOut.Invoke(null, arguments), Is.True,
                            $"{unit.TypeName}.TryFrom{unit.NameSuffix}(5, out _) must succeed!");
                Assert.That(ValueOf(arguments[1]!), Is.EqualTo(expected),
                            $"{unit.TypeName}.TryFrom{unit.NameSuffix}(5, out _) must produce the value it reported success for!");

            }

        }

        #endregion

        #region Every_conversion_property_undoes_its_own_prefix()

        /// <summary>
        /// A value of five kilowatts must report 5 through its kW property.
        /// The property and the factory of a prefix are written independently
        /// - one divides, the other multiplies - so they can disagree.
        /// </summary>
        [Test]
        public void Every_conversion_property_undoes_its_own_prefix()
        {

            foreach (var conversion in conversions)
            {

                var instance  = BaseValue(conversion.TypeName, Scale(5m, conversion.Exponent));
                var property  = TypeOf(conversion.TypeName).GetProperty(conversion.Property);

                Assert.That(property, Is.Not.Null,
                            $"{conversion.TypeName}.{conversion.Property} is missing!");

                Assert.That((Decimal) property.GetValue(instance)!, Is.EqualTo(5m),
                            $"{conversion.TypeName}.{conversion.Property} of 5e{conversion.Exponent} base units must be 5!");

            }

        }

        #endregion

        #region Every_prefix_parser_agrees_with_its_factory()

        /// <summary>
        /// ParseKW("5") takes a bare number and applies the prefix of its own
        /// name, exactly as FromKW(5) does. None of these roughly 190 methods
        /// was called by a test before.
        /// </summary>
        [Test]
        public void Every_prefix_parser_agrees_with_its_factory()
        {

            foreach (var unit in prefixedUnits)
            {

                var type      = TypeOf(unit.TypeName);
                var expected  = Scale(5m, unit.Exponent);

                var parse     = MethodOf(type, $"Parse{unit.NameSuffix}", 1);
                Assert.That(parse, Is.Not.Null,
                            $"{unit.TypeName}.Parse{unit.NameSuffix}(Text) is missing!");
                Assert.That(ValueOf(parse.Invoke(null, ["5"])!), Is.EqualTo(expected),
                            $"{unit.TypeName}.Parse{unit.NameSuffix}(\"5\") must agree with From{unit.NameSuffix}(5)!");

                var tryParse  = MethodOf(type, $"TryParse{unit.NameSuffix}", 2);
                Assert.That(tryParse, Is.Not.Null,
                            $"{unit.TypeName}.TryParse{unit.NameSuffix}(Text, out {unit.TypeName}) is missing!");

                var arguments = new Object?[] { "5", null };
                Assert.That(tryParse.Invoke(null, arguments), Is.True,
                            $"{unit.TypeName}.TryParse{unit.NameSuffix}(\"5\", out _) must succeed!");
                Assert.That(ValueOf(arguments[1]!), Is.EqualTo(expected),
                            $"{unit.TypeName}.TryParse{unit.NameSuffix}(\"5\", out _) must produce the value it reported success for!");

            }

        }

        #endregion

        #region Every_text_suffix_is_stripped_at_its_own_length()

        /// <summary>
        /// The parsers cut a fixed number of characters off the end. "kOhm" is
        /// four characters long, and a branch that cuts three leaves an 'O'
        /// behind that no number parser will accept - such a branch is dead
        /// code that reports failure for perfectly valid input.
        /// </summary>
        [Test]
        public void Every_text_suffix_is_stripped_at_its_own_length()
        {

            foreach (var suffix in textSuffixes)
            {

                var parse     = MethodOf(TypeOf(suffix.TypeName), "Parse", 1);
                Assert.That(parse, Is.Not.Null,
                            $"{suffix.TypeName}.Parse(Text) is missing!");

                var expected  = Scale(5m, suffix.Exponent);

                foreach (var text in new[] { $"5 {suffix.Suffix}",
                                             $"5{suffix.Suffix}",
                                             $"  5   {suffix.Suffix}  " })
                {
                    Assert.That(ValueOf(parse.Invoke(null, [text])!), Is.EqualTo(expected),
                                $"{suffix.TypeName}.Parse(\"{text}\") must be 5e{suffix.Exponent} in the base unit!");
                }

            }

        }

        #endregion

        #region Every_format_specifier_selects_its_own_prefix()

        /// <summary>
        /// ToString("kW") must render the value in kilowatts. The general
        /// specifier "G" is compared case-insensitively by nearly every type,
        /// so a unit symbol differing from it only in case - the gram against
        /// the general format - is shadowed and never reached.
        /// </summary>
        [Test]
        public void Every_format_specifier_selects_its_own_prefix()
        {

            foreach (var spec in formatSpecs)
            {

                var instance  = BaseValue(spec.TypeName, Scale(5m, spec.Exponent));
                var formatted = ((IFormattable) instance).ToString(spec.Specifier, CultureInfo.InvariantCulture);

                // Decimal keeps its scale, so five microfarad legitimately reads
                // "5.000000 µF" - the number has to be compared, not the text.
                Assert.That(Decimal.TryParse(formatted.Split(' ')[0],
                                             NumberStyles.Number,
                                             CultureInfo.InvariantCulture,
                                             out var rendered), Is.True,
                            $"{spec.TypeName}.ToString(\"{spec.Specifier}\") must start with a number, but was '{formatted}'!");

                Assert.That(rendered, Is.EqualTo(5m),
                            $"{spec.TypeName}.ToString(\"{spec.Specifier}\") of 5e{spec.Exponent} base units must render the number as 5, but was '{formatted}'!");

            }

        }

        #endregion

        #region TryFormat_agrees_with_ToString_and_refuses_a_short_buffer()

        /// <summary>
        /// ToString delegates to TryFormat and falls back to its own branches
        /// when the buffer is too small, so the two can drift apart. And a
        /// TryFormat that writes past a short buffer, or claims to have
        /// written into one, is a memory defect rather than a cosmetic one.
        /// </summary>
        [Test]
        public void TryFormat_agrees_with_ToString_and_refuses_a_short_buffer()
        {

            Span<Char> big       = stackalloc Char[64];
            Span<Char> tooSmall  = stackalloc Char[1];

            foreach (var spec in formatSpecs)
            {

                var instance   = (ISpanFormattable) BaseValue(spec.TypeName, Scale(5m, spec.Exponent));
                var expected   = ((IFormattable) instance).ToString(spec.Specifier, CultureInfo.InvariantCulture);

                Assert.That(instance.TryFormat(big, out var written, spec.Specifier, CultureInfo.InvariantCulture), Is.True,
                            $"{spec.TypeName}.TryFormat(\"{spec.Specifier}\") must succeed on a 64 character buffer!");
                Assert.That(new String(big[..written]), Is.EqualTo(expected),
                            $"{spec.TypeName}.TryFormat(\"{spec.Specifier}\") must agree with ToString!");

                Assert.That(instance.TryFormat(tooSmall, out var none, spec.Specifier, CultureInfo.InvariantCulture), Is.False,
                            $"{spec.TypeName}.TryFormat(\"{spec.Specifier}\") must refuse a single character buffer!");
                Assert.That(none, Is.Zero,
                            $"{spec.TypeName}.TryFormat(\"{spec.Specifier}\") must report zero characters written when it fails!");

            }

        }

        #endregion

        #region (private static) AssertBothParserPathsAgree<T>(TypeName)

        /// <summary>
        /// Feed every suffix of a type through both of its parsers. The span
        /// based and the string based implementation are written out twice and
        /// do drift apart - one member of this family once accepted "kByte/s"
        /// in the one and "kBit/s" in the other.
        /// </summary>
        /// <param name="TypeName">The name of the metrology struct under test.</param>
        private static void AssertBothParserPathsAgree<T>(String TypeName)

            where T : IMetrology<T>

        {

            foreach (var suffix in textSuffixes.Where(suffix => suffix.TypeName == TypeName))
            {

                foreach (var number in new[] { "5", "0.5", "1.10" })
                {

                    var text        = $"{number} {suffix.Suffix}";

                    var fromString  = T.Parse(text,          CultureInfo.InvariantCulture);
                    var fromSpan    = T.Parse(text.AsSpan(), CultureInfo.InvariantCulture);

                    Assert.That(fromSpan, Is.EqualTo(fromString),
                                $"{TypeName}: the span parser and the string parser disagree on '{text}'!");

                    Assert.That(T.TryParse(text,          CultureInfo.InvariantCulture, out var triedString), Is.True,
                                $"{TypeName}.TryParse(String) must accept '{text}'!");
                    Assert.That(T.TryParse(text.AsSpan(), CultureInfo.InvariantCulture, out var triedSpan),   Is.True,
                                $"{TypeName}.TryParse(Span) must accept '{text}'!");

                    Assert.That(triedSpan,   Is.EqualTo(triedString),
                                $"{TypeName}: the two TryParse implementations disagree on '{text}'!");
                    Assert.That(triedString, Is.EqualTo(fromString),
                                $"{TypeName}: TryParse and Parse disagree on '{text}'!");

                }

                // Whatever the suffix, text that is not a number must be refused by both
                var nonsense = $"x {suffix.Suffix}";

                Assert.That(T.TryParse(nonsense,          CultureInfo.InvariantCulture, out _), Is.False,
                            $"{TypeName}.TryParse(String) must refuse '{nonsense}'!");
                Assert.That(T.TryParse(nonsense.AsSpan(), CultureInfo.InvariantCulture, out _), Is.False,
                            $"{TypeName}.TryParse(Span) must refuse '{nonsense}'!");

            }

        }

        #endregion

        #region The_span_parser_agrees_with_the_string_parser()

        /// <summary>
        /// Both parser paths of every type, against every suffix that type
        /// accepts. Neither path was exercised for any unit struct before.
        /// </summary>
        [Test]
        public void The_span_parser_agrees_with_the_string_parser()
        {

            AssertBothParserPathsAgree<Ampere>            ("Ampere");
            AssertBothParserPathsAgree<BitPerSecond>      ("BitPerSecond");
            AssertBothParserPathsAgree<BytePerSecond>     ("BytePerSecond");
            AssertBothParserPathsAgree<Celsius>           ("Celsius");
            AssertBothParserPathsAgree<Farad>             ("Farad");
            AssertBothParserPathsAgree<Henry>             ("Henry");
            AssertBothParserPathsAgree<Hertz>             ("Hertz");
            AssertBothParserPathsAgree<Kelvin>            ("Kelvin");
            AssertBothParserPathsAgree<Kilogram>          ("Kilogram");
            AssertBothParserPathsAgree<Meter>             ("Meter");
            AssertBothParserPathsAgree<Ohm>               ("Ohm");
            AssertBothParserPathsAgree<Siemens>           ("Siemens");
            AssertBothParserPathsAgree<Tonne>             ("Tonne");
            AssertBothParserPathsAgree<Volt>              ("Volt");
            AssertBothParserPathsAgree<VoltAmpere>        ("VoltAmpere");
            AssertBothParserPathsAgree<VoltAmpereReactive>("VoltAmpereReactive");
            AssertBothParserPathsAgree<Watt>              ("Watt");
            AssertBothParserPathsAgree<WattHour>          ("WattHour");

        }

        #endregion

        #region A_failed_parse_throws_the_same_exception_across_the_family()

        /// <summary>
        /// Parse throws when the text is not a valid representation, and .NET
        /// spells that FormatException. A caller who writes one catch clause
        /// around a metrology parser must not have it work for the Watt and
        /// slip through for the Meter.
        /// </summary>
        [Test]
        public void A_failed_parse_throws_the_same_exception_across_the_family()
        {

            foreach (var unit in prefixedUnits)
            {

                var type   = TypeOf(unit.TypeName);

                foreach (var name in new[] { "Parse", $"Parse{unit.NameSuffix}" })
                {

                    var parse  = MethodOf(type, name, 1);
                    Assert.That(parse, Is.Not.Null, $"{unit.TypeName}.{name}(Text) is missing!");

                    var thrown = Assert.Catch(() => parse.Invoke(null, ["not a number"]))!;

                    Assert.That(thrown.InnerException ?? thrown, Is.TypeOf<FormatException>(),
                                $"{unit.TypeName}.{name}(\"not a number\") must throw a FormatException!");

                }

            }

        }

        #endregion

        #region The_tables_describe_every_member_of_the_family()

        /// <summary>
        /// The completeness net. A unit struct copied from the template brings
        /// its whole surface along, and a member that no table mentions is a
        /// member no test above has looked at.
        /// </summary>
        [Test]
        public void The_tables_describe_every_member_of_the_family()
        {

            foreach (var typeName in TypeNames)
            {

                var type      = TypeOf(typeName);
                var suffixes  = prefixedUnits.Where(unit => unit.TypeName == typeName).
                                              Select(unit => unit.NameSuffix).
                                              ToHashSet();

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {

                    // A factory that is not generic over its number type converts from
                    // another unit instead of from a number. The family has two of those,
                    // Kilogram.FromTonne and Tonne.FromKilogram, and they belong to
                    // The_bridges_between_two_units_convert_in_both_directions() rather
                    // than to the prefix table - but they must be bridges, not strays.
                    if (method.Name.StartsWith("From", StringComparison.Ordinal) &&
                       !method.IsGenericMethodDefinition)
                    {

                        var parameter = method.GetParameters().SingleOrDefault();

                        Assert.That(parameter?.ParameterType.GetInterfaces().
                                        Any(@interface => @interface.IsGenericType &&
                                                          @interface.GetGenericTypeDefinition() == typeof(IMetrology<>)),
                                    Is.True,
                                    $"{typeName}.{method.Name} is neither a prefixed factory nor a bridge from another unit!");

                        continue;

                    }

                    var covered = method.Name switch {

                        "Parse" or "TryParse"                        => true,

                        var name when name.StartsWith("TryFrom",  StringComparison.Ordinal)
                            => suffixes.Contains(name["TryFrom".Length..]),

                        var name when name.StartsWith("From",     StringComparison.Ordinal)
                            => suffixes.Contains(name["From".Length..]),

                        var name when name.StartsWith("TryParse", StringComparison.Ordinal)
                            => suffixes.Contains(name["TryParse".Length..]),

                        var name when name.StartsWith("Parse",    StringComparison.Ordinal)
                            => suffixes.Contains(name["Parse".Length..]),

                        _   => true

                    };

                    Assert.That(covered, Is.True,
                                $"{typeName}.{method.Name} is not described by the prefixedUnits table!");

                }


                var described  = conversions.Where(conversion => conversion.TypeName == typeName).
                                             Select(conversion => conversion.Property).
                                             ToHashSet();

                var backing    = type.GetProperty("Value") is not null ? "Value" : "m";

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).
                                              Where(property => property.PropertyType == typeof(Decimal) &&
                                                                property.Name        != backing))
                {

                    Assert.That(described.Contains(property.Name), Is.True,
                                $"{typeName}.{property.Name} is not described by the conversions table!");

                }

            }

        }

        #endregion

    }

}
