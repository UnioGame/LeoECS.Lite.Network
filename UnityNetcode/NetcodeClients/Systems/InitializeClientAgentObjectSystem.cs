namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Components;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using UniGame.Core.Runtime;
    using UniGame.Core.Runtime.Extension;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.Runtime.ObjectPool.Extensions;
    using Unity.Netcode;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;
    using UniGame.Core.Runtime.Extension;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class InitializeClientAgentObjectSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkObject _clientAgentPrefab;
        private EcsWorld _world;
        private ILifeTime _lifeTime;
        private NetcodeAspect _netcodeAspect;

        private EcsFilter _netcodeFilter;
        private EcsFilter _clientFilter;
        

        public InitializeClientAgentObjectSystem(NetworkObject clientAgentPrefab)
        {
            _clientAgentPrefab = clientAgentPrefab;
        }
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _lifeTime = _world.GetLifeTime();

            _clientFilter = _world
                .Filter<NetcodeClientObjectComponent>()
                .End();

            _netcodeFilter = _world
                .Filter<NetcodeManagerComponent>()
                .End();

        }

        public void Run(IEcsSystems systems)
        {
            if (_clientFilter.GetEntitiesCount() > 0)
            { 
                return;
            }

            var netcodeEntity = _netcodeFilter.First();
            if (netcodeEntity < 0)
            {
                return;
            }
            
            ref var managerComponent = ref _netcodeAspect.Manager.Get(netcodeEntity);
            var manager = managerComponent.Value;
            
            if(!manager.IsListening || !manager.IsClient)return;

            var clientAgentInstanceObject = _clientAgentPrefab.gameObject
                .Spawn()
                .DespawnWith(_lifeTime);
            
            /*var rpcInstance = clientAgentInstanceObject.GetComponent<NetworkObject>();
            rpcInstance.Spawn();*/
        }
    }
}