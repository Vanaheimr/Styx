/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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
    public class FutureFilterPipeTest
    {

        #region testBasicFutureFilter()

        [Test]
        public void testBasicFutureFilter()
        {

            //var _Names	= new List<String>() { "marko", "povel", "peter", "josh" };
            //var _Pipe1	= new FutureFilterPipe<String>(new IdentityPipe<String>());
            //_Pipe1.SetSourceCollection(_Names);

            //var _Counter = 0;
            //while (_Pipe1.MoveNext())
            //{
            //    _Counter++;
            //}

            //ClassicAssert.AreEqual(4, _Counter);

        }

        #endregion

        #region testAdvancedFutureFilter()

        [Test]
        public void testAdvancedFutureFilter()
        {

            //var _Names = new List<String>() { "marko", "povel", "peter", "josh" };
            //var _Pipe  = new FutureFilterPipe<String>(new CollectionFilterPipe<String>(new List<String>() { "marko", "povel" }, ComparisonFilter.EQUAL));
            //_Pipe.SetSourceCollection(_Names);

            //var _Counter = 0;
            //while (_Pipe.MoveNext())
            //{
            //    _Counter++;
            //    var _Name = _Pipe.Current;
            //    ClassicAssert.IsTrue(_Name.Equals("peter") || _Name.Equals("josh"));
            //}

            //ClassicAssert.AreEqual(2, _Counter);

        }

        #endregion

    }

}
