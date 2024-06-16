namespace Game.Ecs.Network.UnityNetcode.Components
{
    using System;
    using NetworkCommands.Data;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetcodeStatusComponent
    {
        public bool IsMaster;
        public bool IsOnMasterServer;
        public bool IsConnected;
        public bool IsInRoom;
        public ConnectionStatus Status;
    }
}