namespace Game.Ecs.Network.NetworkCommands.Components.Requests
{
    using System;
    using Data;

    /// <summary>
    /// custom network message request
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkMessageRequest
    {
        public NetworkMessageTarget Target;
        public byte[] Data;
    }
}