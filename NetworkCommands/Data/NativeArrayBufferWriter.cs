namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif
    
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NativeArrayBufferWriter : IBufferWriter<byte>
    {
        public int writenCount;
        public NativeArray<byte> nativeArray;
        
        public NativeArrayBufferWriter(ref NativeArray<byte> dataWriter)
        {
            nativeArray = dataWriter;
            writenCount = 0;
        }

        public void Clear()
        {
            writenCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            writenCount += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            var length = sizeHint <= 0 ? nativeArray.Length - writenCount : sizeHint;
            return nativeArray.AsSpan().Slice(writenCount,length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value)
        {
            nativeArray[writenCount] = value;
            writenCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] values)
        {
            unsafe
            {
                var bytePtr = nativeArray.GetUnsafePtr();
                fixed (byte* buffer = values)
                {
                    UnsafeUtility.MemCpy(bytePtr, buffer, values.Length);
                }
            }
        }
    }
}