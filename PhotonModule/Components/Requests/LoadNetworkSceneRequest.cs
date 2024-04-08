namespace Girand.Ecs.Server.Components.Requests
{
    using System;

    /// <summary>
    ///load new scene request
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct LoadNetworkSceneRequest
    {
        public string Value;
    }
}