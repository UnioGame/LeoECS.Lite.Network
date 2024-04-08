namespace Girand.Ecs.GameSettings.Systems
{
    using System;

    [Serializable]
    public enum RoomCallbackType
    {
        PlayerEnteredRoom,
        PlayerLeftRoom,
        RoomPropertiesUpdate,
        PlayerPropertiesUpdate,
        MasterClientSwitched
    }
}