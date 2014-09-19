/*
 * Copyright (c) 2011-2013, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// An delegate representing a pipeline of arrows.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    /// <param name="ArrowSender">The sender of the messages/objects.</param>
    public delegate IArrowSender<TOut> ArrowPipeline<TIn, TOut>(IArrowSender<TIn> ArrowSender);


    /// <summary>
    /// A static helper class for styx pipelines.
    /// </summary>
    public static class Arrow
    {

        #region CreatePipeline(Pipeline)

        /// <summary>
        /// Create a arrow pipeline.
        /// </summary>
        /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
        /// <param name="Pipeline">The pipeline commands.</param>
        public static ArrowPipeline<TIn, TOut> CreatePipeline<TIn, TOut>(ArrowPipeline<TIn, TOut>  Pipeline)
        {
            return Pipeline;
        }

        #endregion

        #region AttachPipeline(this ArrowSender, Pipeline)

        /// <summary>
        /// Attach the given arrow pipeline to the given arrow sender.
        /// </summary>
        /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="Pipeline">The arrow processing pipeline.</param>
        public static IArrowSender<TOut> AttachPipeline<TIn, TOut>(this IArrowSender<TIn>    ArrowSender,
                                                                   ArrowPipeline<TIn, TOut>  Pipeline)
        {
            return Pipeline(ArrowSender);
        }

        #endregion

    }

}
