namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Text;
    using Aspects;
    using Componenets;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
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
    public class NetcodeDebugStatisticsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetworkSyncAspect _networkSyncAspect;
        private NetcodeMessageAspect _messageAspect;
        private NetworkMessageAspect _networkMessageAspect;
        
        private EcsWorld _world;
        private EcsFilter _receiveFilter;
        private EcsFilter _networkFilter;
        private EcsFilter _messageFilter;
        
        private EcsNetworkSettings _networkData;
        private StringBuilder _stringBuilder = new StringBuilder(512);
        

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _networkData = _world.GetGlobal<EcsNetworkSettings>();
            
            _networkFilter = _world.Filter<NetcodeManagerComponent>()
                .Inc<NetworkConnectionTypeComponent>()
                .End();
            
            _receiveFilter = _world
                .Filter<NetworkReceiveResultComponent>()
                .End();
            
            _receiveFilter = _world
                .Filter<NetworkReceiveResultComponent>()
                .End();
            
            _messageFilter = _world
                .Filter<NetworkMessageDataComponent>()
                .End();
            
        }

        public void Run(IEcsSystems systems)
        {
            if (!_networkData.enableDebug) return;
            
            var networkEntity = _networkFilter.First();
            if (networkEntity < 0) return;
            
            ref var connection = ref _networkAspect.ConnectionType.Get(networkEntity);
            ref var managerComponent = ref _netcodeAspect.Manager.Get(networkEntity);
            ref var timeComponent = ref _netcodeAspect.NetworkTime.Get(networkEntity);
            
            if(!connection.IsActive)return;
            
            if(_receiveFilter.GetEntitiesCount() == 0 && _messageFilter.GetEntitiesCount() == 0) return;
            
            _stringBuilder.Clear();
            
            var tick = timeComponent.Tick;
            var time = timeComponent.Time;

            _stringBuilder.AppendLine($"RECEIVE DATA: TICK: {tick} NET TIME: {time}");
    
            
            foreach (var entity in _messageFilter)
            {
                ref var messageDataComponent = ref _messageAspect.MessageData.Get(entity);
                
                var bytes = messageDataComponent.Size;
                var kb = messageDataComponent.Size / 1024f;
                var mb = kb / 1024f;
                
                _stringBuilder.AppendLine($"RECEIVE: {bytes} bytes | {kb} KB | {mb} MB");
            }
            
            foreach (var entity in _receiveFilter)
            {
                ref var receiveResult = ref _networkMessageAspect.ReceiveResult.Get(entity);
                
                var bytes = receiveResult.Size;
                var kb = receiveResult.Size / 1024f;
                var mb = kb / 1024f;
                
                _stringBuilder.AppendLine($"RECEIVE UNPACKED: {bytes} bytes | {kb} KB | {mb} MB | SYNC_COUNT: {receiveResult.Count}");
            }
            
            GameLog.LogRuntime(_stringBuilder.ToString(),Color.yellow);
        }
    }
}