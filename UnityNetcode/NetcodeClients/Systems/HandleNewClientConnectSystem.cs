namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Leopotam.EcsLite;
    using NetworkCommands.Aspects;
    using Shared.Aspects;
    using Shared.Components.Events;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    
    /// <summary>
    /// handle new client connect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class HandleNewClientConnectSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;

        private NetworkClientAspect _clientAspect;
        private NetworkMessageAspect _networkMessage;
        
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world
                .Filter<NetworkClientConnectedSelfEvent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var resendComponent = ref _networkMessage
                    .ForceResend
                    .GetOrAddComponent(entity);

                ref var clientComponent = ref _clientAspect.ClientId.Get(entity);
                resendComponent.ClientId = (int)clientComponent.Id;
            }
            
        }
    }
}