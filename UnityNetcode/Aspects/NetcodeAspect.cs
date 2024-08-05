namespace Game.Ecs.Network.UnityNetcode.Aspects
{
    using System;
    using Componenets.Requests;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcsLite.LeoEcs.Shared.Components;

    /// <summary>
    /// netcode aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetcodeAspect : EcsAspect
    {
        public EcsPool<NetcodeManagerComponent> Manager;
        public EcsPool<NetcodeSharedRPCComponent> RPCAsset;
        public EcsPool<UnityTransportComponent> Transport;
        public EcsPool<LifeTimeComponent> LifeTime;
        public EcsPool<NetworkTimeComponent> NetworkTime;
        
        public EcsPool<NetcodeAgentComponent> Agent;
        public EcsPool<NetcodeStatusComponent> Status;

        public EcsPool<NetcodeMessageSenderId> SenderId;
        
        public EcsPool<NetworkConnectionTypeComponent> ConnectionType;
        //requests
        //initialize netcode and create new entity if not exists
        public EcsPool<InitializeNetcodeSelfRequest> InitializeSelf;
    }
}