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

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.SideeffectPipes
{

    [TestFixture]
    public class GroupCountPipeTests
    {

        #region testCountCombinePipeNormal()

        [Test]
        public void testCountCombinePipeNormal()
        {

            //var _Names = new List<String>() { "marko", "josh", "peter", "peter", "peter", "josh" };
            //var _Pipe  = new GroupCountPipe<String>(_Names);
            //_Pipe.SetSourceCollection(_Names);

            //var _Counter = 0;
            //foreach (var name in _Pipe)
            //{
            //    ClassicAssert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
            //    _Counter++;
            //}

            //ClassicAssert.AreEqual(6UL, _Counter);
            //ClassicAssert.AreEqual(1UL, _Pipe.SideEffect["marko"]);
            //ClassicAssert.AreEqual(2UL, _Pipe.SideEffect["josh"]);
            //ClassicAssert.AreEqual(3UL, _Pipe.SideEffect["peter"]);

            //ClassicAssert.IsFalse(_Pipe.SideEffect.ContainsKey("povel"));

        }

        #endregion

        #region testCountCombinePipeZero()

        [Test]
        public void testCountCombinePipeZero()
        {

            //var _Names = new List<String>();
            //var _Pipe  = new GroupCountPipe<String>();
            //_Pipe.SetSourceCollection(_Names);

            //ClassicAssert.IsFalse(_Pipe.MoveNext());
            //ClassicAssert.IsFalse(_Pipe.SideEffect.ContainsKey("povel"));

        }

        #endregion

    }

}
