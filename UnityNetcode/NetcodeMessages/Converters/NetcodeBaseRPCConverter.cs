namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Converters
{
    using System;
    using Components;
    using Data;
    using Leopotam.EcsLite;
    using NetworkCommands.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// register netcode base rpc
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public class NetcodeBaseRPCConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target, EcsWorld world, int entity)
        {
            var messageObject = target.GetComponent<NetcodeRPCChannelObject>();
            messageObject ??= target.AddComponent<NetcodeRPCChannelObject>();
            
            ref var rpcChannel = ref world.GetOrAddComponent<NetworkMessageChannelSource>(entity);
            ref var netcodeRpcSource = ref world.GetOrAddComponent<NetcodeMessageChannelComponent>(entity);
            
            netcodeRpcSource.Value = messageObject;
        }
    }
}