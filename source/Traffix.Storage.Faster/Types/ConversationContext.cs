using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Traffix.Storage.Faster
{
    internal class ConversationContext
    {
        private static List<ConversationOutput> _outputValues = new List<ConversationOutput>();
        public static ConversationContext Empty => new ConversationContext();
        public IReadOnlyList<ConversationOutput> OutputValues => _outputValues;

        public void AddOutputValue(ConversationOutput output)
        {
            _outputValues.Add(output);
        }
    }
}
