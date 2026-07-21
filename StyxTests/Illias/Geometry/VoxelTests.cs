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
    /// Unit tests for the 3-dimensional Voxel&lt;T&gt;.
    /// </summary>
    [TestFixture]
    public class VoxelTests
    {

        #region Voxel_Properties()

        [Test]
        public void Voxel_Properties()
        {

            var voxel = new Voxel<Double>(2.0, 3.0, 6.0);

            Assert.That(voxel.X, Is.EqualTo(2.0));
            Assert.That(voxel.Y, Is.EqualTo(3.0));
            Assert.That(voxel.Z, Is.EqualTo(6.0));

        }

        #endregion

        #region Voxel_DistanceTo_Coordinates()

        [Test]
        public void Voxel_DistanceTo_Coordinates()
        {

            var voxel = new Voxel<Double>(0.0, 0.0, 0.0);

            // Euclidean distance: sqrt(2² + 3² + 6²) = sqrt(49) = 7
            Assert.That(voxel.DistanceTo(2.0, 3.0, 6.0), Is.EqualTo(7.0));

        }

        #endregion

        #region Voxel_DistanceTo_Voxel()

        [Test]
        public void Voxel_DistanceTo_Voxel()
        {

            var voxel = new Voxel<Double>(0.0, 0.0, 0.0);

            Assert.That(voxel.DistanceTo(new Voxel<Double>(2.0, 3.0, 6.0)), Is.EqualTo(7.0));

            // Distance to itself is zero.
            Assert.That(voxel.DistanceTo(new Voxel<Double>(0.0, 0.0, 0.0)), Is.EqualTo(0.0));

        }

        #endregion

        #region Voxel_Equals_And_Operators()

        [Test]
        public void Voxel_Equals_And_Operators()
        {

            var voxel1 = new Voxel<Double>(2.0, 3.0, 6.0);
            var voxel2 = new Voxel<Double>(2.0, 3.0, 6.0);
            var voxel3 = new Voxel<Double>(2.0, 3.0, 9.0);

            Assert.That(voxel1.Equals(voxel2),      Is.True);
            Assert.That(voxel1 == voxel2,           Is.True);
            Assert.That(voxel1 != voxel3,           Is.True);
            Assert.That(voxel1.Equals(voxel3),      Is.False);

            // Differing only in Z must not be considered equal.
            Assert.That(voxel1.GetHashCode(), Is.EqualTo(voxel2.GetHashCode()));

        }

        #endregion

        #region Voxel_ToString()

        [Test]
        public void Voxel_ToString()
        {

            Assert.That(new Voxel<Double>(2.0, 3.0, 6.0).ToString(),
                        Is.EqualTo("Voxel: X=2, Y=3, Z=6"));

        }

        #endregion

    }

}
