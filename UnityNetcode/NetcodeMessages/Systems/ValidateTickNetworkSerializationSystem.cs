namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Componenets;
    using Components;
    using Leopotam.EcsLite;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
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
    public class ValidateTickNetworkSerializationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _messageAspect;
        
        private EcsWorld _world;
        
        private EcsFilter _filter;
        private EcsFilter _netcodeFilter;
        private EcsFilter _historyFilter;
        private EcsFilter _networkValueFilter;

        private EcsNetworkSettings _networkSettings;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkSettings = _world.GetGlobal<EcsNetworkSettings>();
            
            _filter = _world
                .Filter<NetcodeMessageChannelComponent>()
                .End();

            _netcodeFilter = _world
                .Filter<NetcodeManagerComponent>()
                .Inc<NetworkConnectionTypeComponent>()
                .Inc<NetworkTimeComponent>()
                .End();

            _historyFilter = _world
                .Filter<NetworkHistoryComponent>()
                .End();

            _networkValueFilter = _world
                .Filter<NetworkIdComponent>()
                .Exc<NetworkSyncComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var netcodeEntity = _netcodeFilter.First();
            if (netcodeEntity < 0) return;
            
            var rpcEntity = _filter.First();
            if (rpcEntity < 0) return;
            
            var historyEntity = _historyFilter.First();
            if(historyEntity < 0) return;
            
            ref var connectionType = ref _netcodeAspect.ConnectionType.Get(netcodeEntity);
            ref var historyComponent = ref _messageAspect.History.Get(historyEntity);
            ref var timeComponent = ref _netcodeAspect.NetworkTime.Get(netcodeEntity);
            
            var time = timeComponent.Time;
            var tick = timeComponent.Tick;
            var historyTick = historyComponent.Tick;

            var useTickFilter = _networkSettings.useTickSendingOnClient || connectionType.IsServer;
            if(useTickFilter && tick == historyTick) return;
                      
            var serializeEntity = _world.NewEntity();
            //todo allow target to be a server from client side
            ref var transferRequest = ref _messageAspect.Transfer.Add(serializeEntity);
            ref var serializeComponent = ref _messageAspect.SerializeEntity.Add(serializeEntity);
            
            transferRequest.Tick = tick;
            transferRequest.Time = time;
            transferRequest.TargetId = 0;
            
            //todo allow custom target override for server side
            transferRequest.Target = connectionType.IsServer 
                ? _networkSettings.defaultServerTarget
                : _networkSettings.defaultClientTarget;
        }
    }
}