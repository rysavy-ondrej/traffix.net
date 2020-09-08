using System.Runtime.CompilerServices;

namespace Traffix.Core.Flows
{
    class Utility
    {
        // Taken from https://github.com/microsoft/FASTER/blob/master/cs/src/core/Utilities/Utility.cs
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe long HashBytes(byte* pbString, int len)
        {
            const long magicno = 40343;
            char* pwString = (char*)pbString;
            int cbBuf = len / 2;
            ulong hashState = (ulong)len;

            for (int i = 0; i < cbBuf; i++, pwString++)
                hashState = magicno * hashState + *pwString;

            if ((len & 1) > 0)
            {
                byte* pC = (byte*)pwString;
                hashState = magicno * hashState + *pC;
            }

            return (long)Rotr64(magicno * hashState, 4);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Rotr64(ulong x, int n)
        {
            return (((x) >> n) | ((x) << (64 - n)));
        }


        public static unsafe int Memcmp(byte *ptr1, byte *ptr2, int count)
        {
            int v = 0;
            byte* a = ptr1, b = ptr2;
            while (count-- > 0 && v == 0) v = *(a++) - *(b++);
            return v;
        }
    }
}
