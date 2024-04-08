namespace Girand.Ecs.GameSettings.Converters
{
    using System;
    using Leopotam.EcsLite;
    using Server.Components.Requests;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// create new photon agent
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public class CreatePhotonAgentConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target, EcsWorld world, int entity)
        {
            ref var photonAgent = ref world.GetOrAddComponent<CreatePhotonAgentSelfRequest>(entity);
        }
        
    }
}