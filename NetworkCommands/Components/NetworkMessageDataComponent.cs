namespace Game.Ecs.Network.NetworkCommands.Components
{
    using System;
    using System.Buffers;
    using Leopotam.EcsLite;
    using Unity.Collections;
    using UnityEngine.Serialization;

    /// <summary>
    /// rpc data component
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct NetworkMessageDataComponent : IEcsAutoReset<NetworkMessageDataComponent>
    {
        //must be temp allocated
        public byte[] Value;
        public int Size;
        
        public void AutoReset(ref NetworkMessageDataComponent c)
        {
            c.Value = null;
            c.Size = 0;
        }
    }
}