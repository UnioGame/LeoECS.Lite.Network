namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Componenets;
    using EcsThreads.Systems;
    using Leopotam.EcsLite;
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
    using Unity.Mathematics;
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
    public sealed class SerializeNetworkDataToHistoryTaskSystem 
        : EcsDataTaskSystem<EcsSerializationTask>
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _netcodeMessageAspect;
        private NetworkMessageAspect _networkMessageAspect;
        
        private NativeArray<SerializationTaskData> _taskData;
        private NativeArray<EcsEntityNetworkData> _taskResult;
        private int _serializedMaxCount;
        
        private EcsWorld _world;

        private EcsNetworkData _networkData;
        private NetworkData _defaultEcsData;
        private EcsNetworkSettings _networkSettings;

        private EcsFilter _netcodeFilter;
        private EcsFilter _historyFilter;
        private EcsFilter _serializeFilter;
        private EcsFilter _networkSyncFilter;

        public override bool IsMultithreaded => _networkSettings.multithreaded;

        protected override void OnInit(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkData = _world.GetGlobal<EcsNetworkData>();
            _networkSettings = _world.GetGlobal<EcsNetworkSettings>();
            _defaultEcsData = _networkSettings.networkData;

            _serializedMaxCount = 0;
            _taskData = new NativeArray<SerializationTaskData>(_serializedMaxCount, Allocator.Persistent);
            _taskResult = new NativeArray<EcsEntityNetworkData>(_serializedMaxCount, Allocator.Persistent);
            
            _netcodeFilter = _world
                .Filter<NetcodeManagerComponent>()
                .Inc<NetworkConnectionTypeComponent>()
                .Inc<NetworkTimeComponent>()
                .End();

            _historyFilter = _world
                .Filter<NetworkHistoryComponent>()
                .End();

            _serializeFilter = _world
                .Filter<SerializeNetworkEntityRequest>()
                .End();
            
            _networkSyncFilter = _world
                .Filter<NetworkIdComponent>()
                .Exc<NetworkSyncComponent>()
                .End();
        }
        
        public override int SetupTask(ref EcsSerializationTask task)
        {
            var netcodeEntity = _netcodeFilter.First();
            if (netcodeEntity < 0) return default;

            var historyEntity = _historyFilter.First();
            if (historyEntity < 0) return default;

            ref var historyComponent = ref _netcodeMessageAspect.History.Get(historyEntity);
            ref var timeComponent = ref _netcodeAspect.NetworkTime.Get(netcodeEntity);

            ref var history = ref historyComponent.History;
            var index = historyComponent.Index;
            var previousIndex = historyComponent.LastIndex;
            var previousData = historyComponent.History[previousIndex];

            ref var historyData = ref history[index];
            
            historyData.Tick = historyComponent.Tick;
            historyData.Time = timeComponent.Time;

            var serializeRequested = false;
            var forceSerialize = false;
            
            foreach (var requestEntity in _serializeFilter)
            {
                serializeRequested = true;
                ref var request = ref _netcodeMessageAspect.SerializeEntity.Get(requestEntity);
                forceSerialize = forceSerialize || request.Force;
            }
            
            if(!serializeRequested) return 0;
            
            var entityMap = historyData.EntityMap;
            var useHashFiltering = _networkSettings.useHashFiltering;
            var serializationCount = _networkSyncFilter.GetEntitiesCount();
            
            if(serializationCount <= 0) return 0;

            var minBufferSize = serializationCount * _defaultEcsData.serializationEntityChunkSize;
            var entityByteArray = historyData.SerializedData;
            var bufferSize = entityByteArray.Length;
            var startOffset = historyData.Count * _defaultEcsData.serializationEntityChunkSize;
            var newEntitiesCount = historyData.Count + serializationCount;
            
            var needResize = bufferSize < minBufferSize || newEntitiesCount > _serializedMaxCount;
            
            //resize buffers if needed
            if (needResize)
            {
                _serializedMaxCount = _serializedMaxCount == 0 ? newEntitiesCount : newEntitiesCount * 2;
                var byteBufferSize = _serializedMaxCount * _defaultEcsData.serializationEntityChunkSize;
                byteBufferSize = math.max(entityByteArray.Length, byteBufferSize);
                
                var newByteArray = new NativeArray<byte>(byteBufferSize, Allocator.Persistent);
                var sourceSpan = entityByteArray.AsReadOnlySpan();
                newByteArray.WriteData(ref sourceSpan,0);
                
                entityByteArray.Dispose();
                _taskData.Dispose();
                _taskResult.Dispose();
                
                _taskData = new NativeArray<SerializationTaskData>(_serializedMaxCount, Allocator.Persistent);
                _taskResult = new NativeArray<EcsEntityNetworkData>(_serializedMaxCount, Allocator.Persistent);
                
                historyData.SerializedData = newByteArray;
                entityByteArray = newByteArray;
            }

            var data = _taskData;
            var dataIndex = 0;
            
            foreach (var entity in _networkSyncFilter)
            {
                ref var idComponent = ref _networkMessageAspect.NetworkId.Get(entity);
                var taskData = new SerializationTaskData()
                {
                    Id = idComponent.Id,
                    Entity = entity,
                };
                
                data[dataIndex] = taskData;
                dataIndex++;
            }

            var length = entityByteArray.Length - startOffset;
            
            task.source = _taskData;
            task.results = _taskResult;
            task.length = dataIndex;
            task.forceSerialize = forceSerialize;
            task.serializedResult = entityByteArray.GetSubArray(startOffset,length);
            task.chunkSize = _defaultEcsData.serializationEntityChunkSize;
            
            task.world = _world;
            task.previousData = previousData;
            task.historyData = historyData;
            task.networkData = _networkData;
            task.useHashFiltering = useHashFiltering;
            
            return dataIndex;
        }

        public override void OnTaskComplete(ref EcsSerializationTask task)
        {
            ref var historyData = ref task.historyData;
            ref var results = ref task.results;
            ref var data = ref historyData.EntityMap;
            
            historyData.Offset += results.Length * _defaultEcsData.serializationEntityChunkSize;
            
            for (var i = 0; i < task.length; i++)
            {
                var entityData = task.results[i];
                var id = entityData.Id;
                if (!data.ContainsKey(id))
                    historyData.Size += entityData.Size;
                data[id] = entityData;
            }
            
            historyData.Count = task.length;
        }
    }
}