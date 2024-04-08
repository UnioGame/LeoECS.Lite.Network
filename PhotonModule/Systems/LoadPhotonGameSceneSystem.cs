namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using System.Collections.Generic;
    using Aspects;
    using Components.Events;
    using ExitGames.Client.Photon;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Photon.Realtime;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// handle photon event and send to ecs
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class LoadPhotonGameSceneSystem : 
        IEcsInitSystem, IEcsRunSystem,
        IEcsDestroySystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;

        private EcsFilter _filter;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();


            _filter = _world
                .Filter<LoadNetworkSceneRequest>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var request = ref _photonAspect.LoadScene.Get(entity);
                PhotonNetwork.LoadLevel(request.Value);
            }
        }

        public void Destroy(IEcsSystems systems)
        {
            
        }
    }
}