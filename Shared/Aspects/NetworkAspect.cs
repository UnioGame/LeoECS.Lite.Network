namespace Game.Ecs.Network.Shared.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Components.Requests;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

    /// <summary>
    /// shared network aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetworkAspect : EcsAspect
    {
        public EcsPool<NetworkLinkComponent> NetworkLink;
        public EcsPool<NetworkSourceComponent> NetworkAgent;
        public EcsPool<NetworkAddressComponent> Address;
        public EcsPool<NetworkConnectionTypeComponent> ConnectionType;
        
        //netcode runtime info
        //public EcsPool<NetworkActiveComponent> Active;
        
        //server time
        public EcsPool<NetworkTimeComponent> NetworkTime;
        
        //requests
        
        // create new host
        public EcsPool<StartNetworkSelfRequest> StartNetwork;
        public EcsPool<StopNetworkSelfRequest> StopNetwork;

        /// <summary>
        /// server connected event
        /// </summary>
        public EcsPool<NetworkServerConnectedSelfEvent> ServerConnected;
    }
}