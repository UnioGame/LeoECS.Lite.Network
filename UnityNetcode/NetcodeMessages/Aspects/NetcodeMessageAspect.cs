namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Aspects
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UnityEngine.Serialization;

    /// <summary>
    /// netcode rpc aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetcodeMessageAspect : EcsAspect
    {
        public NetworkMessageAspect MessageAspect;
        
        public EcsPool<NetworkMessageChannelSource> Source;
        public EcsPool<NetcodeMessageChannelComponent> Channel;
        public EcsPool<NetworkSerializationResult> SerializationResult;
        public EcsPool<NetworkSyncComponent> ServerEntity;
        public EcsPool<NetworkReceiveResultComponent> ReceiveResult;
        
        /// <summary>
        /// history of sync values during several ticks
        /// </summary>
        public EcsPool<NetworkHistoryComponent> History;
        
        /// <summary>
        /// network value id
        /// </summary>
        public EcsPool<NetworkSyncValuesComponent> SyncValues;
        public EcsPool<NetworkIdComponent> Id;
        
        //data to receive
        public EcsPool<NetworkMessageDataComponent> MessageData;
        
        //=== requests ===
        
        //request to serialize current ecs data to history
        public EcsPool<NetworkSerializeRequest> Serialize;
        //request to send history data 
        public EcsPool<NetworkTransferRequest> Transfer;
        public EcsPool<SerializeNetworkEntityRequest> SerializeEntity;
    }
}