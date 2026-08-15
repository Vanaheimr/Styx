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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The contract of IMetrology&lt;T&gt;, checked once per member of the family.
    ///
    /// Every unit struct declares the same eleven operators, and the whole set
    /// used to be untested: not one comparison, not one sum. An inverted '!='
    /// is invisible to the eye and to the compiler, has been found twice in
    /// this repository, and is caught here by a single line.
    ///
    /// The helpers are generic over IMetrology&lt;T&gt;, so a type that is added
    /// to the family and left out below fails to be checked - which is why
    /// The_family_is_checked_in_full() counts the call sites against the
    /// interface implementations found in the assembly.
    /// </summary>
    [TestFixture]
    public class MetrologyContractTests
    {

        #region (private static) AssertEqualityAndOrdering  (Smaller, Larger)

        /// <summary>
        /// Check that the equality and the comparison operators of a metrology
        /// struct do what their symbols say.
        /// </summary>
        /// <param name="Smaller">The smaller of two different values.</param>
        /// <param name="Larger">The larger of two different values.</param>
        private static void AssertEqualityAndOrdering<T>(T  Smaller,
                                                         T  Larger)

            where T : IMetrology<T>

        {

            var name  = typeof(T).Name;
            var same  = Smaller;

            // '==' must not negate, '!=' must...
            Assert.That(Smaller == same,               Is.True,     $"{name}: 'a == a' must hold!");
            Assert.That(Smaller != same,               Is.False,    $"{name}: 'operator !=' must negate 'operator =='!");
            Assert.That(Smaller == Larger,             Is.False,    $"{name}: two different values must not be equal!");
            Assert.That(Smaller != Larger,             Is.True,     $"{name}: two different values must be unequal!");

            // ... and every ordering operator must compare in its own direction
            Assert.That(Smaller <  Larger,             Is.True,     $"{name}: 'operator <' must compare upwards!");
            Assert.That(Larger  <  Smaller,            Is.False,    $"{name}: 'operator <' must not hold downwards!");
            Assert.That(Smaller >  Larger,             Is.False,    $"{name}: 'operator >' must not hold upwards!");
            Assert.That(Larger  >  Smaller,            Is.True,     $"{name}: 'operator >' must compare downwards!");

            Assert.That(Smaller <= Larger,             Is.True,     $"{name}: 'operator <=' must compare upwards!");
            Assert.That(Smaller <= same,               Is.True,     $"{name}: 'operator <=' must include equality!");
            Assert.That(Larger  <= Smaller,            Is.False,    $"{name}: 'operator <=' must not hold downwards!");
            Assert.That(Larger  >= Smaller,            Is.True,     $"{name}: 'operator >=' must compare downwards!");
            Assert.That(Smaller >= same,               Is.True,     $"{name}: 'operator >=' must include equality!");
            Assert.That(Smaller >= Larger,             Is.False,    $"{name}: 'operator >=' must not hold upwards!");

            // CompareTo must agree with the operators...
            Assert.That(Smaller.CompareTo(Larger),     Is.Negative, $"{name}: CompareTo must be negative towards a larger value!");
            Assert.That(Larger. CompareTo(Smaller),    Is.Positive, $"{name}: CompareTo must be positive towards a smaller value!");
            Assert.That(Smaller.CompareTo(same),       Is.Zero,     $"{name}: CompareTo must be zero towards an equal value!");

            // ... and so must Equals and GetHashCode
            Assert.That(Smaller.Equals(same),          Is.True,     $"{name}: Equals must accept an equal value!");
            Assert.That(Smaller.Equals(Larger),        Is.False,    $"{name}: Equals must reject a different value!");
            Assert.That(Smaller.Equals((Object) same), Is.True,     $"{name}: Equals(Object) must accept an equal value!");
            Assert.That(Smaller.Equals("not a unit"),  Is.False,    $"{name}: Equals(Object) must reject a foreign type!");
            Assert.That(Smaller.Equals(null),          Is.False,    $"{name}: Equals(Object) must reject null!");

            Assert.That(Smaller.GetHashCode(),         Is.EqualTo(same.GetHashCode()),
                                                                    $"{name}: equal values must share their hash code!");

            // The non-generic IComparable is the one place where a foreign type is an error
            Assert.That(((IComparable) Smaller).CompareTo(Larger), Is.Negative,
                                                                    $"{name}: IComparable.CompareTo must agree with IComparable<T>!");
            Assert.That(((IComparable) Smaller).CompareTo(null),   Is.Positive,
                                                                    $"{name}: IComparable.CompareTo(null) must sort a value after nothing!");

            Assert.Throws<ArgumentException>(() => ((IComparable) Smaller).CompareTo("not a unit"),
                                                                    $"{name}: IComparable.CompareTo must reject a foreign type!");

        }

        #endregion

        #region (private static) AssertArithmeticAndRoundTrip(Smaller, Larger, Value)

        /// <summary>
        /// Check that the arithmetic operators of a metrology struct compute on
        /// the stored value, that the additive identity is neutral, and that a
        /// value survives being written out and read back in.
        /// </summary>
        /// <param name="Smaller">The smaller of two different values.</param>
        /// <param name="Larger">The larger of two different values.</param>
        /// <param name="Value">How to read the stored value of this type.</param>
        private static void AssertArithmeticAndRoundTrip<T>(T                Smaller,
                                                            T                Larger,
                                                            Func<T, Decimal> Value)

            where T : IMetrology<T>

        {

            var name = typeof(T).Name;

            Assert.That(Value(Smaller + Larger),    Is.EqualTo(Value(Smaller) + Value(Larger)),
                        $"{name}: 'operator +' must add the values!");
            Assert.That(Value(Larger  - Smaller),   Is.EqualTo(Value(Larger)  - Value(Smaller)),
                        $"{name}: 'operator -' must subtract the values!");
            Assert.That(Value(Smaller * 3m),        Is.EqualTo(Value(Smaller) * 3m),
                        $"{name}: 'operator *' must scale the value!");
            Assert.That(Value(Smaller / 4m),        Is.EqualTo(Value(Smaller) / 4m),
                        $"{name}: 'operator /' must divide the value!");

            Assert.That(Value(T.AdditiveIdentity),  Is.Zero,
                        $"{name}: the additive identity must be zero!");
            Assert.That(Smaller + T.AdditiveIdentity, Is.EqualTo(Smaller),
                        $"{name}: adding the additive identity must change nothing!");

            // A value must be readable in the notation it writes - through both
            // parser paths, which are separate implementations and do drift apart.
            var text       = Smaller.ToString() ?? String.Empty;

            var fromString = T.Parse(text, CultureInfo.InvariantCulture);
            Assert.That(fromString, Is.EqualTo(Smaller),
                        $"{name}: Parse must read back what ToString wrote, but '{text}' did not survive!");

            var fromSpan   = T.Parse(text.AsSpan(), CultureInfo.InvariantCulture);
            Assert.That(fromSpan,   Is.EqualTo(Smaller),
                        $"{name}: the span parser must read back what ToString wrote, but '{text}' did not survive!");

            Assert.That(T.TryParse(text, CultureInfo.InvariantCulture, out var tried), Is.True,
                        $"{name}: TryParse must accept '{text}'!");
            Assert.That(tried, Is.EqualTo(Smaller),
                        $"{name}: TryParse must produce the value it reported success for!");

        }

        #endregion


        #region Equality_and_ordering_operators_compare_in_their_own_direction()

        /// <summary>
        /// The whole family at once: 278 operators are declared across these
        /// structs, and until now not a single one was exercised by a test.
        /// </summary>
        [Test]
        public void Equality_and_ordering_operators_compare_in_their_own_direction()
        {

            AssertEqualityAndOrdering(Ampere.            FromA  (100), Ampere.            FromA  (250));
            AssertEqualityAndOrdering(BitPerSecond.      FromBPS(100), BitPerSecond.      FromBPS(250));
            AssertEqualityAndOrdering(BytePerSecond.     FromBPS(100), BytePerSecond.     FromBPS(250));
            AssertEqualityAndOrdering(Celsius.           FromC  (100), Celsius.           FromC  (250));
            AssertEqualityAndOrdering(Farad.             FromF  (100), Farad.             FromF  (250));
            AssertEqualityAndOrdering(Henry.             FromH  (100), Henry.             FromH  (250));
            AssertEqualityAndOrdering(Hertz.             FromHz (100), Hertz.             FromHz (250));
            AssertEqualityAndOrdering(Kelvin.            FromK  (100), Kelvin.            FromK  (250));
            AssertEqualityAndOrdering(Kilogram.          FromKG (100), Kilogram.          FromKG (250));
            AssertEqualityAndOrdering(Meter.             From_m (100), Meter.             From_m (250));
            AssertEqualityAndOrdering(Ohm.               From_\u2126 (100), Ohm.               From_\u2126 (250));
            AssertEqualityAndOrdering(Siemens.           FromS  (100), Siemens.           FromS  (250));
            AssertEqualityAndOrdering(Tonne.             FromT  (100), Tonne.             FromT  (250));
            AssertEqualityAndOrdering(Volt.              FromV  (100), Volt.              FromV  (250));
            AssertEqualityAndOrdering(VoltAmpere.        FromVA (100), VoltAmpere.        FromVA (250));
            AssertEqualityAndOrdering(VoltAmpereReactive.FromVAr(100), VoltAmpereReactive.FromVAr(250));
            AssertEqualityAndOrdering(Watt.              FromW  (100), Watt.              FromW  (250));
            AssertEqualityAndOrdering(WattHour.          FromWh (100), WattHour.          FromWh (250));

        }

        #endregion

        #region Arithmetic_operators_compute_on_the_value_and_survive_a_round_trip()

        /// <summary>
        /// Addition, subtraction, scaling and division, the neutral element,
        /// and the journey from a value through its own text representation
        /// and back - through both the string and the span parser.
        /// </summary>
        [Test]
        public void Arithmetic_operators_compute_on_the_value_and_survive_a_round_trip()
        {

            AssertArithmeticAndRoundTrip(Ampere.            FromA  (100), Ampere.            FromA  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(BitPerSecond.      FromBPS(100), BitPerSecond.      FromBPS(250), value => value.Value);
            AssertArithmeticAndRoundTrip(BytePerSecond.     FromBPS(100), BytePerSecond.     FromBPS(250), value => value.Value);
            AssertArithmeticAndRoundTrip(Celsius.           FromC  (100), Celsius.           FromC  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Farad.             FromF  (100), Farad.             FromF  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Henry.             FromH  (100), Henry.             FromH  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Hertz.             FromHz (100), Hertz.             FromHz (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Kelvin.            FromK  (100), Kelvin.            FromK  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Kilogram.          FromKG (100), Kilogram.          FromKG (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Meter.             From_m (100), Meter.             From_m (250), value => value.m);
            AssertArithmeticAndRoundTrip(Ohm.               From_\u2126 (100), Ohm.               From_\u2126 (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Siemens.           FromS  (100), Siemens.           FromS  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Tonne.             FromT  (100), Tonne.             FromT  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(Volt.              FromV  (100), Volt.              FromV  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(VoltAmpere.        FromVA (100), VoltAmpere.        FromVA (250), value => value.Value);
            AssertArithmeticAndRoundTrip(VoltAmpereReactive.FromVAr(100), VoltAmpereReactive.FromVAr(250), value => value.Value);
            AssertArithmeticAndRoundTrip(Watt.              FromW  (100), Watt.              FromW  (250), value => value.Value);
            AssertArithmeticAndRoundTrip(WattHour.          FromWh (100), WattHour.          FromWh (250), value => value.Value);

        }

        #endregion

        #region Statistics_extensions_stay_in_the_base_unit()

        /// <summary>
        /// Sum, Avg and StdDev compute on the raw decimal and must feed the
        /// result back through a factory of exponent zero. A factory with a
        /// prefix here returns statistics scaled by a thousand, which looks
        /// entirely plausible until somebody bills it.
        /// </summary>
        [Test]
        public void Statistics_extensions_stay_in_the_base_unit()
        {

            // 100, 200, 300 -> sum 600, average 200, population deviation 81.649...
            Assert.That(new[] { Ampere.FromA  (100), Ampere.FromA  (200), Ampere.FromA  (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Ampere.FromA  (100), Ampere.FromA  (200), Ampere.FromA  (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Celsius.FromC (100), Celsius.FromC (200), Celsius.FromC (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Celsius.FromC (100), Celsius.FromC (200), Celsius.FromC (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Farad.FromF   (100), Farad.FromF   (200), Farad.FromF   (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Farad.FromF   (100), Farad.FromF   (200), Farad.FromF   (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Henry.FromH   (100), Henry.FromH   (200), Henry.FromH   (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Henry.FromH   (100), Henry.FromH   (200), Henry.FromH   (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Hertz.FromHz  (100), Hertz.FromHz  (200), Hertz.FromHz  (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Hertz.FromHz  (100), Hertz.FromHz  (200), Hertz.FromHz  (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Kelvin.FromK  (100), Kelvin.FromK  (200), Kelvin.FromK  (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Kelvin.FromK  (100), Kelvin.FromK  (200), Kelvin.FromK  (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Meter.From_m  (100), Meter.From_m  (200), Meter.From_m  (300) }.Sum().m,     Is.EqualTo(600m));
            Assert.That(new[] { Meter.From_m  (100), Meter.From_m  (200), Meter.From_m  (300) }.Avg().m,     Is.EqualTo(200m));

            Assert.That(new[] { Ohm.From_\u2126    (100), Ohm.From_\u2126    (200), Ohm.From_\u2126    (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Ohm.From_\u2126    (100), Ohm.From_\u2126    (200), Ohm.From_\u2126    (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Siemens.FromS (100), Siemens.FromS (200), Siemens.FromS (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Siemens.FromS (100), Siemens.FromS (200), Siemens.FromS (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Tonne.FromT   (100), Tonne.FromT   (200), Tonne.FromT   (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Tonne.FromT   (100), Tonne.FromT   (200), Tonne.FromT   (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Volt.FromV    (100), Volt.FromV    (200), Volt.FromV    (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Volt.FromV    (100), Volt.FromV    (200), Volt.FromV    (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { VoltAmpere.FromVA         (100), VoltAmpere.FromVA         (200), VoltAmpere.FromVA         (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { VoltAmpere.FromVA         (100), VoltAmpere.FromVA         (200), VoltAmpere.FromVA         (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { VoltAmpereReactive.FromVAr(100), VoltAmpereReactive.FromVAr(200), VoltAmpereReactive.FromVAr(300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { VoltAmpereReactive.FromVAr(100), VoltAmpereReactive.FromVAr(200), VoltAmpereReactive.FromVAr(300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { BitPerSecond.FromBPS      (100), BitPerSecond.FromBPS      (200), BitPerSecond.FromBPS      (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { BitPerSecond.FromBPS      (100), BitPerSecond.FromBPS      (200), BitPerSecond.FromBPS      (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { BytePerSecond.FromBPS     (100), BytePerSecond.FromBPS     (200), BytePerSecond.FromBPS     (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { BytePerSecond.FromBPS     (100), BytePerSecond.FromBPS     (200), BytePerSecond.FromBPS     (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Kilogram.FromKG(100), Kilogram.FromKG(200), Kilogram.FromKG(300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Kilogram.FromKG(100), Kilogram.FromKG(200), Kilogram.FromKG(300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { Watt.FromW    (100), Watt.FromW    (200), Watt.FromW    (300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { Watt.FromW    (100), Watt.FromW    (200), Watt.FromW    (300) }.Avg().Value, Is.EqualTo(200m));

            Assert.That(new[] { WattHour.FromWh(100), WattHour.FromWh(200), WattHour.FromWh(300) }.Sum().Value, Is.EqualTo(600m));
            Assert.That(new[] { WattHour.FromWh(100), WattHour.FromWh(200), WattHour.FromWh(300) }.Avg().Value, Is.EqualTo(200m));


            // The standard deviation carries both its mean and its spread in the base unit...
            var deviation = new[] { Watt.FromW(100), Watt.FromW(200), Watt.FromW(300) }.StdDev(IsSampleData: false);

            Assert.That(deviation.Mean.Value,               Is.EqualTo(200m));
            Assert.That(deviation.StandardDeviation.Value,  Is.EqualTo(81.649658092772603273242802490m).Within(0.000000001m));

            // ... and an empty sequence has no average to report
            Assert.Throws<InvalidOperationException>(() => Array.Empty<Watt>().Avg());

        }

        #endregion

        #region The_two_types_outside_IMetrology_follow_the_same_operator_laws()

        /// <summary>
        /// SquareMeter and QubicMeter carry the same operators but not the
        /// interface, so the generic helpers above cannot reach them and they
        /// would silently stay untested.
        /// </summary>
        [Test]
        public void The_two_types_outside_IMetrology_follow_the_same_operator_laws()
        {

            var smallArea  = SquareMeter.ParseSM("100");
            var largeArea  = SquareMeter.ParseSM("250");

            Assert.That(smallArea == smallArea,  Is.True,     "SquareMeter: 'a == a' must hold!");
            Assert.That(smallArea != smallArea,  Is.False,    "SquareMeter: 'operator !=' must negate 'operator =='!");
            Assert.That(smallArea != largeArea,  Is.True,     "SquareMeter: two different areas must be unequal!");
            Assert.That(smallArea <  largeArea,  Is.True,     "SquareMeter: 'operator <' must compare upwards!");
            Assert.That(largeArea >  smallArea,  Is.True,     "SquareMeter: 'operator >' must compare downwards!");
            Assert.That(smallArea <= smallArea,  Is.True,     "SquareMeter: 'operator <=' must include equality!");
            Assert.That(smallArea >= smallArea,  Is.True,     "SquareMeter: 'operator >=' must include equality!");
            Assert.That(smallArea.CompareTo(largeArea), Is.Negative, "SquareMeter: CompareTo must agree with the operators!");
            Assert.That((smallArea + largeArea).Value,  Is.EqualTo(350m), "SquareMeter: 'operator +' must add the values!");
            Assert.That((largeArea - smallArea).Value,  Is.EqualTo(150m), "SquareMeter: 'operator -' must subtract the values!");
            Assert.That(new[] { smallArea, largeArea }.Sum().Value, Is.EqualTo(350m), "SquareMeter: Sum must stay in square meters!");


            var smallVolume  = QubicMeter.ParseQM("100");
            var largeVolume  = QubicMeter.ParseQM("250");

            Assert.That(smallVolume == smallVolume, Is.True,  "QubicMeter: 'a == a' must hold!");
            Assert.That(smallVolume != smallVolume, Is.False, "QubicMeter: 'operator !=' must negate 'operator =='!");
            Assert.That(smallVolume != largeVolume, Is.True,  "QubicMeter: two different volumes must be unequal!");
            Assert.That(smallVolume <  largeVolume, Is.True,  "QubicMeter: 'operator <' must compare upwards!");
            Assert.That(largeVolume >  smallVolume, Is.True,  "QubicMeter: 'operator >' must compare downwards!");
            Assert.That(smallVolume <= smallVolume, Is.True,  "QubicMeter: 'operator <=' must include equality!");
            Assert.That(smallVolume >= smallVolume, Is.True,  "QubicMeter: 'operator >=' must include equality!");
            Assert.That(smallVolume.CompareTo(largeVolume), Is.Negative, "QubicMeter: CompareTo must agree with the operators!");
            Assert.That((smallVolume + largeVolume).Value,  Is.EqualTo(350m), "QubicMeter: 'operator +' must add the values!");
            Assert.That((largeVolume - smallVolume).Value,  Is.EqualTo(150m), "QubicMeter: 'operator -' must subtract the values!");
            Assert.That(new[] { smallVolume, largeVolume }.Sum().Value, Is.EqualTo(350m), "QubicMeter: Sum must stay in qubic meters!");


            // Their parsers must fail the same way as those of the rest of the family
            Assert.Throws<FormatException>(() => SquareMeter.Parse    ("not a number"), "SquareMeter.Parse must throw a FormatException!");
            Assert.Throws<FormatException>(() => SquareMeter.ParseSM  ("not a number"), "SquareMeter.ParseSM must throw a FormatException!");
            Assert.Throws<FormatException>(() => SquareMeter.ParseSKM ("not a number"), "SquareMeter.ParseSKM must throw a FormatException!");
            Assert.Throws<FormatException>(() => QubicMeter. Parse    ("not a number"), "QubicMeter.Parse must throw a FormatException!");
            Assert.Throws<FormatException>(() => QubicMeter. ParseQM  ("not a number"), "QubicMeter.ParseQM must throw a FormatException!");
            Assert.Throws<FormatException>(() => QubicMeter. ParseQKM ("not a number"), "QubicMeter.ParseQKM must throw a FormatException!");

            // ... while a foreign type stays an ArgumentException, as everywhere else
            Assert.Throws<ArgumentException>(() => ((IComparable) smallArea).  CompareTo("not an area!"),   "SquareMeter: CompareTo must reject a foreign type!");
            Assert.Throws<ArgumentException>(() => ((IComparable) smallVolume).CompareTo("not a volume!"),  "QubicMeter: CompareTo must reject a foreign type!");

        }

        #endregion

        #region The_bridges_between_two_units_convert_in_both_directions()

        /// <summary>
        /// The family has two factories that take another unit instead of a
        /// number. A tonne is a thousand kilograms in one direction and a
        /// thousandth in the other, which is exactly the kind of factor that
        /// gets copied in the wrong direction.
        /// </summary>
        [Test]
        public void The_bridges_between_two_units_convert_in_both_directions()
        {

            Assert.That(Kilogram.FromTonne   (Tonne.   FromT (2)).      Value, Is.EqualTo(2_000m),
                        "Two tonnes must be two thousand kilograms!");
            Assert.That(Tonne.   FromKilogram(Kilogram.FromKG(2_000)).  Value, Is.EqualTo(2m),
                        "Two thousand kilograms must be two tonnes!");

            // ... and back again, so neither direction can drift on its own
            var mass = Kilogram.FromKG(1_234.5m);
            Assert.That(Kilogram.FromTonne(Tonne.FromKilogram(mass)), Is.EqualTo(mass),
                        "A mass must survive the trip through the tonne and back!");

            var load = Tonne.FromT(7.5m);
            Assert.That(Tonne.FromKilogram(Kilogram.FromTonne(load)), Is.EqualTo(load),
                        "A load must survive the trip through the kilogram and back!");

        }

        #endregion

        #region The_family_is_checked_in_full()

        /// <summary>
        /// The generic helpers above are called by hand, once per type, so a
        /// unit struct copied into the family tomorrow would be checked by
        /// nothing at all. This counts the implementations of IMetrology&lt;T&gt;
        /// in the assembly against the number of call sites.
        /// </summary>
        [Test]
        public void The_family_is_checked_in_full()
        {

            var implementations = typeof(Watt).Assembly.
                                      GetTypes().
                                      Where(type => type.IsValueType &&
                                                    type.GetInterfaces().Any(@interface => @interface.IsGenericType &&
                                                                                           @interface.GetGenericTypeDefinition() == typeof(IMetrology<>))).
                                      Select(type => type.Name).
                                      OrderBy(name => name).
                                      ToArray();

            Assert.That(implementations, Is.EqualTo(new[] {
                            "Ampere",
                            "BitPerSecond",
                            "BytePerSecond",
                            "Celsius",
                            "Farad",
                            "Henry",
                            "Hertz",
                            "Kelvin",
                            "Kilogram",
                            "Meter",
                            "Ohm",
                            "Siemens",
                            "Tonne",
                            "Volt",
                            "VoltAmpere",
                            "VoltAmpereReactive",
                            "Watt",
                            "WattHour"
                        }),
                        "A type implementing IMetrology<T> was added or removed - add it to the checks of this fixture as well!");

        }

        #endregion

    }

}
