/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{
    public static class TimeSpanExt
    {

        public static TimeSpan Min(params TimeSpan[] TimeSpans)
        {

            if (TimeSpans == null)
                throw new ArgumentNullException("values");

            switch (TimeSpans.Length)
            {

                case 0: throw new ArgumentException();

                case 1: return TimeSpans[0];

                case 2:
                    return TimeSpans[0] < TimeSpans[1] ? TimeSpans[0] : TimeSpans[1];

                default:

                    TimeSpan best = TimeSpans[0];

                    for (int i = 1; i < TimeSpans.Length; i++)
                    {
                        if (TimeSpans[i] < best)
                        {
                            best = TimeSpans[i];
                        }
                    }

                    return best;

            }

        }

        public static TimeSpan Max(params TimeSpan[] TimeSpans)
        {

            if (TimeSpans == null)
                throw new ArgumentNullException("values");

            switch (TimeSpans.Length)
            {

                case 0: throw new ArgumentException();

                case 1: return TimeSpans[0];

                case 2:
                    return TimeSpans[0] > TimeSpans[1] ? TimeSpans[0] : TimeSpans[1];

                default:

                    TimeSpan best = TimeSpans[0];

                    for (int i = 1; i < TimeSpans.Length; i++)
                    {
                        if (TimeSpans[i] > best)
                        {
                            best = TimeSpans[i];
                        }
                    }

                    return best;

            }

        }

    }
}
