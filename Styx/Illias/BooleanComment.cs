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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A boolean with a comment.
    /// </summary>
    public struct BooleanComment
    {

        #region Boolean

        /// <summary>
        /// The boolean value.
        /// </summary>
        public readonly Boolean Boolean;

        #endregion

        #region Comment

        /// <summary>
        /// The comment text.
        /// </summary>
        public readonly String Comment;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new boolean with a comment.
        /// </summary>
        /// <param name="myBoolean">The boolean value.</param>
        /// <param name="myComment">The maximum value.</param>
        public BooleanComment(Boolean myBoolean, String myComment = null)
        {
            Boolean = myBoolean;
            Comment = myComment;
        }

        #endregion


        #region (implicit) to Boolean

        /// <summary>
        /// Convert this object to a Boolean.
        /// </summary>
        /// <param name="BooleanWithComment">A BooleanComment object.</param>
        /// <returns>A boolean.</returns>
        public static implicit operator Boolean(BooleanComment BooleanWithComment)
        {
            return BooleanWithComment.Boolean;
        }

        #endregion

        #region (static) True

        /// <summary>
        /// Will return a value of 'true' without any comment text.
        /// </summary>
        public static BooleanComment True
        {
            get
            {
                return new BooleanComment(true);
            }
        }

        #endregion

    }

}
