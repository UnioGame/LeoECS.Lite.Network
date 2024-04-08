namespace Girand.Ecs.GameSettings.Components
{
    using System;
    using Leopotam.EcsLite;
    using Photon.Realtime;

    /// <summary>
    /// photon player data component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct PhotonPlayerComponent : IEcsAutoReset<PhotonPlayerComponent>
    {
        public int Id;
        public bool IsMaster;
        public bool IsLocal;
        public bool IsActive;
        public Player Player;

        public void AutoReset(ref PhotonPlayerComponent c)
        {
            c.Id = -1;
            c.Player = null;
        }
    }
}