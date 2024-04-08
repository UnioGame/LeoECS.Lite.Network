namespace Game.Ecs.Network.Shared.UnityComponents
{
    using System;
    using MemoryPack;
    using NetworkCommands.Data;
    using Unity.Mathematics;
    /// <summary>
    /// position of object
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [MemoryPackable]
    public partial struct NetworkTransformComponent : IEcsNetworkValue
    {
        public float3 Position;  
        public quaternion Rotation;
    }
}