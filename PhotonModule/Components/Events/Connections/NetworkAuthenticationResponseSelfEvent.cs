namespace Girand.Ecs.GameSettings.Components.Events
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsLite;
    
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif

    /// <summary>
    /// Represents a network authentication response event.
    /// </summary>
    /// <remarks>
    /// This event is used to handle authentication response from a network server.
    /// </remarks>
    /// <seealso cref="Leopotam.EcsLite.IEcsAutoReset{T}" />
    [Serializable]
    public struct NetworkAuthenticationResponseSelfEvent :
        IEcsAutoReset<NetworkAuthenticationResponseSelfEvent>
    {
        public Dictionary<string, object> Value;
        
        public void AutoReset(ref NetworkAuthenticationResponseSelfEvent c)
        {
            c.Value ??= new Dictionary<string, object>(8);
            c.Value.Clear();
        }
    }
}