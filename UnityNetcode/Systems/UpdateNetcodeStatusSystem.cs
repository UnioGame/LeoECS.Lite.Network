namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// initialize netcode data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateNetcodeStatusSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        
        private EcsFilter _networkLinkFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<NetcodeManagerComponent>()
                .End();

            _networkLinkFilter = _world
                .Filter<NetworkLinkComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var managerComponent = ref _netcodeAspect.Manager.Get(entity);
                var manager = managerComponent.Value;
                
                ref var agentComponent = ref _netcodeAspect.Agent.Get(entity);
                ref var connectionTypeComponent = ref _networkAspect.ConnectionType.Get(entity);
                
                var isClient = manager.IsClient || manager.IsHost;
                var isServer = manager.IsServer;
        
                ref var statusComponent = ref _netcodeAspect.Status.Get(entity);
                statusComponent.IsConnected = true;
                statusComponent.Status = ConnectionStatus.Connected;
                connectionTypeComponent.IsClient = isClient;
                connectionTypeComponent.IsServer = isServer;
                connectionTypeComponent.IsActive = isClient || isServer;
        
                agentComponent.Id = manager.LocalClientId;
            }

            foreach (var linkEntity in _networkLinkFilter)
            {
                var linkComponent = _networkAspect.NetworkLink.Get(linkEntity);
            }
        }
    }
}