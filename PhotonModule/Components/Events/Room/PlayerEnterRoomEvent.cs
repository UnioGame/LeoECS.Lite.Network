namespace Girand.Ecs.GameSettings.Components.Events.Room
{
    using System;
    using Photon.Realtime;

    /// <summary>
    /// master client switched event
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct PlayerEnterRoomEvent
    {
        public Player Player;
    }
}