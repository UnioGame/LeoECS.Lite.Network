namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine.Serialization;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkHistoryData
    {
        public int Tick;
        public float Time;
        public int PreviousTick;
        public int Size;
        public int Count;
        public int Offset;
        
        public NativeArray<byte> SerializedData;
        public NativeHashMap<int,EcsEntityNetworkData> EntityMap;
    }
}