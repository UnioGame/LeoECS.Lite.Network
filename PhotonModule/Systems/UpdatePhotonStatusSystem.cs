namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using System.Linq;
    using Aspects;
    using Components;
    using Components.Events;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Photon.Realtime;
    using UniGame.Core.Runtime.Extension;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;
    using UnityEngine.Pool;

    /// <summary>
    /// update photon info
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdatePhotonStatusSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;
        private PhotonCallbacksAspect _callbacksAspect;
        
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world
                .Filter<PhotonAgentComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var photonAgent = ref _photonAspect.PhotonAgent.Get(entity);
                ref var status = ref _photonAspect.Status.GetOrAddComponent(entity);
                
                if(_callbacksAspect.Connected.Has(entity))
                {
                    status.IsConnected = true;
                    status.Status = ConnectionStatus.Connected;
                }
                
                if(_callbacksAspect.Disconnected.Has(entity))
                {
                    status.IsConnected = false;
                    status.IsOnMasterServer = false;
                    status.Status = ConnectionStatus.Disconnected;
                }
                
                if(_callbacksAspect.ConnectedToMaster.Has(entity))
                {
                    status.IsConnected = true;
                    status.IsOnMasterServer = true;
                    status.Status = ConnectionStatus.Connected;
                }

                status.State = PhotonNetwork.NetworkClientState;
                status.AppVersion = PhotonNetwork.AppVersion;
                status.IsMaster = PhotonNetwork.IsMasterClient;
                status.IsInRoom = PhotonNetwork.InRoom;
                status.CountOfPlayersOnMaster = PhotonNetwork.CountOfPlayersOnMaster;
                status.CountOfPlayersInRooms = PhotonNetwork.CountOfPlayersInRooms;

                var room = PhotonNetwork.CurrentRoom;
                status.ServerName = room == null ? string.Empty : room.Name;

                if (status.IsMaster)
                {
                    _photonAspect.Master.GetOrAddComponent(entity);
                }
                else
                {
                    _photonAspect.Master.TryRemove(entity);
                }
            }
        }
    }
}