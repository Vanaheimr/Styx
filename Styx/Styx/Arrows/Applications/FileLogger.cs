/*
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    public class FileLogger<TEnum> : ILogger<TEnum>
        where TEnum : struct, IComparable, IFormattable, IConvertible
    {

        #region Data

        private StreamWriter LogfileWriter;

        #endregion

        #region Properties

        #region Logfile

        private String _Filename;

        public String Filename
        {

            get
            {
                return _Filename;
            }

            set
            {

                if (!String.IsNullOrEmpty(value))
                {

                    _Filename = value;

                    if (LogfileWriter != null)
                        LogfileWriter.Close();

                    // Default encoding will be "UTF8"!
                    LogfileWriter = File.AppendText(this._Filename);
                    LogfileWriter.AutoFlush = true;

                }

            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        public FileLogger(String Filename)
        {
            // The property setter will open the file for writting!
            this.Filename = Filename;
        }

        #endregion


        #region Log(Topics, params Arguments)

        public void Log(TEnum Topics, params Object[] Arguments)
        {
            Log(Timestamp.Now, Topics, Arguments);
        }

        #endregion

        #region Log(Timestamp, Topics, params Arguments)

        public void Log(DateTimeOffset Timestamp, TEnum Topics, params Object[] Arguments)
        {

            LogfileWriter.WriteLine(Timestamp + " - " + Arguments.Select(v => v.ToString()).AggregateOrDefault((a, b) => a + ", " + b, String.Empty));

        }

        #endregion

    }



}
