/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    #region (class) GeoCalculations

    /// <summary>
    /// A bunch of extension methods to transform geo coordinates
    /// into their corresponding screen positions.
    /// </summary>
    public static partial class GeoCalculations
    {

        #region GeoCoordinate2ScreenXY(GeoCoordinate, ZoomLevel)

        /// <summary>
        /// Get the corresponding screen position for the given geo coordinate.
        /// </summary>
        /// <param name="GeoCoordinate">The geo coordinate.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        public static ScreenXY GeoCoordinate2ScreenXY(GeoCoordinate GeoCoordinate, UInt32 ZoomLevel)
        {
            return GeoCoordinate2ScreenXY(GeoCoordinate.Latitude, GeoCoordinate.Longitude, ZoomLevel);
        }

        #endregion

        #region GeoCoordinate2ScreenXY(Latitude, Longitude, ZoomLevel)

        /// <summary>
        /// Get the corresponding screen position for the given geo coordinate.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        public static ScreenXY GeoCoordinate2ScreenXY(Latitude Latitude, Longitude Longitude, UInt32 ZoomLevel)
        {

            var MapSize = Math.Pow(2.0, ZoomLevel) * 256;

            return new ScreenXY(

                (Int64) (((Longitude.Value + 180.0) / 360.0) * MapSize),

                (Int64) (((1.0 - Math.Log(
                                                             Math.Tan(Latitude.Value * Math.PI / 180.0) +
                                                       1.0 / Math.Cos(Latitude.Value * Math.PI / 180.0)
                                                   ) / Math.PI)
                                   / 2.0) * MapSize)

            );

        }

        #endregion


        #region ToScreenXY(this GeoCoordinates, ZoomLevel)

        /// <summary>
        /// Transform the given enumeration of geo coordinates
        /// into their corresponding screen positions.
        /// </summary>
        /// <param name="GeoCoordinates">An enumeration of geo coordinates.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        public static GeoCoordinate2ScreenXYPipe ToScreenXY(this IEnumerable<GeoCoordinate>  GeoCoordinates,
                                                            UInt32                           ZoomLevel)
        {
            return new GeoCoordinate2ScreenXYPipe(ZoomLevel, GeoCoordinates);
        }

        #endregion

        #region ToScreenXY(ArrowSender, ZoomLevel, OnError)

        /// <summary>
        /// Transform the given enumeration of geo coordinates
        /// into their corresponding screen positions.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        public static GeoCoordinate2ScreenXYArrow ToScreenXY(this IArrowSender<GeoCoordinate>  ArrowSender,
                                                             UInt32                            ZoomLevel,
                                                             Func<Exception, Exception>?       OnError   = null)
        {
            return new GeoCoordinate2ScreenXYArrow(ZoomLevel, OnError, ArrowSender);
        }

        #endregion

    }

    #endregion


    #region GeoCoordinate2ScreenXYPipe(ZoomLevel, IEnumerable = null, IEnumerator = null)

    /// <summary>
    /// A pipe transforming an enumeration of geo coordinates
    /// into their corresponding screen positions.
    /// </summary>
    public class GeoCoordinate2ScreenXYPipe : SelectPipe<GeoCoordinate, ScreenXY>
    {

        /// <summary>
        /// Create a new pipe to transform an enumeration of geo coordinates
        /// into their corresponding screen positions.
        /// </summary>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;S&gt; as element source.</param>
        public GeoCoordinate2ScreenXYPipe(UInt32                       ZoomLevel,
                                          IEnumerable<GeoCoordinate>?  IEnumerable   = null,
                                          IEnumerator<GeoCoordinate>?  IEnumerator   = null)

            : base(IEnumerable,
                   Item => GeoCalculations.GeoCoordinate2ScreenXY(Item, ZoomLevel))

        { }

    }

    #endregion

    #region GeoCoordinate2ScreenXYArrow(ZoomLevel, OnError = null, ArrowSender = null)

    /// <summary>
    /// An arrow transforming an enumeration of geo coordinates
    /// into their corresponding screen positions.
    /// </summary>
    public class GeoCoordinate2ScreenXYArrow : MapArrow<GeoCoordinate, ScreenXY>
    {

        /// <summary>
        /// Create a new arrow to transform an enumeration of geo coordinates
        /// into their corresponding screen positions.
        /// </summary>
        /// <param name="ZoomLevel">The current zoom level of the Aegir map.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public GeoCoordinate2ScreenXYArrow(UInt32                        ZoomLevel,
                                           Func<Exception, Exception>?   OnError       = null,
                                           IArrowSender<GeoCoordinate>?  ArrowSender   = null)

            : base(Item => GeoCalculations.GeoCoordinate2ScreenXY(Item, ZoomLevel),
                   OnError,
                   ArrowSender)

        { }

    }

    #endregion

}
