namespace Game.Ecs.Network.UnityNetcode.NetcodeClients
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.ExtendedSystems;
    using Shared.Components.Events;
    using Systems;
    using UniGame.AddressableTools.Runtime;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Converter.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    [Serializable]
    public class NetcodeClientsFeature : LeoEcsFeature
    {
        [SerializeField]
        private AssetReferenceT<LeoEcsMonoConverter> _clientConverter;
        
        protected sealed override async UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            var lifetime = ecsSystems.GetLifeTime();
            var converterPrefab = await _clientConverter.LoadAssetTaskAsync(lifetime);
            var converterInstance = Object.Instantiate(converterPrefab);
            
            ecsSystems.DelHere<NetworkClientConnectedSelfEvent>();
            ecsSystems.DelHere<NetworkClientDisconnectedEvent>();
            
            //start client by request and connect to server
            ecsSystems.Add(new StartNetcodeClientSystem());
            
            //update client info
            ecsSystems.Add(new UpdateNetcodeClientsSystem());
            
            //handle new client connect and ask to resend all data
            ecsSystems.Add(new HandleNewClientConnectSystem());
        }
    }

}