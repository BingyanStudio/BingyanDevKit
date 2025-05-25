using System;

namespace Bingyan
{
    public struct LockBit
    {
        private uint value;

        public void Lock(int bit) => value |= 1u << bit;
        public void Unlock(int bit) => value &= ~(1u << bit);
        public void Clear() => value = 0;

        public static implicit operator uint(LockBit bit) => bit.value;
        public static implicit operator LockBit(uint bit) => new() { value = bit };

        public static implicit operator bool(LockBit bit) => bit.value == 0;

        public override readonly string ToString() => Convert.ToString(value, 2);
    }
}