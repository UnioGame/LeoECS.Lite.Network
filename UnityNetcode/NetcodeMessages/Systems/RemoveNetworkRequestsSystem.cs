namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using Shared.Components;
    using UniCore.Runtime.ProfilerTools;
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
    public class RemoveNetworkRequestsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkMessageAspect _messageAspect;
        private NetcodeMessageAspect _rpcAspect;
        
        private EcsWorld _world;
        
        private EcsFilter _filter;
        private EcsFilter _transferFilter;
        private EcsFilter _networkFilter;
        private EcsFilter _eventFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _networkFilter = _world
                .Filter<NetworkConnectionTypeComponent>()
                .End();
            
            _transferFilter = _world
                .Filter<NetworkTransferRequest>()
                .End();
            
            _eventFilter = _world
                .Filter<SerializeNetworkEntityRequest>()
                .Inc<NetworkEventComponent>()
                .Exc<NetworkSyncComponent>()
                .End();
            
            _filter = _world
                .Filter<NetworkIdComponent>()
                .Exc<NetworkSyncComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _eventFilter)
            {
                GameLog.Log($"Removing serialized message {eventEntity}");
                _world.DelEntity(eventEntity);
            }
            
            var networkEntity = _networkFilter.First();
            if (networkEntity < 0) return;
            
            ref var connectionComponent = ref _networkAspect.ConnectionType.Get(networkEntity);
            if (connectionComponent.IsServer) return;
            
            var transferEntity = _transferFilter.First();
            if(transferEntity<0) return;
            
            foreach (var entity in _filter)
            {
                _messageAspect.NetworkId.Del(entity);
                _messageAspect.Target.TryRemove(entity);
            }
        }
    }
}