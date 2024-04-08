namespace Game.Ecs.Network.Shared.Components
{
    using System;

    /// <summary>
    /// server time
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkTimeComponent
    {
        public float Time;
        public int Tick;
    }
}