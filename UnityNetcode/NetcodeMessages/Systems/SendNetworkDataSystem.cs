namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Buffers;
    using Aspects;
    using Components;
    using Extensions;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;

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
    public class SendNetworkDataSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkMessageAspect _networkMessageAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _messageAspect;
        
        private EcsFilter _netcodeFilter;
        private EcsFilter _transferFilter;
        private EcsFilter _filter;
        private EcsFilter _dataFilter;
        private EcsFilter _historyFilter;
        
        private EcsWorld _world;
        private EcsNetworkSettings _networkSettings;
        private object[] _components;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkSettings = _world.GetGlobal<EcsNetworkSettings>();
            
            _filter = _world
                .Filter<NetcodeMessageChannelComponent>()
                .End();

            _netcodeFilter = _world
                .Filter<NetcodeManagerComponent>()
                .Inc<NetworkTimeComponent>()
                .End();
            
            _historyFilter = _world
                .Filter<NetworkHistoryComponent>()
                .End();

            _transferFilter = _world
                .Filter<NetworkTransferRequest>()
                .Inc<NetworkSerializationResult>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            var transferEntity = _transferFilter.First();
            if (transferEntity < 0) return;
            
            ref var transferComponent = ref _messageAspect.Transfer.Get(transferEntity);
            ref var seializationResult = ref _messageAspect.SerializationResult.Get(transferEntity);
            
            var netcodeEntity = _netcodeFilter.First();
            if (netcodeEntity < 0) return;
            
            var rpcEntity = _filter.First();
            if (rpcEntity < 0) return;
            
            var historyEntity = _historyFilter.First();
            if (historyEntity < 0) return;
            
            ref var channel = ref _messageAspect.Channel.Get(rpcEntity);
            var channelObject = channel.Value;

            ref var resultArray = ref seializationResult.Value;
            var targetArray = ArrayPool<byte>.Shared.Rent(seializationResult.Size);
            var size = seializationResult.Size;
            
            resultArray.AsSpan()
                .Slice(0,size)
                .CopyTo(targetArray);
            
            var target = channelObject.GetRpcTarget(transferComponent.Target);
            channelObject.SendRPC(targetArray,size,target);
            
            ArrayPool<byte>.Shared.Return(targetArray);
            
            var sendEvent = _world.NewEntity();
            ref var sendComponent = ref _networkMessageAspect.DataSendEvent.Add(sendEvent);
            sendComponent.Size = size;
            sendComponent.TargetType = transferComponent.Target;
        }
    }
}