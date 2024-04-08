namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Components
{
    using System;
    using Leopotam.EcsLite;
    using NetworkCommands.Data;
    using Unity.Collections;

    /// <summary>
    /// request to serialize data for network
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkSerializeRequest
    {
    }
}