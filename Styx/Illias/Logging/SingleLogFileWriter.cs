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

using System.Text;
using System.Threading.Channels;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Logging
{

    public sealed class SingleLogFileWriter : IAsyncDisposable
    {

        private readonly Channel<String>          channel;
        private readonly String                   fileName;
        private readonly CancellationTokenSource  cts = new();
        private readonly Task                     backgroundTask;

        public SingleLogFileWriter(String  FileName,
                                   Int32   Capacity   = 1000)
        {

            fileName        = FileName;

            channel         = Channel.CreateBounded<String>(
                                   new BoundedChannelOptions(Capacity) {
                                       FullMode      = BoundedChannelFullMode.Wait,
                                       SingleReader  = true,
                                       SingleWriter  = false
                                   }
                               );

            backgroundTask  = Task.Run(WriteLoopAsync);

        }

        public ValueTask EnqueueAsync(String             Line,
                                      CancellationToken  CancellationToken = default)

            => channel.Writer.WriteAsync(
                   Line,
                   CancellationToken
               );


        private async Task WriteLoopAsync()
        {
            try
            {
                await foreach (var line in channel.Reader.ReadAllAsync(cts.Token))
                {
                    try
                    {

                        await File.AppendAllTextAsync(
                                  fileName,
                                  line + Environment.NewLine,
                                  Encoding.UTF8,
                                  cts.Token
                              );

                    }
                    catch (Exception ex)
                    {
                        DebugX.Log($"Write error: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException) {
                /* Shutdown */
            }
        }

        public async ValueTask DisposeAsync()
        {

            channel.Writer.Complete();
            cts.Cancel();

            await backgroundTask.ConfigureAwait(false);
            cts.Dispose();

        }

    }

}
