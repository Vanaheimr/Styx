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

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.util
{

    [TestFixture]
    public class GatherPipeTest
    {

        #region testBasicGather()

        //[Test]
        //public void testBasicGather()
        //{

        //    var _Graph    = TinkerGraphFactory.CreateTinkerGraph();

        //    var _Pipe0    = new OutEdgesPipe<UInt64, Int64, String, String, Object,
        //                                     UInt64, Int64, String, String, Object,
        //                                     UInt64, Int64, String, String, Object,
        //                                     UInt64, Int64, String, String, Object>();
            
        //    var _Pipe1    = new InVertexPipe<UInt64, Int64, String, String, Object,
        //                                     UInt64, Int64, String, String, Object,
        //                                     UInt64, Int64, String, String, Object,
        //                                     UInt64, Int64, String, String, Object>();

        //    var _Pipe2 = new GatherPipe<IGenericPropertyVertex<UInt64, Int64, String, String, Object,
        //                                                       UInt64, Int64, String, String, Object,
        //                                                       UInt64, Int64, String, String, Object,
        //                                                       UInt64, Int64, String, String, Object>>();

        //    var _Pipeline = new Pipeline<IGenericPropertyVertex<UInt64, Int64, String, String, Object,
        //                                                        UInt64, Int64, String, String, Object,
        //                                                        UInt64, Int64, String, String, Object,
        //                                                        UInt64, Int64, String, String, Object>,

        //                                 IEnumerable<IGenericPropertyVertex<UInt64, Int64, String, String, Object,
        //                                                                    UInt64, Int64, String, String, Object,
        //                                                                    UInt64, Int64, String, String, Object,
        //                                                                    UInt64, Int64, String, String, Object>>>(_Pipe0, _Pipe1, _Pipe2);

        //    _Pipeline.SetSource(new SingleEnumerator<IGenericPropertyVertex<UInt64, Int64, String, String, Object,
        //                                                                    UInt64, Int64, String, String, Object,
        //                                                                    UInt64, Int64, String, String, Object,
        //                                                                    UInt64, Int64, String, String, Object>>(_Graph.VertexById(1).AsMutable()));

        //    while (_Pipeline.MoveNext())
        //        Console.WriteLine(_Pipeline.Current + "--->");// + pipeline.Path);

        //}

        #endregion

    }

}
