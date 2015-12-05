/*
 * Copyright (c) 2010-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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

using IlliasC = org.GraphDefined.Vanaheimr.Illias.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// Extention methods for zip pipes.
    /// </summary>
    public static class ZipTuplePipeExtensions
    {

        #region ZipTuple(this SourcePipe1, SourcePipe2, ZipDelegate)

        public static ZipTuplePipe<S1, S2> Zip<S1, S2>(this IEndPipe<S1>                        SourcePipe1,
                                                            IEndPipe<S2>                        SourcePipe2,
                                                            Func<S1, S2, IlliasC.Tuple<S1, S2>> ZipDelegate)
        {
            return new ZipTuplePipe<S1, S2>(SourcePipe1, SourcePipe2, ZipDelegate);
        }

        #endregion

        #region ZipTuple(this SourcePipe1, SourcePipe2, CountedZipDelegate)

        /// <summary>
        /// Starts with 1!
        /// </summary>
        public static ZipTuplePipe<S1, S2> Zip<S1, S2>(this IEndPipe<S1>                         SourcePipe1,
                                                            IEndPipe<S2>                         SourcePipe2,
                                                            Func<S1, S2, UInt64, IlliasC.Tuple<S1, S2>> CountedZipDelegate)
        {
            return new ZipTuplePipe<S1, S2>(SourcePipe1, SourcePipe2, CountedZipDelegate);
        }

        #endregion


        #region ZipTuple(this SourcePipe1, SourcePipe2, SourcePipe3, ZipDelegate)

        public static ZipTuplePipe<S1, S2, S3> Zip<S1, S2, S3>(this IEndPipe<S1>                     SourcePipe1,
                                                                    IEndPipe<S2>                     SourcePipe2,
                                                                    IEndPipe<S3>                     SourcePipe3,
                                                                    Func<S1, S2, S3, IlliasC.Tuple<S1, S2>> ZipDelegate)
        {
            return new ZipTuplePipe<S1, S2, S3>(SourcePipe1, SourcePipe2, SourcePipe3, ZipDelegate);
        }

        #endregion

        #region ZipTuple(this SourcePipe1, SourcePipe2, SourcePipe3, CountedZipDelegate)

        /// <summary>
        /// Starts with 1!
        /// </summary>
        public static ZipTuplePipe<S1, S2, S3> Zip<S1, S2, S3>(this IEndPipe<S1>                             SourcePipe1,
                                                                    IEndPipe<S2>                             SourcePipe2,
                                                                    IEndPipe<S3>                             SourcePipe3,
                                                                    Func<S1, S2, S3, UInt64, IlliasC.Tuple<S1, S2>> CountedZipDelegate)
        {
            return new ZipTuplePipe<S1, S2, S3>(SourcePipe1, SourcePipe2, SourcePipe3, CountedZipDelegate);
        }

        #endregion

    }


    #region ZipTuplePipe<S1, S2>

    /// <summary>
    /// Maps/converts pairs of consuming objects to emitting objects.
    /// </summary>
    /// <typeparam name="S1">The type of the consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the consuming objects.</typeparam>
    public class ZipTuplePipe<S1, S2> : AbstractPipe<S1, S2, IlliasC.Tuple<S1, S2>>
    {

        #region Data

        private readonly Func<Boolean>                        ProcessingDelegate;
        private readonly Func<S1, S2, IlliasC.Tuple<S1, S2>> ZipDelegate;
        private readonly Func<S1, S2, UInt64, IlliasC.Tuple<S1, S2>> CountedZipDelegate;
        private          UInt64                               Counter;

        #endregion

        #region Constructor(s)

        #region ZipTuplePipe(SourceValue1, SourceValue2, ZipDelegate)

        public ZipTuplePipe(S1                           SourceValue1,
                            S2                           SourceValue2,
                            Func<S1, S2, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourceValue1, SourceValue2)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourcePipe1, SourcePipe2, ZipDelegate)

        public ZipTuplePipe(IEndPipe<S1>                 SourcePipe1,
                            IEndPipe<S2>                 SourcePipe2,
                            Func<S1, S2, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourcePipe1, SourcePipe2)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerator1, SourceEnumerator2, ZipDelegate)

        public ZipTuplePipe(IEnumerator<S1>              SourceEnumerator1,
                            IEnumerator<S2>              SourceEnumerator2,
                            Func<S1, S2, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourceEnumerator1, SourceEnumerator2)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerable1, SourceEnumerable2, ZipDelegate)

        public ZipTuplePipe(IEnumerable<S1>              SourceEnumerable1,
                            IEnumerable<S2>              SourceEnumerable2,
                            Func<S1, S2, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourceEnumerable1, SourceEnumerable2)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion


        #region ZipTuplePipe(SourceValue1, SourceValue2, CountedZipDelegate)

        public ZipTuplePipe(S1                                   SourceValue1,
                            S2                                   SourceValue2,
                            Func<S1, S2, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourceValue1, SourceValue2)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourcePipe1, SourcePipe2, CountedZipDelegate)

        public ZipTuplePipe(IEndPipe<S1>                         SourcePipe1,
                            IEndPipe<S2>                         SourcePipe2,
                            Func<S1, S2, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourcePipe1, SourcePipe2)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerator1, SourceEnumerator2, CountedZipDelegate)

        public ZipTuplePipe(IEnumerator<S1>                      SourceEnumerator1,
                            IEnumerator<S2>                      SourceEnumerator2,
                            Func<S1, S2, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourceEnumerator1, SourceEnumerator2)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate     = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerable1, SourceEnumerable2, CountedZipDelegate)

        public ZipTuplePipe(IEnumerable<S1>                      SourceEnumerable1,
                            IEnumerable<S2>                      SourceEnumerable2,
                            Func<S1, S2, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourceEnumerable1, SourceEnumerable2)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate     = GetProcessingDelegate();

        }

        #endregion

        #endregion

        #region (private) GetProcessingDelegate()

        private Func<Boolean> GetProcessingDelegate()
        {

            if (SourcePipe1 == null || SourcePipe2 == null)
                return () => false;

            if (ZipDelegate != null)
                return () =>
                {

                    if (SourcePipe1.MoveNext() && SourcePipe2.MoveNext())
                    {

                        _CurrentElement = ZipDelegate(SourcePipe1.Current,
                                                         SourcePipe2.Current);

                        return true;

                    }

                    return false;

                };

            if (CountedZipDelegate != null)
                return () =>
                {

                    if (SourcePipe1.MoveNext() && SourcePipe2.MoveNext())
                    {

                        _CurrentElement = CountedZipDelegate(SourcePipe1.Current,
                                                                SourcePipe2.Current,
                                                                Counter++);

                        return true;

                    }

                    return false;

                };

            return () => false;

        }

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {
            return ProcessingDelegate();
        }

        #endregion

    }

    #endregion

    #region ZipTuplePipe<S1, S2, S3>

    /// <summary>
    /// Maps/converts pairs of consuming objects to emitting objects.
    /// </summary>
    /// <typeparam name="S1">The type of the consuming objects.</typeparam>
    /// <typeparam name="S2">The type of the consuming objects.</typeparam>
    /// <typeparam name="S3">The type of the consuming objects.</typeparam>
    public class ZipTuplePipe<S1, S2, S3> : AbstractPipe<S1, S2, S3, IlliasC.Tuple<S1, S2>>
    {

        #region Data

        private readonly Func<Boolean>                            ProcessingDelegate;
        private readonly Func<S1, S2, S3, IlliasC.Tuple<S1, S2>>          ZipDelegate;
        private readonly Func<S1, S2, S3, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate;
        private          UInt64                                   Counter;

        #endregion

        #region Constructor(s)

        #region ZipTuplePipe(SourceValue1, SourceValue2, SourceValue3, ZipDelegate)

        public ZipTuplePipe(S1                               SourceValue1,
                            S2                               SourceValue2,
                            S3                               SourceValue3,
                            Func<S1, S2, S3, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourceValue1, SourceValue2, SourceValue3)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourcePipe1, SourcePipe2, SourcePipe3, ZipDelegate)

        public ZipTuplePipe(IEndPipe<S1>                     SourcePipe1,
                            IEndPipe<S2>                     SourcePipe2,
                            IEndPipe<S3>                     SourcePipe3,
                            Func<S1, S2, S3, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourcePipe1, SourcePipe2, SourcePipe3)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerator1, SourceEnumerator2, SourceEnumerator3, ZipDelegate)

        public ZipTuplePipe(IEnumerator<S1>                  SourceEnumerator1,
                            IEnumerator<S2>                  SourceEnumerator2,
                            IEnumerator<S3>                  SourceEnumerator3,
                            Func<S1, S2, S3, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourceEnumerator1, SourceEnumerator2, SourceEnumerator3)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerable1, SourceEnumerable2, SourceEnumerable3, ZipDelegate)

        public ZipTuplePipe(IEnumerable<S1>                  SourceEnumerable1,
                            IEnumerable<S2>                  SourceEnumerable2,
                            IEnumerable<S3>                  SourceEnumerable3,
                            Func<S1, S2, S3, IlliasC.Tuple<S1, S2>>  ZipDelegate)

            : base(SourceEnumerable1, SourceEnumerable2, SourceEnumerable3)

        {

            this.ZipDelegate         = ZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion


        #region ZipTuplePipe(SourceValue1, SourceValue2, SourceValue3, CountedZipDelegate)

        public ZipTuplePipe(S1                                       SourceValue1,
                            S2                                       SourceValue2,
                            S3                                       SourceValue3,
                            Func<S1, S2, S3, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourceValue1, SourceValue2, SourceValue3)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourcePipe1, SourcePipe2, SourcePipe3, CountedZipDelegate)

        public ZipTuplePipe(IEndPipe<S1>                             SourcePipe1,
                            IEndPipe<S2>                             SourcePipe2,
                            IEndPipe<S3>                             SourcePipe3,
                            Func<S1, S2, S3, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourcePipe1, SourcePipe2, SourcePipe3)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerator1, SourceEnumerator2, SourceEnumerator3, CountedZipDelegate)

        public ZipTuplePipe(IEnumerator<S1>                          SourceEnumerator1,
                            IEnumerator<S2>                          SourceEnumerator2,
                            IEnumerator<S3>                          SourceEnumerator3,
                            Func<S1, S2, S3, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourceEnumerator1, SourceEnumerator2, SourceEnumerator3)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #region ZipTuplePipe(SourceEnumerable1, SourceEnumerable2, SourceEnumerable3, CountedZipDelegate)

        public ZipTuplePipe(IEnumerable<S1>                          SourceEnumerable1,
                            IEnumerable<S2>                          SourceEnumerable2,
                            IEnumerable<S3>                          SourceEnumerable3,
                            Func<S1, S2, S3, UInt64, IlliasC.Tuple<S1, S2>>  CountedZipDelegate)

            : base(SourceEnumerable1, SourceEnumerable2, SourceEnumerable3)

        {

            this.CountedZipDelegate  = CountedZipDelegate;
            this.ProcessingDelegate  = GetProcessingDelegate();

        }

        #endregion

        #endregion

        #region (private) GetProcessingDelegate()

        private Func<Boolean> GetProcessingDelegate()
        {

            if (SourcePipe1 == null || SourcePipe2 == null || SourcePipe3 == null)
                return () => false;

            if (ZipDelegate != null)
                return () =>
                {

                    if (SourcePipe1.MoveNext() && SourcePipe2.MoveNext() && SourcePipe3.MoveNext())
                    {

                        _CurrentElement = ZipDelegate(SourcePipe1.Current,
                                                         SourcePipe2.Current,
                                                         SourcePipe3.Current);

                        return true;

                    }

                    return false;

                };

            if (CountedZipDelegate != null)
                return () =>
                {

                    if (SourcePipe1.MoveNext() && SourcePipe2.MoveNext() && SourcePipe3.MoveNext())
                    {

                        _CurrentElement = CountedZipDelegate(SourcePipe1.Current,
                                                                SourcePipe2.Current,
                                                                SourcePipe3.Current,
                                                                Counter++);

                        return true;

                    }

                    return false;

                };

            return () => false;

        }

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {
            return ProcessingDelegate();
        }

        #endregion

    }

    #endregion

}
