namespace Game.Ecs.Network.NetworkCommands.Components
{
    using System;
    using Data;
    using Unity.Collections;
    using UnityEngine.Serialization;

    /// <summary>
    /// network data history
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkHistoryComponent
    {
        public int Tick;
        public float Time;
        public int Index;
        public int LastIndex;
        public NetworkHistoryData[] History;
    }
}