/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

namespace org.GraphDefined.Vanaheimr.CLI
{

    public interface ICLICommand
    {
        IEnumerable<SuggestionResponse> Suggest(String[] Arguments);
        Task<String[]> Execute(String[] Arguments, CancellationToken CancellationToken);
        String Help();

        /// <summary>
        /// Whether the given command line may be kept in a history file, beyond
        /// the session it was typed in - false for one that carries a secret,
        /// such as a key given to be added. It is kept in the session's own
        /// history either way, as it stands on the screen anyway.
        /// </summary>
        /// <param name="Arguments">The command line, split into its words.</param>
        Boolean MayBeKept(String[] Arguments)
            => true;

    }

}
