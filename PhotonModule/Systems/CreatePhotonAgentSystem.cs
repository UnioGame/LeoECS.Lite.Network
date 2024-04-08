namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

    /// <summary>
    /// initialize photon connection
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class CreatePhotonAgentSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;
        private PhotonRoomAspect _roomAspect;
        private EcsFilter _filter;
        private EcsFilter _agentFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<CreatePhotonAgentSelfRequest>()
                .End();
            
            _agentFilter = _world
                .Filter<PhotonAgentComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var linkComponent = ref _photonAspect.Link.GetOrAddComponent(entity);
                
                var agentEntity = -1;
                
                foreach (var agent in _agentFilter)
                {
                    agentEntity = agent;
                }

                agentEntity = agentEntity < 0 ? _world.NewEntity() : agentEntity;

                ref var agentComponent = ref _photonAspect.PhotonAgent.GetOrAddComponent(agentEntity);
                ref var playersComponent = ref _photonAspect.Players.GetOrAddComponent(agentEntity);
                ref var roomStatusComponent = ref _roomAspect.Status.GetOrAddComponent(agentEntity);
                
                linkComponent.Value = _world.PackEntity(agentEntity);
            }
        }
    }
}