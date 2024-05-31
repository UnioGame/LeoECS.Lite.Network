namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using EcsThreads.Systems;
    using Leopotam.EcsLite;
    using Network.Serializer;
    using NetworkCommands.Data;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class EcsSerializationTask : IEcsDataTask<EcsSerializationTask>
    {
        public NetworkHistoryData historyData;
        public NetworkHistoryData previousData;
        public EcsNetworkData networkData;
        public bool useHashFiltering;
        public int chunkSize;
        public bool forceSerialize;
        
        public EcsWorld world;
        public int length;
        public NativeArray<SerializationTaskData> source;
        public NativeArray<EcsEntityNetworkData> results;
        public NativeArray<byte> serializedResult;

#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(int fromIndex, int beforeIndex)
        {
            var headerSize = UnsafeUtility.SizeOf<NetworkComponentHeader>();
            var entityHeaderSize = UnsafeUtility.SizeOf<NetworkEntityHeader>();
            
            for (var i = fromIndex; i < beforeIndex; i++)
            {
                var startOffset = i * chunkSize;
                var networkWriter = serializedResult.GetSubArray(startOffset, chunkSize);
                var offset = 0;
                var itemOffset = offset;
                
                var data = source[i];

                var id = data.Id;
                var entity = data.Entity;
                var syncComponentsCount = 0;
                var currentHash = 0;
                

                previousData.EntityMap.TryGetValue(id, out var previousValue);
                var previousHash = previousValue.Hash;
                
                offset += entityHeaderSize;

                foreach (var componentType in networkData.Types)
                {
                    var serializer = componentType.serializer;
                    var headerOffset = offset;
                    var size = serializer.Serialize(world, entity, ref networkWriter,offset + headerSize);
                    if (size < 0) continue;
                    
                    syncComponentsCount++;

                    var componentHeader = new NetworkComponentHeader()
                    {
                        TypeId = componentType.id,
                        Size = size,
                    };

                    networkWriter.WriteData(ref componentHeader, offset);
                    offset += size + headerSize;
                }

                if (syncComponentsCount == 0)
                    continue;
                
                currentHash = ByteHashCalculator.ComputeHash(ref networkWriter,itemOffset+entityHeaderSize,offset);
                var isSameValues =  (useHashFiltering && previousHash == currentHash);
                var isValueChanged = forceSerialize || !isSameValues;
                
                var entityHeader = new NetworkEntityHeader()
                {
                    Id = id,
                    IsValueChanged = isValueChanged,
                    Count = syncComponentsCount
                };

                networkWriter.WriteData(ref entityHeader, itemOffset);
                
                results[i] = new EcsEntityNetworkData()
                {
                    Id = id,
                    Hash = currentHash,
                    IsValueChanged = isValueChanged,
                    Count = syncComponentsCount,
                    Offset = startOffset + itemOffset,
                    Size = offset - itemOffset,
                };
            }
        }
    }
}