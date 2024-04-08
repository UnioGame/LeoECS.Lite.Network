namespace Game.Ecs.Network.NetworkCommands.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Components.Requests;
    using Leopotam.EcsLite;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

    /// <summary>
    /// rpc aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetworkMessageAspect : EcsAspect
    {
        public EcsPool<NetworkMessageChannelSource> Source;
        public EcsPool<NetworkIdComponent> NetworkId;
        public EcsPool<NetworkSyncValuesComponent> SyncValues;
        public EcsPool<NetworkTargetComponent> Target;
        public EcsPool<NetworkHistoryComponent> History;
        public EcsPool<NetworkSerializationResult> SerializationResult;
        public EcsPool<NetworkSyncComponent> ServerEntity;
        public EcsPool<NetworkReceiveResultComponent> ReceiveResult;
        public EcsPool<NetworkEventComponent> NetworkEvent;
        
        //received byte array from network
        public EcsPool<NetworkMessageDataComponent> MessageData;
        
        // === requests ===
        
        //request to remove entity from network
        public EcsPool<NetworkTransferRequest> Transfer;
        public EcsPool<NetworkMessageRequest> SendMessage;
        public EcsPool<SerializeNetworkEntityRequest> SerializeEntity;
        public EcsPool<NetworkForceResendRequest> ForceResend;
        
        //events
        public EcsPool<EcsNetworkDataSendEvent> DataSendEvent;
    }
}