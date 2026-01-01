/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <https://www.github.com/Vanaheimr/Hermod>
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

    public interface IInternalDataBuilder
    {

        void     AddInternalData     (String Key, Object Value);

        Object?  GetInternalData     (String Key);

        T?       GetInternalDataAs<T>(String Key);

        void     IfDefined           (String Key, Action<Object> ValueDelegate);

        void     IfDefinedAs<T>      (String Key, Action<T>      ValueDelegate);

        Boolean  IsDefined           (String Key);

    }

}
