/*
 * Copyright (c) 2010-2021, Achim Friedland <achim.friedland@graphdefined.com>
 * This file is part of Aegir <https://www.github.com/Vanaheimr/Aegir>
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
using System.Linq;
using System.Text;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    #region (class) SDL

    /// <summary>
    /// A class representing a shape definition.
    /// </summary>
    public class SDL
    {

        #region Data

        private readonly String Text;

        #endregion

        #region Properties

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public String Value
        {
            get
            {
                return Text;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new shape definition.
        /// </summary>
        /// <param name="Text"></param>
        public SDL(String Text)
        {
            this.Text = Text;
        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return Text;
        }

        #endregion

    }

    #endregion

    #region (class) GeoCalculations

    /// <summary>
    /// A bunch of extention methods to transform
    /// geo coordinates into shape definitions.
    /// </summary>
    public static partial class GeoCalculations
    {

        #region GeoCoordinates2ShapeDefinition(this GeoCoordinates, OnScreenUpperLeft, ZoomLevel, CloseShape)

        /// <summary>
        /// Transform the given enumeration of geo coordinates into a shape definition.
        /// </summary>
        /// <param name="GeoCoordinates">An enumeration of geo coordinates.</param>
        /// <param name="OnScreenUpperLeft">The upper-left screen offset.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="CloseShape">Whether to close the shape (polygon), or not (line).</param>
        public static SDL GeoCoordinates2ShapeDefinition(this IEnumerable<GeoCoordinate>  GeoCoordinates,
                                                         ScreenXY                         OnScreenUpperLeft,
                                                         UInt32                           ZoomLevel,
                                                         Boolean                          CloseShape)

        {

            if (GeoCoordinates == null)
                throw new ArgumentNullException("GeoCoordinates");

            var StringBuilder = new StringBuilder("M ");

            Action<ScreenXY> ApplyScreenOffset = ScreenXY => {
                StringBuilder.Append(((Int64) ScreenXY.X) - OnScreenUpperLeft.X);
                StringBuilder.Append(" ");
                StringBuilder.Append(((Int64) ScreenXY.Y) - OnScreenUpperLeft.Y);
                StringBuilder.Append(" ");
            };

            var GeoCoordinates2 = GeoCoordinates.ToArray();
            var BoundingBox     = GeoCoordinates2.GeoCoordinate2BoundingBox();


            GeoCoordinates2.

                // Project the geo coordinate onto the current screen (without offset!)
                Select(GeoCoordinate => GeoCalculations.GeoCoordinate2ScreenXY(GeoCoordinate, ZoomLevel)).

                ForEach(
                    // For the first...
                    OnScreenXY => {
                                      ApplyScreenOffset(OnScreenXY);
                                      StringBuilder.Append("L ");
                                  },

                    // For the rest..
                    OnScreenXY => ApplyScreenOffset(OnScreenXY));


            if (CloseShape)
                StringBuilder.Append("Z ");

            return new SDL(StringBuilder.ToString());

        }

        #endregion


        #region ToShapeDefinition(this GeoCoordinates, OnScreenUpperLeft, ZoomLevel, CloseShape)

        /// <summary>
        /// Transform the given enumeration of geo coordinates into a shape definition.
        /// </summary>
        /// <param name="GeoCoordinates">An enumeration of geo coordinates.</param>
        /// <param name="OnScreenUpperLeft">The upper-left screen offset.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="CloseShape">Whether to close the shape (polygon), or not (line).</param>
        public static GeoCoordinates2ShapeDefinitionPipe ToShapeDefinition(this IEnumerable<IEnumerable<GeoCoordinate>>  GeoCoordinates,
                                                                           ScreenXY                                      OnScreenUpperLeft,
                                                                           UInt32                                        ZoomLevel,
                                                                           Boolean                                       CloseShape)
        {
            return new GeoCoordinates2ShapeDefinitionPipe(OnScreenUpperLeft, ZoomLevel, CloseShape, GeoCoordinates);
        }

        #endregion

        #region ToShapeDefinition(ArrowSender, OnScreenUpperLeft, ZoomLevel, CloseShape, OnError)

        /// <summary>
        /// Transform the given enumeration of geo coordinates into a shape definition.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="OnScreenUpperLeft">The upper-left screen offset.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="CloseShape">Whether to close the shape (polygon), or not (line).</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        public static GeoCoordinates2ShapeDefinitionArrow ToShapeDefinition(this IArrowSender<IEnumerable<GeoCoordinate>>  ArrowSender,
                                                                            ScreenXY                                       OnScreenUpperLeft,
                                                                            UInt32                                         ZoomLevel,
                                                                            Boolean                                        CloseShape,
                                                                            Func<Exception, Exception>                     OnError     = null)
        {
            return new GeoCoordinates2ShapeDefinitionArrow(OnScreenUpperLeft, ZoomLevel, CloseShape, OnError, ArrowSender);
        }

        #endregion

    }

    #endregion


    #region GeoCoordinates2ShapeDefinitionPipe(OnScreenUpperLeft, ZoomLevel, CloseShape, IEnumerable = null, IEnumerator = null)

    /// <summary>
    /// A pipe transforming an enumeration of
    /// geo coordinates into shape definitions.
    /// </summary>
    public class GeoCoordinates2ShapeDefinitionPipe : SelectPipe<IEnumerable<GeoCoordinate>, SDL>
    {

        /// <summary>
        /// Create a new pipe to transform an enumeration of
        /// geo coordinates into shape definitions.
        /// </summary>
        /// <param name="OnScreenUpperLeft">The upper-left screen offset.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="CloseShape">Whether to close the shape (polygon), or not (line).</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;S&gt; as element source.</param>
        public GeoCoordinates2ShapeDefinitionPipe(ScreenXY                                 OnScreenUpperLeft,
                                                  UInt32                                   ZoomLevel,
                                                  Boolean                                  CloseShape,
                                                  IEnumerable<IEnumerable<GeoCoordinate>>  IEnumerable = null,
                                                  IEnumerator<IEnumerable<GeoCoordinate>>  IEnumerator = null)

            : base(IEnumerable,
                   Item => Item.GeoCoordinates2ShapeDefinition(OnScreenUpperLeft, ZoomLevel, CloseShape))

        { }

    }

    #endregion

    #region GeoCoordinates2ShapeDefinitionArrow(OnScreenUpperLeft, ZoomLevel, CloseShape, OnError = null, ArrowSender = null)

    /// <summary>
    /// An arrow transforming an enumeration of
    /// geo coordinates into shape definitions.
    /// </summary>
    public class GeoCoordinates2ShapeDefinitionArrow : MapArrow<IEnumerable<GeoCoordinate>, SDL>
    {

        /// <summary>
        /// Create a new arrow to transform an enumeration of
        /// geo coordinates into shape definitions.
        /// </summary>
        /// <param name="OnScreenUpperLeft">The upper-left screen offset.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="CloseShape">Whether to close the shape (polygon), or not (line).</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public GeoCoordinates2ShapeDefinitionArrow(ScreenXY                                  OnScreenUpperLeft,
                                                   UInt32                                    ZoomLevel,
                                                   Boolean                                   CloseShape,
                                                   Func<Exception, Exception>                OnError     = null,
                                                   IArrowSender<IEnumerable<GeoCoordinate>>  ArrowSender = null)

            : base(Item => Item.GeoCoordinates2ShapeDefinition(OnScreenUpperLeft, ZoomLevel, CloseShape),
                   OnError,
                   ArrowSender)

        { }

    }

    #endregion

}
