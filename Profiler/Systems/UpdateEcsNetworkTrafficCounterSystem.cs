using System;
using Game.Ecs.Network.NetworkCommands.Aspects;
using Game.Ecs.Network.NetworkCommands.Components.Events;
using Game.Ecs.Network.Shared.Profiler;
using Leopotam.EcsLite;
using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

namespace Game.Ecs.Network.Profiler.Systems
{
    /// <summary>
    /// update profiler counter
    /// </summary>
#if ENABLE_IL2CPP
    using System;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components.Events;
    using Shared.Profiler;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateEcsNetworkTrafficCounterSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _sendFilter;

        private NetworkMessageAspect _messageAspect;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _sendFilter = _world
                .Filter<EcsNetworkDataSendEvent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _sendFilter)
            {
                ref var dataEvent = ref _messageAspect.DataSendEvent.Get(entity);

                // EcsNetworkTrafficCounter.EcsSendTrafficCounter.Value += dataEvent.Size;
                // EcsNetworkTrafficCounter.EcsSendTraffic.Sample(dataEvent.Size);
            }
        }
    }
}