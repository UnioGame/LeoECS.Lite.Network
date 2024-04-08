namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Componenets;
    using Components;
    using Leopotam.EcsLite;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using NetworkCommands.Data;
    using NetworkCommands.Systems;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
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
    public class ValidateNetworkEventSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeAspect _netcodeAspect;
        private NetcodeMessageAspect _messageAspect;
        
        private EcsWorld _world;
        
        private EcsFilter _networkValueFilter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _networkValueFilter = _world
                .Filter<NetworkIdComponent>()
                .Inc<NetworkEventComponent>()
                .Exc<NetworkSyncComponent>()
                .Exc<SerializeNetworkEntityRequest>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var valueEntity in _networkValueFilter)
            {
                _messageAspect.SerializeEntity.Add(valueEntity);
            }
        }
    }
}