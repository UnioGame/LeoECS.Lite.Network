﻿namespace Game.Ecs.Network.NetworkCommands.Components
{
    using System;

    /// <summary>
    /// mark entity for network data sync
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkMarkerComponent
    {
        
    }
}