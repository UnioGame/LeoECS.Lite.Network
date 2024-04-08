namespace Game.Ecs.Network.NetworkCommands.Components.Requests
{
    using System;
    using Data;
    using Leopotam.EcsLite;

    /// <summary>
    /// public mark entity as ready to transfer with netcode
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkTransferRequest : IEcsAutoReset<NetworkTransferRequest>
    {
        public ulong TargetId;
        public int Tick;
        public float Time;
        public NetworkMessageTarget Target;
        
        public void AutoReset(ref NetworkTransferRequest c)
        {
            c.TargetId = 0;
            c.Tick = -1;
            c.Target = NetworkMessageTarget.NotServer;
            c.Time = -1;
        }
    }
}