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

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{
    public interface IBinarySearchTree<TKey, TValue> : IImmutableMap<TKey, TValue>
        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

    {
        TKey Key { get; }
        IBinarySearchTree<TKey, TValue> Left  { get; }
        IBinarySearchTree<TKey, TValue> Right { get; }
        IBinarySearchTree<TKey, TValue> Add(TKey key, TValue value);
        IBinarySearchTree<TKey, TValue> Remove(TKey key);
        IBinarySearchTree<TKey, TValue> Search(TKey key);
    }
}
