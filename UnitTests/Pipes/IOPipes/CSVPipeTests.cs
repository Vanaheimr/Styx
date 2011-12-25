/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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

using NUnit.Framework;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes.UnitTests.Pipes
{

    [TestFixture]
    public class CSVPipeTests
    {

        #region testCSVPipeNormal()

        [Test]
        public void testCSVPipeNormal()
        {

            var _Pipe = new CSVPipe(ExpectedNumberOfColumns:    5,
                                    FailOnWrongNumberOfColumns: true,
                                    IEnumerable: new List<String>() {
                                                     "#Id,Name,Verb,Help,Action",
                                                     "0,Alice,loves,to,read",
                                                     "1,Bob,likes,to,ski"
                                                 });

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                Assert.IsTrue(_Pipe.Current[1] == "Alice" | _Pipe.Current[1] == "Bob");
                _Counter++;
            }

            Assert.AreEqual(_Counter, 2);

        }

        #endregion

        #region testCSVPipeNormal__()

        [Test]
        public void testCSVPipeNormal__()
        {

            var _Result = new List<String>() { "#Id,Name,Verb,Help,Action",
                                               "0, Alice ,loves,to,read",
                                               "1,Bob ,likes,to,ski" }.
                          CSVPipe(ExpectedNumberOfColumns:    5,
                                  FailOnWrongNumberOfColumns: true,
                                  TrimColumns:                true).
                          ToArray();

            Assert.AreEqual(2, _Result.Length);
            Assert.AreEqual("Alice", _Result[0][1]);
            Assert.AreEqual("Bob",   _Result[1][1]);

        }

        #endregion

        #region testCSVPipeNormal2()

        [Test]
        public void testCSVPipeNormal2()
        {

            var _Pipe = new CSVPipe(StringSplitOptions: StringSplitOptions.RemoveEmptyEntries,
                                    IEnumerable:        new List<String>() {
                                                            "#Id,Name,Friendlist",
                                                            "    0,Alice,   a,,b,c, ,d,e     ,f,g   ",
                                                            "",
                                                            ",",
                                                            "1,Bob,a,g,h"
                                                        });

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                Assert.IsTrue(_Pipe.Current[1] == "Alice" | _Pipe.Current[1] == "Bob");
                Assert.IsTrue(_Pipe.Current[3] == "b"     | _Pipe.Current[3] == "g");
                _Counter++;
            }

            Assert.AreEqual(_Counter, 2);

        }

        #endregion

        //#region testActionPipeZero

        //[Test]
        //public void testActionPipeZero()
        //{

        //    var _Sum     = 0;
        //    var _Numbers = new List<Int32>();
        //    var _Pipe    = new ActionPipe<Int32>((_Int32) => _Sum += _Int32);
        //    _Pipe.SetSourceCollection(_Numbers);

        //    var _Counter = 0;
        //    Assert.IsFalse(_Pipe.Any());
        //    Assert.AreEqual(_Counter, 0);
        //    Assert.AreEqual(_Sum,     0);
        //    Assert.IsFalse(_Pipe.Any());

        //}

        //#endregion

        //#region testActionPipeNull()

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void testActionPipeNull()
        //{

        //    Action<Int32> myAction = null;
        //    var _Pipe = new ActionPipe<Int32>(myAction);

        //}

        //#endregion

    }

}
