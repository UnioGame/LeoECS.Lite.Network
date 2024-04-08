namespace Game.Ecs.Network.UnityNetcode.Componenets.Requests
{
    using System;

    /// <summary>
    /// initialize new netcode
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct InitializeNetcodeSelfRequest
    {
        
    }
}