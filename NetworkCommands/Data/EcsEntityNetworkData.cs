namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;
    using Leopotam.EcsLite;
    using Unity.Collections;
    using UnityEngine.Serialization;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [GenerateTestsForBurstCompatibility]
    public struct EcsEntityNetworkData
    {
        public int Id;
        public int Hash;
        public bool IsValueChanged;
        public int Count;
        public int Offset;
        public int Size;
    }
}