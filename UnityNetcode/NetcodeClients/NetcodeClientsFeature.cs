namespace Game.Ecs.Network.UnityNetcode.NetcodeClients
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.ExtendedSystems;
    using Shared.Components.Events;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using Unity.Netcode;
    using UnityEngine;

    [Serializable]
    public class NetcodeClientsFeature : LeoEcsFeature
    {
        [SerializeField]
        private NetworkObject _clientAgentPrefab;
        
        protected sealed override UniTask OnInitializeFeatureAsync(IEcsSystems ecsSystems)
        {
            ecsSystems.Add(new InitializeClientAgentObjectSystem(_clientAgentPrefab));
            
            ecsSystems.DelHere<NetworkClientConnectedSelfEvent>();
            ecsSystems.DelHere<NetworkClientDisconnectedEvent>();
            
            //start client by request and connect to server
            ecsSystems.Add(new StartNetcodeClientSystem());
            
            //update client info
            ecsSystems.Add(new UpdateNetcodeClientsSystem());
            
            //handle new client connect and ask to resend all data
            ecsSystems.Add(new HandleNewClientConnectSystem());
            
            return UniTask.CompletedTask;
        }
    }

}