namespace Girand.Ecs.GameSettings.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Leopotam.EcsLite;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class PhotonCallbacksAspect : EcsAspect
    {
        //events
        public EcsPool<NetworkAuthenticationFailedSelfEvent> AuthenticationFailed;
        public EcsPool<NetworkAuthenticationResponseSelfEvent> AuthenticationResponse;
        public EcsPool<NetworkConnectedSelfEvent> Connected;
        public EcsPool<NetworkConnectedToMasterSelfEvent> ConnectedToMaster;
        public EcsPool<NetworkDisconnectedSelfEvent> Disconnected;
        public EcsPool<NetworkRegionListReceivedSelfEvent> RegionReceived;
    }
}