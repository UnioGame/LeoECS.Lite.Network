using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems;
using MemoryPack;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Game.Ecs.Network.Network.Serializer
{
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using MemoryPack;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityNetcode.NetcodeMessages.Systems;
#if ENABLE_IL2CPP
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using MemoryPack;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.IL2CPP.CompilerServices;
    using UnityNetcode.NetcodeMessages.Systems;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public static class NativeStreamSerializeExtensions
    {
        [ThreadStatic]
        private static ArrayBufferWriter<byte> _arrayBufferWriter;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ref NativeStream stream, ref TValue value)
            where TValue : struct
        {
            var writer = stream.AsWriter();
            return writer.Serialize(ref value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Read<TValue>(this ref NativeStream stream)
            where TValue : unmanaged
        {
            var writer = stream.AsReader();
            return writer.Read<TValue>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Read<TValue>(this ref NativeStream stream,int index)
            where TValue : unmanaged
        {
            var writer = stream.AsReader();
            writer.BeginForEachIndex(index);
            var resultItem = writer.Read<TValue>();
            writer.EndForEachIndex();
            return resultItem;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ref NativeStream stream,int index, ref TValue value)
            where TValue : struct
        {
            var writer = stream.AsWriter();
            writer.BeginForEachIndex(index);
            
            var count = writer.Serialize(ref value);
            
            writer.EndForEachIndex();
            return count;
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeHash(this ref NativeStream.Reader data,int length)
        {
            return ByteHashCalculator.ComputeHash(ref data,length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ref NativeStream.Writer writer,ref TValue value)
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
                var bytePtr = writer.Allocate(length);
                UnsafeUtility.CopyStructureToPtr(ref value, bytePtr);
                return length;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBytes(this ref NativeStream.Writer writer,byte[] values)
        {
            unsafe
            {
                var length = values.Length;
                var bytePtr = writer.Allocate(values.Length);
                fixed (byte* buffer = values)
                {
                    UnsafeUtility.MemCpy(bytePtr, buffer, values.Length);
                }

                return length;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int WriteStream(this ref NativeStream.Writer writer,ref NativeStream.Reader data,int length)
        {
            // Get a pointer to the span
            var spanPtr = data.ReadUnsafePtr(length);
            
            var target = writer.Allocate(length);
            
            // Copy the span to the array
            UnsafeUtility.MemCpy(target,spanPtr, length);

            return length;
        }

    }
}