namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using System.Linq;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Photon.Realtime;
    using Server.Components.Requests;
    using UniGame.Core.Runtime.Extension;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;
    using UnityEngine.Pool;

    /// <summary>
    /// create new room on server
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class JoinGameRoomSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;

        private PhotonCallbacksAspect _photonCallbacksAspect;
        private PhotonAspect _photonAspect;
        private PhotonRoomAspect _roomAspect;

        private EcsFilter _roomFilter;
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<JoinRoomRequest>()
                .End();
            
            _roomFilter = _world
                .Filter<PhotonAgentComponent>()
                .Inc<PhotonStatusComponent>()
                .Inc<PhotonRoomStatusComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var requestEntity in _filter)
            {
                foreach (var entity in _roomFilter)
                {
                    ref var roomStatus = ref _roomAspect.Status.Get(entity);
                    ref var photonStatus = ref _photonAspect.Status.Get(entity);
                    
                    if(!photonStatus.IsConnected) continue;
                    
                    //is client is master server then mark server as a valid client and create room
                    if (photonStatus.IsMaster)
                    {
                        roomStatus.CountMasterAsPlayer = true;
                        _roomAspect.CreateRoom.GetOrAddComponent(entity);
                    }
                    else
                    {
                        roomStatus.IsConnecting = PhotonNetwork.JoinRandomRoom();
                    }
                    
                    break;
                }
            }
            
        }
    }
}