/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Balder <https://www.github.com/Vanaheimr/Balder>
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

using org.GraphDefined.Vanaheimr.Styx;

using NUnit.Framework;

#endregion

namespace org.GraphDefined.Vanaheimr.Balder.UnitTests.FilterPipes
{

    [TestFixture]
    public class DuplicateFilterPipeTest
    {
        
        #region testDuplicateFilter()

        [Test]
        public void testDuplicateFilter()
        {    

            //var _Starts = new List<String>() { "marko", "josh", "peter", "marko", "marko" };
            //var _dfp    = new DuplicateFilterPipe<String>();
            //_dfp.SetSource(_Starts.GetEnumerator());

            //int _Counter = 0;
            //int _Counter2 = 0;
            //while (_dfp.MoveNext())
            //{
            //    var next = _dfp.Current;
            //    ClassicAssert.IsTrue(next.Equals("josh") || next.Equals("peter") || next.Equals("marko"));
            //    if (next.Equals("marko"))
            //        _Counter2++;
            //    _Counter++;
            //}
            
            //ClassicAssert.AreEqual(3, _Counter);
            //ClassicAssert.AreEqual(1, _Counter2);

        }

        #endregion
    
    }
    
}

