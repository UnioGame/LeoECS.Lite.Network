namespace Game.Ecs.Network.Shared.Components
{
    using System;
    using UnityEngine.Serialization;

    /// <summary>
    /// information about active network connection
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkConnectionTypeComponent
    {
        public bool IsActive;
        public bool IsClient;
        public bool IsServer;
    }
}