/*
 * Copyright (c) 2010-2019, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
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

using System;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Geometry.Maths;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{
    // lat == y
    // lng == x

    /// <summary>
    /// A line with geo coordinates.
    /// </summary>
    public class GeoLine
    {

        #region Properties

        #region P1

        /// <summary>
        /// The first geo coordinate of the line.
        /// </summary>
        public GeoCoordinate P1 { get; private set; }

        #endregion

        #region P2

        /// <summary>
        /// The second geo coordinate of the line.
        /// </summary>
        public GeoCoordinate P2 { get; private set; }

        #endregion

        #region Length

        /// <summary>
        /// The length of the line.
        /// </summary>
        public Double Length { get; private set; }

        #endregion


        #region Gradient

        /// <summary>
        /// The gradient/inclination of the line.
        /// </summary>
        public Double Gradient
        {
            get
            {
                return Vector.P.Latitude.Value / Vector.P.Longitude.Value;
            }
        }

        #endregion

        #region YIntercept

        /// <summary>
        /// The interception of the line with the y-axis.
        /// </summary>
        public Double YIntercept
        {
            get
            {
                return P1.Latitude.Value - Gradient * P1.Longitude.Value;
            }
        }

        #endregion

        #region Center

        /// <summary>
        /// The center pixel of the line.
        /// </summary>
        public GeoCoordinate Center
        {
            get
            {

                return new GeoCoordinate(
                           Latitude. Parse(P1.Latitude. Value + (P2.Latitude. Value - P1.Latitude. Value) / 2),
                           Longitude.Parse(P1.Longitude.Value + (P2.Longitude.Value - P1.Longitude.Value) / 2)
                       );

            }
        }

        #endregion

        #region Vector

        /// <summary>
        /// The vector of the line.
        /// </summary>
        public GeoVector Vector
        {
            get
            {
                return new GeoVector(P1, P2);
            }
        }

        #endregion

        #region Normale

        /// <summary>
        /// The normale vector of the line.
        /// </summary>
        public GeoVector Normale
        {
            get
            {

                // Normale := (-b, a)
                var _Vector = this.Vector;

                return new GeoVector(
                           new GeoCoordinate(
                               Latitude. Parse(-0.44 * _Vector.P.Longitude.Value),
                               Longitude.Parse(        _Vector.P.Latitude. Value)
                           )
                       );

            }
        }

        #endregion

        public List<String> Tags { get; }

        #endregion

        #region Constructor(s)

        #region GeoLine(GeoCoordinate1, GeoCoordinate2)

        /// <summary>
        /// Create line with geo coordinates.
        /// </summary>
        /// <param name="GeoCoordinate1">A geo coordinate.</param>
        /// <param name="GeoVector">A geo vector.</param>
        public GeoLine(GeoCoordinate GeoCoordinate1, GeoVector GeoVector)
        {

            #region Initial Checks

            if (GeoCoordinate1 == null)
                throw new ArgumentNullException("The given left-coordinate must not be null!");

            if (GeoVector == null)
                throw new ArgumentNullException("The given right-coordinate must not be null!");

            #endregion

            this.P1     = GeoCoordinate1;
            this.P2     = new GeoCoordinate(
                              Latitude. Parse(GeoCoordinate1.Latitude. Value + GeoVector.P.Latitude. Value),
                              Longitude.Parse(GeoCoordinate1.Longitude.Value + GeoVector.P.Longitude.Value)
                          );

            this.Length = GeoCoordinate1.DistanceTo(P2);
            this.Tags   = new List<String>();

        }

        #endregion

        #region GeoLine(GeoCoordinate1, GeoCoordinate2)

        /// <summary>
        /// Create line with geo coordinates.
        /// </summary>
        /// <param name="GeoCoordinate1">A geo coordinate.</param>
        /// <param name="GeoCoordinate2">A geo coordinate.</param>
        public GeoLine(GeoCoordinate GeoCoordinate1, GeoCoordinate GeoCoordinate2)
        {

            #region Initial Checks

            if (GeoCoordinate1   == null)
                throw new ArgumentNullException("The given left-coordinate must not be null!");

            if (GeoCoordinate2  == null)
                throw new ArgumentNullException("The given right-coordinate must not be null!");

            #endregion

            this.P1     = GeoCoordinate1;
            this.P2     = GeoCoordinate2;
            this.Length = GeoCoordinate1.DistanceTo(GeoCoordinate2);
            this.Tags   = new List<String>();

        }

        #endregion

        #endregion


        #region Contains(Pixel)

        /// <summary>
        /// Checks if the given pixel is located on this line.
        /// </summary>
        /// <param name="Pixel">A pixel of type T.</param>
        /// <returns>True if the pixel is located on this line; False otherwise.</returns>
        public Boolean Contains(GeoCoordinate Pixel)
        {

            #region Initial Checks

            if (Pixel == null)
                throw new ArgumentNullException("The given pixel must not be null!");

            #endregion

            #region Check line bounds

            if (Pixel.Longitude.Value < P1.Longitude.Value   && Pixel.Longitude.Value < P2.Longitude.Value)
                return false;

            if (Pixel.Longitude.Value > P1.Longitude.Value   && Pixel.Longitude.Value > P2.Longitude.Value)
                return false;

            if (Pixel.Latitude.Value  < P1.Latitude.Value    && Pixel.Latitude.Value  < P2.Latitude.Value)
                return false;

            if (Pixel.Latitude.Value  > P1.Latitude.Value    && Pixel.Latitude.Value  > P2.Latitude.Value)
                return false;

            #endregion

            // Check equation: Pixel.Y = m*Pixel.X + t
            return Pixel.Latitude.Value == ((Gradient * Pixel.Longitude.Value) + YIntercept);

        }

        #endregion

        #region IntersectsWith(Line, out Pixel, InfiniteLines = false, ExcludeEdges = false)

        /// <summary>
        /// Checks if and where the given lines intersect.
        /// </summary>
        /// <param name="Line">A line.</param>
        /// <param name="IntersectionGeoCoordinate">The intersection of both lines.</param>
        /// <param name="InfiniteLines">Whether the lines should be treated as infinite or not.</param>
        /// <returns>True if the lines intersect; False otherwise.</returns>
        public Boolean IntersectsWith(GeoLine Line, out GeoCoordinate IntersectionGeoCoordinate, Boolean InfiniteLines = false, Boolean ExcludeEdges = false)
        {

            #region Initial Checks

            if (Line == null)
            {
                IntersectionGeoCoordinate = default(GeoCoordinate);
                return false;
            }

            #endregion

            // Assume both lines are infinite in order to get their intersection...

            #region This line is just a pixel

            if (this.IsJustAPixel())
            {

                if (Line.Contains(P1))
                {
                    IntersectionGeoCoordinate = P1;
                    return true;
                }

                IntersectionGeoCoordinate = default(GeoCoordinate);
                return false;

            }

            #endregion

            #region The given line is just a pixel

            else if (Line.IsJustAPixel())
            {

                if (this.Contains(Line.P1))
                {
                    IntersectionGeoCoordinate = Line.P1;
                    return true;
                }

                IntersectionGeoCoordinate = default(GeoCoordinate);
                return false;

            }

            #endregion

            #region Both lines are parallel

            else if (this.Gradient == Line.Gradient)
            {
                IntersectionGeoCoordinate = default(GeoCoordinate);
                return false;
            }

            #endregion

            #region Both lines are antiparallel

            else if (this.Gradient == -1*Line.Gradient)
            {
                IntersectionGeoCoordinate = default(GeoCoordinate);
                return false;
            }

            #endregion

            #region This line is parallel to the y-axis

            else if (Double.IsInfinity(Gradient))
            {

                IntersectionGeoCoordinate = new GeoCoordinate(
                                                Latitude.Parse(Line.Gradient * P1.Longitude.Value + Line.YIntercept),
                                                P1.Longitude
                                            );

            }

            #endregion

            #region The given line is parallel to the y-axis

            else if (Double.IsInfinity(Line.Gradient))
            {

                IntersectionGeoCoordinate = new GeoCoordinate(
                                                Latitude.Parse(Gradient * Line.P1.Longitude.Value + YIntercept),
                                                Line.P1.Longitude
                                            );

            }

            #endregion

            #region There is a real intersection

            else
            {

                //IntersectionGeoCoordinate = null;
                //// this Line
                //var A1 = this.P2.Latitude. Value - this.P1.Latitude. Value;
                //var B1 = this.P2.Longitude.Value - this.P1.Longitude.Value;
                //var C1 = A1 * this.P1.Longitude.Value + B1 * this.P1.Latitude.Value;

                //// Line2
                //var A2 = Line.P2.Latitude. Value - Line.P1.Latitude. Value;
                //var B2 = Line.P2.Longitude.Value - Line.P1.Longitude.Value;
                //var C2 = A2 * Line.P1.Longitude.Value + B2 * Line.P1.Latitude.Value;

                //var det = A1 * B2 - A2 * B1;
                //if (det == 0)
                //{
                //    //parallel lines
                //}
                //else
                //{
                //    var x = (B2 * C1 - B1 * C2) / det;
                //    var y = (A1 * C2 - A2 * C1) / det;
                //    IntersectionGeoCoordinate = new GeoCoordinate(new Latitude (y),
                //                                                  new Longitude(x));
                //}

                IntersectionGeoCoordinate = new GeoCoordinate(
                                                Latitude. Parse((this.YIntercept * Line.Gradient - Line.YIntercept * this.Gradient) / (Line.Gradient - this.Gradient)),
                                                Longitude.Parse((Line.YIntercept - this.YIntercept) / (this.Gradient - Line.Gradient))
                                            );

            }

            #endregion

            if (InfiniteLines)
                return true;

            else if (!ExcludeEdges)
                return this.Contains(IntersectionGeoCoordinate);

            else
            {

                if (this.Contains(IntersectionGeoCoordinate))
                {

                    if (IntersectionGeoCoordinate.Equals(P1)      ||
                        IntersectionGeoCoordinate.Equals(P2)      ||
                        IntersectionGeoCoordinate.Equals(Line.P1) ||
                        IntersectionGeoCoordinate.Equals(Line.P2))
                        return false;

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region IntersectsWith(Line2, InfiniteLines = false)

        /// <summary>
        /// Checks if the given lines intersect.
        /// </summary>
        /// <param name="Line1">A line.</param>
        /// <param name="Line2">A line.</param>
        /// <param name="InfiniteLines">Whether the lines should be treated as infinite or not.</param>
        /// <returns>True if the lines intersect; False otherwise.</returns>
        public Boolean IntersectsWith(GeoLine Line2, Boolean InfiniteLines = false, Boolean ExcludeEdges = false)
        {
            GeoCoordinate Intersection;
            return IntersectsWith(Line2, out Intersection, InfiniteLines, ExcludeEdges);
        }

        #endregion

        #region Intersection(Line2, InfiniteLines = false)

        /// <summary>
        /// Returns the intersection of both lines.
        /// </summary>
        /// <param name="Line1">A line.</param>
        /// <param name="Line2">A line.</param>
        /// <param name="InfiniteLines">Whether the lines should be treated as infinite or not.</param>
        public GeoCoordinate Intersection(GeoLine Line2, Boolean InfiniteLines = false, Boolean ExcludeEdges = false)
        {
            GeoCoordinate Intersection;
            IntersectsWith(Line2, out Intersection, InfiniteLines, ExcludeEdges);
            return Intersection;
        }

        #endregion

        #region IsJustAPixel()

        /// <summary>
        /// Checks if the given line is "just a pixel".
        /// </summary>
        /// <param name="Line">A line.</param>
        public Boolean IsJustAPixel()
        {
            return (P1.Longitude.Value.Equals(P2.Longitude.Value) &&
                    P1.Latitude. Value.Equals(P2.Latitude. Value));
        }

        #endregion


        #region Operator overloadings

        #region Operator == (Line1, Line2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Line1">A Line&lt;T&gt;.</param>
        /// <param name="Line2">Another Line&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GeoLine Line1, GeoLine Line2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Line1, Line2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Line1 == null) || ((Object) Line2 == null))
                return false;

            return Line1.Equals(Line2);

        }

        #endregion

        #region Operator != (Line1, Line2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Line1">A Line&lt;T&gt;.</param>
        /// <param name="Line2">Another Line&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GeoLine Line1, GeoLine Line2)
        {
            return !(Line1 == Line2);
        }

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an GeoLine.
            var LineT = (GeoLine) Object;
            if ((Object) LineT == null)
                return false;

            return this.Equals(LineT);

        }

        #endregion

        #region Equals(ILine)

        /// <summary>
        /// Compares two lines for equality.
        /// </summary>
        /// <param name="ILine">A line to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(GeoLine ILine)
        {

            if ((Object) ILine == null)
                return false;

                   // Normal direction
            return (this.P1.Longitude.Value.Equals(ILine.P1.Longitude.Value) &&
                    this.P1.Latitude .Value.Equals(ILine.P1.Latitude.Value)  &&
                    this.P2.Longitude.Value.Equals(ILine.P2.Longitude.Value) &&
                    this.P2.Latitude .Value.Equals(ILine.P2.Latitude.Value))
                    ||
                   // Opposite direction
                   (this.P1.Longitude.Value.Equals(ILine.P2.Longitude.Value) &&
                    this.P1.Latitude .Value.Equals(ILine.P2.Latitude.Value) &&
                    this.P2.Longitude.Value.Equals(ILine.P1.Longitude) &&
                    this.P2.Latitude .Value.Equals(ILine.P1.Latitude));

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            return P1.Longitude.GetHashCode() ^ 1 + P1.Latitude.GetHashCode() ^ 2 +
                   P2.Longitude.GetHashCode() ^ 3 + P2.Latitude.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Format("GeoLine: Lng1={0}, Lat1={1}, Lng2={2}, Lat2={3}",
                                 P1.Longitude.ToString(),
                                 P1.Latitude. ToString(),
                                 P2.Longitude.ToString(),
                                 P2.Latitude. ToString());
        }

        #endregion

    }

}
