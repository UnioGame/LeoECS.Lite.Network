namespace Game.Ecs.Network.Network.Serializer
{
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public static class EcsNetworkBurstSerializer
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int WriteData(this ref NativeList<byte>.ParallelWriter writer,ref ReadOnlySpan<byte> span,int offset = 0)
        {
            unsafe
            {
                var length = span.Length;
                // Get a pointer to the span
                fixed (byte* spanPtr = span)
                {
                    // Copy the span to the array
                    writer.AddRangeNoResize(spanPtr,length);
                }
                return length;
            }
        }
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte* GetPointerToIndex(this NativeArray<byte> nativeArray, int index)
        {
            var ptr = (byte*)nativeArray.GetUnsafePtr();
            return ptr + index;
        }
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteData<TData>(this ref DataStreamWriter writer,ref TData value)
            where TData : struct
        {
            unsafe
            {
                var size = UnsafeUtility.SizeOf<TData>();
                using var byteArray = new NativeArray<byte>(size,Allocator.Persistent);
                var arrayPtr = byteArray.GetUnsafePtr();
                UnsafeUtility.CopyStructureToPtr(ref value, arrayPtr);
                return size;
            }
        }
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteData<TData>(this ref NativeArray<byte> writer,ref TData value,int offset = 0)
            where TData : struct
        {
            unsafe
            {
                var size = UnsafeUtility.SizeOf<TData>();
                if (size <= 0) return 0;
                
                var arrayPtr = writer.GetPointerToIndex(offset);
                UnsafeUtility.CopyStructureToPtr(ref value, arrayPtr);
                return size;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int WriteData(this ref NativeStream.Writer writer,ref ReadOnlySpan<byte> span)
        {
            unsafe
            {
                var length = span.Length;
                // Get a pointer to the span
                fixed (byte* spanPtr = span)
                {
                    var target = writer.Allocate(length);
                    // Copy the span to the array
                    UnsafeUtility.MemCpy(target,spanPtr, length);
                }

                return length;
            }
        }
                       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int WriteData(this ref DataStreamWriter writer,ref ReadOnlySpan<byte> span)
        {
            unsafe
            {
                var length = span.Length;
                // Get a pointer to the span
                fixed (byte* spanPtr = span)
                {
                    using var byteArray = NativeArrayUnsafeUtility
                        .ConvertExistingDataToNativeArray<byte>(spanPtr, length, Allocator.Persistent);
                    // Copy the span to the array
                    writer.WriteBytes(byteArray);
                }

                return length;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int WriteData(ref this NativeArray<byte> writer,ref ReadOnlySpan<byte> span,int offset = 0)
        {
            unsafe
            {
                var length = span.Length;
                // Get a pointer to the span
                fixed (byte* spanPtr = span)
                {
                    var arrayPtr = writer.GetPointerToIndex(offset);
                    // Copy the span to the array
                    UnsafeUtility.MemCpy(arrayPtr,spanPtr,length);
                }
                return length;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int CopyTo(ref this NativeArray<byte> writer,byte[] span,int length = 0)
        {
            unsafe
            {
                // Get a pointer to the span
                fixed (byte* spanPtr = span)
                {
                    var arrayPtr = writer.GetUnsafePtr();
                    // Copy the span to the array
                    UnsafeUtility.MemCpy(spanPtr,arrayPtr, length);
                }
                return length;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int ReadData<TData>(this ref NativeArray<byte> writer,ref TData data,int offset = 0)
            where TData : struct
        {
            unsafe
            {
                var size = UnsafeUtility.SizeOf<TData>();
                var arrayPtr = writer.GetPointerToIndex(offset);
                UnsafeUtility.CopyPtrToStructure(arrayPtr, out data);
                return size;
            }
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int ReadData<TData>(this ref ReadOnlySpan<byte> source,ref TData data)
            where TData : struct
        {
            unsafe
            {
                var size = UnsafeUtility.SizeOf<TData>();
                if(size <= 0 || source.Length <= 0) return 0;
                
                fixed (byte* spanPtr = source)
                {
                    // Copy the span to the array
                    UnsafeUtility.CopyPtrToStructure(spanPtr,out data);
                }
                return size;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int ReadData<TData>(this ref NativeSlice<byte> source,ref TData data)
            where TData : struct
        {
            unsafe
            {
                var size = UnsafeUtility.SizeOf<TData>();
                if(size <= 0 || source.Length <= 0) return 0;

                var slicePtr = source.GetUnsafePtr();
                var spanPtr = (byte*)slicePtr;
                // Copy the span to the array
                UnsafeUtility.CopyPtrToStructure(spanPtr,out data);
                return size;
            }
        }
        
    }
}