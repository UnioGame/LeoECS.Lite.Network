namespace Girand.Ecs.GameSettings.Systems
{
    using System;
    using System.Collections.Generic;
    using Aspects;
    using Components;
    using ExitGames.Client.Photon;
    using Leopotam.EcsLite;
    using Photon.Pun;
    using Photon.Realtime;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    /// <summary>
    /// Class responsible for handling Photon connection callback events.
    /// </summary>
    [Serializable]
    [ECSDI]
    public class PhotonRoomCallbacksSystem : 
        IEcsInitSystem, 
        IEcsRunSystem,
        IInRoomCallbacks
    {
        private EcsWorld _world;
        private List<RoomCallbackEvent> _callbackEvents = new(128);
        private PhotonRoomAspect _roomAspect;
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<PhotonAgentComponent>()
                .End();
            
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventItem in _callbackEvents)
            {
                foreach (var entity in _filter)
                {
                    switch (eventItem.Type)
                    {
                        case RoomCallbackType.PlayerEnteredRoom:
                            ref var enterRoomEvent = ref _roomAspect.PlayerEnteredRoom.GetOrAddComponent(entity);
                            enterRoomEvent.Player = (Player) eventItem.Data;
                            break;
                        
                        case RoomCallbackType.PlayerLeftRoom:
                            ref var leftEvent = ref _roomAspect.PlayerLeftRoom.GetOrAddComponent(entity);
                            leftEvent.Player = (Player) eventItem.Data;
                            break;
                        
                        case RoomCallbackType.MasterClientSwitched:
                            ref var masterClientSwitched = ref _roomAspect.MasterClientSwitched.GetOrAddComponent(entity);
                            masterClientSwitched.NewMaster = (Player) eventItem.Data;
                            break;
                        
                        case RoomCallbackType.PlayerPropertiesUpdate:
                            
                            ref var regionListReceivedEvent = ref _roomAspect
                                .PlayerPropertiesUpdate
                                .GetOrAddComponent(entity);
                            
                            var value = (PlayerPropertiesData) eventItem.Data;
                            regionListReceivedEvent.ChangedProps = value.ChangedProps;
                            regionListReceivedEvent.TargetPlayer = value.TargetPlayer;
                            break;
                        
                        case RoomCallbackType.RoomPropertiesUpdate:
                            ref var roomUpdateEvent = ref _roomAspect
                                .RoomPropertiesUpdate
                                .GetOrAddComponent(entity);
                            
                            roomUpdateEvent.PropertiesThatChanged = (Hashtable) eventItem.Data;
                            break;
                    }
                }
            }
            
            _callbackEvents.Clear();
        }

        #region IRoomCallbacks
        
        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            _callbackEvents.Add(new RoomCallbackEvent
            {
                Type = RoomCallbackType.PlayerEnteredRoom,
                Data = newPlayer
            });
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            _callbackEvents.Add(new RoomCallbackEvent
            {
                Type = RoomCallbackType.PlayerLeftRoom,
                Data = otherPlayer
            });
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            _callbackEvents.Add(new RoomCallbackEvent
            {
                Type = RoomCallbackType.RoomPropertiesUpdate,
                Data = propertiesThatChanged
            });
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            _callbackEvents.Add(new RoomCallbackEvent
            {
                Type = RoomCallbackType.PlayerPropertiesUpdate,
                Data = new PlayerPropertiesData()
                {
                    TargetPlayer = targetPlayer,
                    ChangedProps = changedProps,
                }
            });
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            _callbackEvents.Add(new RoomCallbackEvent
            {
                Type = RoomCallbackType.MasterClientSwitched,
                Data = newMasterClient
            });
        }
        
        #endregion
    }
}