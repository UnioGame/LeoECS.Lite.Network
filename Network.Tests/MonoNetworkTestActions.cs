using UnityEngine;

namespace Game.Ecs.Network.Network.Tests
{
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using MemoryPack;
    using NetworkCommands.Data;
    using Serializer;
    using Shared.Data;
    using Sirenix.OdinInspector;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine.Profiling;
    using UnityNetcode.NetcodeMessages.Systems;

    public class MonoNetworkTestActions : MonoBehaviour
    {
        public int testAmount = 1000;
        public bool activate1Action = false;
        public bool activateComputeHash = false;
        public bool activateTestTask = false;

        public void Update()
        {
            if (activate1Action)
                SerializeTest();

            if (activateComputeHash)
                HashTest();

            if (activateTestTask)
                SerializeTaskTest();            
            
        }

        [Button]
        public void HashTest()
        {
            var valueStream = new NativeStream(1, Allocator.Persistent);
            var valueWriter = valueStream.AsWriter();
            valueWriter.BeginForEachIndex(0);

            for (var i = 0; i < testAmount; i++)
            {
                var value = new TemplateSerializeType()
                {
                    Value = 11111,
                    Value2 = 2222,
                };
                valueWriter.Write(value);
            }

            valueWriter.EndForEachIndex();
            var valueReader = valueStream.AsReader();

            valueReader.BeginForEachIndex(0);

            var hash = valueReader.ComputeHash(0);

            valueReader.EndForEachIndex();

            Debug.Log($"HASH: {hash}");
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Serialize(ref NativeStream.Writer stream)
        {
            var size = Unsafe.SizeOf<TemplateSerializeType>();
            var value = new TemplateSerializeType()
            {
                Value = 11111,
                Value2 = 2222,
            };

            // Get a writer for the stream
            // Write some data into the stream
            stream.Write<TemplateSerializeType>(value);
            return size;
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeManaged(IBufferWriter<byte> writer)
        {
            var value = new TemplateSerializeType()
            {
                Value = 1111,
                Value2 = 22414,
            };
            var start = writer.GetSpan().Length;
            //var size = Unsafe.SizeOf<TemplateSerializeType>();
            MemoryPackSerializer.Serialize(writer, value, MemoryPackSerializerOptions.Utf16);
            return writer.GetSpan().Length - start;
        }
        
        [Button]
        public void SerializeTest()
        {
            var amount = testAmount;
            var valueStream = new NativeStream(amount, Allocator.Persistent);
            var arrayBufferWriter = new ArrayBufferWriter<byte>();

            Profiler.BeginSample("Demo.Serialize");

            var valueWriter = valueStream.AsWriter();

            arrayBufferWriter.Clear();

            for (var i = 0; i < amount; i++)
            {
                valueWriter.BeginForEachIndex(i);
                arrayBufferWriter.Clear();
                SerializeManaged(arrayBufferWriter);
                //valueWriter.Allocate(arrayBufferWriter.WrittenSpan.Length)
                unsafe
                {
                    var length = arrayBufferWriter.WrittenCount;
                    // Get a pointer to the span
                    fixed (byte* spanPtr = arrayBufferWriter.WrittenSpan)
                    {
                        var target = valueWriter.Allocate(length);
                        // Copy the span to the array
                        UnsafeUtility.MemCpy(target, spanPtr, length);
                    }
                }

                valueWriter.EndForEachIndex();
            }

            Profiler.EndSample();

            var valueReader = valueStream.AsReader();

            for (int i = 0; i < amount; i++)
            {
                valueReader.BeginForEachIndex(i);
                var value = valueReader.Read<TemplateSerializeType>();
                //Debug.Log($"WRITE COUNT: {valueStream.Count()}");
                //Debug.Log($"TemplateSerializeType: {value.Value} {value.Value2}");
                valueReader.EndForEachIndex();
            }

            valueStream.Dispose();
        }
        
        
        [Button]
        public unsafe void SerializeTaskTest()
        {
            var chunkSize =  1024;
            var bufferSize = chunkSize * testAmount;
            var componentsCount = 3;
            
            var serializeTask = new TestSerializationTask
            {
                source = new NativeArray<TestSerializationData>(testAmount, Allocator.Temp),
                results = new NativeArray<EcsEntityNetworkData>(testAmount, Allocator.Temp),
                serializedBytes = new NativeArray<byte>(bufferSize, Allocator.Temp),
                components = new NativeArray<TemplateSerializeType>(testAmount * 3, Allocator.Temp),
                chunkSize = chunkSize,
            };

            var componentIndex = 0;
            for (var i = 0; i < testAmount; i++)
            {
                serializeTask.source[i] = new TestSerializationData()
                {
                    Id = 1131414,
                    Entity = 141414,
                    PreviousHash = 141414,
                    ComponentsCount = componentsCount,
                    ComponentsIndex = componentIndex,
                };

                for (var j = 0; j < componentsCount; j++)
                {
                    serializeTask.components[componentIndex] = new TemplateSerializeType()
                    {
                        Value = j,
                        Value2 = j,
                    };
                    componentIndex++;
                }
            }

            Profiler.BeginSample("serializeTask.Execute");
            serializeTask.Execute(0, testAmount);
            Profiler.EndSample();

            ref var result = ref serializeTask.serializedBytes;
            var totalSize = serializeTask.totalSize;
            var reader = result.GetSubArray(0, totalSize);

            Debug.Log($"STREAM ARRAY: total size: {reader.Length}");

            Profiler.BeginSample("serializeTask.Deserialize");
            serializeTask.Deserialize(ref reader);
            Profiler.EndSample();
        }
        
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct TestSerializationData
    {
        public int Id;
        public int Entity;
        public int PreviousHash;
        public int ComponentsIndex;
        public int ComponentsCount;
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct TestSerializationTask
    {
        public int chunkSize;
        public int totalSize;
        public bool useHashFiltering;

        public NativeArray<TestSerializationData> source;
        public NativeArray<TemplateSerializeType> components;
        
        public NativeArray<byte> serializedBytes;
        public NativeArray<EcsEntityNetworkData> results;

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(int fromIndex, int beforeIndex)
        {
            totalSize = 0;

            for (var i = fromIndex; i < beforeIndex; i++)
            {
                var data = source[i];

                var id = data.Id;
                var entity = data.Entity;
                var componentsCount = 0;
                var currentHash = 0;
                var previousHash = data.PreviousHash;
                var networkWriter = serializedBytes.GetSubArray(i * chunkSize, chunkSize);

                var componentsWritenSize = 0;
                var offset = 0;

                var dataComponentsCount = data.ComponentsCount;
                var componentIndex = data.ComponentsIndex;
                var componentLastIndex =  componentIndex + dataComponentsCount;
                
                for (var j = componentIndex; j < componentLastIndex; j++)
                {
                    var headerIndex = offset;

                    var componentHeader = new NetworkComponentHeader()
                    {
                        TypeId = -1,
                        Size = 0,
                    };

                    var size = networkWriter.WriteData(ref componentHeader, offset);
                    offset += size;

                    var component = components[j];
                    size = networkWriter.WriteData(ref component, offset);
                    offset += size;

                    componentHeader.TypeId = j;
                    componentHeader.Size = size;

                    networkWriter.WriteData(ref componentHeader, headerIndex);

                    componentsCount++;
                }

                if (componentsCount == 0) continue;

                currentHash = ByteHashCalculator.ComputeHash(ref networkWriter,0, offset);

                var isSameValues = useHashFiltering && previousHash == currentHash;

                var value = new EcsEntityNetworkData()
                {
                    Id = id,
                    Hash = currentHash,
                    Count = componentsCount,
                    Size = offset,
                    IsValueChanged = !isSameValues,
                    Offset = 0,
                };

                results[i] = value;
                totalSize += offset;
            }

            Debug.Log($"SERIALIZE: total size: {totalSize}");
        }

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deserialize(ref NativeArray<byte> sourceBytes)
        {
            var size = sourceBytes.Length;
            var readSize = 0;

            while (readSize < size)
            {
                var componentHeader = new NetworkComponentHeader();
                var readCount = sourceBytes.Deserialize(readSize, ref componentHeader);
                readSize += readCount;

                var componentSize = componentHeader.Size;
                var templateValue = new TemplateSerializeType();
                readCount = sourceBytes.Deserialize(readSize, ref templateValue);
                readSize += readCount;
            }
        }
    }


}