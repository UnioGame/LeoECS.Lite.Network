namespace Girand.Ecs.GameSettings.Systems
{
    using System;

    [Serializable]
    public struct ConnectionCallbackEvent
    {
        public ConnectionsCallbackType Type;
        public object Data;
    }
}