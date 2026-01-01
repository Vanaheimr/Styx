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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    public class RemoveAllCommand(CLI CLI) : ACLICommand(CLI),
                                             ICLICommand
    {

        public static readonly String CommandName = nameof(RemoveAllCommand)[..^7].ToLowerFirstChar();

        public override IEnumerable<SuggestionResponse> Suggest(String[] args)
        {

            if (args.Length == 1 &&
                CommandName.StartsWith(args[0], StringComparison.CurrentCultureIgnoreCase))
            {
                return [ SuggestionResponse.CommandCompleted(CommandName) ];
            }

            return [];

        }

        public override Task<String[]> Execute(String[]           Arguments,
                                               CancellationToken  CancellationToken)
        {

            cli.Environment.Clear();

            return Task.FromResult<String[]>(["All environment keys removed."]);

        }

        public override String Help()
        {
            return "removeAll - Removes all environment keys.";
        }

    }




}
