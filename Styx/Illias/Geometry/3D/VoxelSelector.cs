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

#region Usings

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Geometry
{

    /// <summary>
    /// A delegate selecting which voxels to return.
    /// </summary>
    /// <typeparam name="T">The internal datatype of the voxel.</typeparam>
    /// <param name="Voxel">A voxel of type T.</param>
    /// <returns>True if the voxel is selected; False otherwise.</returns>
    public delegate Boolean VoxelSelector<T>(IVoxel<T> Voxel)
        where T : IEquatable<T>, IComparable<T>, IComparable;

}
