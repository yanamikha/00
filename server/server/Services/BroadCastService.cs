using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using BroadCastView;
using Authenticity;

namespace GRPCServer.Services
{
    public class BroadCastServiceImpl : BroadCastService.BroadCastServiceBase
    {
        private readonly ILogger Logger; // можно юзать для лога какая операция и как выполнилась или на каком этапе зависло во время прогресса.

        public BroadCastServiceImpl(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<BroadCastServiceImpl>();
        }

        public BroadCastServiceImpl()
        {

        }

        public override async Task<Empty> PostMessage(Message request, ServerCallContext context)
        {
            List<MessageStreamingContext> currStreams = new List<MessageStreamingContext>();
            List<MessageStreamingContext> streamsToRemove = new List<MessageStreamingContext>();
            
            lock (messageStreams_)
            {
                currStreams = messageStreams_.ToList();
            }

            foreach (MessageStreamingContext c in currStreams)
            {
                try
                {
                    await c.MessageStream.WriteAsync(new OptionalMessage() { Message = request });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error writing");
                    Console.WriteLine(e.ToString());
                    streamsToRemove.Add(c);
                    c.AsyncMonitor.Pulse();
                }
            }

            lock (messageStreams_)
            {
                if (streamsToRemove.Count > 0)
                {
                    for (int i = 0; i < messageStreams_.Count; i++)
                    {
                        MessageStreamingContext c = messageStreams_[i];

                        foreach (MessageStreamingContext remove in streamsToRemove)
                        {
                            if (c == remove)
                            {
                                messageStreams_.RemoveAt(i); i--;
                                continue;
                            }
                        }
                    }
                }
            }

            return new Empty();
        }

        static private List<MessageStreamingContext> messageStreams_ = new List<MessageStreamingContext>();

        public override async Task SubscribeMessageStream(Token token, IServerStreamWriter<OptionalMessage> responseStream, ServerCallContext context)
        {
            MessageStreamingContext cContext = GetMessageStreamingContext(context);

            if (cContext.MessageStream != null || cContext.AsyncMonitor != null)
            {
                await responseStream.WriteAsync(new OptionalMessage() { NotAMessage = true } );
                return;
            }

            cContext.MessageStream = responseStream;
            cContext.AsyncMonitor = new AsyncMonitor();
            messageStreams_.Add(cContext);

            await cContext.AsyncMonitor.WaitAsync();

            cContext.MessageStream = null;
            cContext.AsyncMonitor = null;
            //await responseStream.WriteAsync(new OptionalMessage() { NotAMessage = true });
        }

        private static MessageStreamingContext GetMessageStreamingContext(ServerCallContext callConext)
        {
            lock (callConext)
            {
                callConext.UserState.TryGetValue("context", value: out object cObject);

                MessageStreamingContext context = cObject as MessageStreamingContext;

                if (context == null)
                {
                    context = new MessageStreamingContext();

                    callConext.UserState.Add("context", context);
                }

                return context;
            }
        }

        private class MessageStreamingContext
        {
            public IServerStreamWriter<OptionalMessage> MessageStream { get; set; }
            public AsyncMonitor AsyncMonitor { get; set; }
        }
    }
}
