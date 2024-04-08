﻿namespace Girand.Ecs.GameSettings.Components.Events
{
    using System;
    
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    /// <summary>
    /// Event struct for when network authentication fails.
    /// </summary>
    [Serializable]
    public struct NetworkAuthenticationFailedSelfEvent
    {
        public string Value;
    }
}