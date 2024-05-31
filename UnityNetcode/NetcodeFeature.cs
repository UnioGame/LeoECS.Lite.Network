namespace Game.Ecs.Network.UnityNetcode
{
    using System;
    using Cysharp.Threading.Tasks;
    using Data;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.ExtendedSystems;
    using NetcodeClients;
    using NetcodeMessages;
    using NetworkCommands.Data;
    using Profiler;
    using Shared.Components.Events;
    using Shared.Components.Requests;
    using Shared.Data;
    using Systems;
    using UniGame.AddressableTools.Runtime;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(menuName = "Game/Feature/Network/Netcode Feature", fileName = "Netcode Feature")]
    public class NetcodeFeature : BaseLeoEcsFeature
    {
        public AssetReferenceT<UnityNetcodeSettingsAsset> netcodeSettings;
        public AssetReferenceT<EcsNetworkSettingsAsset> networkSettings;
        
        public NetworkEcsProfilerFeature profilerFeature = new();
        public NetcodeClientsFeature clientsFeature = new();
        public NetcodeMessagingFeature messagingFeature = new();
        
        public sealed override async UniTask InitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();
            var lifeTime = world.GetWorldLifeTime();

#if ENABLE_ECS_DEBUG
            await profilerFeature.InitializeFeatureAsync(ecsSystems);
#endif
            
            var netcodeData = await netcodeSettings
                .LoadAssetInstanceTaskAsync(lifeTime, true);
            
            var settingsAsset = await networkSettings
                .LoadAssetInstanceTaskAsync(lifeTime, true);
            
            var settings = settingsAsset.networkSettings;
            var networkData = settings.networkData;
            var typesCount = networkData.networkTypes.Length;
            
            var ecsNetworkData = new EcsNetworkData()
            {
                Types = new NetworkSyncType[typesCount],
                IdTypeMap = new Type[typesCount],
                ClientIsTypeMap = new Type[typesCount],
                ClientTypes = new NetworkSyncType[typesCount],
            };
            
            foreach (var syncType in networkData.networkTypes)
            {
                var type = syncType.type;
                var id = syncType.id;
                ecsNetworkData.Types[id] = syncType;
                ecsNetworkData.SyncTypes[id] = BitConverter.GetBytes(id);
                ecsNetworkData.IdTypeMap[id] = type;
                ecsNetworkData.TypesMap[type] = id;
            }

            foreach (var clientType in networkData.clientTypes)
            {
                ecsNetworkData.ClientTypes[clientType.id] = clientType;
                ecsNetworkData.ClientIsTypeMap[clientType.id] = clientType.type;
                ecsNetworkData.ClientTypeMap[clientType.type] = clientType.id;
                ecsNetworkData.ClientIdTypeMap[clientType.id] = clientType.id;
            }
            
            //set global settings of network configuration
            world.SetGlobal(ecsNetworkData);
            world.SetGlobal(netcodeData.settings);
            world.SetGlobal(settings);
            world.SetGlobal(netcodeData);
            world.SetGlobal(networkData);
            
            //if get request to start network and netcode not initialized - start netcode
            ecsSystems.Add(new StartNetcodeOnNonInitializedSystem());
            //link request entity ot network agent
            ecsSystems.Add(new InitializeNetcodeSystem());

            //remove server connected event
            ecsSystems.DelHere<NetworkServerConnectedSelfEvent>();
            //start netcode server and fire server connected if success
            ecsSystems.Add(new StartNetcodeServerSystem());
            //stop netcode server
            ecsSystems.Add(new StopNetcodeSystem());
            
            ecsSystems.Add(new UpdateNetcodeStatusSystem());
            ecsSystems.Add(new UpdateNetcodeTimeSystem());
            
            //additional feature for clients
            await clientsFeature.InitializeFeatureAsync(ecsSystems);
            //register rpc commands
            await messagingFeature.InitializeFeatureAsync(ecsSystems);
   
            
            //remove stop request
            ecsSystems.DelHere<StopNetworkSelfRequest>();
            //ecsSystems.DelHere<StartNetworkSelfRequest>();
        }
    }

}