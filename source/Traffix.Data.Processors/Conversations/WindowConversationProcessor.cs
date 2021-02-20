using System;
using System.Collections.Generic;
using System.Linq;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Processors
{
    internal class TakeConversationProcessor<TTarget> : IConversationProcessor<TTarget>
    {
        private readonly IConversationProcessor<TTarget> _processor;
        private readonly int _count;

        public TakeConversationProcessor(IConversationProcessor<TTarget> processor, int count)
        {
            _processor = processor;
            _count = count;
        }  
        public TTarget Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
        {
            return _processor.Invoke(flowKey, frames.Take(_count));  
        } 
    }
    public static class TakeConversationProcessor
    {
         public static IConversationProcessor<Target> ApplyToTake<Target>(this IConversationProcessor<Target> source, int count)
        {
            return new TakeConversationProcessor<Target>(source, count);
        }       
    }
    internal class WindowConversationProcessor<TTarget> : IConversationProcessor<TTarget>
    {
        private readonly IConversationProcessor<TTarget> _processor;
        private readonly DateTime _windowStart;
        private readonly TimeSpan _duration;

        public WindowConversationProcessor(IConversationProcessor<TTarget> processor, DateTime windowStart, TimeSpan duration)
        {
            this._processor = processor;
            this._windowStart = windowStart;
            this._duration = duration;
        }
        public TTarget Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
        {
            var firstTicks = _windowStart.Ticks;
            var lastTicks = firstTicks + _duration.Ticks;
            bool IsInWindow(Memory<byte> frame)
            {
                FrameMetadata frameMetadata = default;
                FrameMetadata.GetFrameFromMemory(frame, ref frameMetadata);
                return (frameMetadata.Ticks >= firstTicks && frameMetadata.Ticks < lastTicks);
            }
            return _processor.Invoke(flowKey, frames.Where(IsInWindow));
        }
    }
    public static class WindowConversationProcessor
    {
        /// <summary>
        /// Creates a new conversation processor that produces values of <typeparamref name="Target"/> objects 
        /// for the conversation limited to the given time window.
        /// </summary>
        /// <param name="source">The source conversation processor.</param>
        /// <param name="windowStart">The start of the window.</param>
        /// <param name="duration">The duration of the window.</param>
        /// <typeparam name="Target">The type of results.</typeparam>
        /// <returns>A new converdation processor that limits the frames to the window.</returns>
        public static IConversationProcessor<Target> ApplyToWindow<Target>(this IConversationProcessor<Target> source, DateTime windowStart, TimeSpan duration)
        {
            return new WindowConversationProcessor<Target>(source, windowStart, duration);
        }
    }
}
