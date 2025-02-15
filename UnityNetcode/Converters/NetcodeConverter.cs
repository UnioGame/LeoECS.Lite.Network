﻿namespace Game.Ecs.Network.UnityNetcode.Converters
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.LeoEcsLite.LeoEcs.Shared.Components;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using Unity.Collections;
    using Unity.IL2CPP.CompilerServices;
    using Unity.Netcode;
    using Unity.Netcode.Transports.UTP;
    using UnityEngine;

    /// <summary>
    /// converter for netcode prefab to ecs world
    /// </summary>
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetcodeConverter : GameObjectConverter
    {
        public NetworkManager networkManager;
        public UnityTransport unityTransport;
        
        protected override void OnApply(GameObject target, EcsWorld world, int entity)
        {
            ref var networkTime = ref world.GetOrAddComponent<NetworkTimeComponent>(entity);
            ref var networkManagerComponent = ref world.GetOrAddComponent<NetcodeManagerComponent>(entity);
            ref var netcodeAgentComponent = ref world.GetOrAddComponent<NetcodeAgentComponent>(entity);
            ref var netcodeStatusComponent = ref world.GetOrAddComponent<NetcodeStatusComponent>(entity);
            ref var unityTransportComponent = ref world.GetOrAddComponent<UnityTransportComponent>(entity);
            ref var targetComponent = ref world.GetOrAddComponent<NetcodeSharedRPCComponent>(entity);
            ref var networkSourceComponent = ref world.GetOrAddComponent<NetworkSourceComponent>(entity);
            ref var connectionTypeComponent = ref world.GetOrAddComponent<NetworkConnectionTypeComponent>(entity);
            ref var syncValuesComponent = ref world.GetOrAddComponent<NetworkSyncValuesComponent>(entity);
            ref var lifeTimeComponent = ref world.GetOrAddComponent<LifeTimeComponent>(entity);
            
            unityTransportComponent.Value = unityTransport;
            networkManagerComponent.Value = networkManager;
            targetComponent.Value = target;

            netcodeStatusComponent.Status = ConnectionStatus.Disconnected;
            netcodeStatusComponent.IsConnected = false;
            netcodeStatusComponent.IsInRoom = false;
            
            networkTime.Time = networkManager.ServerTime.TimeAsFloat;
            networkTime.Tick = networkManager.ServerTime.Tick;

            var lifeTime = target.GetAssetLifeTime();
            syncValuesComponent.Values = 
                new NativeHashMap<int, EcsPackedEntity>(128, Allocator.Persistent)
                    .AddTo(lifeTime);
        }

        
    }
}