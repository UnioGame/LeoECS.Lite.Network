namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

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
    public class ConnectToPhotonSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<ConnectToPhotonSelfRequest>()
                .Inc<PhotonAgentComponent>()
                .Inc<PhotonStatusComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var statusComponent = ref _photonAspect.Status.Get(entity);
                if(PhotonNetwork.IsConnected) continue;

                var result = PhotonNetwork.ConnectUsingSettings();
                if (!result) return;

                statusComponent.Status = ConnectionStatus.Connecting;
            }
        }
    }
}