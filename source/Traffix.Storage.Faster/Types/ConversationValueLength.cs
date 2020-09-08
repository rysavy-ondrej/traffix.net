using FASTER.core;
using System.Runtime.CompilerServices;

namespace Traffix.Storage.Faster
{
    internal class ConversationValueLength : IVariableLengthStruct<ConversationValue>
    {
        public int GetInitialLength()
        {
            return Unsafe.SizeOf<ConversationValue>();
        }

        public int GetLength(ref ConversationValue t)
        {
            return Unsafe.SizeOf<ConversationValue>();
        }
    }
}
