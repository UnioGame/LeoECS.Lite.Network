namespace Game.Ecs.Network.NetworkCommands.Components.Requests
{
    using System;
    using Leopotam.EcsLite;
    
    /// <summary>
    /// ask to resend all data to client's
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkForceResendRequest : IEcsAutoReset<NetworkForceResendRequest>
    {
        public int ClientId;
        
        public void AutoReset(ref NetworkForceResendRequest c)
        {
            c.ClientId = -1;
        }
    }
}