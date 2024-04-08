namespace Girand.Ecs.GameSettings.Systems
{
    using System;

    [Serializable]
    public struct RoomCallbackEvent
    {
        public RoomCallbackType Type;
        public object Data;
    }
}