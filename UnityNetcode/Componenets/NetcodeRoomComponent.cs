﻿namespace Game.Ecs.Network.UnityNetcode.Components
{
    using System;
    using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetcodeRoomComponent
    {
        public EcsPackedEntity[] Players;
        public bool IsOpen;
        public bool IsVisible;
        public int MasterClientId;
    }
}