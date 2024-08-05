namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class MessageCleanerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _sentMessagesFilter;
        private EcsFilter _receivedMessagesFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _receivedMessagesFilter = _world
                .Filter<ReceivedMessageComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var receivedMessageEntity in _receivedMessagesFilter)
            {
                _world.DelEntity(receivedMessageEntity);
            }
        }
    }
}