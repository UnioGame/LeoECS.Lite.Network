namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

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
    public class RemoveClientMissingSyncValuesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkSyncAspect _networkSyncAspect;
        private NetcodeMessageAspect _messageAspect;
        private NetworkMessageAspect _networkMessageAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsFilter _requestFilter;
        private EcsFilter _syncValueFilter;
        private EcsFilter _networkFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _networkFilter = _world
                .Filter<NetworkConnectionTypeComponent>()
                .Inc<NetworkSyncValuesComponent>()
                .End();
            
            _filter = _world
                .Filter<NetworkIdComponent>()
                .Inc<NetworkSyncComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var networkEntity = _networkFilter.First();
            if (networkEntity < 0) return;
            
            ref var syncValuesComponent = ref _messageAspect.SyncValues.Get(networkEntity);
            
            foreach (var entity in _filter)
            {
                ref var syncIdComponent = ref _networkMessageAspect.NetworkId.Get(entity);
                var syncId = syncIdComponent.Id;
                var found = syncValuesComponent.Values.TryGetValue(syncId, out var packedEntity);
                var exists = packedEntity.Unpack(_world, out var syncEntity);

                if (!found || !exists || entity != syncEntity)
                {
                    _world.DelEntity(entity);
                }
            }
        }
    }
}