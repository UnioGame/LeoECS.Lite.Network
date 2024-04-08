namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;

    [Serializable]
    public enum RPCTarget : byte
    {
        Server,
        NotServer,
        All,
        Me,
        NotMe,
    }
}