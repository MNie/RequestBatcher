using System;

namespace RequestBatcher
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using ResultType;
    using ResultType.Factories;
    using ResultType.Results;

    public class Batcher<TMessage> : IBatcher<TMessage>
    {
        private static readonly BufferBlock<TMessage> BufferBlock = new BufferBlock<TMessage>(new DataflowBlockOptions
        {
            EnsureOrdered = true
        });

        private static readonly TransformBlock<TMessage, TMessage> BufferInterceptor = new TransformBlock<TMessage, TMessage>(x =>
        {
            Console.WriteLine($"Get a message with value: {x}");
            return x;
        });
        private static readonly TransformBlock<TMessage, TMessage> TimeoutInterceptor = new TransformBlock<TMessage, TMessage>(x =>
        {
            Console.WriteLine($"Move out from transformation block with a value: {x}");
            return x;
        });

        public Batcher(int size, int interval, Action<IEnumerable<TMessage>> triggerFunc)
        {
            var batchBlock = new BatchBlock<TMessage>(size, new GroupingDataflowBlockOptions()
            {
                EnsureOrdered = true
            });
            var timer = new Timer(async _ =>
            {
                try
                {
                    batchBlock.TriggerBatch();
                    var data = await batchBlock.ReceiveAsync();
                    triggerFunc(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error occurs while trying to invoke action on batch", e);
                }
            }, null, 0, 500);
            var timeoutBlock = new TransformBlock<TMessage, TMessage>(v =>
            {
                timer.Change(interval, Timeout.Infinite);
                return v;
            });
            
            TimeoutInterceptor.LinkTo(batchBlock);
            timeoutBlock.LinkTo(TimeoutInterceptor);
            BufferInterceptor.LinkTo(timeoutBlock);
            BufferBlock.LinkTo(BufferInterceptor);    
        }

        public async Task<Result<Unit>> SendAsync(TMessage msg, CancellationToken token = new CancellationToken())
        {
            try
            {
                var result = await BufferBlock.SendAsync(msg, token);
                return result
                    ? ResultFactory.CreateSuccess()
                    : ResultFactory.CreateFailure<Unit>("Message was refused by queue");
            }
            catch (Exception e)
            {
                return ResultFactory.CreateFailure<Unit>(e.Message);
            }
        }
    }
}