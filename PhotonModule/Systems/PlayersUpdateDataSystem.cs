namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using System.Collections.Generic;
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
    public class PlayersUpdateDataSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;
        private PhotonRoomAspect _roomAspect;
        private PhotonPlayerAspect _playerAspect;
        
        private EcsFilter _players;
        private EcsFilter _roomStatusFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _players = _world
                .Filter<PhotonPlayerComponent>()
                .End();
            
            _roomStatusFilter = _world
                .Filter<PhotonRoomStatusComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var statusEntity in _roomStatusFilter)
            {
                ref var roomStatus = ref _roomAspect.Status.Get(statusEntity);
                ref var roomComponent = ref _roomAspect.Room.Get(statusEntity);
                ref var playersComponent = ref _roomAspect.Players.Get(statusEntity);
                
                if(!roomStatus.IsInRoom) continue;

                var room = roomComponent.Value;
                if(room ==null) continue;

                var roomPlayers = room.Players;
                var playersMap = playersComponent.Value;
                
                foreach (var roomPlayerPair in room.Players)
                {
                    var id = roomPlayerPair.Key;
                    var player = roomPlayerPair.Value;
                    
                    if(player.IsMasterClient && !roomStatus.CountMasterAsPlayer) continue;

                    var isActivePlayer = !player.IsInactive;
                    
                    if (!playersMap.TryGetValue(id, out var playerPacked) || 
                        !playerPacked.Unpack(_world,out _))
                    {
                        var newPlayerEntity = _world.NewEntity();
                        ref var newPlayerComponent = ref _playerAspect.Player.Add(newPlayerEntity);
                        newPlayerComponent.Player = player;
                        playerPacked = _world.PackEntity(newPlayerEntity);
                        playersMap[id] = playerPacked;
                    }

                    playerPacked.Unpack(_world,out var playerEntity);
                    
                    ref var playerComponent = ref _playerAspect.Player.Get(playerEntity);

                    playerComponent.Player = player;
                    playerComponent.Id = player.ActorNumber;
                    playerComponent.IsLocal = player.IsLocal;
                    playerComponent.IsMaster = player.IsMasterClient;
                    playerComponent.IsActive = isActivePlayer;
                }

                break;
            }
            
        }
    }
}