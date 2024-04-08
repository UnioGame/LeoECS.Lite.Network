namespace Game.Ecs.Network.UnityNetcode.Componenets
{
    using System;
    using Unity.Netcode.Transports.UTP;

    /// <summary>
    /// UnityTransport component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct UnityTransportComponent
    {
        public UnityTransport Value;
    }
}