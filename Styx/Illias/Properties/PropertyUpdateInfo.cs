﻿/*
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    public class PropertyUpdateInfo
    {

        public String   PropertyName    { get; }
        public Object?  OldValue        { get; }
        public Object?  NewValue        { get; }

        public PropertyUpdateInfo(String   PropertyName,
                                  Object?  OldValue,
                                  Object?  NewValue)
        {

            this.PropertyName  = PropertyName;
            this.OldValue      = OldValue;
            this.NewValue      = NewValue;

        }

        public override String ToString()

            => String.Concat(
                   "Update of '", PropertyName, "' '",
                   OldValue != null ? OldValue.ToString() : "",
                   "' -> '",
                   NewValue != null ? NewValue.ToString() : "",
                   "'!"
               );

    }

    public class PropertyUpdateInfo<TId> : PropertyUpdateInfo
        where TId : IId
    {

        public TId  Id    { get; }

        public PropertyUpdateInfo(TId      Id,
                                  String   PropertyName,
                                  Object?  OldValue,
                                  Object?  NewValue)

            : base(PropertyName,
                   OldValue,
                   NewValue)

        {
            this.Id = Id;
        }

        public override String ToString()

            => String.Concat(
                   "Update of '", Id, "'.'", PropertyName, "' '",
                   OldValue != null ? OldValue.ToString() : "",
                   "' -> '",
                   NewValue != null ? NewValue.ToString() : "",
                   "'!"
               );

    }

}