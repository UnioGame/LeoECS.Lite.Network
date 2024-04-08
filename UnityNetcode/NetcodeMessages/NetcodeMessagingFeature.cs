namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages
{
    using System;
    using Components;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.ExtendedSystems;
    using NetworkCommands;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Events;
    using NetworkCommands.Components.Requests;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using Unity.Netcode;

    [Serializable]
    public class NetcodeMessagingFeature : LeoEcsFeature
    {
#region inspector

        public NetworkObject networkObject;

        public NetworkCommandsFeature networkCommandsFeature = new();
        
#endregion
        
        protected sealed override async UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            await networkCommandsFeature.InitializeFeatureAsync(ecsSystems);
            
            ecsSystems.Add(new InitializeNetcodeMessagingSystem(networkObject));
            ecsSystems.Add(new SendChannelMessageRPCSystem());
            
            //validate network events and add it to serialization queue
            ecsSystems.Add(new ValidateNetworkEventSystem());
            
            //validate is ecs data should be serialized
            //if conditions is true, when add request to serialize data
            ecsSystems.Add(new ValidateTickNetworkSerializationSystem());
            //is value of entities must be fully send to target
            ecsSystems.Add(new ValidateForceResendNetworkSystem());
            
            //serialize target entities to history data with task support
            ecsSystems.Add(new SerializeNetworkDataToHistoryTaskSystem());
            
            //serialize data to send for server side
            ecsSystems.Add(new SerializeNetworkDataSystem());
            //compress serialized data before send if option enabled
            ecsSystems.Add(new CompressNetworkDataSystem());

            ecsSystems.DelHere<EcsNetworkDataSendEvent>();
            //receive ready to send data to target and fire data send event
            ecsSystems.Add(new SendNetworkDataSystem());
                        
            //when server tick changed = update history point
            ecsSystems.Add(new UpdateActiveHistoryPointSystem());

            //destroy entities marked as one shot
            ecsSystems.Add(new RemoveNetworkRequestsSystem());
            
            //receive network requests and process it
            ecsSystems.Add(new ReceiveNetworkDataSystem());
            
            //process incoming data on client side
            ecsSystems.Add(new ProcessClientNetcodeDataSystem());
            
            //process incoming network data results
            ecsSystems.Add(new ProcessNetcodeServerDataSystem());

#if UNITY_EDITOR || GAME_DEBUG
            //debug statistics
            ecsSystems.Add(new NetcodeDebugSendStatisticsSystem());
            ecsSystems.Add(new NetcodeDebugStatisticsSystem());
#endif
            
            //remove incoming rpc data entity after processing
            ecsSystems.Add(new RemoveNetworkReceivedDataSystem());
            
            //remove network sync entity by network id
            ecsSystems.Add(new RemoveClientMissingSyncValuesSystem());

            ecsSystems.DelHere<SerializeNetworkEntityRequest>();
            ecsSystems.DelHere<NetworkEventComponent>();
            ecsSystems.DelHere<NetworkSerializationResult>();
            ecsSystems.DelHere<NetworkTransferRequest>();
            ecsSystems.DelHere<NetworkSerializeRequest>();
        }
    }

}