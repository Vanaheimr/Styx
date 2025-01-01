/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Aegir <https://www.github.com/Vanaheimr/Aegir>
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

namespace org.GraphDefined.Vanaheimr.Aegir
{

    public enum HelmerttransformationsArt
    {
        DeutschlandWGS84nachDNDHPotsdamm2001,
        OesterreichWGS84nachMGIFerro
    }


    public class GaussKrueger2GeoCoordinate
    {

        //  Based on: GK_in_WGS84 Class
        //  Copyright (c) 2008 by Ingo Peczynski
        //  http://www.sky-maps.de
        //  ingo.peczynski@sky-maps.de

        // Compare to http://calc.gknavigation.de/

        private HelmerttransformationsArt HelmerttransformationsArt; 

        public GaussKrueger2GeoCoordinate(HelmerttransformationsArt HelmerttransformationsArt)
        {
            this.HelmerttransformationsArt = HelmerttransformationsArt;
        }

        public GeoCoordinate Transform(Double  Hochwert,
                                       Double  Rechtswert,
                                       Double  Hoehe,
                                       String  Bezugsmeridian = "Automatische Ermittlung")

        {

            //  Based on: GK_in_WGS84.cs
            //  Copyright (c) 2008 by Ingo Peczynski
            //  http://www.sky-maps.de
            //  ingo.peczynski@sky-maps.de

            // Compare to http://calc.gknavigation.de/


            #region Konstante Parameter

            // WGS84 Ellipsoid
            var WGS84_a   = 6378137.0;                                                              // große Halbachse
            var WGS84_b   = 6356752.3141;                                                           // kleine Halbachse
            var WGS84_e2  = (Math.Pow(WGS84_a, 2) - Math.Pow(WGS84_b, 2)) / Math.Pow(WGS84_a, 2);   // 1.Numerische Exzentrität
            var WGS84_f   = (WGS84_a - WGS84_b) / WGS84_a;                                          // Abplattung 1: fW

            // Bessel Ellipsoid
            var Bessel_a  = 6377397.155;
            var Bessel_b  = 6356078.962;
            var Bessel_e2 = (Bessel_a * Bessel_a - Bessel_b * Bessel_b) / (Bessel_a * Bessel_a);

            #endregion

            #region MeridianUmrechnung

            Int32 Meridianneu;

            if (Bezugsmeridian == "Automatische Ermittlung")
                Meridianneu = 3 * Convert.ToInt32(Rechtswert.ToString().Substring(0, 1));

            else
                Meridianneu = Convert.ToInt32(Bezugsmeridian);

            #endregion

            #region GK nach BL

            // Bessel Ellipsoid
            var n       = (Bessel_a - Bessel_b) / (Bessel_a + Bessel_b);
            var alpha   = (Bessel_a + Bessel_b) / 2.0 * (1.0 + 1.0 / 4.0 * n * n + 1.0 / 64.0 * Math.Pow(n, 4));
            var beta    =    3.0 /   2.0 * n     - 27.0 / 32.0 * Math.Pow(n, 3) + 269.0 / 512.0 * Math.Pow(n, 5);
            var gamma   =   21.0 /  16.0 * n * n - 55.0 / 32.0 * Math.Pow(n, 4);
            var delta   =  151.0 /  96.0 * Math.Pow(n, 3) - 417.0 / 128.0 * Math.Pow(n, 5);
            var epsilon = 1097.0 / 512.0 * Math.Pow(n, 4);

            var y0 = Meridianneu / 3.0;
            var y  = Rechtswert - y0 * 1000000 - 500000;

            var B0 = Hochwert / alpha;
            var Bf = B0 + beta * Math.Sin(2 * B0) + gamma * Math.Sin(4 * B0) + delta * Math.Sin(6 * B0) + epsilon * Math.Sin(8 * B0);

            var Nf = Bessel_a / Math.Sqrt(1.0 - Bessel_e2 * Math.Pow(Math.Sin(Bf), 2));
            var ETAf = Math.Sqrt((Bessel_a * Bessel_a) / (Bessel_b * Bessel_b) * Bessel_e2 * Math.Pow(Math.Cos(Bf), 2));
            var tf = Math.Tan(Bf);

            var b1 = tf / 2.0 / (Nf * Nf) * (-1.0 - (ETAf * ETAf)) * (y * y);
            var b2 = tf / 24.0 / Math.Pow(Nf, 4) * (5.0 + 3.0 * (tf * tf) + 6.0 * (ETAf * ETAf) - 6.0 * (tf * tf) * (ETAf * ETAf) - 4.0 * Math.Pow(ETAf, 4) - 9.0 * (tf * tf) * Math.Pow(ETAf, 4)) * Math.Pow(y, 4);

            var g_B = (Bf + b1 + b2) * 180 / Math.PI;

            var l1 = 1.0 / Nf / Math.Cos(Bf) * y;
            var l2 = 1.0 / 6.0 / Math.Pow(Nf, 3) / Math.Cos(Bf) * (-1.0 - 2.0 * (tf * tf) - (ETAf * ETAf)) * Math.Pow(y, 3);

            var g_L = Meridianneu + (l1 + l2) * 180 / Math.PI;

            #endregion

            #region Ellipsoid Vektoren in DHDN

            // Querkrümmunsradius
            var N = Bessel_a / Math.Sqrt(1.0 - Bessel_e2 * Math.Pow(Math.Sin(g_B / 180 * Math.PI), 2));

            // Ergebnis Vektoren
            var Bessel_x = (N + Hoehe) * Math.Cos(g_B / 180 * Math.PI) * Math.Cos(g_L / 180 * Math.PI);
            var Bessel_y = (N + Hoehe) * Math.Cos(g_B / 180 * Math.PI) * Math.Sin(g_L / 180 * Math.PI);
            var Bessel_z = (N * (Bessel_b * Bessel_b) / (Bessel_a * Bessel_a) + Hoehe) * Math.Sin(g_B / 180 * Math.PI);

            #endregion

            #region Parameter HelmertTransformation

            Double g_dx;
            Double g_dy;
            Double g_dz;
            Double g_ex;
            Double g_ey;
            Double g_ez;
            Double g_m;

            // HelmertTransformation
            switch (HelmerttransformationsArt)
            {

                // Deutschland von WGS84 nach DNDH/Potsdamm2001
                default:
                    g_dx = 598.1;       // Translation in X
                    g_dy =  73.7;       // Translation in Y
                    g_dz = 418.2;       // Translation in Z
                    g_ex =  -0.202;     // Drehwinkel in Bogensekunden un die x-Achse
                    g_ey =  -0.045;     // Drehwinkel in Bogensekunden un die y-Achse
                    g_ez =   2.455;     // Drehwinkel in Bogensekunden un die z-Achse
                    g_m  =   6.7;       // Maßstabsfaktor in ppm 
                    break;

                // Österreich von WGS84 nach MGI Ferro
                case HelmerttransformationsArt.OesterreichWGS84nachMGIFerro:
                    g_dx = 577.326;     // Translation in X
                    g_dy =  90.129;     // Translation in Y
                    g_dz = 463.919;     // Translation in Z
                    g_ex =  -5.137;     // Drehwinkel in Bogensekunden un die x-Achse
                    g_ey =  -1.474;     // Drehwinkel in Bogensekunden un die y-Achse
                    g_ez =  -5.297;     // Drehwinkel in Bogensekunden un die z-Achse
                    g_m  =   2.423;     // Maßstabsfaktor  in ppm
                    break;

            }

            #endregion

            #region Helmert

            // Umrechnung der Drehwinkel in Bogenmaß
            var exRad = (g_ex * Math.PI / 180.0) / 3600.0;
            var eyRad = (g_ey * Math.PI / 180.0) / 3600.0;
            var ezRad = (g_ez * Math.PI / 180.0) / 3600.0;

            // Maßstabsumrechnung
            var mEXP = 1 - g_m * Math.Pow(10, -6);

            // Drehmatrix
            //   1       Ez    -Ez
            // -Ez        1     Ex
            //  Ey      -Ex      1

            // Rotierende Vektoren = Drehmatrix * Vektoren in WGS84
            var RotVektor1 = 1.0 * Bessel_x + ezRad * Bessel_y + (-1.0 * eyRad * Bessel_z);
            var RotVektor2 = (-1.0 * ezRad) * Bessel_x + 1 * Bessel_y + exRad * Bessel_z;
            var RotVektor3 = (eyRad) * Bessel_x + (-1.0 * exRad) * Bessel_y + 1 * Bessel_z;

            // Maßstab berücksichtigen
            var RotVectorM1 = RotVektor1 * mEXP;
            var RotVectorM2 = RotVektor2 * mEXP;
            var RotVectorM3 = RotVektor3 * mEXP;

            // Translation anbringen
            // dxT = Drehmatrix * dx * m
            var dxT = 1.0 * g_dx * mEXP + ezRad * g_dy * mEXP + (-1.0 * eyRad) * g_dz * mEXP;
            var dyT = (-1.0 * ezRad) * g_dx * mEXP + 1.0 * g_dy * mEXP + exRad * g_dz * mEXP;
            var dzT = (eyRad) * g_dx * mEXP + (-1.0 * exRad) * g_dy * mEXP + 1 * g_dz * mEXP;

            // Vektoren jetzt in WGS84
            var WGS84_x = RotVectorM1 + dxT;
            var WGS84_y = RotVectorM2 + dyT;
            var WGS84_z = RotVectorM3 + dzT;

            #endregion

            #region Vektorenumrechnung

            var s  = Math.Sqrt(WGS84_x * WGS84_x + WGS84_y * WGS84_y);
            var T  = Math.Atan(WGS84_z * WGS84_a / (s * WGS84_b));
            var Bz = Math.Atan((WGS84_z + WGS84_e2 * (WGS84_a * WGS84_a) / WGS84_b * Math.Pow(Math.Sin(T), 3)) / (s - WGS84_e2 * WGS84_a * Math.Pow(Math.Cos(T), 3)));

            var Lz = Math.Atan(WGS84_y / WGS84_x);
            var N2 = WGS84_a / Math.Sqrt(1 - WGS84_e2 * Math.Pow(Math.Sin(Bz), 2));

            #endregion

            return new GeoCoordinate(Latitude. Parse(Bz * 180 / Math.PI),
                                     Longitude.Parse(Lz * 180 / Math.PI),
                                     Altitude .Parse(s / Math.Cos(Bz)));

        }

    }

}
