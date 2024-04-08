namespace Girand.Ecs.GameSettings.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Leopotam.EcsLite;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UnityEngine.Serialization;

    /// <summary>
    /// photon aspect data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class PhotonAspect : EcsAspect
    {
        public PhotonCallbacksAspect Callbacks;
        public PhotonRoomAspect Room;
        public PhotonPlayerAspect photonPlayer;
        
        public EcsPool<PhotonAgentComponent> PhotonAgent;
        public EcsPool<MasterServerComponent> Master;
        public EcsPool<PhotonStatusComponent> Status;
        public EcsPool<PhotonAgentLinkComponent> Link;
        public EcsPool<RoomPlayersComponent> Players;
        
        //requests
        public EcsPool<CreatePhotonAgentSelfRequest> CreateAgent;
        public EcsPool<MakeNewRoomSelfRequest> NewRoomRequest;
        public EcsPool<ConnectToPhotonSelfRequest> Connect;
        public EcsPool<JoinRoomRequest> Join;
        public EcsPool<PhotonEventRequest> SendEvent;
        public EcsPool<PhotonOthersEventRequest> ToOthersEvent;
        public EcsPool<PhotonAllEventRequest> ToAllEvent;
        public EcsPool<PhotonServerEventRequest> ToServerEvent;
        public EcsPool<LoadNetworkSceneRequest> LoadScene;
        
        //events
        public EcsPool<PhotonNetworkDataEvent> DataEvent;
    }
}