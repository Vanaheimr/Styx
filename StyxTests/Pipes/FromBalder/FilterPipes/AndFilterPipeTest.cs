/*
 * Copyright (c) 2010-2023, Achim Friedland <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.FilterPipes
{

    [TestFixture]
    public class AndFilterPipeTest
    {

        #region testAndPipeBasic()

        [Test]
        public void testAndPipeBasic()
        {

            //var _Names = new List<String>() { "marko", "povel", "peter", "povel", "marko" };
            //var _Pipe1 = new ObjectFilterPipe<String>("marko", ComparisonFilter.NOT_EQUAL);
            //var _Pipe2 = new ObjectFilterPipe<String>("povel", ComparisonFilter.NOT_EQUAL);
            //var _AndFilterPipe = new AndFilterPipe<String>(new HasNextPipe<String>(_Pipe1), new HasNextPipe<String>(_Pipe2));
            //_AndFilterPipe.SetSourceCollection(_Names);

            //var _Counter = 0;
            //while (_AndFilterPipe.MoveNext())
            //    _Counter++;

            //ClassicAssert.AreEqual(0, _Counter);

        }

        #endregion

        #region testAndPipeBasic2()

        [Test]
        public void testAndPipeBasic2()
        {

            //var _Names          = new List<String>() { "marko", "povel", "peter", "povel", "marko" };
            //var _Pipe1          = new ObjectFilterPipe<String>("marko", ComparisonFilter.NOT_EQUAL);
            //var _Pipe2          = new ObjectFilterPipe<String>("marko", ComparisonFilter.NOT_EQUAL);
            //var _AndFilterPipe  = new AndFilterPipe<String>(new HasNextPipe<String>(_Pipe1), new HasNextPipe<String>(_Pipe2));
            //_AndFilterPipe.SetSourceCollection(_Names);

            //var _Counter = 0;
            //while (_AndFilterPipe.MoveNext())
            //    _Counter++;

            //ClassicAssert.AreEqual(2, _Counter);

        }

        #endregion

    }

}
