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
    using UniCore.Runtime.ProfilerTools;
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
    public class MakeNewServerRoomSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;

        private PhotonCallbacksAspect _photonCallbacksAspect;
        private PhotonAspect _photonAspect;
        private PhotonRoomAspect _roomAspect;

        private EcsFilter _roomFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _roomFilter = _world
                .Filter<MakeNewRoomSelfRequest>()
                .Inc<PhotonAgentComponent>()
                .Inc<PhotonStatusComponent>()
                .Inc<PhotonRoomStatusComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _roomFilter)
            {
                ref var statusComponent = ref _photonAspect.Status.Get(entity);
                ref var requestComponent = ref _roomAspect.CreateRoom.Get(entity);
                ref var roomComponent = ref _roomAspect.Status.Get(entity);
                var state = statusComponent.State;
                
                var isConnected = statusComponent.IsOnMasterServer ||
                                  state == ClientState.ConnectedToMasterServer ||
                                  state == ClientState.ConnectedToNameServer;
                
                if(!isConnected || roomComponent.IsConnecting) continue;
                
                var roomName = string.IsNullOrEmpty(requestComponent.RoomName) 
                    ? null : requestComponent.RoomName;
                
                roomComponent.IsConnecting = PhotonNetwork.CreateRoom(roomName, new RoomOptions 
                {
                    IsVisible = requestComponent.Visible,
                    IsOpen = requestComponent.Open,
                    MaxPlayers = requestComponent.MaxPlayers
                });

#if DEBUG
                GameLog.Log($"[MakeNewServerRoomSystem] creating new photon room: {roomName}",Color.green);       
#endif
            }
        }
    }
}