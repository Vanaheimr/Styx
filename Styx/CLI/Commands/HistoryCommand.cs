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

    public class HistoryCommand(CLI CLI) : ACLICommand(CLI),
                                           ICLICommands
    {

        public static readonly String CommandName = nameof(HistoryCommand)[..^7].ToLowerFirstChar();


     //   private readonly IEnumerable<String> commandHistory = CLI.CommandHistory;

        public override IEnumerable<SuggestionResponse> Suggest(String[] args)
        {

            if (CommandName.StartsWith(args[0], StringComparison.CurrentCultureIgnoreCase))
            {
                return [ SuggestionResponse.Complete(CommandName) ];
            }

            return [];

        }

        public override Task<String[]> Execute(String[]           Arguments,
                                               CancellationToken  CancellationToken)
        {

            if (!cli.CommandHistory.Any())
                return Task.FromResult<String[]>(["No commands in history!"]);


            var list = new List<String>() { "Command History:" };

            foreach (var command in cli.CommandHistory)
                list.Add(command);

            return Task.FromResult(list.ToArray());

        }

        public override String Help()
        {
            return "history - Displays the command history.";
        }

    }

}
