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

using org.GraphDefined.Vanaheimr.Illias.Geometry.Maths;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Geometry
{

    /// <summary>
    /// The contract of IMaths&lt;T&gt;, checked against each of its five
    /// implementations.
    ///
    /// The whole geometry of this namespace - Pixel, Voxel, Line1D, Line2D,
    /// Circle, Rectangle, Quadtree, Octree - is generic over T and computes
    /// exclusively through IMaths&lt;T&gt;. The five implementations are copies
    /// of one template, four of them 365 lines and the fifth 358, and the
    /// places where they legitimately differ are exactly the places where a
    /// copy goes wrong: a square root has to round for integers, an infinity
    /// becomes a MinValue, an absolute value is pointless for unsigned types.
    ///
    /// These tests therefore assert algebraic laws rather than a table of
    /// expected numbers. A law needs no external oracle - doubling must equal
    /// adding to itself, a square root must undo a square, and a distance must
    /// be symmetric - and it holds for every numeric type, so one helper can
    /// interrogate all five.
    /// </summary>
    [TestFixture]
    public class MathsContractTests
    {

        #region (private static) AssertCommonLaws  <T>(Of)

        /// <summary>
        /// The laws that hold for every numeric type of the family.
        /// </summary>
        /// <param name="Of">How to turn a small integer into a T.</param>
        private static void AssertCommonLaws<T>(Func<Int32, T> Of)

            where T : IEquatable<T>, IComparable<T>, IComparable

        {

            var math   = MathsFactory<T>.Instance;
            var name   = typeof(T).Name;

            var two    = Of(2);
            var three  = Of(3);
            var four   = Of(4);
            var six    = Of(6);


            // Zero is what it says and leaves an addition alone...
            Assert.That(math.Zero,                        Is.EqualTo(Of(0)),  $"{name}: Zero must be zero!");
            Assert.That(math.Add(three, math.Zero),       Is.EqualTo(three),  $"{name}: Zero must be the additive identity!");

            // ... and the empty fold of an operation is its neutral element:
            // the empty sum is zero, the empty product is one.
            Assert.That(math.Add(),                       Is.EqualTo(Of(0)),  $"{name}: the sum of nothing must be zero!");
            Assert.That(math.Mul(),                       Is.EqualTo(Of(1)),  $"{name}: the product of nothing must be one!");

            // A single operand passes straight through...
            Assert.That(math.Add(three),                  Is.EqualTo(three),  $"{name}: Add of one summand must return it!");
            Assert.That(math.Mul(three),                  Is.EqualTo(three),  $"{name}: Mul of one factor must return it!");
            Assert.That(math.Min(three),                  Is.EqualTo(three),  $"{name}: Min of one value must return it!");
            Assert.That(math.Max(three),                  Is.EqualTo(three),  $"{name}: Max of one value must return it!");

            // ... and every further one must be folded in, not dropped
            Assert.That(math.Add(Of(1), two, three),      Is.EqualTo(six),    $"{name}: Add must fold every summand!");
            Assert.That(math.Mul(Of(1), two, three),      Is.EqualTo(six),    $"{name}: Mul must fold every factor!");

            // Doubling and halving are an addition and a division in disguise
            Assert.That(math.Mul2(three),                 Is.EqualTo(math.Add(three, three)),
                                                                              $"{name}: Mul2 must equal adding a value to itself!");
            Assert.That(math.Div2(math.Mul2(three)),      Is.EqualTo(three),  $"{name}: Div2 must undo Mul2!");

            // Subtraction and division undo their counterparts
            Assert.That(math.Sub(six, two),               Is.EqualTo(four),   $"{name}: Sub must subtract!");
            Assert.That(math.Div(six, two),               Is.EqualTo(three),  $"{name}: Div must divide!");
            Assert.That(math.Div(math.Mul(three, two), two), Is.EqualTo(three),
                                                                              $"{name}: Div must undo Mul!");

            // A square and a square root undo each other, and squaring is a power of two
            Assert.That(math.Pow(three, two),             Is.EqualTo(math.Mul(three, three)),
                                                                              $"{name}: Pow(a, 2) must equal a * a!");
            Assert.That(math.Sqrt(math.Mul(three, three)), Is.EqualTo(three), $"{name}: Sqrt must undo a square!");

            // Min and Max pick their own end of the range
            Assert.That(math.Min(four, two, six),         Is.EqualTo(two),    $"{name}: Min must return the smallest value!");
            Assert.That(math.Max(four, two, six),         Is.EqualTo(six),    $"{name}: Max must return the largest value!");

            // The two infinities bound every value - whether they are real
            // infinities or the MinValue/MaxValue standing in for them - and
            // are therefore neutral as the seed of a Min or a Max
            Assert.That(math.NegativeInfinity.CompareTo(three), Is.Negative,  $"{name}: NegativeInfinity must sort below an ordinary value!");
            Assert.That(math.PositiveInfinity.CompareTo(three), Is.Positive,  $"{name}: PositiveInfinity must sort above an ordinary value!");
            Assert.That(math.Min(three, math.PositiveInfinity), Is.EqualTo(three),
                                                                              $"{name}: PositiveInfinity must be neutral for Min!");
            Assert.That(math.Max(three, math.NegativeInfinity), Is.EqualTo(three),
                                                                              $"{name}: NegativeInfinity must be neutral for Max!");

            // An absolute value is never below zero
            Assert.That(math.Abs(three).CompareTo(math.Zero), Is.Not.Negative, $"{name}: Abs must not return a negative value!");

            // A distance vanishes only between equals, and is symmetric - the
            // one law that no numeric type may bend, however it represents its
            // values. On an unsigned type the naive Abs(Sub(a, b)) breaks it.
            Assert.That(math.Distance(three, three),      Is.EqualTo(math.Zero),
                                                                              $"{name}: the distance of a value to itself must be zero!");
            Assert.That(math.Distance(six, two),          Is.EqualTo(four),   $"{name}: Distance must measure downwards!");
            Assert.That(math.Distance(two, six),          Is.EqualTo(four),   $"{name}: Distance must measure upwards just as far!");
            Assert.That(math.Distance(two, six),          Is.EqualTo(math.Distance(six, two)),
                                                                              $"{name}: a distance must be symmetric!");

            // Missing operands are an error, not an empty fold
            Assert.Throws<ArgumentException>(() => math.Add((T[]) null!), $"{name}: Add(null) must be refused!");
            Assert.Throws<ArgumentException>(() => math.Mul((T[]) null!), $"{name}: Mul(null) must be refused!");
            Assert.Throws<ArgumentException>(() => math.Min((T[]) null!), $"{name}: Min(null) must be refused!");
            Assert.Throws<ArgumentException>(() => math.Max((T[]) null!), $"{name}: Max(null) must be refused!");

        }

        #endregion

        #region (private static) AssertSignedLaws  <T>(Of)

        /// <summary>
        /// The laws that need a sign, and therefore hold for four of the five.
        /// </summary>
        /// <param name="Of">How to turn a small integer into a T.</param>
        private static void AssertSignedLaws<T>(Func<Int32, T> Of)

            where T : IEquatable<T>, IComparable<T>, IComparable

        {

            var math  = MathsFactory<T>.Instance;
            var name  = typeof(T).Name;

            // Inv is documented as "the inverse value of a: -a", so it negates
            Assert.That(math.Inv(Of(7)),             Is.EqualTo(Of(-7)),  $"{name}: Inv must negate!");
            Assert.That(math.Inv(math.Inv(Of(7))),   Is.EqualTo(Of(7)),   $"{name}: Inv applied twice must return the original value!");
            Assert.That(math.Inv(math.Zero),         Is.EqualTo(math.Zero), $"{name}: the negation of zero is zero!");

            Assert.That(math.Abs(Of(-4)),            Is.EqualTo(Of(4)),   $"{name}: Abs must drop the sign!");
            Assert.That(math.Sub(Of(3), Of(5)),      Is.EqualTo(Of(-2)),  $"{name}: Sub must be able to go below zero!");

            Assert.That(math.Distance(Of(-3), Of(2)), Is.EqualTo(Of(5)),  $"{name}: Distance must span the sign!");
            Assert.That(math.Distance(Of(2), Of(-3)), Is.EqualTo(Of(5)),  $"{name}: Distance must span the sign in both directions!");

        }

        #endregion


        #region Every_implementation_obeys_the_common_laws()

        /// <summary>
        /// The five implementations against the laws that bind all of them.
        /// </summary>
        [Test]
        public void Every_implementation_obeys_the_common_laws()
        {

            AssertCommonLaws<Double>(number =>           number);
            AssertCommonLaws<Single>(number =>           number);
            AssertCommonLaws<Int32> (number =>           number);
            AssertCommonLaws<Int64> (number =>           number);
            AssertCommonLaws<UInt32>(number => (UInt32)  number);

        }

        #endregion

        #region The_signed_implementations_negate_and_go_below_zero()

        /// <summary>
        /// Everything that presupposes a sign, for the four types that have one.
        /// </summary>
        [Test]
        public void The_signed_implementations_negate_and_go_below_zero()
        {

            AssertSignedLaws<Double>(number => number);
            AssertSignedLaws<Single>(number => number);
            AssertSignedLaws<Int32> (number => number);
            AssertSignedLaws<Int64> (number => number);

        }

        #endregion

        #region The_unsigned_implementation_says_so_instead_of_lying()

        /// <summary>
        /// UInt32 is the one implementation that cannot answer every question
        /// of the interface. Where it has no answer it has to say so - an
        /// operation that silently returns its input instead feeds a wrong
        /// number into the geometry built on top of it.
        /// </summary>
        [Test]
        public void The_unsigned_implementation_says_so_instead_of_lying()
        {

            var math = MathsFactory<UInt32>.Instance;

            // There is no negative UInt32, so the negation documented by the
            // interface has no result. Returning the input unchanged would make
            // Vector2D<UInt32> believe a vector is its own opposite.
            Assert.Throws<NotSupportedException>(() => math.Inv(7u),
                        "UInt32: Inv has no answer here and must not invent one!");

            // Every UInt32 already is its own magnitude
            Assert.That(math.Abs(7u), Is.EqualTo(7u),  "UInt32: Abs must be the identity!");
            Assert.That(math.Abs(0u), Is.EqualTo(0u),  "UInt32: Abs of zero must be zero!");

            // Sub wraps around on purpose - which is precisely why Distance
            // must not be built on top of it, and is checked separately below.
            Assert.That(math.Sub(3u, 5u), Is.EqualTo(4294967294u),
                        "UInt32: Sub is expected to wrap around, the type has nowhere else to go!");

            Assert.That(math.Distance(3u, 5u), Is.EqualTo(2u),
                        "UInt32: Distance must order its operands rather than inherit the wrap-around of Sub!");

            // The surrogate infinities of an unsigned type collapse onto its range
            Assert.That(math.NegativeInfinity, Is.EqualTo(UInt32.MinValue), "UInt32: NegativeInfinity must be the smallest value!");
            Assert.That(math.PositiveInfinity, Is.EqualTo(UInt32.MaxValue), "UInt32: PositiveInfinity must be the largest value!");

        }

        #endregion

        #region A_distance_is_symmetric_across_the_whole_range()

        /// <summary>
        /// The symmetry of a distance, walked over a range of pairs rather than
        /// asserted for a single one. This is the law that an unsigned
        /// subtraction breaks, and it breaks it only in one of the two
        /// directions - so a test that measures downwards alone sees nothing.
        /// </summary>
        [Test]
        public void A_distance_is_symmetric_across_the_whole_range()
        {

            var mathDouble  = MathsFactory<Double>.Instance;
            var mathInt32   = MathsFactory<Int32>. Instance;
            var mathUInt32  = MathsFactory<UInt32>.Instance;

            for (var a = 0; a <= 8; a++)
            {
                for (var b = 0; b <= 8; b++)
                {

                    var expected = Math.Abs(a - b);

                    Assert.That(mathDouble.Distance(a, b), Is.EqualTo((Double) expected),
                                $"Double: the distance between {a} and {b} must be {expected}!");
                    Assert.That(mathInt32. Distance(a, b), Is.EqualTo(expected),
                                $"Int32: the distance between {a} and {b} must be {expected}!");
                    Assert.That(mathUInt32.Distance((UInt32) a, (UInt32) b), Is.EqualTo((UInt32) expected),
                                $"UInt32: the distance between {a} and {b} must be {expected}!");

                }
            }

        }

        #endregion

        #region The_factory_serves_exactly_the_five_implemented_types()

        /// <summary>
        /// The factory dispatches on typeof(T) through a chain of comparisons,
        /// so a type it forgets falls off the end rather than failing to compile.
        /// </summary>
        [Test]
        public void The_factory_serves_exactly_the_five_implemented_types()
        {

            Assert.That(MathsFactory<Double>.Instance, Is.TypeOf<MathsDouble>(), "Double must be served by MathsDouble!");
            Assert.That(MathsFactory<Single>.Instance, Is.TypeOf<MathsSingle>(), "Single must be served by MathsSingle!");
            Assert.That(MathsFactory<Int32>. Instance, Is.TypeOf<MathsInt32>(),  "Int32 must be served by MathsInt32!");
            Assert.That(MathsFactory<Int64>. Instance, Is.TypeOf<MathsInt64>(),  "Int64 must be served by MathsInt64!");
            Assert.That(MathsFactory<UInt32>.Instance, Is.TypeOf<MathsUInt32>(), "UInt32 must be served by MathsUInt32!");

            // ... and a numeric type it does not implement is refused rather
            // than silently served with the arithmetic of another one
            Assert.Throws<Exception>(() => { var _ = MathsFactory<Decimal>.Instance; },
                        "An unimplemented numeric type must be refused!");

        }

        #endregion

    }

}
