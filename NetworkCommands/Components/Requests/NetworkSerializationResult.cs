namespace Game.Ecs.Network.NetworkCommands.Components.Requests
{
    using System;
    using System.Buffers;
    using Data;
    using Leopotam.EcsLite;
    using Unity.Collections;

    /// <summary>
    /// public mark entity as ready to transfer with netcode
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkSerializationResult : IEcsAutoReset<NetworkSerializationResult>
    {
        public NativeArray<byte> Value;
        public int Size;
        
        public void AutoReset(ref NetworkSerializationResult c)
        {
            c.Size = 0;
        }
    }
}