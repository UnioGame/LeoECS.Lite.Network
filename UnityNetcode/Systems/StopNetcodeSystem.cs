namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Aspects;
    using Shared.Components.Requests;
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
    public class StopNetcodeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        
        private bool _isLoading;
        private EcsFilter _networkFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<StopNetworkSelfRequest>()
                .End();

            _networkFilter = _world
                .Filter<NetcodeManagerComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                foreach (var netEntity in _networkFilter)
                {
                    ref var managerComponent = ref _netcodeAspect.Manager.Get(netEntity);
                    var manager = managerComponent.Value;
                    //stop host
                    manager.Shutdown();
                }
            }
        }

    }
}