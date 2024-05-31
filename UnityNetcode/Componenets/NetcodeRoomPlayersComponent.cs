﻿namespace Game.Ecs.Network.UnityNetcode.Components
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsLite;

    /// <summary>
    /// ADD DESCRIPTION HERE
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetcodeRoomPlayersComponent
    {
        public Dictionary<int, EcsPackedEntity> Value;
        
        public void AutoReset(ref NetcodeRoomPlayersComponent c)
        {
            c.Value ??= new Dictionary<int, EcsPackedEntity>(8);
            c.Value.Clear();
        }
    }
}