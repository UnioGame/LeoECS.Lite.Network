namespace Game.Ecs.Network.NetworkCommands.Data
{
    using MemoryPack;
    using UnityEngine.Serialization;

    [MemoryPackable]
    public partial struct NetworkHeader
    {
        public int Tick;
        public float Time;
        public int Count;
        public int Size;
        public int Components;
        public bool IsHashed;
    }
}