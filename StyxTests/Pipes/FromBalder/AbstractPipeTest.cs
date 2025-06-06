﻿/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;
using System.Collections.Generic;

using NUnit.Framework;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Pipes
{

    [TestFixture]
    public class AbstractPipeTest
    {

        #region TestIEnumerator()

        [Test]
        public void TestIEnumerator()
        {
            
            //var names = new List<String>() { "marko", "josh", "peter" };

            //IPipe<String, String> pipe = new IdentityPipe<String>();
            //pipe.SetSourceCollection(names);

            //var counter = 0UL;
            //while (pipe.MoveNext())
            //{
            //    counter++;
            //    String name = pipe.Current;
            //    ClassicAssert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
            //}
            
            //ClassicAssert.AreEqual(counter, 3UL);
            //pipe.SetSourceCollection(names);
            //counter = 0UL;
            
            //foreach (var name in pipe)
            //{
            //    ClassicAssert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
            //    counter++;
            //}
            
            //ClassicAssert.AreEqual(counter, 3UL);

        }

        #endregion

        #region testPathConstruction

        //[Test]
        //public void testPathConstruction()
        //{

        //    var _Graph = TinkerGraphFactory.CreateTinkerGraph();

        //    var _Marko = _Graph.VertexById(1).AsMutable();

        //    var _Pipe1 = new OutEdgesPipe<UInt64, Int64, String, String, Object,
        //                                  UInt64, Int64, String, String, Object,
        //                                  UInt64, Int64, String, String, Object,
        //                                  UInt64, Int64, String, String, Object>();

        //    var _Pipe2 = new InVertexPipe<UInt64, Int64, String, String, Object,
        //                                  UInt64, Int64, String, String, Object,
        //                                  UInt64, Int64, String, String, Object,
        //                                  UInt64, Int64, String, String, Object>();

        //    var _Pipe3 = new PropertyPipe<String, Object>(Keys: "name");

        //    _Pipe3.SetSource(_Pipe2);
        //    _Pipe2.SetSource(_Pipe1);

        //    var _MarkoList = new List<IGenericPropertyVertex<UInt64, Int64, String, String, Object,
        //                                                     UInt64, Int64, String, String, Object,
        //                                                     UInt64, Int64, String, String, Object,
        //                                                     UInt64, Int64, String, String, Object>>() { _Marko };

        //    _Pipe1.SetSource(_MarkoList.GetEnumerator());

        //    foreach (var _Name in _Pipe3)
        //    {

        //        var path = _Pipe3.Path;

        //        ClassicAssert.AreEqual(_Marko,                 path[0]);
        //        ClassicAssert.AreEqual(typeof(IGenericPropertyEdge<UInt64, Int64, String, String, Object,
        //                                                    UInt64, Int64, String, String, Object,
        //                                                    UInt64, Int64, String, String, Object,
        //                                                    UInt64, Int64, String, String, Object>), path[1].GetType());

        //        ClassicAssert.AreEqual(typeof(IGenericPropertyVertex<UInt64, Int64, String, String, Object,
        //                                                      UInt64, Int64, String, String, Object,
        //                                                      UInt64, Int64, String, String, Object,
        //                                                      UInt64, Int64, String, String, Object>), path[2].GetType());

        //        ClassicAssert.AreEqual(typeof(String),         path[3].GetType());

        //        if (_Name.ToString() == "vadas")
        //        {
        //            ClassicAssert.AreEqual(_Graph.EdgeById(7),     path[1]);
        //            ClassicAssert.AreEqual(_Graph.VertexById(2), path[2]);
        //            ClassicAssert.AreEqual("vadas", path[3]);
        //        }

        //        else if (_Name.ToString() == "lop")
        //        {
        //            ClassicAssert.AreEqual(_Graph.EdgeById(9),     path[1]);
        //            ClassicAssert.AreEqual(_Graph.VertexById(3), path[2]);
        //            ClassicAssert.AreEqual("lop", path[3]);
        //        }

        //        else if (_Name.ToString() == "josh")
        //        {
        //            ClassicAssert.AreEqual(_Graph.EdgeById(8),     path[1]);
        //            ClassicAssert.AreEqual(_Graph.VertexById(4), path[2]);
        //            ClassicAssert.AreEqual("josh", path[3]);
        //        }

        //        else
        //            Assert.Fail();

        //    }

        //}

        #endregion

    }

}
