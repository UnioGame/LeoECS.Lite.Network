namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Buffers;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.Runtime.ObjectPool.Extensions;

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
    public class RemoveNetworkReceivedDataSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeMessageAspect _messageAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsFilter _requestFilter;
        private EcsFilter _dataFilter;
        private EcsFilter _messageFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<NetworkMessageDataComponent>()
                .End();
            
            _messageFilter = _world
                .Filter<NetworkReceiveResultComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
                _world.DelEntity(entity);
            
            foreach (var entity in _messageFilter)
                _world.DelEntity(entity);
        }
    }
}