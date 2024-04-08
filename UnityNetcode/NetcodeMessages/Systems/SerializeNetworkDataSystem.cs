namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Buffers;
    using Aspects;
    using Componenets;
    using Leopotam.EcsLite;
    using MemoryPack.Compression;
    using Network.Serializer;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
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
    public class SerializeNetworkDataSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _netcodeMessageAspect;
        private NetworkMessageAspect _networkMessageAspect;

        private EcsWorld _world;

        private EcsNetworkSettings _networkSettings;

        private EcsFilter _netcodeFilter;
        private EcsFilter _historyFilter;
        private EcsFilter _transferRequestFilter;
        private EcsFilter _forceResendFilter;
        
        private NativeArray<byte> _arrayBuffer;
        private ArrayBufferWriter<byte> _arrayBufferWriter;
        private int _headerSize;
        private int _entityHeaderSize;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkSettings = _world.GetGlobal<EcsNetworkSettings>();
            _arrayBuffer = new NativeArray<byte>(512,Allocator.Persistent);
            _headerSize = UnsafeUtility.SizeOf<NetworkHeader>();
            _entityHeaderSize = UnsafeUtility.SizeOf<NetworkEntityHeader>();
            _arrayBufferWriter = new ArrayBufferWriter<byte>(512);
            
            _netcodeFilter = _world
                .Filter<NetcodeManagerComponent>()
                .Inc<NetworkConnectionTypeComponent>()
                .Inc<NetworkTimeComponent>()
                .End();

            _historyFilter = _world
                .Filter<NetworkHistoryComponent>()
                .End();

            _transferRequestFilter = _world
                .Filter<NetworkTransferRequest>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var serializeEntity = _transferRequestFilter.First();
            if (serializeEntity < 0) return;
            
            var netcodeEntity = _netcodeFilter.First();
            if (netcodeEntity < 0) return;

            var historyEntity = _historyFilter.First();
            if (historyEntity < 0) return;

            ref var historyComponent = ref _netcodeMessageAspect.History.Get(historyEntity);
            ref var timeComponent = ref _netcodeAspect.NetworkTime.Get(netcodeEntity);

            ref var history = ref historyComponent.History;
            var index = historyComponent.Index;
            var previousIndex = historyComponent.LastIndex;
            var previousData = historyComponent.History[previousIndex];
            
            ref var historyData = ref history[index];
            ref var byteData = ref historyData.SerializedData;
            var useHashFiltering = _networkSettings.useHashFiltering;
            var syncCount = historyData.EntityMap.Count;
            
            if(syncCount == 0 && previousData.EntityMap.Count == syncCount) return;

            _arrayBufferWriter.Clear();
            
            var resultMaxSize = byteData.Length + _headerSize;
            if (_arrayBuffer.Length < resultMaxSize)
            {
                _arrayBuffer.Dispose();
                _arrayBuffer = new NativeArray<byte>(resultMaxSize, Allocator.Persistent);
            }

            var offset = _headerSize;
            var byteArray = historyData.SerializedData;
            var readonlySpan = byteArray.AsReadOnlySpan();
            var componentsCount = 0;
            
            foreach (var historyPair in  historyData.EntityMap)
            {
                ref var value = ref historyPair.Value;
                componentsCount += value.Count;

                var writeSize = value.IsValueChanged ? value.Size : _entityHeaderSize;
                var slice = readonlySpan.Slice(value.Offset,writeSize);
                offset += _arrayBuffer.WriteData(ref slice, offset);
            }
            
            var header = new NetworkHeader()
            {
                Tick = historyComponent.Tick,
                Time = timeComponent.Time,
                Count = syncCount,
                Size = offset,
                Components = componentsCount,
                IsHashed = useHashFiltering,
            };

            _arrayBuffer.Serialize(ref header,0);
            var size = offset;
            
            ref var serializeResult = ref _netcodeMessageAspect.SerializationResult.Add(serializeEntity);
            serializeResult.Value =  _arrayBuffer;
            serializeResult.Size = size;
        }
       
    }
    
   
}