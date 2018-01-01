/*
 * Copyright (c) 2014-2016, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr BouncyCastle <http://www.github.com/Vanaheimr/BouncyCastle>
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.BouncyCastle
{

    /// <summary>
    /// Security-related visualizations.
    /// </summary>
    public static partial class SecurityVisualization
    {

        #region DrunkenBishop(Fingerprint, SizeX = 17, SizeY = 9)

        /// <summary>
        /// Convert the given fingerprint into a security visualization.
        /// http://www.dirk-loss.de/sshvis/drunken_bishop.pdf
        /// </summary>
        /// <param name="Fingerprint">A fingerprint.</param>
        /// <param name="SizeX">The x-size of the visualization.</param>
        /// <param name="SizeY">The y-size of the visualization.</param>
        public static Byte[] DrunkenBishop(String Fingerprint, Byte SizeX = 17, Byte SizeY = 9)
        {

            return SecurityVisualization.DrunkenBishop(Fingerprint.
                                                           Replace(":", "").
                                                           Replace("-", "").
                                                           Replace(" ", "").
                                                           ToLower().
                                                           HexStringToByteArray(),
                                                       SizeX,
                                                       SizeY);

        }

        #endregion

        #region DrunkenBishop(Fingerprint, SizeX = 17, SizeY = 9)

        /// <summary>
        /// Convert the given fingerprint into a security visualization.
        /// http://www.dirk-loss.de/sshvis/drunken_bishop.pdf
        /// </summary>
        /// <param name="Fingerprint">A fingerprint.</param>
        /// <param name="SizeX">The x-size of the visualization.</param>
        /// <param name="SizeY">The y-size of the visualization.</param>
        public static Byte[] DrunkenBishop(Byte[] Fingerprint, Byte SizeX = 17, Byte SizeY = 9)
        {

            return SecurityVisualization.DrunkenBishop(Fingerprint,
                                                       new Byte[SizeX * SizeY],
                                                       (Data, CurrentPosition) => Data[CurrentPosition] += 1,
                                                       SizeX,
                                                       SizeY);

        }

        #endregion

        #region DrunkenEarl(Fingerprint, SizeX = 17, SizeY = 9)

        /// <summary>
        /// Convert the given fingerprint into a security visualization.
        /// http://www.dirk-loss.de/sshvis/drunken_bishop.pdf
        /// </summary>
        /// <param name="Fingerprint">A fingerprint.</param>
        /// <param name="SizeX">The x-size of the visualization.</param>
        /// <param name="SizeY">The y-size of the visualization.</param>
        public static List<UInt32> DrunkenEarl(String Fingerprint, UInt32 SizeX = 17, UInt32 SizeY = 9)
        {

            return SecurityVisualization.DrunkenBishop(Fingerprint.
                                                           Replace(":", "").
                                                           Replace("-", "").
                                                           Replace(" ", "").
                                                           ToLower().
                                                           HexStringToByteArray(),
                                                       new List<UInt32>() { SizeY/2*SizeX + SizeX/2 },
                                                       (Data, CurrentPosition) => Data.Add(CurrentPosition),
                                                       SizeX,
                                                       SizeY);

        }

        #endregion

        #region DrunkenEarl(Fingerprint, SizeX = 17, SizeY = 9)

        /// <summary>
        /// Convert the given fingerprint into a security visualization.
        /// http://www.dirk-loss.de/sshvis/drunken_bishop.pdf
        /// </summary>
        /// <param name="Fingerprint">A fingerprint.</param>
        /// <param name="SizeX">The x-size of the visualization.</param>
        /// <param name="SizeY">The y-size of the visualization.</param>
        public static List<UInt32> DrunkenEarl(Byte[] Fingerprint, UInt32 SizeX = 17, UInt32 SizeY = 9)
        {

            return SecurityVisualization.DrunkenBishop(Fingerprint,
                                                       new List<UInt32>() { SizeY/2*SizeX + SizeX/2 },
                                                       (Data, CurrentPosition) => Data.Add(CurrentPosition),
                                                       SizeX,
                                                       SizeY);

        }

        #endregion

        #region DrunkenBishop(Fingerprint, Data, Delegate, SizeX = 17, SizeY = 9)

        /// <summary>
        /// Convert the given fingerprint into a security visualization.
        /// http://www.dirk-loss.de/sshvis/drunken_bishop.pdf
        /// </summary>
        /// <param name="Fingerprint">A fingerprint.</param>
        /// <param name="Data">A data structure to work with.</param>
        /// <param name="Delegate">A delegate which might modify the given data structure at each step.</param>
        /// <param name="SizeX">The x-size of the visualization.</param>
        /// <param name="SizeY">The y-size of the visualization.</param>
        public static T DrunkenBishop<T>(String Fingerprint, T Data, Action<T, UInt32> Delegate, UInt32 SizeX = 17, UInt32 SizeY = 9)
        {

            return SecurityVisualization.DrunkenBishop(Fingerprint.
                                                           Replace(":", "").
                                                           Replace("-", "").
                                                           Replace(" ", "").
                                                           ToLower().
                                                           HexStringToByteArray(),
                                                       Data,
                                                       Delegate,
                                                       SizeX,
                                                       SizeY);

        }

        #endregion

        #region DrunkenBishop(Fingerprint, Data, Delegate, SizeX = 17, SizeY = 9)

        /// <summary>
        /// Convert the given fingerprint into a security visualization.
        /// http://www.dirk-loss.de/sshvis/drunken_bishop.pdf
        /// </summary>
        /// <param name="Fingerprint">A fingerprint.</param>
        /// <param name="Data">A data structure to work with.</param>
        /// <param name="Delegate">A delegate which might modify the given data structure at each step.</param>
        /// <param name="SizeX">The x-size of the visualization.</param>
        /// <param name="SizeY">The y-size of the visualization.</param>
        public static T DrunkenBishop<T>(Byte[] Fingerprint, T Data, Action<T, UInt32> Delegate, UInt32 SizeX = 17, UInt32 SizeY = 9)
        {

            #region Initial checks...

            if (SizeX < 5 || SizeX % 2 != 1)
                throw new ArgumentException();

            if (SizeY < 5 || SizeY % 2 != 1)
                throw new ArgumentException();

            #endregion

            // Start in the exact middle of the matrix, at '^'
            UInt32 CurrentPosition = (SizeX/2) * SizeY + (SizeY/2);

            foreach (var Move in Fingerprint.
                                     Select(ByteValue => Convert.ToString(ByteValue, 2).
                                                             PadLeft(8, '0').
                                                             SubTokens(2)).
                                     Select(part => part.Reverse().ToArray()).
                                     SelectMany(v => v))
            {

                //             1111111
                //   01234567890123456
                // +-----------------+x
                // 0|aTTTTTTTTTTTTTTTb|
                // 1|LMMMMMMMMMMMMMMMR|
                // 2|LMMMMMMMMMMMMMMMR|
                // 3|LMMMMMMMMMMMMMMMR|
                // 4|LMMMMMMM^MMMMMMMR|
                // 5|LMMMMMMMMMMMMMMMR|
                // 6|LMMMMMMMMMMMMMMMR|
                // 7|LMMMMMMMMMMMMMMMR|
                // 8|cBBBBBBBBBBBBBBBd|
                // +-----------------+
                // y

                #region a

                if (CurrentPosition == 0)
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition +=       0; break;
                        case "01": CurrentPosition +=       1; break;
                        case "10": CurrentPosition += SizeX  ; break;
                        case "11": CurrentPosition += SizeX+1; break;
                    }
                }

                #endregion

                #region T

                else if (CurrentPosition > 0 &
                         CurrentPosition < SizeX-1)
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -=       1; break;
                        case "01": CurrentPosition +=       1; break;
                        case "10": CurrentPosition += SizeX-1; break;
                        case "11": CurrentPosition += SizeX+1; break;
                    }
                }

                #endregion

                #region b

                else if (CurrentPosition == SizeX-1)
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -=       1; break;
                        case "01": CurrentPosition +=       0; break;
                        case "10": CurrentPosition += SizeX-1; break;
                        case "11": CurrentPosition += SizeX  ; break;
                    }
                }

                #endregion

                #region c

                else if (CurrentPosition == SizeX * (SizeY-1))
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -= SizeX  ; break;
                        case "01": CurrentPosition -= SizeX-1; break;
                        case "10": CurrentPosition +=       0; break;
                        case "11": CurrentPosition +=       1; break;
                    }
                }

                #endregion

                #region B

                else if (CurrentPosition > SizeX * (SizeY-1) &
                         CurrentPosition < SizeY *  SizeX-1)
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -= SizeX+1; break;
                        case "01": CurrentPosition -= SizeX-1; break;
                        case "10": CurrentPosition -=       1; break;
                        case "11": CurrentPosition +=       1; break;
                    }
                }

                #endregion

                #region d

                else if (CurrentPosition == SizeY * SizeX - 1)
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -= SizeX+1; break;
                        case "01": CurrentPosition -= SizeX  ; break;
                        case "10": CurrentPosition -=       1; break;
                        case "11": CurrentPosition +=       0; break;
                    }
                }

                #endregion

                #region L

                else if (CurrentPosition % SizeX == 0)
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -= SizeX  ; break;
                        case "01": CurrentPosition -= SizeX-1; break;
                        case "10": CurrentPosition += SizeX  ; break;
                        case "11": CurrentPosition += SizeX+1; break;
                    }
                }

                #endregion

                #region R

                else if (CurrentPosition % SizeX == SizeX-1)
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -= SizeX+1; break;
                        case "01": CurrentPosition -= SizeX  ; break;
                        case "10": CurrentPosition += SizeX-1; break;
                        case "11": CurrentPosition += SizeX  ; break;
                    }
                }

                #endregion

                #region M

                else
                {
                    switch (Move)
                    {
                        case "00": CurrentPosition -= SizeX+1; break;
                        case "01": CurrentPosition -= SizeX-1; break;
                        case "10": CurrentPosition += SizeX-1; break;
                        case "11": CurrentPosition += SizeX+1; break;
                    }
                }

                #endregion

                Delegate(Data, CurrentPosition);

            }

            return Data;

        }

        #endregion

    }

}
