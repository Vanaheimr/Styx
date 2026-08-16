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

using org.GraphDefined.Vanaheimr.Illias.Geometry;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Geometry
{

    /// <summary>
    /// The arithmetic of the geometry types that no test covered.
    ///
    /// Pixel, Voxel, Cube, Rectangle and Quadtree have had tests for a long
    /// time; Line1D, Line2D, Circle, Sphere and Vector2D never did. When the
    /// IMaths&lt;T&gt; abstraction was dissolved into the generic maths of
    /// .NET, every expression in those five was rewritten by hand from
    /// Math.Add(Math.Mul(a, b), c) into a * b + c - and an algebra slip in
    /// such a rewrite produces a plausible wrong number, not a crash.
    ///
    /// The values below are computed by hand from figures chosen to make that
    /// possible: a 3-4-5 triangle, lines that meet where two integers do.
    /// </summary>
    [TestFixture]
    public class GeometryArithmeticTests
    {

        #region A_line_in_one_dimension_measures_its_own_length()

        /// <summary>
        /// Line1D.Length was Math.Distance(Left, Right) and is now T.Abs(Left - Right).
        /// </summary>
        [Test]
        public void A_line_in_one_dimension_measures_its_own_length()
        {

            Assert.That(new Line1D<Double>( 2.0,  7.0).Length, Is.EqualTo(5.0), "A line from 2 to 7 is five long!");
            Assert.That(new Line1D<Double>( 7.0,  2.0).Length, Is.EqualTo(5.0), "... and just as long measured backwards!");
            Assert.That(new Line1D<Double>(-3.0,  2.0).Length, Is.EqualTo(5.0), "... and across zero as well!");

        }

        #endregion

        #region A_line_in_two_dimensions_knows_its_gradient_intercept_and_middle()

        /// <summary>
        /// Gradient was Math.Div(Vector.Y, Vector.X), YIntercept was
        /// Math.Sub(Pixel1.Y, Math.Mul(Gradient, Pixel1.X)) and the centre was
        /// Math.Add(X1, Math.Div2(Math.Sub(X2, X1))) - three nested rewrites.
        /// </summary>
        [Test]
        public void A_line_in_two_dimensions_knows_its_gradient_intercept_and_middle()
        {

            // From (1, 3) to (5, 11): four to the right, eight up
            var line = new Line2D<Double>(1.0, 3.0, 5.0, 11.0);

            Assert.That(line.Gradient,   Is.EqualTo( 2.0), "Eight up over four across is a gradient of two!");
            Assert.That(line.YIntercept, Is.EqualTo( 1.0), "y = 2x + 1 passes through (1, 3), so it crosses at 1!");

            Assert.That(line.Center.X,   Is.EqualTo( 3.0), "The middle of 1 and 5 is 3!");
            Assert.That(line.Center.Y,   Is.EqualTo( 7.0), "The middle of 3 and 11 is 7!");

            // A 3-4-5 triangle, so the length is exact in binary floating point
            Assert.That(new Line2D<Double>(0.0, 0.0, 3.0, 4.0).Length, Is.EqualTo(5.0), "A 3-4-5 triangle has a hypotenuse of five!");

            // The line y = 2x + 1 contains (2, 5) and misses (2, 6)
            Assert.That(line.Contains(new Pixel<Double>(2.0, 5.0)), Is.True,  "The point (2, 5) lies on y = 2x + 1!");
            Assert.That(line.Contains(new Pixel<Double>(2.0, 6.0)), Is.False, "The point (2, 6) does not!");

        }

        #endregion

        #region Two_lines_meet_where_the_arithmetic_says_they_do()

        /// <summary>
        /// The intersection was the deepest nesting of the whole namespace,
        /// five Math calls inside one another per coordinate, and no test
        /// touched it. Two lines meeting at a point with integer coordinates
        /// make the answer checkable by eye.
        /// </summary>
        [Test]
        public void Two_lines_meet_where_the_arithmetic_says_they_do()
        {

            // y = x  and  y = -x + 4  meet at (2, 2)
            var rising   = new Line2D<Double>(0.0, 0.0, 4.0,  4.0);
            var falling  = new Line2D<Double>(0.0, 4.0, 4.0,  0.0);

            Assert.That(rising.IntersectsWith(falling, out var meeting, InfiniteLines: true), Is.True,
                        "Two lines of opposite gradient must meet!");
            Assert.That(meeting!.X, Is.EqualTo(2.0), "y = x and y = -x + 4 meet where x is 2!");
            Assert.That(meeting!.Y, Is.EqualTo(2.0), "... and where y is 2!");

            // y = 2x + 1  and  y = -x + 10  meet at (3, 7)
            var steep    = new Line2D<Double>(0.0, 1.0, 4.0,  9.0);
            var shallow  = new Line2D<Double>(0.0, 10.0, 10.0, 0.0);

            Assert.That(steep.IntersectsWith(shallow, out var crossing, InfiniteLines: true), Is.True,
                        "These two lines must meet as well!");
            Assert.That(crossing!.X, Is.EqualTo(3.0), "y = 2x + 1 and y = -x + 10 meet where x is 3!");
            Assert.That(crossing!.Y, Is.EqualTo(7.0), "... and where y is 7!");

            // Parallel lines never do - y = 2x + 5 runs alongside y = 2x + 1
            var parallel = new Line2D<Double>(0.0, 5.0, 4.0, 13.0);
            Assert.That(parallel.Gradient, Is.EqualTo(steep.Gradient), "The two lines must really be parallel!");
            Assert.That(steep.IntersectsWith(parallel, out _, InfiniteLines: true), Is.False,
                        "Parallel lines must not report an intersection!");

        }

        #endregion

        #region A_vector_adds_scales_and_measures()

        /// <summary>
        /// Vector2D carried 25 of the rewritten expressions, more than any
        /// other file: the components, the length, the normalisation, the four
        /// operators and the distance.
        /// </summary>
        [Test]
        public void A_vector_adds_scales_and_measures()
        {

            // A 3-4-5 triangle again, so the length is exact
            var vector = new Vector2D<Double>(3.0, 4.0);

            Assert.That(vector.X,      Is.EqualTo(3.0), "The x-component must survive!");
            Assert.That(vector.Y,      Is.EqualTo(4.0), "The y-component must survive!");
            Assert.That(vector.Length, Is.EqualTo(5.0), "A 3-4-5 vector is five long!");

            // A normalised vector keeps its direction and loses its magnitude
            Assert.That(vector.NormVector.X,      Is.EqualTo(0.6), "Three fifths!");
            Assert.That(vector.NormVector.Y,      Is.EqualTo(0.8), "Four fifths!");
            Assert.That(vector.NormVector.Length, Is.EqualTo(1.0).Within(1e-12), "A unit vector is one long!");

            // Built from two pixels it is their difference
            var fromPixels = new Vector2D<Double>(new Pixel<Double>(1.0, 2.0),
                                                  new Pixel<Double>(4.0, 6.0));

            Assert.That(fromPixels.X,      Is.EqualTo(-3.0), "From (1,2) to (4,6) is minus three across!");
            Assert.That(fromPixels.Y,      Is.EqualTo(-4.0), "... and minus four up!");
            Assert.That(fromPixels.Length, Is.EqualTo( 5.0), "... which is five long either way!");

            // The distance between two vectors is measured like any other distance
            Assert.That(new Vector2D<Double>(1.0, 2.0).DistanceTo(4.0, 6.0), Is.EqualTo(5.0),
                        "The distance from (1,2) to (4,6) is five!");

        }

        #endregion

        #region A_vector_obeys_the_axioms_of_a_vector_space()

        /// <summary>
        /// What a two-dimensional vector can do is not a matter of taste: the
        /// vector space axioms settle it. These are the operations the type
        /// carries because it is a vector, and the laws are what makes them
        /// checkable without an oracle.
        /// </summary>
        [Test]
        public void A_vector_obeys_the_axioms_of_a_vector_space()
        {

            var u = new Vector2D<Double>(3.0,  4.0);
            var v = new Vector2D<Double>(1.0, -2.0);
            var w = new Vector2D<Double>(5.0,  7.0);

            // Addition is commutative and associative...
            Assert.That(u + v,             Is.EqualTo(v + u),             "Addition must be commutative!");
            Assert.That((u + v) + w,       Is.EqualTo(u + (v + w)),       "Addition must be associative!");

            // ... the zero vector changes nothing, and every vector has an opposite
            Assert.That(u + Vector2D<Double>.Zero, Is.EqualTo(u),         "The zero vector must be the additive identity!");
            Assert.That(u + (-u),          Is.EqualTo(Vector2D<Double>.Zero), "A vector plus its opposite must vanish!");
            Assert.That(u - v,             Is.EqualTo(u + (-v)),          "Subtraction must be addition of the opposite!");

            // Scaling distributes over addition and is the same from either side
            Assert.That(2.0 * (u + v),     Is.EqualTo(2.0 * u + 2.0 * v), "Scaling must distribute over addition!");
            Assert.That(u * 3.0,           Is.EqualTo(3.0 * u),           "Scaling must not care which side the number stands on!");
            Assert.That(u * 1.0,           Is.EqualTo(u),                 "Scaling by one must change nothing!");
            Assert.That((u * 6.0) / 2.0,   Is.EqualTo(u * 3.0),           "Dividing must undo scaling!");
            Assert.That((u * 2.0).Length,  Is.EqualTo(u.Length * 2.0),    "Scaling must scale the length!");

            // The dot product measures alignment: zero exactly when perpendicular
            Assert.That(u.DotProduct(v),   Is.EqualTo(3.0 * 1.0 + 4.0 * -2.0), "The dot product is the sum of the products of the components!");
            Assert.That(u.DotProduct(v),   Is.EqualTo(v.DotProduct(u)),   "The dot product must be symmetric!");
            Assert.That(u.DotProduct(u),   Is.EqualTo(u.Length * u.Length).Within(1e-12),
                                                                          "A vector dotted with itself is its length squared!");
            Assert.That(new Vector2D<Double>(1.0, 0.0).DotProduct(new Vector2D<Double>(0.0, 1.0)), Is.Zero,
                        "Perpendicular vectors have a dot product of zero!");

            // The cross product measures the spanned area: zero exactly when parallel
            Assert.That(u.CrossProduct(v), Is.EqualTo(3.0 * -2.0 - 4.0 * 1.0), "The cross product is the determinant of the two!");
            Assert.That(u.CrossProduct(v), Is.EqualTo(-v.CrossProduct(u)), "Swapping the operands must flip the sign!");
            Assert.That(u.CrossProduct(u), Is.Zero,                        "A vector is parallel to itself, so the area is zero!");
            Assert.That(u.CrossProduct(u * 2.0), Is.Zero,                  "... and to any multiple of itself!");
            Assert.That(new Vector2D<Double>(1.0, 0.0).CrossProduct(new Vector2D<Double>(0.0, 1.0)), Is.EqualTo(1.0),
                        "The unit square has an area of one!");

        }

        #endregion

        #region Parallel_vectors_are_recognised_however_they_are_scaled()

        /// <summary>
        /// IsParallelTo used to normalise both vectors and compare their
        /// components, which asks the question through two square roots and
        /// four divisions - and the rounding of those made it answer wrongly
        /// for pairs as plain as (1, 1) and (3, 3), where 1/sqrt(2) and
        /// 3/sqrt(18) differ in their last bit. The cross product decides it
        /// exactly instead.
        /// </summary>
        [Test]
        public void Parallel_vectors_are_recognised_however_they_are_scaled()
        {

            // Every one of these pairs is a vector and a multiple of itself
            foreach (var (x, y, factor) in new[] { (1.0, 1.0, 3.0),  (1.0, 3.0, 2.0),  (2.0, 7.0,  2.0),
                                                   (3.0, 4.0, 2.0),  (5.0, 12.0, 2.0), (1.0, 1.0, 7.0),
                                                   (2.0, 3.0, 5.0),  (7.0, 11.0, 13.0) })
            {

                var vector   = new Vector2D<Double>(x, y);
                var multiple = new Vector2D<Double>(x * factor, y * factor);

                Assert.That(vector.IsParallelTo(multiple), Is.True,
                            $"({x}, {y}) must be parallel to itself scaled by {factor}!");

                // ... and to itself scaled the other way, which is antiparallel
                Assert.That(vector.IsParallelTo(-multiple), Is.True,
                            $"({x}, {y}) must be parallel to its opposite scaled by {factor}!");

            }

            // Vectors that genuinely point elsewhere are not parallel
            Assert.That(new Vector2D<Double>(1.0, 1.0).IsParallelTo(new Vector2D<Double>(1.0, 2.0)), Is.False,
                        "(1,1) and (1,2) point in different directions!");
            Assert.That(new Vector2D<Double>(1.0, 0.0).IsParallelTo(new Vector2D<Double>(0.0, 1.0)), Is.False,
                        "Perpendicular vectors are not parallel!");

        }

        #endregion

        #region A_circle_and_a_sphere_span_twice_their_radius()

        /// <summary>
        /// Diameter was Math.Add(Radius, Radius) in both, and the containment
        /// tests compared a distance against Math.Sub / Math.Add of two radii.
        /// </summary>
        [Test]
        public void A_circle_and_a_sphere_span_twice_their_radius()
        {

            var circle = new Circle<Double>(0.0, 0.0, 5.0);

            Assert.That(circle.Diameter, Is.EqualTo(10.0), "A circle of radius five spans ten!");

            // A 3-4-5 triangle puts this point exactly on the circumference
            Assert.That(circle.Contains(new Pixel<Double>(3.0, 4.0)), Is.True,  "The point (3,4) lies on a circle of radius five!");
            Assert.That(circle.Contains(new Pixel<Double>(1.0, 1.0)), Is.True,  "... and (1,1) lies well inside it!");
            Assert.That(circle.Contains(new Pixel<Double>(5.0, 5.0)), Is.False, "... while (5,5) lies outside!");

            var sphere = new Sphere<Double>(0.0, 0.0, 0.0, 5.0);

            Assert.That(sphere.Diameter, Is.EqualTo(10.0), "A sphere of radius five spans ten as well!");

        }

        #endregion

    }

}
