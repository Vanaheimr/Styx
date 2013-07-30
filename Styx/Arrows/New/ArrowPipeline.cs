﻿/*
 * Copyright (c) 2011-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
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

namespace eu.Vanaheimr.Styx
{

    public delegate IArrowSender<TOut> ArrowPipeline<TIn, TOut>(IArrowSender<TIn> INotification);


    public static partial class Pipeline
    {

        public static ArrowPipeline<TIn, TOut>
            CreateArrow<TIn, TOut>(ArrowPipeline<TIn, TOut>  Pipeline)
        {
            return Pipeline;
        }

        public static IArrowSender<TOut>
            AddPipeline<TIn, TOut>(this IArrowSender<TIn>   In,
                                   ArrowPipeline<TIn, TOut>  Pipeline)
        {
            return Pipeline(In);
        }

    }

}