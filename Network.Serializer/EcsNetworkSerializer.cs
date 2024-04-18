namespace Game.Ecs.Network.Network.Serializer
{
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using MemoryPack;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public static class EcsNetworkSerializer
    {
        [ThreadStatic]
        private static ArrayBufferWriter<byte> _arrayBufferWriter;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ref DataStreamWriter writer,ref TValue value)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (!isBlittable)
                {
                    _arrayBufferWriter ??= new ArrayBufferWriter<byte>();
                    _arrayBufferWriter.Clear();
                    MemoryPackSerializer.Serialize(_arrayBufferWriter, value, MemoryPackSerializerOptions.Utf16);
                    var span = _arrayBufferWriter.WrittenSpan;
                    return writer.WriteData(ref span);
                }
                
                var length = UnsafeUtility.SizeOf<TValue>();
                writer.WriteData(ref value);
    
                return length;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ref NativeArray<byte> writer,ref TValue value,int offset)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (isBlittable) return writer.WriteData(ref value,offset);
                
                _arrayBufferWriter ??= new ArrayBufferWriter<byte>(512);
                _arrayBufferWriter.Clear();
                
                MemoryPackSerializer.Serialize(_arrayBufferWriter, value, MemoryPackSerializerOptions.Utf16);
                
                var span = _arrayBufferWriter.WrittenSpan;
                return writer.WriteData(ref span, offset);
            }
        }
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ArrayBufferWriter<byte> writer,ref TValue value)
            where TValue : struct
        {
            unsafe
            {
                var start = writer.WrittenCount;
                MemoryPackSerializer.Serialize(value, MemoryPackSerializerOptions.Utf16);
                return writer.WrittenCount - start;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Deserialize(this ref NativeArray<byte> buffer,int offset,Type type)
        {
            var span = buffer.AsReadOnlySpan();
            //if (UnsafeUtility.IsBlittable(type)) return span.ReadData(type);
            var slice = span.Slice(offset, span.Length - offset);
            return MemoryPackSerializer.Deserialize(type,slice, MemoryPackSerializerOptions.Utf16);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Deserialize<TValue>(this ref NativeArray<byte> buffer,int offset, ref TValue value)
            where TValue : struct
        {
            var span = buffer.AsReadOnlySpan();
            var slice = span.Slice(offset, span.Length - offset);
            return Deserialize(ref slice, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Deserialize<TValue>(this ref ReadOnlySpan<byte> buffer,ref TValue value)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (isBlittable) return buffer.ReadData(ref value);
                
                var readCount = MemoryPackSerializer.Deserialize(buffer, ref value);
                return readCount;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int Deserialize<TValue>(this ref NativeSlice<byte> buffer,ref TValue value)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (isBlittable) return buffer.ReadData(ref value);
                
                var readCount = MemoryPackSerializer
                    .Deserialize(buffer.ToArray(), ref value);
                return readCount;
            }
        }
        
    }
}