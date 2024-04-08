namespace Game.Ecs.Network.NetworkCommands.Data
{
    using MemoryPack;

    [MemoryPackable]
    public partial struct NetworkComponentHeader
    {
        public int TypeId;
        public int Size;
    }
}