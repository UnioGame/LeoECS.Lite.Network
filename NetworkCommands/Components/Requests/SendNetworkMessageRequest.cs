namespace Game.Ecs.Network.NetworkCommands.Components.Requests
{
    using System;

    /// <summary>
    /// send message to targets
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct SendNetworkMessageRequest
    {
        public int MessageCode;
        public object Message;
    }
}