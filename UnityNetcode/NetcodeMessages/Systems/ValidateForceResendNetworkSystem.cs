namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Componenets;
    using Components;
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
    public class ValidateForceResendNetworkSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkMessageAspect _messageAspect;
        
        private EcsWorld _world;
        
        private EcsFilter _transferRequestFilter;
        private EcsFilter _forceResendFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _transferRequestFilter = _world
                .Filter<SerializeNetworkEntityRequest>()
                .End();
            
            _forceResendFilter = _world
                .Filter<NetworkForceResendRequest>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            if(_transferRequestFilter.GetEntitiesCount() <= 0) return;
            
            var forceResend = false;
            
            foreach (var resendEntity in _forceResendFilter)
            {
                forceResend = true;
                _messageAspect.ForceResend.Del(resendEntity);
            }
            
            if(!forceResend) return;
            
            foreach (var entity in _transferRequestFilter)
            {
                ref var transferRequest = ref _messageAspect.SerializeEntity.Get(entity);
                transferRequest.Force = true;
            }
        }
    }
}