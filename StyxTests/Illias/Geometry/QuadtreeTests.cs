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
    /// Unit tests for the 2-dimensional Quadtree&lt;T&gt;.
    /// </summary>
    [TestFixture]
    public class QuadtreeTests
    {

        #region Quadtree_Add_WithinBounds_CountIncreases()

        [Test]
        public void Quadtree_Add_WithinBounds_CountIncreases()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0);

            Assert.That(quadtree.Count,         Is.EqualTo((UInt64) 0));

            quadtree.Add(10.0, 10.0);
            quadtree.Add(20.0, 20.0);
            quadtree.Add(30.0, 30.0);

            Assert.That(quadtree.Count,         Is.EqualTo((UInt64) 3));
            Assert.That(quadtree.EmbeddedCount, Is.EqualTo((UInt64) 3));  // no split yet

        }

        #endregion

        #region Quadtree_Split_WhenExceedingThreshold()

        [Test]
        public void Quadtree_Split_WhenExceedingThreshold()
        {

            // Capacity 4 => the 5th pixel forces a split; the embedded pixels
            // are pushed down into the four sub-trees and the root node keeps
            // none of them, while the total Count stays at 5.
            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0,
                                                MaxNumberOfEmbeddedPixels: 4);

            quadtree.Add(10.0, 10.0);
            quadtree.Add(90.0, 10.0);
            quadtree.Add(10.0, 90.0);
            quadtree.Add(90.0, 90.0);

            Assert.That(quadtree.EmbeddedCount, Is.EqualTo((UInt64) 4));  // still no split

            quadtree.Add(50.0, 50.0);                                    // triggers the split

            Assert.That(quadtree.Count,         Is.EqualTo((UInt64) 5));
            Assert.That(quadtree.EmbeddedCount, Is.EqualTo((UInt64) 0));  // moved into sub-trees

        }

        #endregion

        #region Quadtree_Add_OutOfBounds_Throws()

        [Test]
        public void Quadtree_Add_OutOfBounds_Throws()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0);

            Assert.That(
                () => quadtree.Add(150.0, 150.0),
                Throws.TypeOf<QT_OutOfBoundsException<Double>>()
            );

        }

        #endregion

        #region Quadtree_ZeroDimension_Throws()

        [Test]
        public void Quadtree_ZeroDimension_Throws()
        {

            // Left == Right => zero width (rejected by the Rectangle base).
            Assert.That(
                () => new Quadtree<Double>(0.0, 0.0, 0.0, 100.0),
                Throws.TypeOf<ArgumentException>()
            );

        }

        #endregion

        #region Quadtree_Get_Rectangle_ReturnsOnlyContainedPixels()

        [Test]
        public void Quadtree_Get_Rectangle_ReturnsOnlyContainedPixels()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0);

            quadtree.Add(10.0, 10.0);
            quadtree.Add(20.0, 20.0);
            quadtree.Add(80.0, 80.0);

            var withinTopLeft = quadtree.Get(new Rectangle<Double>(0.0, 0.0, 50.0, 50.0)).ToList();

            Assert.That(withinTopLeft.Count, Is.EqualTo(2));   // (10,10) and (20,20)

        }

        #endregion

        #region Quadtree_Get_Rectangle_TallQueryAcrossSubtree_FindsPixel()

        [Test]
        public void Quadtree_Get_Rectangle_TallQueryAcrossSubtree_FindsPixel()
        {

            // Regression test for the sub-tree pruning via Rectangle.Overlaps:
            // a tall, thin query band whose corners all fall *outside* the
            // top-left sub-tree used to be pruned by the old corner-based
            // Overlaps, silently dropping the contained pixel.
            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0,
                                                MaxNumberOfEmbeddedPixels: 2);

            quadtree.Add(25.0, 25.0);   // -> top-left sub-tree
            quadtree.Add(10.0, 45.0);   // -> top-left sub-tree
            quadtree.Add(60.0, 60.0);   // -> forces the split (bottom-right)

            // x in [20,30] slices through the top-left sub-tree (0..50, 0..50);
            // y in [-5,55] straddles it completely, so no query corner is inside.
            var tallBand = quadtree.Get(new Rectangle<Double>(20.0, -5.0, 30.0, 55.0)).ToList();

            Assert.That(tallBand.Count, Is.EqualTo(1));
            Assert.That(tallBand.Any(p => p.X == 25.0 && p.Y == 25.0), Is.True);

        }

        #endregion

        #region Quadtree_Remove_DecreasesCount()

        [Test]
        public void Quadtree_Remove_DecreasesCount()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0);

            quadtree.Add(10.0, 10.0);
            quadtree.Add(20.0, 20.0);

            Assert.That(quadtree.Count, Is.EqualTo((UInt64) 2));

            quadtree.Remove(new Pixel<Double>(10.0, 10.0));

            Assert.That(quadtree.Count, Is.EqualTo((UInt64) 1));

        }

        #endregion


        #region Quadtree_Add_DuplicatePixel_IsDeduplicated()

        [Test]
        public void Quadtree_Add_DuplicatePixel_IsDeduplicated()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0);

            quadtree.Add(10.0, 10.0);
            quadtree.Add(10.0, 10.0);   // same coordinates => value-equal pixel

            Assert.That(quadtree.Count, Is.EqualTo((UInt64) 1));

        }

        #endregion

        #region Quadtree_RecursiveSplit_PreservesAllPixels()

        [Test]
        public void Quadtree_RecursiveSplit_PreservesAllPixels()
        {

            // A tiny capacity forces several levels of splitting; no pixel may
            // get lost or duplicated, and enumeration must still yield them all.
            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0,
                                                MaxNumberOfEmbeddedPixels: 2);

            var pixels = new[] {
                (  5.0,  5.0 ), ( 15.0, 25.0 ), ( 35.0, 15.0 ), ( 45.0, 45.0 ),
                ( 55.0, 65.0 ), ( 75.0, 35.0 ), ( 85.0, 85.0 ), ( 25.0, 75.0 ),
                ( 65.0, 15.0 ), ( 95.0, 55.0 )
            };

            foreach (var (x, y) in pixels)
                quadtree.Add(x, y);

            Assert.That(quadtree.Count,       Is.EqualTo((UInt64) pixels.Length));
            Assert.That(quadtree.ToList().Count, Is.EqualTo(pixels.Length));   // enumeration agrees

        }

        #endregion

        #region Quadtree_Enumeration_YieldsAllPixels()

        [Test]
        public void Quadtree_Enumeration_YieldsAllPixels()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0,
                                                MaxNumberOfEmbeddedPixels: 2);

            quadtree.Add(10.0, 10.0);
            quadtree.Add(20.0, 80.0);
            quadtree.Add(80.0, 20.0);
            quadtree.Add(90.0, 90.0);

            var enumerated = quadtree.ToList();

            Assert.That(enumerated.Count,                             Is.EqualTo(4));
            Assert.That((UInt64) enumerated.Count,                    Is.EqualTo(quadtree.Count));
            Assert.That(enumerated.Any(p => p.X == 10.0 && p.Y == 10.0), Is.True);
            Assert.That(enumerated.Any(p => p.X == 90.0 && p.Y == 90.0), Is.True);

        }

        #endregion

        #region Quadtree_Get_PixelSelector_FiltersByPredicate()

        [Test]
        public void Quadtree_Get_PixelSelector_FiltersByPredicate()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0);

            quadtree.Add(10.0, 10.0);
            quadtree.Add(20.0, 20.0);
            quadtree.Add(80.0, 80.0);

            var leftHalf = quadtree.Get(pixel => pixel.X < 50.0).ToList();

            Assert.That(leftHalf.Count, Is.EqualTo(2));   // (10,10) and (20,20)

        }

        #endregion

        #region Quadtree_Remove_Rectangle_RemovesContainedPixels()

        [Test]
        public void Quadtree_Remove_Rectangle_RemovesContainedPixels()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0);

            quadtree.Add(10.0, 10.0);
            quadtree.Add(20.0, 20.0);
            quadtree.Add(80.0, 80.0);

            quadtree.Remove(new Rectangle<Double>(0.0, 0.0, 50.0, 50.0));

            Assert.That(quadtree.Count, Is.EqualTo((UInt64) 1));   // only (80,80) survives

        }

        #endregion

        #region Quadtree_OnTreeSplit_EventFires()

        [Test]
        public void Quadtree_OnTreeSplit_EventFires()
        {

            var quadtree = new Quadtree<Double>(0.0, 0.0, 100.0, 100.0,
                                                MaxNumberOfEmbeddedPixels: 2);

            var splitCount = 0;
            quadtree.OnTreeSplit += (tree, pixel) => splitCount++;

            quadtree.Add(10.0, 10.0);
            quadtree.Add(90.0, 90.0);

            Assert.That(splitCount, Is.EqualTo(0));   // still within capacity

            quadtree.Add(50.0, 50.0);                 // triggers the split

            Assert.That(splitCount, Is.GreaterThanOrEqualTo(1));

        }

        #endregion

    }

}
