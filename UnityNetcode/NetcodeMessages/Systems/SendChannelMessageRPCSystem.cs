namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Components;
    using Extensions;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using Shared.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

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
    public class SendChannelMessageRPCSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeMessageAspect _rpcAspect;
        private NetworkMessageAspect _messageAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsFilter _requestFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _filter = _world
                .Filter<NetcodeMessageChannelComponent>()
                .End();

            _requestFilter = _world
                .Filter<NetworkMessageRequest>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var requestEntity in _requestFilter)
            {
                ref var request = ref _messageAspect.SendMessage.Get(requestEntity);
                var channelEntity = _filter.First();
                
                if(channelEntity < 0) continue;
                
                ref var channel = ref _rpcAspect.Channel.Get(channelEntity);
                var channelObject = channel.Value;
                var target = channelObject.GetRpcTarget(request.Target);
                
                channelObject.SendMessageRPC(request.Data,target);
                
                _messageAspect.SendMessage.Del(requestEntity);
            }
        }
    }
}