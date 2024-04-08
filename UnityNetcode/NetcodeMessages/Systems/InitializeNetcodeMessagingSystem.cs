namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Linq;
    using Aspects;
    using Componenets;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Components.Events;
    using UniGame.Core.Runtime;
    using UniGame.Core.Runtime.Extension;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;
    using UnityEngine.Pool;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.Runtime.ObjectPool;
    using Unity.Netcode;
    using UnityNetcode.Aspects;

    /// <summary>
    /// initiaize netcode messaging system
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class InitializeNetcodeMessagingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkObject _rpcPrefab;
        private NetcodeAspect _netcodeAspect;
        
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsFilter _rpcFilter;
        private ILifeTime _lifeTime;

        public InitializeNetcodeMessagingSystem(NetworkObject rpcPrefab)
        {
            _rpcPrefab = rpcPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _lifeTime = _world.GetLifeTime();
            
            _filter = _world
                .Filter<NetcodeManagerComponent>()
                .Inc<NetworkServerConnectedSelfEvent>()
                .End();

            _rpcFilter = _world
                .Filter<NetcodeMessageChannelComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            if (_rpcFilter.GetEntitiesCount() > 0) return;

            var managerEntity = _filter.First();
            if(managerEntity < 0) return;
            
            ref var managerComponent = ref _netcodeAspect.Manager.Get(managerEntity);
            var manager = managerComponent.Value;
            
            if(!manager.IsListening || !manager.IsServer)return;

            var rpcInstanceObject = _rpcPrefab.gameObject
                .Spawn()
                .DespawnWith(_lifeTime);
            
            var rpcInstance = rpcInstanceObject.GetComponent<NetworkObject>();
            rpcInstance.Spawn(false);
        }
    }
}