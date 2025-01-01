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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    #region CommandLineParserOption

    /// <summary>
    /// A fluent interface to add command line parser options.
    /// </summary>
    public class CommandLineParserOption
    {

        #region Data

        private CommandLineParser CommandLineParser;

        private Nullable<Char> _Short;

        #endregion

        #region Properties

        /// <summary>
        /// The short option.
        /// </summary>
        public Char                  ShortOption         { get { return _Short.Value; } }

        /// <summary>
        /// The long option.
        /// </summary>
        public String                LongOption          { get; private set; }

        /// <summary>
        /// An optional regular expression for verification.
        /// </summary>
        public Regex                 RegularExpression   { get; private set; }

        /// <summary>
        /// An optional delegate for verification.
        /// </summary>
        public Func<String, Boolean> Verification              { get; private set; }

        /// <summary>
        /// The action delegate.
        /// </summary>
        public Action<String>        Action              { get; private set; }

        #endregion

        #region Constructor(s)

        #region CommandLineParserOption(CommandLineParser)

        /// <summary>
        /// Create a new fluent interface to add command line parser options.
        /// </summary>
        /// <param name="CommandLineParser">The command line parser.</param>
        public CommandLineParserOption(CommandLineParser CommandLineParser)
        {
            this.CommandLineParser = CommandLineParser;
        }

        #endregion

        #region CommandLineParserOption(ShortOption, CommandLineParser)

        /// <summary>
        /// Create a new fluent interface to add command line parser options.
        /// </summary>
        /// <param name="ShortOption">The short option.</param>
        /// <param name="CommandLineParser">The command line parser.</param>
        public CommandLineParserOption(Char ShortOption, CommandLineParser CommandLineParser)
            : this(CommandLineParser)
        {
            this._Short = new Nullable<Char>(ShortOption);
        }

        #endregion

        #region CommandLineParserOption(LongOption, CommandLineParser)

        /// <summary>
        /// Create a new fluent interface to add command line parser options.
        /// </summary>
        /// <param name="LongOption">The long option.</param>
        /// <param name="CommandLineParser">The command line parser.</param>
        public CommandLineParserOption(String LongOption, CommandLineParser CommandLineParser)
            : this(CommandLineParser)
        {

            if (LongOption == null || LongOption.Trim() == "")
                throw new ArgumentNullException("String", "The parameter must not be null or empty!");

            this.LongOption = LongOption.Trim();

        }

        #endregion

        #endregion


        #region Short(ShortOption)

        /// <summary>
        /// Set the short option.
        /// </summary>
        /// <param name="ShortOption">The short option.</param>
        public CommandLineParserOption Short(Char ShortOption)
        {
            this._Short = new Nullable<Char>(ShortOption);
            return this;
        }

        #endregion

        #region Long(LongOption)

        /// <summary>
        /// Set the long option.
        /// </summary>
        /// <param name="LongOption">The long option.</param>
        public CommandLineParserOption Long(String LongOption)
        {

            if (LongOption == null || LongOption.Trim() == "")
                throw new ArgumentNullException("LongOption", "The parameter must not be null or empty!");

            this.LongOption = LongOption.Trim();

            return this;

        }

        #endregion

        #region Verify(RegularExpression)

        /// <summary>
        /// Set an optional regular expression for verification.
        /// </summary>
        /// <param name="RegularExpression">A regular expression for verification.</param>
        public CommandLineParserOption Verify(String RegularExpression)
        {

            if (RegularExpression == null || RegularExpression.Trim() == "")
                throw new ArgumentNullException("RegularExpression", "The parameter must not be null or empty!");

            this.RegularExpression = new Regex(RegularExpression);

            return this;

        }

        #endregion

        #region Verify(Delegate)

        /// <summary>
        /// Set an optional regular expression for verification.
        /// </summary>
        /// <param name="Delegate">A delegate for verification.</param>
        public CommandLineParserOption Verify(Func<String, Boolean> Delegate)
        {

            if (Delegate == null)
                throw new ArgumentNullException("Delegate", "The delegate must not be null or empty!");

            this.Verification = Delegate;

            return this;

        }

        #endregion

        #region Do(Delegate)

        /// <summary>
        /// Set the action delegate.
        /// </summary>
        /// <param name="Delegate">What to do with the value of the short and/or long option.</param>
        public CommandLineParserOption Do(Action<String> Delegate)
        {

            if (Delegate == null)
                throw new ArgumentNullException("Delegate", "The parameter must not be null!");

            this.Action = Action;
            
            return this;

        }

        #endregion


        #region Apply()

        /// <summary>
        /// Apply/store this command line parser option.
        /// </summary>
        /// <returns></returns>
        public CommandLineParser Apply()
        {

            if (!_Short.HasValue && LongOption == null)
                throw new ArgumentException("Either a short or long option must be defined!");

            if (Action == null)
                throw new ArgumentException("An action has to be defined for this option!");

            if (_Short.HasValue)
                CommandLineParser.AddOption(_Short.Value, Action);

            if (LongOption != null)
                CommandLineParser.AddOption(LongOption, Action);

            return CommandLineParser;

        }

        #endregion

    }

    #endregion

    #region CommandLineParser

    /// <summary>
    /// A command line parser.
    /// </summary>
    public class CommandLineParser
    {

        #region Data

        private readonly IDictionary<Char,   Action<String>> ShortOptions;
        private readonly IDictionary<String, Action<String>> LongOptions;

        #endregion

        #region Constructor(s)

        #region CommandLineParser()

        /// <summary>
        /// Create a new command line parser.
        /// </summary>
        public CommandLineParser()
        {
            this.ShortOptions   = new Dictionary<Char,   Action<String>>();
            this.LongOptions = new Dictionary<String, Action<String>>();
        }

        #endregion

        #endregion


        #region AddOption()

        /// <summary>
        /// Create a new fluent interface to create command line parser options.
        /// </summary>
        public CommandLineParserOption AddOption()
        {
            return new CommandLineParserOption(this);
        }

        #endregion

        #region AddShortOption(ShortOption)

        /// <summary>
        /// Create a new fluent interface to create command line parser options.
        /// </summary>
        /// <param name="ShortOption">Add a short option.</param>
        public CommandLineParserOption AddShortOption(Char ShortOption)
        {
            return new CommandLineParserOption(ShortOption, this);
        }

        #endregion

        #region AddLongOption(LongOption)

        /// <summary>
        /// Create a new fluent interface to create command line parser options.
        /// </summary>
        /// <param name="LongOption">Add a long option.</param>
        public CommandLineParserOption AddLongOption(String LongOption)
        {
            return new CommandLineParserOption(LongOption, this);
        }

        #endregion


        #region AddOption(ShortOption, Delegate, Verification = null)

        /// <summary>
        /// Add the given action for the given character,
        /// e.g. "o" for a "-o" command line option.
        /// </summary>
        /// <param name="ShortOption">A short option.</param>
        /// <param name="Delegate">What to do with the value of the character option.</param>
        /// <param name="Verification">An optional regular expression for verification.</param>
        public CommandLineParser AddOption(Char ShortOption, Action<String> Delegate, String Verification = null)
        {
            ShortOptions.Add(ShortOption, Delegate);
            return this;
        }

        #endregion

        #region AddOption(LongOption, Delegate, Verification = null)

        /// <summary>
        /// Add the given action for the given string,
        /// e.g. "output" for a "--output" command line option.
        /// </summary>
        /// <param name="LongOption">A long option.</param>
        /// <param name="Delegate">What to do with the value of the string option.</param>
        /// <param name="Verification">An optional regular expression for verification.</param>
        public CommandLineParser AddOption(String LongOption, Action<String> Delegate, String Verification = null)
        {
            LongOptions.Add(LongOption, Delegate);
            return this;
        }

        #endregion

        #region AddOption(ShortOption, LongOption, Delegate, Verification = null)

        /// <summary>
        /// Add the given action for the given character and string,
        /// e.g. "o" and "output" for a "-o" and "--output" command line option.
        /// </summary>
        /// <param name="ShortOption">A short option.</param>
        /// <param name="LongOption">A long option.</param>
        /// <param name="Delegate">What to do with the value of the short and/or long option.</param>
        /// <param name="Verification">An optional regular expression for verification.</param>
        public CommandLineParser AddOption(Char ShortOption, String LongOption, Action<String> Delegate, String Verification = null)
        {
            AddOption(ShortOption, Delegate, Verification);
            AddOption(LongOption,  Delegate, Verification);
            return this;
        }

        #endregion


        #region Parse(Arguments)

        /// <summary>
        /// Parse the given array of arguments.
        /// </summary>
        /// <param name="Arguments">An array of arguments.</param>
        public CommandLineParser Parse(String[] Arguments)
        {

            for (var i=0; i<Arguments.Length; i++)
            {

                #region Parse --longoption

                if (Arguments[i].StartsWith("--"))
                {

                    if (LongOptions.ContainsKey(Arguments[i].Remove(0, 2)))
                    {

                        var Action = LongOptions[Arguments[i].Remove(0, 2)];
                        var s = "";
                        var j = i + 1;

                        while (!Arguments[j].StartsWith("-"))
                        {
                            s += " " + Arguments[j];
                            j++;
                        }

                        Action(s.Trim());

                        i = j - 1;

                    }

                    continue;

                }

                #endregion

                #region Parse -shortoption

                if (Arguments[i].StartsWith("-"))
                {

                    if (ShortOptions.ContainsKey(Arguments[i][1]))
                    {

                        var Action = ShortOptions[Arguments[i][1]];
                        var s = "";
                        var j = i + 1;

                        while (!Arguments[j].StartsWith("-"))
                        {
                            s += " " + Arguments[j];
                            j++;
                        }

                        Action(s.Trim());

                        i = j - 1;

                    }

                }

                #endregion

            }

            return this;

        }

        #endregion

    }

    #endregion

}
