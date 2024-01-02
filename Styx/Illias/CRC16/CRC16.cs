/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    public class CRC16
    {

        /// <summary>
        ///Funktion erwartet eine modbus message (länge egal) und gibt ein 2byte array (16bit)-crc aus (big endian).
        /// </summary>
        public static Byte[] GetCRC16(Byte[] message)
        {

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;
            Byte[] CRC = new Byte[2];

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {

                    CRCLSB  = (char)   (CRCFull       & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);

                }
            }

            CRC[1] = CRCHigh = (byte) ((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow  = (byte) ( CRCFull       & 0xFF);

            return CRC;

        }

        ///<summary>
        /// Berechnet den CRC der Angekommenen Daten (n-2 Bytes ) (über GetCRC-Methode) und 
        /// vergleicht sie mit den angekommenen CRC Bytes (byte n, n-1) */
        ///</summary>
        public static Boolean CheckCRC16(byte[] rx_data)
        {

            byte[] rx_crc = new byte[2];
            rx_crc = GetCRC16(rx_data);

            if (rx_crc[0] == rx_data[rx_data.Length - 2] && (rx_crc[1] == rx_data[rx_data.Length - 1]))
                return true;

            return false;

        }

    }

}
