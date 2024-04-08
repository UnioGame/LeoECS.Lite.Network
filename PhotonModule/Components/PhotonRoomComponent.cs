namespace Girand.Ecs.GameSettings.Components
{
    using System;
    using Photon.Realtime;

    /// <summary>
    /// ref to photon room
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct PhotonRoomComponent
    {
        public Room Value;
    }
}