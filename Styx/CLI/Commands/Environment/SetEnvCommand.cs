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

    public class SetEnvCommand(CLI CLI) : ACLICommand(CLI),
                                          ICLICommand
    {

        public static readonly String CommandName = nameof(SetEnvCommand)[..^7].ToLowerFirstChar();

        public override IEnumerable<SuggestionResponse> Suggest(String[] args)
        {

            if (args.Length == 1)
            {

                if (CommandName.Equals(args[0], StringComparison.CurrentCultureIgnoreCase))
                {

                    var keyList = new List<SuggestionResponse> {
                        SuggestionResponse.CommandCompleted(CommandName)
                    };

                    foreach (var key in cli.Environment.Keys.Select(key => key.ToString()))
                        keyList.Add(SuggestionResponse.ParameterPrefix($"{CommandName} {key}"));

                    if (keyList.Count == 1 &&
                        keyList.First().Info == SuggestionInfo.ParameterPrefix)
                    {
                        var suggestion = keyList.First().Suggestion;
                        keyList.Clear();
                        keyList.Add(SuggestionResponse.ParameterCompleted(suggestion));
                    }

                    return keyList;

                }

                if (CommandName.StartsWith(args[0], StringComparison.CurrentCultureIgnoreCase))
                    return [SuggestionResponse.CommandPrefix(CommandName)];

            }

            if (args.Length == 2 &&
                CommandName.Equals(args[0], StringComparison.CurrentCultureIgnoreCase))
            {

                {

                    var name = args[1];
                    var keyList = new List<SuggestionResponse>();

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

            }

            return [];

        }

        public override Task<String[]> Execute(String[]           Arguments,
                                               CancellationToken  CancellationToken)
        {

            if (Arguments.Length >= 3)
            {

                var name   = EnvironmentKey.TryParse(Arguments[1]);
                var values = Arguments.Skip(2).ToArray();

                if (name is not null)
                {

                    cli.Environment.TryAdd(name.Value, []);
                    cli.Environment[name.Value].TrySet(values);

                    return Task.FromResult<String[]>(
                               values.Length == 1
                                   ? [$"Environment key set: '{name}' = {values[0]}"]
                                   : [$"Environment keys set: '{name}' = {values.AggregateWith(", ")}"]
                           );

                }

                return Task.FromResult<String[]>([$"could not be parse environment key '{name}'!"]);

            }

            return Task.FromResult<String[]>([$"Usage: {CommandName} <name> <value> [value2] ... [valueX]"]);

        }

        public override String Help()
        {
            return $"{CommandName} <name> <value> [value2] ... [valueX] - Adds an environment key with the specified name and value(s).";
        }

    }

}
