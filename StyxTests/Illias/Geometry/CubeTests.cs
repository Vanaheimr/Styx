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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias.Geometry;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Geometry
{

    /// <summary>
    /// Unit tests for the 3-dimensional Cube&lt;T&gt;.
    /// </summary>
    [TestFixture]
    public class CubeTests
    {

        #region Cube_FromCoordinates()

        [Test]
        public void Cube_FromCoordinates()
        {

            // Cube(Left, Top, Front, Right, Bottom, Behind)
            var cube = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 20.0, 30.0);

            Assert.That(cube.Width,  Is.EqualTo(10.0));
            Assert.That(cube.Height, Is.EqualTo(20.0));
            Assert.That(cube.Depth,  Is.EqualTo(30.0));

            Assert.That(cube.Contains( 5.0, 10.0, 15.0), Is.True);   // inside
            Assert.That(cube.Contains(11.0, 10.0, 15.0), Is.False);  // right of Right
            Assert.That(cube.Contains( 5.0, 21.0, 15.0), Is.False);  // below Bottom
            Assert.That(cube.Contains( 5.0, 10.0, 31.0), Is.False);  // behind Behind

        }

        #endregion

        #region Cube_FromCoordinates_NormalisesMinMax()

        [Test]
        public void Cube_FromCoordinates_NormalisesMinMax()
        {

            // Left/Right (and the other pairs) given the "wrong" way round
            // must be normalised to Min/Max.
            var cube = new Cube<Double>(10.0, 20.0, 30.0, 0.0, 0.0, 0.0);

            Assert.That(cube.Left,   Is.EqualTo( 0.0));
            Assert.That(cube.Right,  Is.EqualTo(10.0));
            Assert.That(cube.Width,  Is.EqualTo(10.0));
            Assert.That(cube.Height, Is.EqualTo(20.0));
            Assert.That(cube.Depth,  Is.EqualTo(30.0));

        }

        #endregion

        #region Cube_FromVoxelAndDimensions()

        // Regression test: this constructor used to run its "must not be zero"
        // checks against the still-unassigned Left/Right/... properties
        // (default(T) == 0), so it *always* threw.
        [Test]
        public void Cube_FromVoxelAndDimensions()
        {

            var cube = new Cube<Double>(new Voxel<Double>(0.0, 0.0, 0.0),
                                        Width:  10.0,
                                        Height: 20.0,
                                        Depth:  30.0);

            Assert.That(cube.Left,   Is.EqualTo( 0.0));
            Assert.That(cube.Right,  Is.EqualTo(10.0));
            Assert.That(cube.Bottom, Is.EqualTo(20.0));
            Assert.That(cube.Behind, Is.EqualTo(30.0));

            Assert.That(cube.Width,  Is.EqualTo(10.0));
            Assert.That(cube.Height, Is.EqualTo(20.0));
            Assert.That(cube.Depth,  Is.EqualTo(30.0));

            Assert.That(cube.Contains(5.0, 10.0, 15.0), Is.True);

        }

        #endregion

        #region Cube_FromTwoVoxels()

        // Regression test: same latent bug as Cube_FromVoxelAndDimensions.
        [Test]
        public void Cube_FromTwoVoxels()
        {

            // Deliberately unordered: Voxel1 holds the larger coordinates.
            var cube = new Cube<Double>(new Voxel<Double>(10.0, 20.0, 30.0),
                                        new Voxel<Double>( 0.0,  0.0,  0.0));

            Assert.That(cube.Left,   Is.EqualTo( 0.0));
            Assert.That(cube.Right,  Is.EqualTo(10.0));
            Assert.That(cube.Top,    Is.EqualTo( 0.0));
            Assert.That(cube.Bottom, Is.EqualTo(20.0));
            Assert.That(cube.Front,  Is.EqualTo( 0.0));
            Assert.That(cube.Behind, Is.EqualTo(30.0));

            Assert.That(cube.Width,  Is.EqualTo(10.0));

            Assert.That(cube.Contains(5.0, 10.0, 15.0), Is.True);

        }

        #endregion

        #region Cube_Contains_IsInclusiveOnTheBoundaries()

        [Test]
        public void Cube_Contains_IsInclusiveOnTheBoundaries()
        {

            var cube = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 10.0, 10.0);

            Assert.That(cube.Contains( 0.0,  0.0,  0.0), Is.True);   // lower corner
            Assert.That(cube.Contains(10.0, 10.0, 10.0), Is.True);   // upper corner

        }

        #endregion

        #region Cube_FromVoxelAndDimensions_ZeroWidth_Throws()

        [Test]
        public void Cube_FromVoxelAndDimensions_ZeroWidth_Throws()
        {

            Assert.That(
                () => new Cube<Double>(new Voxel<Double>(0.0, 0.0, 0.0), 0.0, 20.0, 30.0),
                Throws.TypeOf<ArgumentException>()
            );

        }

        #endregion

        #region Cube_FromTwoVoxels_ZeroSpan_Throws()

        [Test]
        public void Cube_FromTwoVoxels_ZeroSpan_Throws()
        {

            // Identical X-coordinate => zero width.
            Assert.That(
                () => new Cube<Double>(new Voxel<Double>(5.0, 0.0,  0.0),
                                       new Voxel<Double>(5.0, 10.0, 10.0)),
                Throws.TypeOf<ArgumentException>()
            );

        }

        #endregion


        #region Cube_Contains_Voxel()

        [Test]
        public void Cube_Contains_Voxel()
        {

            var cube = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 10.0, 10.0);

            Assert.That(cube.Contains(new Voxel<Double>( 5.0,  5.0,  5.0)), Is.True);
            Assert.That(cube.Contains(new Voxel<Double>(15.0,  5.0,  5.0)), Is.False);

        }

        #endregion

        #region Cube_Contains_Cube()

        [Test]
        public void Cube_Contains_Cube()
        {

            var outer = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 10.0, 10.0);

            var inner = new Cube<Double>(2.0, 2.0, 2.0,  8.0,  8.0,  8.0);
            Assert.That(outer.Contains(inner), Is.True);    // fully inside

            var sticksOut = new Cube<Double>(2.0, 2.0, 2.0, 12.0, 8.0, 8.0);
            Assert.That(outer.Contains(sticksOut), Is.False);   // reaches past Right

            // Containment is not symmetric.
            Assert.That(inner.Contains(outer), Is.False);

        }

        #endregion

        #region Cube_Overlaps()

        [Test]
        public void Cube_Overlaps()
        {

            var cube = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 10.0, 10.0);

            // A cube whose lower corner sits inside 'cube'.
            var overlapping = new Cube<Double>(5.0, 5.0, 5.0, 15.0, 15.0, 15.0);
            Assert.That(cube.Overlaps(overlapping), Is.True);

            // A completely separate cube.
            var disjoint = new Cube<Double>(20.0, 20.0, 20.0, 30.0, 30.0, 30.0);
            Assert.That(cube.Overlaps(disjoint), Is.False);

        }

        #endregion

        #region Cube_Overlaps_CrossingWithoutContainedCorner()

        [Test]
        public void Cube_Overlaps_CrossingWithoutContainedCorner()
        {

            var cube = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 10.0, 10.0);

            // A bar that pierces 'cube' along the X-axis. None of its eight
            // corners lie inside 'cube' (they are all at x = -5 or x = 15),
            // yet the two cubes clearly share the region x in [0,10], y,z in [3,7].
            // The old corner-based test returned false here.
            var crossingBar = new Cube<Double>(-5.0, 3.0, 3.0, 15.0, 7.0, 7.0);

            Assert.That(cube.Overlaps(crossingBar), Is.True);

            // Overlap must be symmetric.
            Assert.That(crossingBar.Overlaps(cube), Is.True);

        }

        #endregion

        #region Cube_Overlaps_TouchingBoundaryIsInclusive()

        [Test]
        public void Cube_Overlaps_TouchingBoundaryIsInclusive()
        {

            var cube = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 10.0, 10.0);

            // Faces just touching at x = 10 count as an overlap (inclusive).
            var touching = new Cube<Double>(10.0, 0.0, 0.0, 20.0, 10.0, 10.0);
            Assert.That(cube.Overlaps(touching), Is.True);

            // One unit apart => no overlap.
            var apart = new Cube<Double>(11.0, 0.0, 0.0, 20.0, 10.0, 10.0);
            Assert.That(cube.Overlaps(apart), Is.False);

        }

        #endregion

        #region Cube_Equals_And_Operators()

        [Test]
        public void Cube_Equals_And_Operators()
        {

            var cube1 = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 20.0, 30.0);
            var cube2 = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 20.0, 30.0);
            var cube3 = new Cube<Double>(0.0, 0.0, 0.0, 10.0, 20.0, 99.0);

            Assert.That(cube1.Equals(cube2),      Is.True);
            Assert.That(cube1 == cube2,           Is.True);
            Assert.That(cube1 != cube3,           Is.True);
            Assert.That(cube1.Equals(cube3),      Is.False);

            // Equal cubes must share a hash code.
            Assert.That(cube1.GetHashCode(), Is.EqualTo(cube2.GetHashCode()));

        }

        #endregion

    }

}
