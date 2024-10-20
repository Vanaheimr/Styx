﻿/*
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

    public class AddCommand(CLI CLI) : ACLICommand(CLI),
                                       ICLICommand
    {

        public static readonly String CommandName = nameof(AddCommand)[..^7].ToLowerFirstChar();

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

            if (Arguments.Length == 3)
            {

                var name  = Arguments[1];
                var value = Arguments[2];

                cli.Environment[name] = value;

                return Task.FromResult<String[]>([$"Item added: {name} = {value}"]);

            }

            return Task.FromResult<String[]>([$"Usage: {CommandName} <name> <value>"]);

        }

        public override String Help()
        {
            return $"{CommandName} <name> <value> - Adds an item with the specified name and value.";
        }

    }

}
