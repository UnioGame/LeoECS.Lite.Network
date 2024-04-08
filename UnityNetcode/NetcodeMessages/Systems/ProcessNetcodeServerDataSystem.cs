namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
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
    public class ProcessNetcodeServerDataSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _messageAspect;

        private EcsWorld _world;
        private EcsFilter _receiveFilter;
        private EcsFilter _networkFilter;
        
        private EcsNetworkData _networkData;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkData = _world.GetGlobal<EcsNetworkData>();

            _networkFilter = _world
                .Filter<NetworkTimeComponent>()
                .Inc<NetworkConnectionTypeComponent>()
                .Inc<NetworkSyncValuesComponent>()
                .End();
            
            _receiveFilter = _world
                .Filter<NetworkReceiveResultComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var networkEntity = _networkFilter.First();
            if (networkEntity < 0) return;
            
            ref var connection = ref _netcodeAspect.ConnectionType.Get(networkEntity);
            if (!connection.IsServer) return;
            
            foreach (var entity in _receiveFilter)
            {
                ref var receivedDataComponent = ref _messageAspect.ReceiveResult.Get(entity);
                ref var entities = ref receivedDataComponent.Data;
                ref var components = ref receivedDataComponent.Components;
                
                var count = receivedDataComponent.Count;

                for (var i = 0; i < count; i++)
                {
                    var entityData = entities[i];
                    
                    if(!entityData.IsValueChanged) continue;
                    
                    var finalComponent = entityData.ComponentIndexAt + entityData.Count;
                    var newEntity = -1;
                    
                    for (var j = entityData.ComponentIndexAt; j < finalComponent; j++)
                    {
                        var componentData = components[j];
                        
                        //is client type allowed
                        if(!_networkData.TryGetClientType(componentData.TypeId,out var componentType)) continue;

                        var serialized = componentType.serializer;
                        
                        newEntity = newEntity < 0 ? _world.NewEntity() : newEntity;
                        serialized.Deserialize(_world, newEntity, ref componentData.Component);
                    }
                }
            }
        }
    }
}