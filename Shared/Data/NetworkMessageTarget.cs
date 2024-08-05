namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;

    [Serializable]
    public enum NetworkMessageTarget : byte
    {
        Server,
        NotServer,
        All,
        Me,
        NotMe,
        Specified,
    }
}