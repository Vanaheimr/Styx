/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    /// <summary>
    /// Extension methods for "System.IO.File".
    /// </summary>
    public static partial class FileExtensions
    {

        #region TryDelete(Filename)

        /// <summary>
        /// Try to delete the given file.
        /// No exception will be thrown if the file does not exist or could not be deleted.
        /// </summary>
        /// <param name="FileName">The name of the file to delete.</param>
        public static void TryDelete(String FileName)
        {
            try
            {
                File.Delete(FileName);
            }
            catch { }
        }

        #endregion

    }

}
