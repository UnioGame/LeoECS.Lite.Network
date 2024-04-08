namespace Game.Ecs.Network.NetworkCommands.Data
{
    using System;

    [Serializable]
    public struct NetworkMessageValue
    {
        public int Code;
        public string Value;
    }
}