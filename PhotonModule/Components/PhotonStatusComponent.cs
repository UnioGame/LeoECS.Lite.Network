namespace Girand.Ecs.GameSettings.Components
{
    using System;
    using Photon.Realtime;

    /// <summary>
    /// photon info
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct PhotonStatusComponent
    {
        public bool IsMaster;
        public bool IsOnMasterServer;
        public bool IsConnected;
        public bool IsInRoom;
        public ClientState State;
        public string AppVersion;
        public string ServerName;
        public int CountOfPlayersOnMaster;
        public int CountOfPlayersInRooms;
        public ConnectionStatus Status;
    }
}