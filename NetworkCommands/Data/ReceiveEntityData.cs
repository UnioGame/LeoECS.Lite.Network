namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;
    using Unity.Collections;
    using UnityEngine.Serialization;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ReceiveEntityData
    {
        public int Id;
        public int Count;
        public bool IsValueChanged;
        [FormerlySerializedAs("ComponentIndex")]
        public int ComponentIndexAt;
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct ReceiveComponentData
    {
        public int TypeId;
        public int Size;
        [ReadOnly]
        public NativeSlice<byte> Component;
    }
}