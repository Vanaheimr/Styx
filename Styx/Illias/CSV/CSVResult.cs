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

    public enum CSVStatus
    {
        OK,
        warning,
        FILTERED,
        ERROR
    }

    public struct CSVResult
    {

        public readonly CSVStatus  Status;
        public readonly String     Message;

        public CSVResult(CSVStatus _Status, String _Message = "")
        {
            Status   = _Status;
            Message  = _Message;
        }

        public override String ToString()
        {
            return Status + "[" + Message + "]";
        }

    }

}
