namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Aspects
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Shared.Components;

    /// <summary>
    /// network client aspect data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetcodeClientAspect : EcsAspect
    {
        public EcsPool<GameObjectComponent> GameObject;
        //base network client marker
        public EcsPool<NetworkClientComponent> Client;
        //id of client
        public EcsPool<NetworkClientIdComponent> ClientId;
        //link to client game object
        public EcsPool<NetcodeClientObjectComponent> ClientObject;
    }
}