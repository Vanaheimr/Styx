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

    public class RemoveEnvCommand(CLI CLI) : ACLICommand(CLI),
                                             ICLICommand
    {

        public static readonly String CommandName = nameof(RemoveEnvCommand)[..^7].ToLowerFirstChar();

        public override IEnumerable<SuggestionResponse> Suggest(String[] args)
        {

            if (args.Length == 1 &&
                CommandName.StartsWith(args[0], StringComparison.CurrentCultureIgnoreCase))
            {
                return [ SuggestionResponse.CommandCompleted(CommandName) ];
            }

            if (args.Length == 2)
            {

                var name     = args[1];
                var keyList  = new List<SuggestionResponse>();

                foreach (var key in cli.Environment.Keys.Select(key => key.ToString()))
                {

                    if (key.Equals(name, StringComparison.OrdinalIgnoreCase))
                        keyList.Add(SuggestionResponse.ParameterCompleted($"{CommandName} {key}"));

                    else if (key.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                        keyList.Add(SuggestionResponse.ParameterPrefix($"{CommandName} {key}"));

                }

                if (keyList.Count == 1 &&
                    keyList.First().Info == SuggestionInfo.ParameterPrefix)
                {
                    var suggestion = keyList.First().Suggestion;
                    keyList.Clear();
                    keyList.Add(SuggestionResponse.ParameterCompleted(suggestion));
                }

                return keyList;

            }

            return [];

        }

        public override Task<String[]> Execute(String[]           Arguments,
                                               CancellationToken  CancellationToken)
        {

            if (Arguments.Length >= 2)
            {

                var environmentKey = new EnvironmentKey(Arguments[1]);

                if (Arguments.Length == 2)
                {

                    if (cli.Environment.Remove(environmentKey, out _))
                        return Task.FromResult<String[]>([$"Environment key '{environmentKey}' removed"]);

                    return Task.FromResult<String[]>([$"Environment key '{environmentKey}' not found!"]);

                }

                if (Arguments.Length >= 3)
                {

                    var values = Arguments.Skip(2).ToArray();

                    try
                    {

                        var envValues = cli.Environment[environmentKey];

                        envValues.TryRemoveMultiple(values);

                        if (envValues.Count > 0)
                            return Task.FromResult<String[]>(
                                       values.Length == 1
                                           ? [$"Removed value '{values[0]}' from environment key '{environmentKey}'"]
                                           : [$"Removed values '{values.AggregateCSV()}' from environment key '{environmentKey}'"]
                                   );

                        // Empty list of values => Remove entire environment key!
                        if (cli.Environment.TryRemove(environmentKey, out _))
                            return Task.FromResult<String[]>([$"Environment key '{environmentKey}' removed"]);

                        return Task.FromResult<String[]>([$"Failed to remove environment key '{environmentKey}'!"]);

                    }
                    catch
                    {
                        return Task.FromResult<String[]>([$"Environment key '{environmentKey}' not found!"]);
                    }

                }

            }

            return Task.FromResult<String[]>([$"Usage: {CommandName} <key> <value1> [value2] ... [valueX]"]);

        }

        public override String Help()
        {
            return $"{CommandName} <key> <value1> [value2] ... [valueX] - Removes the given environment key or some of its values.";
        }

    }


}
