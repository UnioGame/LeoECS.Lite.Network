namespace Game.Ecs.Network.NetworkCommands.Components
{
    using System;
    using Data;
    using Unity.Collections;
    /// <summary>
    /// result of network incoming data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkReceiveResultComponent
    {
        public int Tick;
        public float Time;
        public int Count;
        public ulong Sender;
        public long Size;
        public NativeArray<byte> RawData;
        public NativeArray<ReceiveEntityData> Data;
        public NativeArray<ReceiveComponentData> Components;
    }
}