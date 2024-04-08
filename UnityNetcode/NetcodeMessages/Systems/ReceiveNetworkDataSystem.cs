namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Buffers;
    using Aspects;
    using Leopotam.EcsLite;
    using MemoryPack.Compression;
    using Network.Serializer;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
    using UnityEngine;
    using UnityNetcode.Aspects;

    /// <summary>
    /// send message with base rpc channel
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class ReceiveNetworkDataSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _messageAspect;

        private EcsWorld _world;
        private EcsFilter _receiveFilter;
        
        private EcsNetworkData _networkData;
        private EcsNetworkSettings _networkSettings;
        
        private byte[] _rentArray;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkData = _world.GetGlobal<EcsNetworkData>();
            _networkSettings = _world.GetGlobal<EcsNetworkSettings>();
            
            _receiveFilter = _world
                .Filter<NetworkMessageDataComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _receiveFilter)
            {
                ref var dataComponent = ref _messageAspect.MessageData.Get(entity);
                
                var dataLength = dataComponent.Size;

#if UNITY_EDITOR || DEBUG
                if (dataLength <= 0)
                {
                    Debug.LogError("ReceiveNetworkDataSystem: data length is zero!");
                    return;
                }
#endif
                
                NativeArray<byte> buffer = default;
                using var decompressor = new BrotliDecompressor();
                
                if (_networkSettings.useNetworkCompression)
                {
                    var decompressValue = decompressor
                        .Decompress(dataComponent.Value.AsSpan(0,dataLength));
                    
                    dataLength = (int)decompressValue.Length;
                    buffer = new NativeArray<byte>(dataLength, Allocator.Temp);
                    decompressValue.CopyTo(buffer);
                }
                else
                {
                    buffer = new NativeArray<byte>(dataComponent.Value, Allocator.Temp);
                }
                
                var networkHeader = new NetworkHeader();
                var readCount = buffer.Deserialize(0,ref networkHeader);
                var syncCount = networkHeader.Count;
                var totalComponents = networkHeader.Components;

#if UNITY_EDITOR
                if (networkHeader.Size != dataLength)
                {
                    Debug.LogError($"ReceiveNetworkDataSystem: data length mismatch! {networkHeader.Size} != {dataLength}");    
                    return;
                }
#endif
                ref var receiveComponent = ref _messageAspect.ReceiveResult.Add(entity);
                
                receiveComponent.RawData = buffer;
                receiveComponent.Tick = networkHeader.Tick;
                receiveComponent.Time = networkHeader.Time;
                receiveComponent.Count = syncCount;
                receiveComponent.Size = dataLength;
                receiveComponent.Data = new NativeArray<ReceiveEntityData>(syncCount, Allocator.Temp);
                receiveComponent.Components = new NativeArray<ReceiveComponentData>(totalComponents, Allocator.Temp);

#if UNITY_EDITOR
                if (syncCount == 0)
                {
                    Debug.Log($"Tick: {receiveComponent.Tick} | Entity Count : {syncCount} | Components : {totalComponents}");
                }
#endif
                
                ref var componentsData = ref receiveComponent.Components;
                
                var componentIndex = 0;
                var offset = readCount;

                for (var entityIndex = 0; entityIndex < syncCount; entityIndex++)
                {
                    var entityHeader = new NetworkEntityHeader();
                    readCount = buffer.Deserialize(offset, ref entityHeader);
                    offset+= readCount;
                    
                    var componentsCount = entityHeader.Count;
                    var entityComponents = entityHeader.IsValueChanged ? componentsCount : 0;
                    
                    var entityData = new ReceiveEntityData
                    {
                        Id = entityHeader.Id,
                        Count = componentsCount,
                        IsValueChanged = entityHeader.IsValueChanged,
                        ComponentIndexAt = componentIndex,
                    };
                    
                    receiveComponent.Data[entityIndex] = entityData;
                    
                    for (var i = 0; i < entityComponents; i++)
                    {
                        var componentHeader = new NetworkComponentHeader();
                        readCount = buffer.Deserialize(offset, ref componentHeader);
                        offset += readCount;

                        var componentValueOffset = offset;
                        var typeId = componentHeader.TypeId;
                        var componentLen = componentHeader.Size;
                        offset += componentLen;
#if UNITY_EDITOR
                        var typeFound =_networkData.TryGetServerType(typeId, out var syncType);
                        if (!typeFound)
                        {
                            Debug.LogError($"ReceiveNetworkDataSystem: component type with id {typeId} not found in types map!");
                            continue;
                        }       
#endif
                        componentsData[componentIndex] = new ReceiveComponentData
                        {
                            TypeId = typeId,
                            Size = componentLen,
                            Component = new NativeSlice<byte>(buffer,componentValueOffset,componentLen),
                        };
                        
                        componentIndex++;
                    }
                }
            }
        }
    }
}