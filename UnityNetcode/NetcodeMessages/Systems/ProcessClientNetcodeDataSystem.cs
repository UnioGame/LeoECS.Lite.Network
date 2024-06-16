namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
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
    public class ProcessClientNetcodeDataSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _messageAspect;
        private NetworkMessageAspect _networkMessageAspect;

        private EcsWorld _world;
        private EcsFilter _receiveFilter;
        private EcsFilter _networkFilter;
        private EcsFilter _historyFilter;

        private EcsNetworkData _networkData;
        public NativeHashMap<int, EcsPackedEntity> _entityCache;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkData = _world.GetGlobal<EcsNetworkData>();
            _entityCache = new NativeHashMap<int, EcsPackedEntity>(256, Allocator.Persistent);
            
            _networkFilter = _world
                .Filter<NetworkConnectionTypeComponent>()
                .Inc<NetworkSyncValuesComponent>()
                .End();

            _historyFilter = _world
                .Filter<NetworkHistoryComponent>()
                .End();
            
            _receiveFilter = _world
                .Filter<NetworkReceiveResultComponent>()
                .End();
        }
        
        public void Destroy(IEcsSystems systems)
        {
            _entityCache.Dispose();
        }

        public void Run(IEcsSystems systems)
        {
            var networkEntity = _networkFilter.First();
            if (networkEntity < 0) return;
            
            //process only data coming from server
            ref var connection = ref _netcodeAspect.ConnectionType.Get(networkEntity);
            ref var syncValuesComponent = ref _messageAspect.SyncValues.Get(networkEntity);
            
            if(!connection.IsClient) return;

            var syncIds = syncValuesComponent.Values;
            if (_receiveFilter.GetEntitiesCount() <= 0) return;
            
            //clear local sync values
            _entityCache.Clear();
            
            foreach (var entity in _receiveFilter)
            {
                ref var receivedComponent = ref _networkMessageAspect.ReceiveResult.Get(entity);
                ref var entities = ref receivedComponent.Data;
                ref var components = ref receivedComponent.Components;
                
                for (var i = 0; i < receivedComponent.Count; i++)
                {
                    var entityData = entities[i];
                    var syncId = entityData.Id;
                    var targetEntity = -1;
                    
                    if (syncIds.TryGetValue(syncId, out var entityPacked))
                    {
                        if(entityPacked.Unpack(_world, out var syncEntity)) 
                            targetEntity = syncEntity;
                    }

                    if (targetEntity < 0)
                    {
                        targetEntity = _world.NewEntity();
                        ref var syncIdComponent = ref _networkMessageAspect.NetworkId.Add(targetEntity);
                        syncIdComponent.Id = syncId;
                    }
                    
                    var packedEntity = _world.PackEntity(targetEntity);
                    _entityCache[syncId] = packedEntity;
                    
                    //mark entity as server
                    _messageAspect.ServerEntity.GetOrAddComponent(targetEntity);

                    if (!entityData.IsValueChanged)
                    {
                        continue;
                    }
                    
                    //remove ald network values
                    _world.RemoveComponents<IEcsNetworkValue>(targetEntity);

                    var finalComponent = entityData.ComponentIndexAt + entityData.Count;
                    //add new network components values
                    for (var j = entityData.ComponentIndexAt; j < finalComponent; j++)
                    {
                        var componentData = components[j];
                        if (!_networkData.TryGetServerType(componentData.TypeId, out var componentDesc))
                        {
                            continue;
                        }
                        
                        var serializer = componentDesc.serializer;
                        serializer.Deserialize(_world, targetEntity, ref componentData.Component);
                        ref var senderIdComponent = ref _netcodeAspect.SenderId.GetOrAddComponent(targetEntity);
                        senderIdComponent.Value = receivedComponent.Sender;
                    }
                }
            }
            
            syncValuesComponent.Values = _entityCache;
            _entityCache = syncIds;
        }

    }
}