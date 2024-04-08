namespace Game.Ecs.Network.NetworkCommands.Components.Events
{
    using System;
    using Data;
    /// <summary>
    /// fire event to send value to network
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct EcsNetworkDataSendEvent
    {
        public int Target;
        public NetworkMessageTarget TargetType;
        public int Size;
    }
}