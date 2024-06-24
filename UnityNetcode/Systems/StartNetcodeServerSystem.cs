namespace Game.Ecs.Network.UnityNetcode.Systems
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
    public class StartNetcodeServerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsFilter _netFilter;
        
        private bool _isLoading;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<StartNetworkSelfRequest>()
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
                ref var request = ref _networkAspect.StartNetwork.Get(entity);
                
                var address = request.Address;
                var port = request.Port;

                var netcodeEntity = _netFilter.First();
                if (netcodeEntity < 0)
                {
                    continue;
                }
      
                ref var managerComponent = ref _netcodeAspect.Manager.Get(netcodeEntity);
                ref var transportComponent = ref _netcodeAspect.Transport.Get(netcodeEntity);

                var manager = managerComponent.Value;
                var transport = transportComponent.Value;
                
                if(manager.IsServer || manager.IsHost) continue;
                    
                transport.ConnectionData.Address = address;
                transport.ConnectionData.Port = (ushort)port;
                
                //start server
                manager.OnClientConnectedCallback += ClientConnected_Callback;
                var connected = request.AllowServerClient 
                    ? manager.StartHost() 
                    : manager.StartServer();
                    
                if(!connected)
                {
                    GameLog.LogError($"Failed to start host for address: {address} | port: {port}");
                    continue;
                }

                var mode = request.AllowServerClient ? "host" : "server";
                GameLog.Log($"Successfully started {mode} for address: {address} | port: {port}");
                
                ref var connectedEvent = ref _networkAspect.ServerConnected.Add(netcodeEntity);
                
                var packedNetEntity = _world.PackEntity(netcodeEntity);
                ref var linkComponent = ref _networkAspect.NetworkLink.GetOrAddComponent(entity);
                linkComponent.Value = packedNetEntity;
                
                _networkAspect.StartNetwork.Del(entity);
            }
        }

        private void ClientConnected_Callback(ulong id)
        {
            GameLog.Log($"Client connected with | id: {id}");
        }

    }
}