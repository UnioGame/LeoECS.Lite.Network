namespace Game.Ecs.Network.Shared.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Components.Requests;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

    /// <summary>
    /// network client aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetworkClientAspect : EcsAspect
    {
        //base network client marker
        public EcsPool<NetworkClientComponent> Client;
        //id of client
        public EcsPool<NetworkClientIdComponent> ClientId;
        //link to network transport
        public EcsPool<NetworkLinkComponent> NetworkLink;
        //connection type data
        public EcsPool<NetworkConnectionTypeComponent> Connection;
        //server address
        public EcsPool<NetworkAddressComponent> ServerAddress;
        
        //=== optional ===
        //mark client as local
        public EcsPool<NetworkLocalClientComponent> Local;
        //mark client as master
        public EcsPool<NetworkMasterClientComponent> Master;
        
        //=== requests ===
        
        /// <summary>
        /// Connect to server as a client
        /// </summary>
        public EcsPool<StartNetworkClientSelfRequest> Connect;
        
        //=== events ====
        
        /// <summary>
        /// send when client connected to server
        /// </summary>
        public EcsPool<NetworkClientConnectedSelfEvent> Connected;

        /// <summary>
        /// send when client disconnected from server
        /// </summary>
        public EcsPool<NetworkClientDisconnectedEvent> Disconnected;
    }
}