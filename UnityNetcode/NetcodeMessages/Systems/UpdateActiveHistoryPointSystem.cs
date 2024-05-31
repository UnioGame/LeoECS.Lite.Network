namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
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
    public sealed class UpdateActiveHistoryPointSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _rpcAspect;

        private EcsWorld _world;

        private EcsFilter _netcodeFilter;
        private EcsFilter _historyFilter;
        private int _historyEntity;
        private NetworkData _networkSettings;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _networkSettings = _world.GetGlobal<NetworkData>();

            _netcodeFilter = _world
                .Filter<NetcodeManagerComponent>()
                .Inc<NetworkTimeComponent>()
                .End();

            _historyFilter = _world
                .Filter<NetworkHistoryComponent>()
                .End();
        }

        public void Run(IEcsSystems ecsSystems)
        {
            var netcodeEntity = _netcodeFilter.First();
            if (netcodeEntity < 0) return;

            _historyEntity = _historyFilter.First();
            if (_historyEntity < 0) return;

            ref var historyComponent = ref _rpcAspect.History.Get(_historyEntity);
            ref var timeComponent = ref _netcodeAspect.NetworkTime.Get(netcodeEntity);

            var time = timeComponent.Time;
            var tick = timeComponent.Tick;
            var historyTick = historyComponent.Tick;

            if (tick == historyTick) return;

            var historyLength = historyComponent.History.Length;
            var index = historyComponent.Index;
            var lastIndex = index;
            index = tick % historyLength;

            ref var historyData = ref historyComponent.History[index];

            historyData.EntityMap.Clear();

            historyData.Size = 0;
            historyData.Count = 0;
            historyData.Offset = 0;
            historyData.Tick = tick;
            historyData.Time = time;
            historyData.PreviousTick = historyComponent.Tick;

            historyComponent.Tick = tick;
            historyComponent.Index = index;
            historyComponent.LastIndex = lastIndex;
            historyComponent.Time = time;
        }
    }
}