namespace Game.Ecs.Network.NetworkCommands.Data
{
    using MemoryPack;
    using UnityEngine.Serialization;

    [MemoryPackable]
    public partial struct NetworkEntityHeader
    {
        public int Id;
        public bool IsValueChanged;
        public int Count;
    }
}