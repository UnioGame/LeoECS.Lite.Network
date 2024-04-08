namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;

    [Serializable]
    public struct RPCMessage
    {
        public NetworkMessageValue Value;
    }
}