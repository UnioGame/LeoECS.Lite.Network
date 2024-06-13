namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Converters
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using Shared.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.IL2CPP.CompilerServices;
    using Unity.Netcode;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// convert GameObject to NetworkClientComponent
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public class NetcodeClientConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target, EcsWorld world, int entity)
        {
            var networkObject = target.GetComponent<NetworkObject>();
            ref var networkObjectComponent = ref world.AddComponent<NetcodeClientObjectComponent>(entity);
            ref var networkClientComponent = ref world.AddComponent<NetworkClientComponent>(entity);
            ref var networkConnectionTypeComponent = ref world.AddComponent<NetworkConnectionTypeComponent>(entity);
            ref var networkClientIdComponent = ref world.AddComponent<NetworkClientIdComponent>(entity);
            
            networkObjectComponent.Value = networkObject;
        }
    }
}