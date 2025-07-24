/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

        public String    PropertyName    { get; }
        public Object?   NewValue        { get; }
        public Object?   OldValue        { get; }
        public Context?  DataSource      { get; }

        public PropertyUpdateInfo(String    PropertyName,
                                  Object?   NewValue,
                                  Object?   OldValue     = null,
                                  Context?  DataSource   = null)
        {

            this.PropertyName  = PropertyName;
            this.NewValue      = NewValue;
            this.OldValue      = OldValue;
            this.DataSource    = DataSource;

        }

        public override String ToString()

            => $"Update of '{PropertyName}' '{OldValue?.ToString() ?? ""}' -> '{NewValue?.ToString() ?? ""}' {(DataSource is not null ? $"'{DataSource}'" : String.Empty)}!";

    }

    public class PropertyUpdateInfo<TId> : PropertyUpdateInfo
        where TId : IId
    {

        public TId  Id    { get; }

        public PropertyUpdateInfo(TId       Id,
                                  String    PropertyName,
                                  Object?   NewValue,
                                  Object?   OldValue     = null,
                                  Context?  DataSource   = null)

            : base(PropertyName,
                   NewValue,
                   OldValue,
                   DataSource)

        {

            this.Id = Id;

        }

        public override String ToString()

            => $"Update of '{Id}'.'{PropertyName}' '{OldValue?.ToString() ?? ""}' -> '{NewValue?.ToString() ?? ""}' {(DataSource is not null ? $"'{DataSource}'" : String.Empty)}!";

    }

}
