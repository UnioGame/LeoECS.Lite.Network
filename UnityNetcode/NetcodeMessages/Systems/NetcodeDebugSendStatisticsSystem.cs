namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Text;
    using Aspects;
    using Componenets;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityNetcode.Aspects;

    /// <summary>
    /// send message with base rpc channel
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class NetcodeDebugSendStatisticsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        
        private NetworkSyncAspect _networkSyncAspect;
        private NetcodeMessageAspect _messageAspect;
        private NetworkMessageAspect _networkMessageAspect;
        
        
        private EcsWorld _world;
        
        private EcsFilter _networkFilter;
        private EcsFilter _transferFilter;
        
        private EcsNetworkSettings _networkData;
        private StringBuilder _stringBuilder = new StringBuilder(512);
        

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _networkData = _world.GetGlobal<EcsNetworkSettings>();
            
            _networkFilter = _world.Filter<NetcodeManagerComponent>()
                .Inc<NetworkConnectionTypeComponent>()
                .End();
            
            _transferFilter = _world
                .Filter<NetworkTransferRequest>()
                .Inc<NetworkSerializationResult>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            if (!_networkData.enableDebug) return;
            
            var networkEntity = _networkFilter.First();
            if (networkEntity < 0) return;
            
            ref var connection = ref _networkAspect.ConnectionType.Get(networkEntity);
            ref var managerComponent = ref _netcodeAspect.Manager.Get(networkEntity);
            ref var networkTime = ref _netcodeAspect.NetworkTime.Get(networkEntity);
            
            if(!connection.IsActive)return;

            _stringBuilder.Clear();
            
            foreach (var entity in _transferFilter)
            {
                ref var serializationComponent = ref _networkMessageAspect.SerializationResult.Get(entity);
                
                var kb = serializationComponent.Value.Length / 1024f;
                var mb = kb / 1024f;
                
                _stringBuilder.AppendLine($"SEND TICK: {networkTime.Tick} NET TIME: {networkTime.Time} | {kb} KB | {mb} MB");
            }
            
            GameLog.LogRuntime(_stringBuilder.ToString(),Color.blue);
        }
    }
}