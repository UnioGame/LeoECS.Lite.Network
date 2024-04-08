namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using Aspects;
    using Components;
    using Components.Events.Room;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

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
    public class PlayersProcessEventsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private PhotonAspect _photonAspect;
        private PhotonPlayerAspect _playerAspect;
        private PhotonRoomAspect _roomAspect;
        
        private EcsFilter _filter;
        private EcsFilter _enterFilter;
        private EcsFilter _exitFilter;
        private EcsFilter _roomFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _enterFilter = _world
                .Filter<PlayerEnterRoomEvent>()
                .End();
            
            _exitFilter = _world
                .Filter<PlayerLeftRoomEvent>()
                .End();

            _roomFilter = _world
                .Filter<RoomPlayersComponent>()
                .Inc<PhotonRoomStatusComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _exitFilter)
            {
                ref var playerLeftRoomEvent = ref _roomAspect.PlayerLeftRoom.Get(eventEntity);
                var targetPlayer = playerLeftRoomEvent.Player;
                
                foreach (var agent in _roomFilter)
                {
                    ref var roomPlayersComponent = ref _photonAspect.Players.Get(agent);
                    var playersMap = roomPlayersComponent.Value;
                    var playerPackedEntity = playersMap[targetPlayer.ActorNumber];
                    
                    roomPlayersComponent.Value.Remove(targetPlayer.ActorNumber);
                    
                    if(!playerPackedEntity.Unpack(_world,out var playerEntity))continue;
       
                    _world.DelEntity(playerEntity);
                    
                    break;
                }
            }
        }
    }
}