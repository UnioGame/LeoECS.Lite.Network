namespace Game.Ecs.Network.NetworkCommands.Systems
{
    using System;
    using System.Runtime.CompilerServices;
    using Aspects;
    using Data;
    using Extensions;
    using Leopotam.EcsLite;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.Core.Runtime;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
    
    /// <summary>
    /// network commands tools
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class NetworkMessageToolsSystem : IEcsInitSystem,IEcsDestroySystem,IEcsRunSystem
    {
        private static int _idCounter = 0;
        private static int _clientIdCounter = -1;
        
        public NetworkMessageAspect messageAspect;
        public NetworkAspect networkAspect;
        
        public EcsWorld world;
        public EcsNetworkSettings networkSettings;
        public EcsNetworkData networkData;
        public NetworkHistoryData[] historyData;
        public ILifeTime lifeTime;
        public int historySize;
        public bool isClient;
        public bool isServer;
        public int _maxNetworkEntities;
        public int _entityChunkSize;
        
        private EcsFilter _connectionFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            networkSettings = world.GetGlobal<EcsNetworkSettings>();
            networkData = world.GetGlobal<EcsNetworkData>();
            
            historySize = networkSettings.tickHistorySize;
            lifeTime = world.GetWorldLifeTime();
            historyData = new NetworkHistoryData[historySize];
            var netData = networkSettings.networkData;

            _maxNetworkEntities = netData.defaultBufferSize;
            _entityChunkSize = netData.serializationEntityChunkSize;
            
            var bufferSize = _maxNetworkEntities * _entityChunkSize;

            for (var i = 0; i < historySize; i++)
            {
                historyData[i] = new NetworkHistoryData
                {
                    Tick = 0,
                    Time = 0,
                    Size = 0,
                    PreviousTick = 0,
                    SerializedData = new NativeArray<byte>(bufferSize, Allocator.Persistent),
                    EntityMap = new NativeHashMap<int,EcsEntityNetworkData>(netData.defaultBufferSize, Allocator.Persistent),
                };
            }
            
            var historyEntity = world.NewEntity();
            ref var historyComponent = ref messageAspect.History.Add(historyEntity);
            historyComponent.History = historyData;
            historyComponent.Index = 0;
            historyComponent.LastIndex = 0;
            historyComponent.Tick = 0;
            historyComponent.Time = 0;

            isServer = false;
            isClient = false;
            
            _connectionFilter = world
                .Filter<NetworkConnectionTypeComponent>()
                .End();
        }
        
        public void Run(IEcsSystems systems)
        {
            var entity = _connectionFilter.First();
            if(entity < 0) return;
            
            ref var connection = ref networkAspect.ConnectionType.Get(entity);
            isClient = connection.IsClient;
            isServer = connection.IsServer;
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TComponent AddNetworkRequest<TComponent>(int entity, NetworkMessageTarget target)
            where TComponent : struct
        {
            return ref world.AddNetworkComponent<TComponent>(entity, target);
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (var value in historyData)
            {
                var stream = value.SerializedData;
                var data = value.EntityMap;

                stream.Dispose();
                data.Dispose();
            }
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TComponent AddNetworkEvent<TComponent>(int entity)
            where TComponent : struct
        {
            return ref AddNetworkComponent<TComponent>(entity,true);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TComponent AddNetworkEvent<TComponent>(int entity, NetworkMessageTarget target)
            where TComponent : struct
        {
            return ref AddNetworkComponent<TComponent>(entity, target,true);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TComponent AddNetworkComponent<TComponent>(int entity,bool markAsEvent = false)
            where TComponent : struct
        {
            var target = isServer ? NetworkMessageTarget.NotServer : NetworkMessageTarget.Server;
            return ref AddNetworkComponent<TComponent>(entity, target,markAsEvent);
        }
        
#if ENABLE_IL2CPP
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TComponent AddNetworkComponent<TComponent>(int entity, NetworkMessageTarget target,bool markAsEvent = false)
            where TComponent : struct
        {
#if UNITY_EDITOR || GAME_DEBUG
            var typeMap = isServer ? networkData.TypesMap: networkData.ClientTypeMap;
            var targetType = typeof(TComponent);
            if (!typeMap.TryGetValue(targetType, out var typeId))
            {
                GameLog.LogError($"AddNetworkComponent: Type {targetType.Name} not found in network type map IS_SERVER:{isServer}");
                return ref world.AddComponent<TComponent>(entity);
            }
#endif 
            if (!messageAspect.NetworkId.Has(entity))
            {
                ref var networkId = ref messageAspect.NetworkId.Add(entity);
                networkId.Id =  isServer ? _idCounter++ : _clientIdCounter--;
            }
            
            //ref var targetComponent = ref messageAspect.Target.GetOrAddComponent(entity);
            if (markAsEvent)
            {
                ref var eventComponent = ref messageAspect.NetworkEvent.GetOrAddComponent(entity);
            }
            
            //targetComponent.Value = target;
            ref var component = ref world.AddComponent<TComponent>(entity);
            return ref component;
        }
        
    }
}