﻿namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Components
{
    using System;
    using Data;

    /// <summary>
    /// handle for base rpc calls
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetcodeMessageChannelComponent
    {
        public NetcodeRPCChannelObject Value;
    }
}