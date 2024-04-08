namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Components
{
    using System;
    using Unity.Netcode;

    /// <summary>
    /// link ot player object
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetcodeClientObjectComponent
    {
        public NetworkObject Value;
    }
}