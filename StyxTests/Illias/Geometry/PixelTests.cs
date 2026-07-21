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
    /// Unit tests for the 2-dimensional Pixel&lt;T&gt;.
    /// </summary>
    [TestFixture]
    public class PixelTests
    {

        #region Pixel_Properties()

        [Test]
        public void Pixel_Properties()
        {

            var pixel = new Pixel<Double>(3.0, 4.0);

            Assert.That(pixel.X, Is.EqualTo(3.0));
            Assert.That(pixel.Y, Is.EqualTo(4.0));

        }

        #endregion

        #region Pixel_DistanceTo_Coordinates()

        [Test]
        public void Pixel_DistanceTo_Coordinates()
        {

            var pixel = new Pixel<Double>(0.0, 0.0);

            // Euclidean distance: sqrt(3² + 4²) = 5
            Assert.That(pixel.DistanceTo(3.0, 4.0), Is.EqualTo(5.0));

        }

        #endregion

        #region Pixel_DistanceTo_Pixel()

        [Test]
        public void Pixel_DistanceTo_Pixel()
        {

            var pixel = new Pixel<Double>(0.0, 0.0);

            Assert.That(pixel.DistanceTo(new Pixel<Double>(3.0, 4.0)), Is.EqualTo(5.0));

            // Distance to itself is zero.
            Assert.That(pixel.DistanceTo(new Pixel<Double>(0.0, 0.0)), Is.EqualTo(0.0));

        }

        #endregion

        #region Pixel_Equals_And_Operators()

        [Test]
        public void Pixel_Equals_And_Operators()
        {

            var pixel1 = new Pixel<Double>(3.0, 4.0);
            var pixel2 = new Pixel<Double>(3.0, 4.0);
            var pixel3 = new Pixel<Double>(3.0, 9.0);

            Assert.That(pixel1.Equals(pixel2),      Is.True);
            Assert.That(pixel1 == pixel2,           Is.True);
            Assert.That(pixel1 != pixel3,           Is.True);
            Assert.That(pixel1.Equals(pixel3),      Is.False);

            // Equal pixels must share a hash code.
            Assert.That(pixel1.GetHashCode(), Is.EqualTo(pixel2.GetHashCode()));

        }

        #endregion

        #region Pixel_Swap()

        [Test]
        public void Pixel_Swap()
        {

            IPixel<Double> pixel1 = new Pixel<Double>(1.0, 2.0);
            IPixel<Double> pixel2 = new Pixel<Double>(3.0, 4.0);

            Pixel<Double>.Swap(ref pixel1, ref pixel2);

            Assert.That(pixel1.X, Is.EqualTo(3.0));
            Assert.That(pixel1.Y, Is.EqualTo(4.0));
            Assert.That(pixel2.X, Is.EqualTo(1.0));
            Assert.That(pixel2.Y, Is.EqualTo(2.0));

        }

        #endregion

        #region Pixel_ToString()

        [Test]
        public void Pixel_ToString()
        {

            Assert.That(new Pixel<Double>(3.0, 4.0).ToString(),
                        Is.EqualTo("Pixel: X=3, Y=4"));

        }

        #endregion

    }

}
