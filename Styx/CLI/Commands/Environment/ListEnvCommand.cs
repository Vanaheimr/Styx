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

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    public class ListEnvCommand(CLI CLI) : ACLICommand(CLI),
                                           ICLICommand
    {

        public static readonly String CommandName = nameof(ListEnvCommand)[..^7].ToLowerFirstChar();

        public override IEnumerable<SuggestionResponse> Suggest(String[] args)
        {

            if (CommandName.StartsWith(args[0], StringComparison.CurrentCultureIgnoreCase))
            {
                return [ SuggestionResponse.CommandCompleted(CommandName) ];
            }

            return [];

        }

        public override Task<String[]> Execute(String[]           Arguments,
                                               CancellationToken  CancellationToken)
        {

            if (cli.Environment.IsEmpty)
                return Task.FromResult<String[]>(["No environment keys to list!"]);


            var list = new List<String>() { "Listing all environment keys:" };

            foreach (var kvp in cli.Environment)
            {
                switch (kvp.Value.Count)
                {

                    case 0:
                        list.Add($"Key: '{kvp.Key}', no value(s)"); 
                        break;

                    case 1:
                        list.Add($"Key: '{kvp.Key}', Value: '{kvp.Value.First()}'");
                        break;

                    default:
                        list.Add($"Key: '{kvp.Key}', Values: '{kvp.Value.AggregateWith(", ")}'");
                        break;

                }
            }

            return Task.FromResult(list.ToArray());

        }

        public override String Help()
        {
            return "list - Lists all environment keys and their values";
        }

    }



}
