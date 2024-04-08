namespace Game.Ecs.Network.NetworkCommands.Components
{
    using System;
    using Data;
    using Leopotam.EcsLite;

    /// <summary>
    /// base root rpc target component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkTargetComponent : IEcsAutoReset<NetworkTargetComponent>
    {
        public ulong Id;
        public NetworkMessageTarget Value;
        
        public void AutoReset(ref NetworkTargetComponent c)
        {
            c.Id = 0;
            c.Value = NetworkMessageTarget.NotServer;
        }
    }
}