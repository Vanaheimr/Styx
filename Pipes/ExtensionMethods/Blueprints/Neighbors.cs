/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET
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
using System.Collections.Generic;

using de.ahzf.blueprints;

#endregion

namespace de.ahzf.Pipes.ExtensionMethods
{

    /// <summary>
    /// A class of specialized pipelines returning the
    /// adjacent vertices (the neighborhood) of a vertex.
    /// </summary>
    public static class NeighborsExtensions
    {

        //#region Neighbor(this myIEnumerable)

        ///// <summary>
        ///// A specialized pipeline returning the first adjacent vertex.
        ///// </summary>
        ///// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        ///// <returns>A single IPropertyVertex.</returns>
        //public static IPropertyVertex Neighbor(this IEnumerable<IPropertyVertex> myIEnumerable)
        //{
        //    return myIEnumerable.Neighbors().First();
        //}

        //#endregion

        //#region Neighbor(this myIEnumerable)

        ///// <summary>
        ///// A specialized pipeline returning the first adjacent vertex
        ///// filtered by the connecting edge label.
        ///// </summary>
        ///// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        ///// <param name="myLabel">The edge label.</param>
        ///// <returns>A single IPropertyVertex.</returns>
        //public static IPropertyVertex Neighbor(this IEnumerable<IPropertyVertex> myIEnumerable, String myLabel)
        //{
        //    return myIEnumerable.Neighbors(myLabel).First();
        //}

        //#endregion

        //#region Neighbors(this myIEnumerable)

        ///// <summary>
        ///// A specialized pipeline returning the adjacent vertices.
        ///// </summary>
        ///// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        ///// <returns>A collection of objects implementing IPropertyVertex.</returns>
        //public static IEnumerable<IPropertyVertex> Neighbors(this IEnumerable<IPropertyVertex> myIEnumerable)
        //{

        //    var _Pipe1 = new VertexEdgePipe(Pipes.Steps.VertexEdgeStep.OUT_EDGES);
        //    _Pipe1.SetSourceCollection(myIEnumerable);

        //    var _Pipe2 = new EdgeVertexPipe(Pipes.Steps.EdgeVertexStep.IN_VERTEX);
        //    _Pipe2.SetSource(_Pipe1);

        //    return _Pipe2.Distinct();

        //}

        //#endregion

        //#region Neighbors(this myIEnumerable, myLabel)

        ///// <summary>
        ///// A specialized pipeline returning the adjacent vertices
        ///// filtered by the connecting edge label.
        ///// </summary>
        ///// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        ///// <param name="myLabel">The edge label.</param>
        ///// <returns>A collection of objects implementing IPropertyVertex.</returns>
        //public static IEnumerable<IPropertyVertex> Neighbors(this IEnumerable<IPropertyVertex> myIEnumerable, String myLabel)
        //{

        //    var _Pipe1 = new VertexEdgePipe(Pipes.Steps.VertexEdgeStep.OUT_EDGES);
        //    _Pipe1.SetSourceCollection(myIEnumerable);

        //    var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
        //    _Pipe2.SetSource(_Pipe1);

        //    var _Pipe3 = new EdgeVertexPipe(Pipes.Steps.EdgeVertexStep.IN_VERTEX);
        //    _Pipe3.SetSource(_Pipe2);

        //    return _Pipe3.Distinct();

        //}

        //#endregion


        //#region Neighbor(this myIEnumerator)

        ///// <summary>
        ///// A specialized pipeline returning the first adjacent vertex.
        ///// </summary>
        ///// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        ///// <returns>A single IPropertyVertex.</returns>
        //public static IPropertyVertex Neighbor(this IEnumerator<IPropertyVertex> myIEnumerator)
        //{
        //    return myIEnumerator.Neighbors().First();
        //}

        //#endregion

        //#region Neighbor(this myIEnumerable)

        ///// <summary>
        ///// A specialized pipeline returning the first adjacent vertex
        ///// filtered by the connecting edge label.
        ///// </summary>
        ///// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        ///// <param name="myLabel">The edge label.</param>
        ///// <returns>A single IPropertyVertex.</returns>
        //public static IPropertyVertex Neighbor(this IEnumerator<IPropertyVertex> myIEnumerator, String myLabel)
        //{
        //    return myIEnumerator.Neighbors(myLabel).First();
        //}

