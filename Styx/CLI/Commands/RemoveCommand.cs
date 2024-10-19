/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    public class RemoveCommand(CLI CLI) : ACLICommand(CLI),
                                          ICLICommands
    {

        public static readonly String CommandName = nameof(RemoveCommand)[..^7].ToLowerFirstChar();

        public override IEnumerable<SuggestionResponse> Suggest(String[] args)
        {

            if (args.Length == 1 &&
                CommandName.StartsWith(args[0], StringComparison.CurrentCultureIgnoreCase))
            {
                return [ SuggestionResponse.Complete(CommandName) ];
            }

            if (args.Length == 2)
            {

                var name     = args[1];
                var keyList  = new List<SuggestionResponse>();

                foreach (var key in cli.Environment.Keys)
                {
                    if (key.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                        keyList.Add(SuggestionResponse.Complete($"{CommandName} {key}"));
                }

                return keyList;

            }

            return [];

        }

        public override Task<String[]> Execute(String[]           Arguments,
                                               CancellationToken  CancellationToken)
        {

            if (Arguments.Length < 2)
                return Task.FromResult<String[]>([$"Usage: {CommandName} <id>"]);

            var name = Arguments[1];

            if (cli.Environment.Remove(name))
                return Task.FromResult<String[]>([$"Item '{name}' removed."]);

            return Task.FromResult<String[]>([$"Item '{name}' not found!"]);

        }

        public override String Help()
        {
            return $"{CommandName} <id> - Removes an item with the specified id.";
        }

    }


}
