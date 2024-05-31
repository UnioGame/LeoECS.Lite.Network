namespace Game.Ecs.Network.UnityNetcode.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using global::Componenets.Requests;
    using global::UnityNetcode.Componenets.Requests;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetcodeRoomAspect : EcsAspect
    {
        public EcsPool<NetcodeRoomStatusComponent> Status;
        public EcsPool<NetcodeRoomComponent> Room;
        public EcsPool<NetcodeRoomPlayersComponent> Players;

        // Requests
        public EcsPool<TryCreateNetcodeRoomSelfRequest> CreateRoomRequest;
        public EcsPool<MakeNewNetcodeRoomSelfRequest> MakeRoomRequest;
        public EcsPool<NetcodePlayerJoinRoomRequest> PlayerJoinRoomRequest;

        // Events
        public EcsPool<NetcodePlayerLeftRoomEvent> PlayerLeftRoomEvent;
        public EcsPool<NetcodePlayerJoinedRoomEvent> PlayerJoinedRoomEvent;
        public EcsPool<NetcodePlayerFailedToJoinRoomEvent> PlayerFailedToJoinRoomEvent;
        public EcsPool<NetcodePlayerRetryJoinRoomEvent> PlayerRetryJoinRoomEvent;
        public EcsPool<NetcodePlayerEnterRoomEvent> PlayerEnterRoomEvent;
        public EcsPool<NetcodeRoomClosedEvent> RoomClosedEvent;
    }
}