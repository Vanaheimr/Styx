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
    /// Unit tests for the 2-dimensional Rectangle&lt;T&gt;.
    /// </summary>
    [TestFixture]
    public class RectangleTests
    {

        #region Rectangle_Dimensions_And_Normalisation()

        [Test]
        public void Rectangle_Dimensions_And_Normalisation()
        {

            // Corners given the "wrong" way round must be normalised to Min/Max.
            var rectangle = new Rectangle<Double>(10.0, 20.0, 0.0, 0.0);

            Assert.That(rectangle.Left,   Is.EqualTo( 0.0));
            Assert.That(rectangle.Top,    Is.EqualTo( 0.0));
            Assert.That(rectangle.Right,  Is.EqualTo(10.0));
            Assert.That(rectangle.Bottom, Is.EqualTo(20.0));
            Assert.That(rectangle.Width,  Is.EqualTo(10.0));
            Assert.That(rectangle.Height, Is.EqualTo(20.0));

        }

        #endregion

        #region Rectangle_Contains_IsInclusive()

        [Test]
        public void Rectangle_Contains_IsInclusive()
        {

            var rectangle = new Rectangle<Double>(0.0, 0.0, 10.0, 10.0);

            Assert.That(rectangle.Contains( 5.0,  5.0), Is.True);    // inside
            Assert.That(rectangle.Contains( 0.0,  0.0), Is.True);    // corner (inclusive)
            Assert.That(rectangle.Contains(10.0, 10.0), Is.True);    // corner (inclusive)
            Assert.That(rectangle.Contains(11.0,  5.0), Is.False);   // outside

        }

        #endregion

        #region Rectangle_Overlaps_CornerInside_And_Disjoint()

        [Test]
        public void Rectangle_Overlaps_CornerInside_And_Disjoint()
        {

            var rectangle = new Rectangle<Double>(0.0, 0.0, 10.0, 10.0);

            var overlapping = new Rectangle<Double>(5.0, 5.0, 15.0, 15.0);
            Assert.That(rectangle.Overlaps(overlapping), Is.True);

            var disjoint = new Rectangle<Double>(20.0, 20.0, 30.0, 30.0);
            Assert.That(rectangle.Overlaps(disjoint), Is.False);

        }

        #endregion

        #region Rectangle_Overlaps_CrossingWithoutContainedCorner()

        [Test]
        public void Rectangle_Overlaps_CrossingWithoutContainedCorner()
        {

            var rectangle = new Rectangle<Double>(0.0, 0.0, 10.0, 10.0);

            // A vertical bar piercing 'rectangle': none of its corners lie
            // inside (they are all at y = -5 or y = 15), yet they clearly share
            // the region x in [3,7], y in [0,10]. The old corner-based test
            // returned false here.
            var crossingBar = new Rectangle<Double>(3.0, -5.0, 7.0, 15.0);

            Assert.That(rectangle.Overlaps(crossingBar), Is.True);

            // Overlap must be symmetric.
            Assert.That(crossingBar.Overlaps(rectangle), Is.True);

        }

        #endregion

        #region Rectangle_Overlaps_TouchingBoundaryIsInclusive()

        [Test]
        public void Rectangle_Overlaps_TouchingBoundaryIsInclusive()
        {

            var rectangle = new Rectangle<Double>(0.0, 0.0, 10.0, 10.0);

            var touching = new Rectangle<Double>(10.0, 0.0, 20.0, 10.0);
            Assert.That(rectangle.Overlaps(touching), Is.True);

            var apart = new Rectangle<Double>(11.0, 0.0, 20.0, 10.0);
            Assert.That(rectangle.Overlaps(apart), Is.False);

        }

        #endregion

    }

}
