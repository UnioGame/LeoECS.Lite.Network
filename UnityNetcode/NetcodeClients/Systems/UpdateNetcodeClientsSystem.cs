namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;

    /// <summary>
    /// update netcode clients list
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateNetcodeClientsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetcodeAspect _netcodeAspect;
        private NetcodeClientAspect _clientAspect;
        private NetworkClientAspect _networkClientAspect;
        
        private EcsWorld _world;
        private EcsFilter _managerFilter;
        
        private NativeHashMap<ulong,EcsPackedEntity> _clients;
        private NativeList<ulong> _removedIds;
        private EcsFilter _newClients;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            var lifeTime = _world.GetWorldLifeTime();
            
            _clients = new NativeHashMap<ulong, EcsPackedEntity>(100, Allocator.Persistent)
                .AddTo(lifeTime);
            _removedIds = new NativeList<ulong>(8,Allocator.Persistent).AddTo(lifeTime);
            
            _managerFilter = _world
                .Filter<NetcodeManagerComponent>()
                .End();

            _newClients = _world
                .Filter<NetworkClientComponent>()
                .Exc<NetworkLinkComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var managerEntity = _managerFilter.First();
            if(managerEntity <= 0) return;
            
            ref var managerComponent = ref _netcodeAspect.Manager.Get(managerEntity);
            ref var connectionTypeComponent = ref _netcodeAspect.ConnectionType.Get(managerEntity);
            var manager = managerComponent.Value;
            
            //add new clients and fire event
            foreach (var newClientEntity in _newClients)
            {
                ref var gameObjectComponent = ref _clientAspect.GameObject.Get(newClientEntity);
                ref var objectComponent = ref _clientAspect.ClientObject.Get(newClientEntity);
                ref var idComponent = ref _networkClientAspect.ClientId.Get(newClientEntity);
                ref var connectionComponent = ref _networkClientAspect.Connection.Get(newClientEntity);
                
                ref var linkComponent = ref _networkClientAspect.NetworkLink.Add(newClientEntity);
                ref var connectedSelfEvent = ref _networkClientAspect.Connected.Add(newClientEntity);
                
                linkComponent.Value = _world.PackEntity(managerEntity);
                var networkClient = objectComponent.Value;

                connectionComponent.IsActive = connectionTypeComponent.IsActive;
                connectionComponent.IsClient = connectionTypeComponent.IsClient;
                connectionComponent.IsServer = connectionTypeComponent.IsServer;
                
                if(networkClient == null) continue;

                var id = networkClient.OwnerClientId;
                idComponent.Id = id;
                
                _clients[id] = _world.PackEntity(newClientEntity);

                if (networkClient.IsLocalPlayer)
                    _networkClientAspect.Local.Add(newClientEntity);
            }

            _removedIds.Clear();
            
            //is client disconnected
            foreach (var client in _clients)
            {
                var clientValue = client.Value;
                var clientId = client.Key;

                if (clientValue.Unpack(_world, out var clientEntity)) continue;

                _removedIds.Add(clientId);
                    
                //fire disconnect event
                var eventEntity = _world.NewEntity();
                ref var clientDisconnectEvent = ref _networkClientAspect.Disconnected.Add(eventEntity);
                clientDisconnectEvent.ClientId = clientId;
            }

            foreach (var id in _removedIds)
                _clients.Remove(id);
        }
    }
}