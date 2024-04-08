namespace Girand.Ecs.GameSettings.Aspects
{
    using System;
    using Components;
    using Components.Events.Room;
    using Leopotam.EcsLite;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

    /// <summary>
    /// room entity description
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class PhotonRoomAspect : EcsAspect
    {
        public EcsPool<PhotonRoomStatusComponent> Status;
        public EcsPool<PhotonRoomComponent> Room;
        public EcsPool<RoomPlayersComponent> Players;
        
        //requests
        public EcsPool<MakeNewRoomSelfRequest> CreateRoom;
        
        //events
        public EcsPool<PlayerEnterRoomEvent> PlayerEnteredRoom;
        public EcsPool<PlayerLeftRoomEvent> PlayerLeftRoom;
        public EcsPool<RoomPropertiesEvent> RoomPropertiesUpdate;
        public EcsPool<PlayerPropertiesUpdateEvent> PlayerPropertiesUpdate;
        public EcsPool<MasterClientSwitchedEvent> MasterClientSwitched;
    }
}