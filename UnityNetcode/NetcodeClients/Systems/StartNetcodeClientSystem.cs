namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Aspects;
    using Componenets.Requests;
    using Leopotam.EcsLite;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Netcode;
    using UnityEngine;
    using UnityNetcode.Aspects;

    /// <summary>
    /// initialize netcode data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class StartNetcodeClientSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetworkClientAspect _clientAspect;
        private NetcodeClientAspect _netcodeClientAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsFilter _netFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<StartNetworkClientSelfRequest>()
                .Exc<InitializeNetcodeSelfRequest>()
                .End();

            _netFilter = _world
                .Filter<NetworkSourceComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var request = ref _clientAspect.Connect.Get(entity);
                
                var address = request.Address;
                var port = request.Port;

                var netcodeEntity = _netFilter.First();
                if (netcodeEntity < 0)
                {
                    ref var initializeComponent = ref _netcodeAspect
                        .InitializeSelf.GetOrAddComponent(entity);
                    continue;
                }
                
                ref var managerComponent = ref _netcodeAspect.Manager.Get(netcodeEntity);
                ref var transportComponent = ref _netcodeAspect.Transport.Get(netcodeEntity);

                var manager = managerComponent.Value;
                var transport = transportComponent.Value;

                if (manager.IsServer || manager.IsClient)
                {
                    continue;
                }

                transport.ConnectionData.Address = address;
                transport.ConnectionData.Port = (ushort)port;

                //start server
                var result = manager.StartClient();
                manager.OnTransportFailure += TransportFailed_Callback;
                manager.OnConnectionEvent += ConnectionEvent_Callback;
                if (!result)
                {
                    GameLog.LogError($"Failed to start client for address: {address} | port: {port}");
                    continue;
                }
                
                GameLog.Log($"Successfully started client for address: {address} | port: {port}");
                
                var packedNetEntity = _world.PackEntity(netcodeEntity);
                ref var linkComponent = ref _networkAspect.NetworkLink.GetOrAddComponent(entity);
                linkComponent.Value = packedNetEntity;
                
                _clientAspect.Connect.Del(entity);
            }
        }

        private void ConnectionEvent_Callback(NetworkManager manager, ConnectionEventData eventData)
        {
            Debug.Log($"Connection event: Type: {eventData.EventType} | client: {eventData.ClientId}.");
        }

        private void TransportFailed_Callback()
        {
            GameLog.Log("Transport failed.");
        }
    }
}