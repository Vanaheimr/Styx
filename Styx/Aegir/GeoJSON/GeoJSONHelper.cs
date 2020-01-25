/*
 * Copyright (c) 2010-2020, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public static class GeoJSONHelper
    {

        // 2.1.1. Positions
        // A position is the fundamental geometry construct. The "coordinates" member
        // of a geometry object is composed of one position (in the case of a Point
        // geometry), an array of positions (LineString or MultiPoint geometries), an
        // array of arrays of positions (Polygons, MultiLineStrings), or a
        // multidimensional array of positions (MultiPolygon).
        // 
        // A position is represented by an array of numbers. There must be at least
        // two elements, and may be more. The order of elements must follow x, y, z
        // order (easting, northing, altitude for coordinates in a projected coordinate
        // reference system, or longitude, latitude, altitude for coordinates in a
        // geographic coordinate reference system). Any number of additional elements
        // are allowed -- interpretation and meaning of additional elements is beyond
        // the scope of this specification.


        // Point
        //
        // Point coordinates are in x, y order (easting, northing for projected
        // coordinates, longitude, latitude for geographic coordinates):
        //
        // {
        //    "type":        "Point",
        //    "coordinates": [100.0, 0.0]
        // }


        // LineString
        //
        // Coordinates of LineString are an array of positions (see 2.1.1. Positions):
        //
        // {
        //    "type":        "LineString",
        //    "coordinates": [ [100.0, 0.0], [101.0, 1.0] ]
        // }


        //Polygon

        //Coordinates of a Polygon are an array of LinearRing coordinate arrays. The first element in the array represents the exterior ring. Any subsequent elements represent interior rings (or holes).

        //No holes:

        //{ "type": "Polygon",
        //  "coordinates": [
        //    [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0] ]
        //    ]
        // }
        //With holes:

        //{ "type": "Polygon",
        //  "coordinates": [
        //    [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0] ],
        //    [ [100.2, 0.2], [100.8, 0.2], [100.8, 0.8], [100.2, 0.8], [100.2, 0.2] ]
        //    ]
        // }
        //MultiPoint

        //Coordinates of a MultiPoint are an array of positions:

        //{ "type": "MultiPoint",
        //  "coordinates": [ [100.0, 0.0], [101.0, 1.0] ]
        //  }
        //MultiLineString

        //Coordinates of a MultiLineString are an array of LineString coordinate arrays:

        //{ "type": "MultiLineString",
        //  "coordinates": [
        //      [ [100.0, 0.0], [101.0, 1.0] ],
        //      [ [102.0, 2.0], [103.0, 3.0] ]
        //    ]
        //  }
        //MultiPolygon

        //Coordinates of a MultiPolygon are an array of Polygon coordinate arrays:

        //{ "type": "MultiPolygon",
        //  "coordinates": [
        //    [[[102.0, 2.0], [103.0, 2.0], [103.0, 3.0], [102.0, 3.0], [102.0, 2.0]]],
        //    [[[100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0]],
        //     [[100.2, 0.2], [100.8, 0.2], [100.8, 0.8], [100.2, 0.8], [100.2, 0.2]]]
        //    ]
        //  }
        //GeometryCollection

        //Each element in the geometries array of a GeometryCollection is one of the geometry objects described above:

        //{ "type": "GeometryCollection",
        //  "geometries": [
        //    { "type": "Point",
        //      "coordinates": [100.0, 0.0]
        //      },
        //    { "type": "LineString",
        //      "coordinates": [ [101.0, 0.0], [102.0, 1.0] ]
        //      }
        //  ]
        //}










        //        2.2. Feature Objects

        //A GeoJSON object with the type "Feature" is a feature object.

        //A feature object must have a member with the name "geometry". The value of the geometry member is a geometry object as defined above or a JSON null value.
        //A feature object must have a member with the name "properties". The value of the properties member is an object (any JSON object or a JSON null value).
        //If a feature has a commonly used identifier, that identifier should be included as a member of the feature object with the name "id".


        // { "type":     "FeatureCollection",
        //   "features": [

        //      {
        //          "type":       "Feature",
        //          "geometry":   {
        //                            "type":        "Point",
        //                            "coordinates": [102.0, 0.5]
        //                        },
        //          "properties": {
        //                            "prop0":       "value0"
        //                        }
        //      },


        //  { "type": "Feature",
        //    "geometry": {
        //      "type": "LineString",
        //      "coordinates": [
        //        [102.0, 0.0], [103.0, 1.0], [104.0, 0.0], [105.0, 1.0]
        //        ]
        //      },
        //    "properties": {
        //      "prop0": "value0",
        //      "prop1": 0.0
        //      }
        //    },


        //  { "type": "Feature",
        //     "geometry": {
        //       "type": "Polygon",
        //       "coordinates": [
        //         [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0],
        //           [100.0, 1.0], [100.0, 0.0] ]
        //         ]
        //     },
        //     "properties": {
        //       "prop0": "value0",
        //       "prop1": {"this": "that"}
        //       }
        //     }
        //   ]
        // }


        public static GeoBoundingBox GetBoundingBox(JObject  GeoJSON,
                                                    Int32    FeatureId = 0,
                                                    Int32    ObjectId  = 0)
        {

            Double lng, lat;
            var min_lng = Double.MaxValue;
            var max_lng = Double.MinValue;
            var min_lat = Double.MaxValue;
            var max_lat = Double.MinValue;

            var GeoCoordinates = GeoJSON?["features"]?[FeatureId]?["geometry"]?["coordinates"]?[ObjectId];

            if (GeoCoordinates == null)
                return new GeoBoundingBox(Latitude.Parse(0), Longitude.Parse(0), Altitude.Parse(0),
                                          Latitude.Parse(0), Longitude.Parse(0), Altitude.Parse(0));

            else
                foreach (var value in GeoCoordinates)
                {

                    lng = Double.Parse(value[0].Value<String>(), CultureInfo.InvariantCulture);
                    lat = Double.Parse(value[1].Value<String>(), CultureInfo.InvariantCulture);

                    if (min_lat > lat) min_lat = lat;
                    if (max_lat < lat) max_lat = lat;
                    if (min_lng > lng) min_lng = lng;
                    if (max_lng < lng) max_lng = lng;

                }

            return new GeoBoundingBox(Latitude.Parse(min_lat), Longitude.Parse(min_lng), Altitude.Parse(0),
                                      Latitude.Parse(max_lat), Longitude.Parse(max_lng), Altitude.Parse(0));

        }


        public static IEnumerable<GeoBoundingBox>

            CreateRaster(GeoBoundingBox  BoundingBox,
                         Double          LatitudeSize,
                         Double          LongitudeSize,
                         IList<Point>    Shape)

        {

            // Longitude west<->east

            var CurrentLatitude    = BoundingBox.Latitude2;
            var CurrentLongitude   = BoundingBox.Longitude;
            var Boxes              = new List<GeoBoundingBox>();

            while (CurrentLatitude >= BoundingBox.Latitude)
            {

                CurrentLongitude = BoundingBox.Longitude;

                while (CurrentLongitude <= BoundingBox.Longitude2)
                {

                    if (PolyContainsPoint(Shape, new Point(CurrentLongitude.Value,                 CurrentLatitude.Value)) ||
                        PolyContainsPoint(Shape, new Point(CurrentLongitude.Value,                 CurrentLatitude.Value - LatitudeSize)) ||
                        PolyContainsPoint(Shape, new Point(CurrentLongitude.Value + LongitudeSize, CurrentLatitude.Value)) ||
                        PolyContainsPoint(Shape, new Point(CurrentLongitude.Value + LongitudeSize, CurrentLatitude.Value - LatitudeSize)))
                    {

                        Boxes.Add(new GeoBoundingBox(CurrentLatitude,
                                                     CurrentLongitude,
                                                     Latitude. Parse(CurrentLatitude. Value - LatitudeSize),
                                                     Longitude.Parse(CurrentLongitude.Value + LongitudeSize)));

                    }

                    CurrentLongitude = Longitude.Parse(CurrentLongitude.Value + LongitudeSize);

                }

                CurrentLatitude  = Latitude.Parse(CurrentLatitude. Value - LatitudeSize);

            }

            return Boxes;

        }


        public static JObject ToGeoJSON(this IEnumerable<GeoBoundingBox> BoundingBoxes)
        {

            return new JObject(
                new JProperty("type",      "FeatureCollection"),
                new JProperty("features",  new JArray(

                    BoundingBoxes.Select((BoundingBox, counter) =>

                        new JObject(

                            new JProperty("type",        "Feature"),

                            new JProperty("properties",  new JObject(
                                new JProperty("BoxId",   counter+1)
                            )),

                            new JProperty("geometry",    new JObject(
                                new JProperty("type",         "Polygon"),
                                new JProperty("coordinates",  new JArray(
                                    (Object) new JArray(
                                        new JArray(BoundingBox.Longitude. Value, BoundingBox.Latitude. Value),
                                        new JArray(BoundingBox.Longitude2.Value, BoundingBox.Latitude. Value),
                                        new JArray(BoundingBox.Longitude2.Value, BoundingBox.Latitude2.Value),
                                        new JArray(BoundingBox.Longitude. Value, BoundingBox.Latitude2.Value),
                                        new JArray(BoundingBox.Longitude. Value, BoundingBox.Latitude. Value)
                                    )
                                ))
                            ))

                        )

                    )

                ))
            );

        }




        /// <summary>
        /// Uses the Douglas Peucker algorithim to reduce the number of points.
        /// https://gist.github.com/oliverheilig/7777382
        /// </summary>
        /// <param name="Points">The points.</param>
        /// <param name="Tolerance">The tolerance.</param>
        /// <returns></returns>
        public static IList<Point> DouglasPeuckerReduction(IList<Point> Points, double Tolerance)
        {

            if (Points == null || Points.Count < 3)
                return Points;

            int firstPoint = 0;
            int lastPoint = Points.Count - 1;
            var pointIndexsToKeep = new List<int>();

            // Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            // The first and the last point can not be the same
            while (Points[firstPoint].Equals(Points[lastPoint]))
                lastPoint--;

            DouglasPeuckerReduction(Points, firstPoint, lastPoint, Tolerance, ref pointIndexsToKeep);

            var returnPoints = new List<Point>();
            pointIndexsToKeep.Sort();
            foreach (int index in pointIndexsToKeep)
            {
                returnPoints.Add(Points[index]);
            }

            return returnPoints;
        }


        /// <summary>
        /// Douglases the peucker reduction.
        /// https://gist.github.com/oliverheilig/7716793
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point indexs to keep.</param>
        private static void DouglasPeuckerReduction(IList<Point> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }

        }

        /// <summary>
        /// The distance of a point from a line made from point1 and point2.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static double PerpendicularDistance(Point Point1, Point Point2, Point Point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = √((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X * Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X * Point2.Y - Point1.X * Point.Y));
            double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
            double height = area / bottom * 2;

            return height;
        }



        public static Boolean PolyContainsPoint(IEnumerable<Point> Polygon,
                                                Point PointToCheck)
        {

            var inside = false;

            // An imaginary closing segment is implied,
            // so begin testing with that.
            var v1 = Polygon.Last();//[Polygon.Count - 1];

            foreach (var PointOfPolygon in Polygon)
            {

                var d1 = (PointToCheck.Y - PointOfPolygon.Y) * (v1.X - PointOfPolygon.X);
                var d2 = (PointToCheck.X - PointOfPolygon.X) * (v1.Y - PointOfPolygon.Y);

                if (PointToCheck.Y < v1.Y)
                {
                    // V1 below ray
                    if (PointOfPolygon.Y <= PointToCheck.Y)
                    {

                        // V0 on or above ray
                        // Perform intersection test
                        if (d1 > d2)
                            inside = !inside; // Toggle state

                    }
                }

                else if (PointToCheck.Y < PointOfPolygon.Y)
                {

                    // V1 is on or above ray, V0 is below ray
                    // Perform intersection test
                    if (d1 < d2)
                        inside = !inside; // Toggle state

                }

                v1 = PointOfPolygon; //Store previous endpoint as next startpoint

            }

            return inside;

        }

    }


    public struct Point : IEquatable<Point>
    {

        public Double X { get; }
        public Double Y { get; }


        public Point(Double x, Double y)
        {
            X = x;
            Y = y;
        }

        public Boolean Equals(Point other)
        {

            if (X != other.X)
                return false;

            if (Y != other.Y)
                return false;

            return true;

        }

    }

}
