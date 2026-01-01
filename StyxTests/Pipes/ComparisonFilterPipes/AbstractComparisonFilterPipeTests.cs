/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using NUnit.Framework;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.FilterPipes
{

    //[TestFixture]
    //public class AbstractComparisonFilterPipeTests
    //{
		
    //    #region testComparisons()

    //    [Test]
    //    public void testComparisons()
    //    {	

    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.EQUAL).CompareObjects(1, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.NOT_EQUAL).CompareObjects(1, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.EQUAL).CompareObjects(1, 2));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.NOT_EQUAL).CompareObjects(1, 2));
	
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN).CompareObjects(2, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN).CompareObjects(2, 2));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN).CompareObjects(2, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN).CompareObjects(2, 2));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN).CompareObjects(1, 2));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN).CompareObjects(1, 2));
	
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN_EQUAL).CompareObjects(2, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN_EQUAL).CompareObjects(2, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN_EQUAL).CompareObjects(1, 2));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN_EQUAL).CompareObjects(1, 2));
	
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN_EQUAL).CompareObjects(1, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN_EQUAL).CompareObjects(2, 1));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN_EQUAL).CompareObjects(1, 2));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN_EQUAL).CompareObjects(1, 1));
	
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.EQUAL).CompareObjects(null, null));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.NOT_EQUAL).CompareObjects(null, null));
    //        ClassicAssert.IsFalse(new BasicComparisonFilterPipe(ComparisonFilter.EQUAL).CompareObjects(1, null));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.NOT_EQUAL).CompareObjects(1, null));
	
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN_EQUAL).CompareObjects(1, null));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN_EQUAL).CompareObjects(null, 1));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.GREATER_THAN).CompareObjects(1, null));
    //        ClassicAssert.IsTrue(new BasicComparisonFilterPipe(ComparisonFilter.LESS_THAN).CompareObjects(null, 1));

    //    }

    //    #endregion
	
    //}


    //public class BasicComparisonFilterPipe : AbstractComparisonFilterPipe<IComparable, IComparable>
    //{

    //    public BasicComparisonFilterPipe(ComparisonFilter myComparisonFilter)
    //        : base(myComparisonFilter)
    //    { }

    //    public override Boolean MoveNext()
    //    {
    //        throw new NotImplementedException();
    //    }

    //}

	
}

