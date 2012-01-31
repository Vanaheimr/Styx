/*
 * Copyright (c) 2011-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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
using System.Threading;
using System.Collections.Generic;

using de.ahzf.Pipes;

#endregion

namespace de.ahzf.Arrows.TestApplication1
{

    public class Program
    {

        public static Boolean PrintMe_A(Object Sender, String MessageIn)
        {
            Console.WriteLine("a" + MessageIn);
            return true;
        }

        public static Boolean PrintMe_B(Object Sender, String MessageIn)
        {
            Console.WriteLine("b" + MessageIn);
            return true;
        }

        
        public static void Main(String[] args)
        {

            var _Sniper1     = new List<Double>() { 22, 22, 12, 11, 10, 09, 09, 22, 08, 19, 09, 20, 21, 22, 22 }.ToSniper();
            var _ActionArrow = new ActionArrow<Double>(message => { Console.WriteLine(" I: " + message); });
            _Sniper1.OnMessageAvailable += _ActionArrow.ReceiveMessage;
            _Sniper1.StartToFire();

            var _Sniper2 = new List<Int32>() { 22, 22, 12, 11, 10, 09, 09, 22, 08, 19, 09, 20, 21, 22, 22 }.
                           ToSniper(Autostart:    true,
                                    StartAsTask:  true,
                                    InitialDelay: TimeSpan.FromSeconds(3)).
                           BandFilter<Int32>(10, 20).
                           SameValueFilter<Int32>().
                           ActionArrow(message => { Console.WriteLine("II: " + message); });


            //var _f1 = new List<Int32>() { 22, 22, 12, 11, 10, 09, 09, 22, 08, 19, 09, 20, 21, 22, 22 }.
            //          ToSniper(InitialDelay: TimeSpan.FromSeconds(10));

            //var _f2 = _f1.
            //       //   ActionArrow((message) => { Console.WriteLine("new: '" + message + "'"); }).
            //          BandFilter<Int32>(10, 20).
            //          SameValueFilter<Int32>().
            //          IdentityArrow().
            //          ActionArrow((message) => { Console.WriteLine("passed: '" + message + "'"); });

            ////_f1.SendTo((sender, message) => { Console.WriteLine("bypass: '" + message + "'"); return true; });

            //_f1.StartToFire(true);

            //_f2.ReceiveMessage(22);
            //_f2.ReceiveMessage(12);
            //_f2.ReceiveMessage(11);
            //_f2.ReceiveMessage(10);
            //_f2.ReceiveMessage(09);
            //_f2.ReceiveMessage(09);
            //_f2.ReceiveMessage(22);
            //_f2.ReceiveMessage(08);
            //_f2.ReceiveMessage(19);
            //_f2.ReceiveMessage(09);
            //_f2.ReceiveMessage(20);
            //_f2.ReceiveMessage(21);
            //_f2.ReceiveMessage(22);
            //_f2.ReceiveMessage(22);

            while (true)
            {
                Thread.Sleep(100);
            }

            var ab = new ActionArrow<String>(msg => Console.WriteLine(msg));
            ab.ReceiveMessage("hello");

            var c = new FuncArrow<Int64,  String>(message => message.ToString());
            var d = new FuncArrow<String, String>(message => ">>>" + message);
            var e = new IdentityArrow<String>(PrintMe_A, PrintMe_B);

            c.OnMessageAvailable += d.ReceiveMessage;
            d.OnMessageAvailable += e.ReceiveMessage;
            e.OnMessageAvailable += PrintMe_B;
            e.OnMessageAvailable += (sender, message) => { Console.WriteLine("Incoming message: '" + message + "'!"); return true; };

            c.ReceiveMessage(new Object(), 1234);

   //         d.OnMessageAvailable += c.ReceiveMessage;

            c.FuncArrow(message => message.ToUpper(), d);

        }

    }

}
