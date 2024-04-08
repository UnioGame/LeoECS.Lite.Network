namespace Game.Ecs.Network.NetworkCommands.Components
{
    using System;
    using Leopotam.EcsLite;

    /// <summary>
    /// network id of entity
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkIdComponent : IEcsAutoReset<NetworkIdComponent>
    {
        public int Id;
        
        public void AutoReset(ref NetworkIdComponent c)
        {
            c.Id = 0;
        }
    }
}