        //#endregion

        //#region Neighbors(this myIEnumerable)

        ///// <summary>
        ///// A specialized pipeline returning the adjacent vertices.
        ///// </summary>
        ///// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        ///// <returns>A collection of objects implementing IPropertyVertex.</returns>
        //public static IEnumerable<IPropertyVertex> Neighbors(this IEnumerator<IPropertyVertex> myIEnumerator)
        //{

        //    var _Pipe1 = new VertexEdgePipe(Pipes.Steps.VertexEdgeStep.OUT_EDGES);
        //    _Pipe1.SetSource(myIEnumerator);

        //    var _Pipe2 = new EdgeVertexPipe(Pipes.Steps.EdgeVertexStep.IN_VERTEX);
        //    _Pipe2.SetSource(_Pipe1);

        //    return _Pipe2.Distinct();

        //}

        //#endregion

        //#region Neighbors(this myIEnumerable, myLabel)

        ///// <summary>
        ///// A specialized pipeline returning the adjacent vertices
        ///// filtered by the connecting edge label.
        ///// </summary>
        ///// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        ///// <param name="myLabel">The edge label.</param>
        ///// <returns>A collection of objects implementing IPropertyVertex.</returns>
        //public static IEnumerable<IPropertyVertex> Neighbors(this IEnumerator<IPropertyVertex> myIEnumerator, String myLabel)
        //{

        //    var _Pipe1 = new VertexEdgePipe(Pipes.Steps.VertexEdgeStep.OUT_EDGES);
        //    _Pipe1.SetSource(myIEnumerator);

        //    var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
        //    _Pipe2.SetSource(_Pipe1);

        //    var _Pipe3 = new EdgeVertexPipe(Pipes.Steps.EdgeVertexStep.IN_VERTEX);
        //    _Pipe3.SetSource(_Pipe2);

        //    return _Pipe3.Distinct();

        //}

        //#endregion



        //#region IsComplicated(this myIEnumerable)

        ///// <summary>
        ///// A specialized pipeline returning the adjacent vertices.
        ///// </summary>
        ///// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        ///// <returns>A collection of objects implementing IPropertyVertex.</returns>
        //public static IEnumerable<Boolean> IsComplicated(this IEnumerable<IPropertyVertex> myIEnumerable)
        //{

        //    foreach (var _User in myIEnumerable)
        //    {

        //        var _IsComplicated = false;

        //        var _Pipe1 = new VertexEdgePipe(Pipes.Steps.VertexEdgeStep.OUT_EDGES);
        //        _Pipe1.SetSource(new SingleEnumerator<IPropertyVertex>(_User));

        //        var _Pipe2 = new LabelFilterPipe("loves", ComparisonFilter.NOT_EQUAL);
        //        _Pipe2.SetSource(_Pipe1);

        //        var _Pipe3 = new EdgeVertexPipe(Pipes.Steps.EdgeVertexStep.IN_VERTEX);
        //        _Pipe3.SetSource(_Pipe2);

        //        var _Lover1 = _Pipe3.ToList();

        //        // More than one lover... it's complicated!
        //        if (_Lover1.Count > 1)
        //            _IsComplicated = true;

        //        else
        //        {

        //            var _Pipe4 = new VertexEdgePipe(Pipes.Steps.VertexEdgeStep.OUT_EDGES);
        //            _Pipe4.SetSourceCollection(_Lover1);

        //            var _Pipe5 = new LabelFilterPipe("loves", ComparisonFilter.NOT_EQUAL);
        //            _Pipe5.SetSource(_Pipe4);

        //            var _Pipe6 = new EdgeVertexPipe(Pipes.Steps.EdgeVertexStep.IN_VERTEX);
        //            _Pipe6.SetSource(_Pipe5);

        //            var _Lover2 = _Pipe6.ToList();

        //            // More than one lover... it's complicated!
        //            if (_Lover2.Count > 1 || _User != _Lover2[0])
        //                _IsComplicated = true;

        //        }

        //        if (_IsComplicated)
        //            yield return true;

        //        else
        //            yield return false;

        //    }

        //}

        //#endregion

    }

}
